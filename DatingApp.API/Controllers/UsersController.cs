using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository repository;
        private readonly IMapper mapper;

        public UsersController(IDatingRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var currentUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var userFromRepo = await this.repository.GetUser(currentUser);

            userParams.UserId = currentUser;

            if (string.IsNullOrEmpty(userParams.Gender)) 
            {
                userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male";
            }

            var users = await repository.GetUsers(userParams);
            
            var result = this.mapper.Map<IEnumerable<UserForListDTO>>(users);
            
            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            
            return Ok(result);
        }

        [HttpGet("{userId}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int userId)
        {
            var user = await repository.GetUser(userId);
            var result = this.mapper.Map<UserForDetailedDTO>(user);
            
            return Ok(result);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, UserForUpdateDTO userForUpdateDTO)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var userFromRepo = await repository.GetUser(userId);
            mapper.Map(userForUpdateDTO, userFromRepo);
            
            if(await repository.SaveAll())
                return NoContent();
            
            throw new Exception($"Updating user {userId} failed on save");
        }

        [HttpPost("{userId}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int userId, int recipientId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var like = await repository.GetLike(userId, recipientId);

            if (like != null)
                return BadRequest("You already like this user");

            if (await repository.GetUser(recipientId) == null)
                return NotFound();

            like = new Like 
            {
                LikerId = userId,
                LikeeId = recipientId
            };

            repository.Add<Like>(like);

            if (await repository.SaveAll())
                return Ok();

            return BadRequest("Failed to like user");
        }
       
    }
}