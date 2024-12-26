namespace FoodWasteReduction.Web.Services.Interfaces
{
    public interface IAuthGuardService
    {
        bool IsAuthenticated { get; }
        bool HasRole(string role);
        void SetToken(string token);
        void ClearToken();
        string? GetToken();
    }
}
