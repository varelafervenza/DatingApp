using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly DataContext context;
        public ValuesController(DataContext dataContext)
        {
            this.context = dataContext;
        }

        [Authorize(Roles = "Admin, Moderator" )]
        [HttpGet]
        public async Task<IActionResult> GetValues()
        {
            return Ok(await context.Values.ToListAsync());
        }

        [Authorize(Roles = "Member" )]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetValue(int id)
        {
            return Ok(await context.Values.FirstOrDefaultAsync(x => x.Id == id));
        }

    }
}