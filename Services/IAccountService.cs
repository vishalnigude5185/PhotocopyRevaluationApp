using Microsoft.AspNetCore.Identity;
using PhotocopyRevaluationAppMVC.Models;

namespace PhotocopyRevaluationAppMVC.Services
{
    public interface IAccountService
    {
        Task<bool> EmailAlreadyExistsAsync(UserRegisterationDTO UserRegisterationDTO);
        Task<IdentityResult> SignupAsync(UserRegisterationDTO UserRegisterationDTO);
        Task<ApplicationUser> FindByEmailAsync(string email);
        Task<IdentityResult> ConfirmEmailAsync(string userId, string token);
        Task<Microsoft.AspNetCore.Identity.SignInResult> SignInAsync(string email, string password, bool rememberMe);
        //Task<IdentityResult> SignupAsync(SignupRequest UserRegisterationDTO);
        Task<bool> IsValidPhoneNumberAsync(string input);
        Task<bool> IsValidEmailAsync(string email);
    }
}
