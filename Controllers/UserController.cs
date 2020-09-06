using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using AppZeroAPI.Interfaces;
using AppZeroAPI.Entities;
using AppZeroAPI.Models;
using AppZeroAPI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AppZeroAPI.Controllers
{
    [ApiController]
    [Route("api/Users")]
    public class UserController : BaseController
    {
        private readonly ILogger<UserController> logger;
        private readonly IUnitOfWork unitOfWork;
      

        public UserController(IUnitOfWork unitOfWork, ILogger<UserController> logger)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
        }
        
        [HttpGet] 
        public async Task<IActionResult> GetAll()
        {
            logger.LogInformation("called Product Controller");
            var data = await unitOfWork.Users.GetAllAsync();
            return AppResponse.Success(data);
        }
       

        // DELETE api/User
        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            var user = GetUser(HttpContext.User);
            await this.unitOfWork.Users.DeleteByUserIdAsync(user.Id.ToString());
            return AppResponse.Success(); 
        }

        // PUT api/User
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] UserProfile newUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(new {ErrorMsg = "No se ha proporcionado un usuario valido"});

            var user = GetUser(HttpContext.User).Result;
            newUser.user_id = user.user_id;
            var updatedUser = await this.unitOfWork.Users.UpdateAsync(newUser);

            return Ok(updatedUser);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserProfile>> GetById(string id)
        {
            var data = await unitOfWork.Users.GetByUserIdAsync(id);
            if (data == null)
            {
                return AppResponse.NotFound("User Not Found");
            } 
            return AppResponse.Success(data);
        }
        [HttpGet("{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserProfile>> GetUserEmailAsync(string email)
        {
            var data = await unitOfWork.Users.GetUserByEmailAsync(email);
            if (data == null)
            {
                return AppResponse.NotFound("User Not Found");
            }
            return AppResponse.Success(data);
        }
        private async Task<UserProfile>  GetUser(ClaimsPrincipal user)
        {
            string textId = user.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier).Value;

            return await this.unitOfWork.Users.GetByUserIdAsync(textId);
        }
    }
}