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
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Bogsi.DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId:int}/[controller]")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _datingRepository;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;

        public PhotosController(
            IDatingRepository datingRepository,
            IMapper mapper,
            IOptions<CloudinarySettings> cloudinaryConfig)
        {
            this._mapper = mapper;
            this._datingRepository = datingRepository;

            var account = new Account(
                cloudinaryConfig.Value.CloudName,
                cloudinaryConfig.Value.Key,
                cloudinaryConfig.Value.Secret);

            _cloudinary = new Cloudinary(account);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await this._datingRepository.GetPhoto(id);

            var photo = this._mapper.Map<PhotoForReturnDto>(photoFromRepo);

            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhoto([FromRoute] int userId, [FromForm] PhotoForCreationDto photoForCreation)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var userFromRepo = await this._datingRepository.GetUser(userId);

            var file = photoForCreation.File;

            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.Name, stream),
                    Transformation = new Transformation()
                        .Width(500)
                        .Height(500)
                        .Crop("fill")
                        .Gravity("face")
                };
                uploadResult = _cloudinary.Upload(uploadParams);
            }

            photoForCreation.Url = uploadResult.Uri.ToString();
            photoForCreation.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreation);

            if (!userFromRepo.Photos.Any(x => x.IsMain))
            {
                photo.IsMain = true;
            }

            userFromRepo.Photos.Add(photo);

            if (await this._datingRepository.SaveAll())
            {
                var photoToReturn = this._mapper.Map<PhotoForReturnDto>(photo);

                return CreatedAtRoute(
                    "GetPhoto", 
                    new {userId = userId, id = photo.Id},
                    photoToReturn);
            }
            else
            {
                return BadRequest("Could not add photo.");
            }
        }
    }
}