using AppZeroAPI.Entities;
using AppZeroAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppZeroAPI.Interfaces
{
    public interface IAuthService
    {
        Task<UserTokenResponse> Authenticate(LoginDto model, string ipAddress = "");
        Task<UserTokenResponse> RenewAccessToken(AuthInfo request, string ipAddress = "");
        Task<int> SignUp(RegisterDto user, string ipAddress = "");
        Task<bool> RevokeToken(string token);
        Task Logout(string email);
    }
}
