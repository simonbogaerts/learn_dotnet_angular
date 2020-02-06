﻿using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Bogsi.DatingApp.API.Data.Repositories;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Bogsi.DatingApp.API.Helpers
{
    public class LogUserActivity: IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context, 
            ActionExecutionDelegate next)
        {
            var resultContext = await next();

            var userId = int.Parse(resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var repository = resultContext.HttpContext.RequestServices.GetService<IDatingRepository>();

            var user = await repository.GetUser(userId);

            user.LastActive = DateTime.Now;

            await repository.SaveAll();
        }
    }
}