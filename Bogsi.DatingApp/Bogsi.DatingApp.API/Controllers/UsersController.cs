using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Bogsi.DatingApp.API.Data.Repositories;
using Bogsi.DatingApp.API.Dtos;
using Bogsi.DatingApp.API.Helpers;
using Bogsi.DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bogsi.DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(LogUserActivity))]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _datingRepository;
        private readonly IMapper _mapper;

        public UsersController(
            IDatingRepository datingRepository,
            IMapper mapper)
        {
            _mapper = mapper;
            _datingRepository = datingRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await this._datingRepository.GetUsers();

            var usersToReturn = this._mapper.Map<IEnumerable<UserForListDto>>(users);

            return Ok(usersToReturn);
        }

        [HttpGet("{id:int}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await this._datingRepository.GetUser(id);

            var userToReturn = this._mapper.Map<UserForDetailDto>(user);

            return Ok(userToReturn);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdate)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var userFromRepo = await this._datingRepository.GetUser(id);

            _mapper.Map(userForUpdate, userFromRepo);

            if (await this._datingRepository.SaveAll())
            {
                return NoContent();
            }
            else
            {
                throw new Exception($"Updating user : [{id}] failed on save.");
            }
        }
    }
}