using AppZeroAPI.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Dynamic;
using Newtonsoft.Json.Converters;
using System.IdentityModel.Tokens.Jwt;
 

namespace AppZeroAPI.Models
{

 
 
    public class RegisterDto
    {
        [Required]
        [MinLength(3)]
        public string Name { get; set; }
       
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(12, MinimumLength = 4, ErrorMessage = "Password must be between 4 and 12 characters in length.")]
        [JsonProperty(Required = Required.DisallowNull)]
        public string Password { get; set; }

        [Required]
        [StringLength(12, MinimumLength = 4, ErrorMessage = "Password must be between 4 and 12 characters in length.")]
        [JsonProperty(Required = Required.DisallowNull)]
        public string ConfirmPassword { get; set; }
    }
    public class LoginDto
    {
        [EmailAddress]
        [Required]
        [JsonProperty(Required = Required.DisallowNull)]
        public string Email { get; set; }

        [Required]
        [StringLength(12, MinimumLength = 4, ErrorMessage = "Password must be between 6 and 12 characters in length.")]
        [JsonProperty(Required = Required.DisallowNull)]
        public string Password { get; set; }

    }
    public class LogoutDto
    {
        [Required]
        [JsonProperty(Required = Required.DisallowNull)]
        public string Token { get; set; }
    }
    public class PasswordChangeDto
    {

        [Required]
        [StringLength(12, MinimumLength = 4, ErrorMessage = "Old Password must be between 6 and 12 characters in length.")]
        [JsonProperty(Required = Required.DisallowNull)]
        public string OldPassword { get; set; }


        [Required]
        [StringLength(12, MinimumLength = 4, ErrorMessage = "Password must be between 4 and 12 characters in length.")]
        [JsonProperty(Required = Required.DisallowNull)]
        public string NewPassword { get; set; }

        [Required]
        [StringLength(12, MinimumLength = 4, ErrorMessage = "Confirm Password must be between 4 and 12 characters in length.")]
        [JsonProperty(Required = Required.DisallowNull)]
        public string ConfirmPassword { get; set; }

    }
    public class FieldError
    {
        public string Field { get; set; }
        public string ErrorMessage { get; set; }
        public List<FieldError> SubErrors { get; set; } = new List<FieldError>();
    }
    public class ValidationResult
    {
        public string Message { get; set; }
        public List<FieldError> Errors { get; set; } = new List<FieldError>();
        public ValidationResult(string errorMessage)
        {
            Message = errorMessage;
        }
    }
    public class ValidationException : Exception
    {
        public ValidationResult ValidationResult { get; set; }

        public ValidationException(ValidationResult validationResult)
        {
            ValidationResult = validationResult;
        }
    }
    public class AuthInfo
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
   
    public class RefreshRequestDto
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }

    public class UserTokenResponse
    { 
        [Required]
        [JsonProperty("access_token", Required = Required.DisallowNull)]
        public string AccessToken { get; set; }
        
        [Required]
        [JsonProperty("refresh_token", Required = Required.DisallowNull)]
        public string RefreshToken { get; set; }
          
        [JsonProperty("expires_at")]
        public DateTime ExpiresAt { get; set; } 
    }
     
    public class UserInfo
    {
        
        public int user_id { get; set; } 
        public string username { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string role { get; set; }
    }
    public class TokenDto
    {  
        public string EncodedToken { get; set; } 
        public JwtSecurityToken TokenModel { get; set; }

    }
 

    public class RequestPasswordDTO
    {
        public string Email { get; set; }
    }

    public class RestorePasswordDTO
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string Token { get; set; }
    }
    public class UpdateRequest
    {
        private string _password;
        private string _confirmPassword;
        private string _role;
        private string _email;

        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Role { get; set; }


        [EmailAddress]
        public string Email
        {
            get => _email;
            set => _email = replaceEmptyWithNull(value);
        }

        [MinLength(6)]
        public string Password
        {
            get => _password;
            set => _password = replaceEmptyWithNull(value);
        }

        [Compare("Password")]
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => _confirmPassword = replaceEmptyWithNull(value);
        }

        // helpers

        private string replaceEmptyWithNull(string value)
        {
            // replace empty string with null to make field optional
            return string.IsNullOrEmpty(value) ? null : value;
        }
    }

    class SmtpSettings
    {
        public string From { get; set; }
        public string Host { get; set; }
        public string Password { get; set; }
        public string Port { get; set; }
        public string Sender { get; set; }
        public string User { get; set; }
        public string CC { get; set; }
    }



    public class LogData
    {
         
        public int Id { get; set; }

        public string Category { get; set; }

        public string Message { get; set; }

        public string User { get; set; }

        public int UserId { get; set; }
        public DateTimeOffset? MessageOn { get; set; }
    }
 
     
   

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AuthStatus
    {
        fail,
        success
    }

        
    [Serializable]
    public sealed class AppResponse
    {
        public int status { get; set; }
        public object payload { get; set; }
        public   AppResponse()
        {
             
        }

        public static object ToResult(object data = null, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {

            if (httpStatusCode == HttpStatusCode.OK)
            {
                var apiResponse = new AppResponse
                {
                    payload = new { data = data },
                    status = 1
                };
                return apiResponse ;
            }
            else
            {
                var apiResponse = new
                {
                    message = data,
                    status = -1
                };
             return  apiResponse  ;
            }
        }
        public static ObjectResult ToObjectResult(object data = null, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
           
            if (httpStatusCode == HttpStatusCode.OK)
            {
                var apiResponse = new AppResponse 
                {
                    payload = new { data = data },
                    status = 1
                }; 
                return new ObjectResult(JsonConvert.SerializeObject(apiResponse));
            } else
            {
                var apiResponse = new  
                {
                    message =  data ,
                    status = -1
                }; 
                return new ObjectResult(JsonConvert.SerializeObject(apiResponse));
            } 
        }
        
        public static ObjectResult Success(Object data = null)
        {
            return ToObjectResult(data, HttpStatusCode.OK);
        }
          
        public static ConflictObjectResult Conflict(string message = "Conflict")
        {
            return new ConflictObjectResult(ToObjectResult(message, HttpStatusCode.NotFound));
        }
        public static BadRequestObjectResult BadRequest(string message = "Bad Request")
        {
            return new BadRequestObjectResult(ToObjectResult(message,HttpStatusCode.BadRequest));
        }
        public static NotFoundObjectResult NotFound(string message =  "Not Found")
        {
            return new NotFoundObjectResult(ToObjectResult(message, HttpStatusCode.NotFound));
        }
        public static NotFoundObjectResult Forbidden(string message = "Forbidden")
        {
            return new NotFoundObjectResult(ToObjectResult(message, HttpStatusCode.Forbidden));
        }
        public static UnauthorizedObjectResult Unauthorized(string message = "Unauthorized")
        {
            return new UnauthorizedObjectResult(ToObjectResult(message, HttpStatusCode.Unauthorized));
        }
        public static ObjectResult ExpectationFailed(string errorMsg)
        { 
            return ToObjectResult(errorMsg, HttpStatusCode.ExpectationFailed);
        }

        public static object Exception(string errorMsg)
        {
            return ToResult(errorMsg, HttpStatusCode.BadRequest);
        }
        public static object UnauthorizedUser(string message)
        {
            return ToResult(message, HttpStatusCode.Unauthorized);
        }
        public static object SystemError(string message)
        {
            return ToResult(message, HttpStatusCode.InternalServerError);
        }
        public static ObjectResult BadRequest(ModelStateDictionary modelState)
        {
            var errorMsg = string.Join(" | ", modelState.Values
                   .SelectMany(v => v.Errors)
                   .Select(e => e.ErrorMessage));
            return ToObjectResult(errorMsg, HttpStatusCode.BadRequest);
            //string errorMsg = null;
            //var error = modelState.SelectMany(x => x.Value.Errors).First();
            //if (!string.IsNullOrEmpty(error.ErrorMessage))
            //    errorMsg = error.ErrorMessage;
            //else if (error.Exception?.Message != null)
            //    errorMsg = error.Exception.Message; 
            //return ToObjectResult(errorMsg, HttpStatusCode.BadRequest);
        }

    }
    public class ErrorResponse
    {
        public string Message { get; set; }
    }
    public class AppException : Exception
    {
        public AppException() : base() { }

        public AppException(string message) : base(message) { }

        public AppException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}


