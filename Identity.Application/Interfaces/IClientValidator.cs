namespace Identity.Application.Interfaces
{
    public interface IClientValidator
    {
        bool IsValid(string? clientId, string? clientSecret);
    }
}
