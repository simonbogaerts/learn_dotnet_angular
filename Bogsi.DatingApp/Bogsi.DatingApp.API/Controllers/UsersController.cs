using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
        public async Task<IActionResult> GetUsers([FromQuery] UserParameters parameters)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var currentUser = await this._datingRepository.GetUser(currentUserId);

            parameters.UserId = currentUserId;

            if (string.IsNullOrWhiteSpace(parameters.Gender))
            {
                parameters.Gender = (currentUser.Gender == "male") ? "female" : "male";
            }

            var users = await this._datingRepository.GetUsers(parameters);

            var usersToReturn = this._mapper.Map<IEnumerable<UserForListDto>>(users);

            Response.AddPagination(
                users.CurrentPage, 
                users.PageSize,
                users.TotalCount, 
                users.TotalPages);

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

        [HttpPost("{id:int}/like/{recipientId:int}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            if (await this._datingRepository.GetUser(recipientId) == null)
            {
                return NotFound();
            }

            var like = await this._datingRepository.GetLike(id, recipientId);

            if (like != null)
            {
                return BadRequest("You already liked this user!");
            }

            like = new Like
            {
                LikerId = id,
                LikeeId = recipientId
            };

            _datingRepository.Add(like);

            if (await this._datingRepository.SaveAll())
            {
                return Ok();
            }
            else
            {
                return BadRequest("Failed to like user!");
            }
        }
    }
}