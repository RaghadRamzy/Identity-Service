using Identity.Application.Common;
using Identity.Application.DTOs.Auth;
using Identity.Application.DTOs.Users;
using Identity.Application.Interfaces;
using Identity.Domain.Entity;
using Identity.Domain.Enum;

namespace Identity.Application.Services
{
    
    public class AuthService
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IAuditLogService _auditLogService;
        private readonly ICurrentUserService _currentUser;
        private readonly IClientValidator _clientValidator;

        private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(30);
        public AuthService(
            IIdentityService identityService,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepository,
            IAuditLogService auditLogService,
            ICurrentUserService currentUser,
            IClientValidator clientValidator)
        {
            _identityService = identityService;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _auditLogService = auditLogService;
            _currentUser = currentUser;
            _clientValidator = clientValidator;
        }

        public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            var result = await _identityService.CreateUserAsync(
                request.Username, request.FullName, request.Email, request.PhoneNumber, request.Password, request.Role);

            if (!result.Succeeded || result.Data is null)
                return Result<AuthResponse>.Failure(result.Errors);

            await _auditLogService.LogAsync(AuditAction.UserCreated, result.Data.Id, _currentUser.IpAddress, _currentUser.UserAgent);

            var authResponse = await IssueTokensAsync(result.Data);
            return Result<AuthResponse>.Success(authResponse);
        }

        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
        {
            var validation = await _identityService.ValidateCredentialsAsync(request.UsernameOrEmail, request.Password);
            if (!validation.Succeeded || validation.Data is null)
            {
                await _auditLogService.LogAsync(AuditAction.Login, null, _currentUser.IpAddress, _currentUser.UserAgent, "Failed login attempt");
                return Result<AuthResponse>.Failure(validation.Errors.Length > 0 ? validation.Errors : new[] { "Invalid username or password." });
            }

            var user = validation.Data;

            if (!user.IsActive)
                return Result<AuthResponse>.Failure("Account is disabled.");

            if (user.IsLocked)
                return Result<AuthResponse>.Failure("Account is locked. Try again later."); 

            var response = await IssueTokensAsync(user);
            await _auditLogService.LogAsync(AuditAction.Login, user.Id, _currentUser.IpAddress, _currentUser.UserAgent);
            return Result<AuthResponse>.Success(response);
        }

        public async Task<Result<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var userId = _tokenService.GetUserIdFromExpiredToken(request.AccessToken);
            if (userId is null)
                return Result<AuthResponse>.Failure("Invalid access token.");

            var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
            if (storedToken is null || !storedToken.IsActive || storedToken.UserId != userId)
                return Result<AuthResponse>.Failure("Invalid or expired refresh token.");

            var user = await _identityService.GetUserByIdAsync(userId.Value);
            if (user is null)
                return Result<AuthResponse>.Failure("User not found.");

            await _refreshTokenRepository.RevokeAsync(storedToken); 
            var response = await IssueTokensAsync(user);
            return Result<AuthResponse>.Success(response);
        }

        public async Task<Result> LogoutAsync(Guid userId, string refreshToken)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (storedToken is not null && storedToken.UserId == userId)
            {
                await _refreshTokenRepository.RevokeAsync(storedToken);
            }

            await _auditLogService.LogAsync(AuditAction.Logout, userId, _currentUser.IpAddress, _currentUser.UserAgent);
            return Result.Success();
        }

        public async Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
        {
            var result = await _identityService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
            if (result.Succeeded)
            {
                await _refreshTokenRepository.RevokeAllForUserAsync(userId);
                await _auditLogService.LogAsync(AuditAction.PasswordChanged, userId, _currentUser.IpAddress, _currentUser.UserAgent);
            }
            return result;
        }

        public async Task<Result<string>> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var user = await _identityService.GetUserByEmailAsync(request.Email);
            if (user is null)
                return Result<string>.Success(string.Empty);

            var token = await _identityService.GeneratePasswordResetTokenAsync(user.Id);
            return Result<string>.Success(token); 
        }

        public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var result = await _identityService.ResetPasswordAsync(request.Email, request.Token, request.NewPassword);
            if (result.Succeeded)
            {
                var user = await _identityService.GetUserByEmailAsync(request.Email);
                if (user is not null)
                {
                    await _refreshTokenRepository.RevokeAllForUserAsync(user.Id);
                    await _auditLogService.LogAsync(AuditAction.PasswordReset, user.Id, _currentUser.IpAddress, _currentUser.UserAgent);
                }
            }
            return result;
        }


        public async Task<Result<TokenResponse>> RequestTokenAsync(TokenRequest request)
        {
            if (!_clientValidator.IsValid(request.ClientId, request.ClientSecret))
                return Result<TokenResponse>.Failure("invalid_client");

            return request.GrantType?.Trim().ToLowerInvariant() switch
            {
                "password" => await HandlePasswordGrantAsync(request),
                "refresh_token" => await HandleRefreshTokenGrantAsync(request),
                _ => Result<TokenResponse>.Failure("unsupported_grant_type")
            };
        }

        private async Task<Result<TokenResponse>> HandlePasswordGrantAsync(TokenRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return Result<TokenResponse>.Failure("invalid_request");

            var validation = await _identityService.ValidateCredentialsAsync(request.Username, request.Password);
            if (!validation.Succeeded || validation.Data is null)
            {
                await _auditLogService.LogAsync(AuditAction.Login, null, _currentUser.IpAddress, _currentUser.UserAgent, "Failed password grant attempt");
                return Result<TokenResponse>.Failure("invalid_grant");
            }

            var user = validation.Data;
            if (!user.IsActive || user.IsLocked)
                return Result<TokenResponse>.Failure("invalid_grant");

            var tokens = await IssueTokensAsync(user);
            await _auditLogService.LogAsync(AuditAction.Login, user.Id, _currentUser.IpAddress, _currentUser.UserAgent, "password grant");

            return Result<TokenResponse>.Success(ToTokenResponse(tokens, request.Scope));
        }

        private async Task<Result<TokenResponse>> HandleRefreshTokenGrantAsync(TokenRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return Result<TokenResponse>.Failure("invalid_request");

            var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
            if (storedToken is null || !storedToken.IsActive)
                return Result<TokenResponse>.Failure("invalid_grant");

            var user = await _identityService.GetUserByIdAsync(storedToken.UserId);
            if (user is null)
                return Result<TokenResponse>.Failure("invalid_grant");

            await _refreshTokenRepository.RevokeAsync(storedToken); 
            var tokens = await IssueTokensAsync(user);

            return Result<TokenResponse>.Success(ToTokenResponse(tokens, request.Scope));
        }

        private static TokenResponse ToTokenResponse(AuthResponse tokens, string? scope) => new()
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            ExpiresIn = (int)Math.Max(0, (tokens.AccessTokenExpiration - DateTime.UtcNow).TotalSeconds),
            Scope = scope
        };

        private async Task<AuthResponse> IssueTokensAsync(UserDto user)
        {
            var (accessToken, expiration) = _tokenService.GenerateAccessToken(user); 
            var refreshTokenValue = _tokenService.GenerateRefreshToken();

            await _refreshTokenRepository.AddAsync(new RefreshTokens
            {
                Token = refreshTokenValue,
                UserId = user.Id,
                ExpirationDate = DateTime.UtcNow.Add(RefreshTokenLifetime),
                CreatedByIp = _currentUser.IpAddress,
                UserAgent = _currentUser.UserAgent
            });

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                AccessTokenExpiration = expiration,
                User = user
            };
        }
    }
}
