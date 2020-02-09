using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public readonly IAuthRepository repository;

        public AuthController(IAuthRepository authRepository)
        {
            this.repository = authRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDTO userForRegister)
        {
            userForRegister.UserName = userForRegister.UserName.ToLower();

            if (await repository.UserExists(userForRegister.UserName))
                return BadRequest("User already exists");

            var userToCreate = new User
            {
                UserName = userForRegister.UserName
            };

            var createdUser = await repository.Register(userToCreate, userForRegister.Password);

            return StatusCode(201);

        }

    }
}