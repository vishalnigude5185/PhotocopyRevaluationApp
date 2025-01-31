﻿using Microsoft.AspNetCore.Identity;
using PhotocopyRevaluationApp.Models;

namespace PhotocopyRevaluationApp.Services {
    public interface IAccountService {
        Task<bool> EmailAlreadyExistsAsync(UserRegisterationDTO UserRegisterationDTO);
        Task<IdentityResult> SignupAsync(UserRegisterationDTO UserRegisterationDTO);
        Task<ApplicationUser> FindByEmailAsync(string email);
        Task<IdentityResult> ConfirmEmailAsync(string userId, string token);
        Task<SignInResult> SignInAsync(string email, string password, bool rememberMe);
        //Task<IdentityResult> SignupAsync(SignupRequest UserRegisterationDTO);
        Task<bool> IsValidPhoneNumberAsync(string input);
        Task<bool> IsValidEmailAsync(string email);
    }
}
