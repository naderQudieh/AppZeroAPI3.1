using AppZeroAPI.Controllers;
using AppZeroAPI.Interfaces;
using AppZeroAPI.Models;
using AppZeroAPI.Services;
using AppZeroAPI.Shared;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;



namespace AppZeroAPI.Controllers
{
    [ApiController]
    [Route("api/Auth")]
    public class AuthController : BaseController
    {

        private readonly IAuthService authService;
        private readonly ILogger<AuthController> logger;
        private readonly IMapper _mapper;
        public AuthController(ILogger<AuthController> logger, IMapper mapper, IAuthService authService)
        {
            _mapper = mapper;
            this.logger = logger;
            this.authService = authService;
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] LoginDto model)
        {
            if (string.IsNullOrEmpty(model.Email)
                || string.IsNullOrEmpty(model.Password) )
            {
                return AppResponse.BadRequest("All fields are required");
            }


            ModelValidator.Validate(model);
            string ipaddress = Helper.getIPAddress(this.Request);
            var refresh = await authService.Authenticate(model, ipaddress);
            if (refresh == null)
                return  AppResponse.Unauthorized("Invalid Token");

            if (string.IsNullOrEmpty(refresh.AccessToken) || string.IsNullOrEmpty(refresh.RefreshToken)  )
                return AppResponse.Unauthorized("Invalid Token");

            setTokenCookie(refresh.RefreshToken);
            return AppResponse.Success(refresh);

        }


        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RenewAccessToken([FromBody] AuthInfo request)
        {
            ModelValidator.Validate(request);
            var refreshToken = Request.Cookies["refreshToken"];
            string ipaddress = Helper.getIPAddress(this.Request); 
            var response = await authService.RenewAccessToken(request, ipaddress);
            if (response == null)
                return AppResponse.Unauthorized("Invalid Token");

            if (string.IsNullOrEmpty(response.AccessToken) || string.IsNullOrEmpty(response.RefreshToken))
                return AppResponse.Unauthorized("Invalid Token");
            setTokenCookie(response.RefreshToken);
            return AppResponse.Success(response); 
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Register(string Name, string Email, string Password, string ConfirmPassword)
        {

            if (string.IsNullOrEmpty(Name)
                || string.IsNullOrEmpty(Email)
                || string.IsNullOrEmpty(Password)
                || string.IsNullOrEmpty(ConfirmPassword))
            {
                return BadRequest(AppResponse.BadRequest("All fields are required"));
            }
            var model = new RegisterDto()
            {
                Name = Name,
                Email = Email,
                Password = Password,
                ConfirmPassword = ConfirmPassword,
            };
            await authService.SignUp(model, Request.Headers["origin"]);
            return AppResponse.Success("Registration successful, please check your email for verification instructions") ;
          }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] string token)
        {
            // accept token from request body or cookie
            var _token = token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            await authService.RevokeToken(_token);

            
            return Ok(new { message = "Token revoked" });
        }


        [NonAction]
        public ActionResult Get(int start, int count)
        {
            throw new NotImplementedException();
        }
        private void setTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }
    }
}