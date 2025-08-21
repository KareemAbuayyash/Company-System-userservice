using CompanySystem.Business.DTOs;

namespace CompanySystem.Business.Interfaces
{
    public interface IAuthApiService
    {
        Task<AuthLoginResponse> LoginAsync(string email, string password);
        Task<bool> ValidateUserAsync(string email, string password);
    }
}
