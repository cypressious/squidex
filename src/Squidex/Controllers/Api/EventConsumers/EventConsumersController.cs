﻿// ==========================================================================
//  EventConsumersController.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Squidex.Controllers.Api.EventConsumers.Models;
using Squidex.Core.Identity;
using Squidex.Infrastructure.CQRS.Events;
using Squidex.Infrastructure.Reflection;
using Squidex.Pipeline;

namespace Squidex.Controllers.Api.EventConsumers
{
    [ApiExceptionFilter]
    [Authorize(Roles = SquidexRoles.Administrator)]
    [SwaggerIgnore]
    public sealed class EventConsumersController : Controller
    {
        private readonly IEventConsumerInfoRepository eventConsumerRepository;

        public EventConsumersController(IEventConsumerInfoRepository eventConsumerRepository)
        {
            this.eventConsumerRepository = eventConsumerRepository;
        }

        [HttpGet]
        [Route("event-consumers/")]
        public async Task<IActionResult> GetEventConsumers()
        {
            var entities = await eventConsumerRepository.QueryAsync();

            var models = entities.Select(x => SimpleMapper.Map(x, new EventConsumerDto())).ToList();

            return Ok(models);
        }

        [HttpPut]
        [Route("event-consumers/{name}/start")]
        public async Task<IActionResult> Start(string name)
        {
            await eventConsumerRepository.StartAsync(name);

            return NoContent();
        }

        [HttpPut]
        [Route("event-consumers/{name}/stop")]
        public async Task<IActionResult> Stop(string name)
        {
            await eventConsumerRepository.StopAsync(name);

            return NoContent();
        }

        [HttpPut]
        [Route("event-consumers/{name}/reset")]
        public async Task<IActionResult> Reset(string name)
        {
            await eventConsumerRepository.ResetAsync(name);

            return NoContent();
        }
    }
}
