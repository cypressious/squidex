﻿// ==========================================================================
//  UserManagementController.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Squidex.Controllers.Api.Users.Models;
using Squidex.Core.Identity;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Security;
using Squidex.Pipeline;
using Squidex.Read.Users.Repositories;

namespace Squidex.Controllers.Api.Users
{
    [ApiExceptionFilter]
    [Authorize(Roles = SquidexRoles.Administrator)]
    [SwaggerIgnore]
    public class UserManagementController : Controller
    {
        private readonly IUserRepository userRepository;

        public UserManagementController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        [HttpGet]
        [Route("user-management")]
        public async Task<IActionResult> GetUsers([FromQuery] string query = null, [FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            var taskForUsers = userRepository.QueryByEmailAsync(query, take, skip);
            var taskForCount = userRepository.CountAsync(query);

            await Task.WhenAll(taskForUsers, taskForCount);

            var model = new UsersDto
            {
                Total = taskForCount.Result,
                Items = taskForUsers.Result.Select(x => SimpleMapper.Map(x, new UserDto())).ToArray()
            };

            return Ok(model);
        }

        [HttpPut]
        [Route("user-management/{id}/lock/")]
        public async Task<IActionResult> Lock(string id)
        {
            if (IsSelf(id))
            {
                throw new ValidationException("Locking user failed.", new ValidationError("You cannot lock yourself."));
            }

            await userRepository.LockAsync(id);

            return NoContent();
        }

        [HttpPut]
        [Route("user-management/{id}/unlock/")]
        public async Task<IActionResult> Unlock(string id)
        {
            if (IsSelf(id))
            {
                throw new ValidationException("Unlocking user failed.", new ValidationError("You cannot unlock yourself."));
            }

            await userRepository.UnlockAsync(id);

            return NoContent();
        }

        private bool IsSelf(string id)
        {
            var subject = User.OpenIdSubject();

            return string.Equals(subject, id, StringComparison.OrdinalIgnoreCase);
        }
    }
}
