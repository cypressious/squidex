﻿// ==========================================================================
//  HandlerTestBase.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using System.Threading.Tasks;
using Squidex.Events;
using Squidex.Infrastructure;
using Squidex.Infrastructure.CQRS;
using Squidex.Infrastructure.CQRS.Commands;

#pragma warning disable IDE0019

namespace Squidex.Write.TestHelpers
{
    public abstract class HandlerTestBase<T> where T : DomainObjectBase
    {
        private sealed class MockupHandler : IAggregateHandler
        {
            private T domainObject;

            public bool IsCreated { get; private set; }
            public bool IsUpdated { get; private set; }

            public void Init(T newDomainObject)
            {
                domainObject = newDomainObject;

                IsCreated = false;
                IsUpdated = false;
            }

            public Task CreateAsync<V>(CommandContext context, Func<V, Task> creator) where V : class, IAggregate
            {
                IsCreated = true;

                return creator(domainObject as V);
            }

            public Task UpdateAsync<V>(CommandContext context, Func<V, Task> updater) where V : class, IAggregate
            {
                IsUpdated = true;

                return updater(domainObject as V);
            }
        }

        private readonly MockupHandler handler = new MockupHandler();

        protected RefToken User { get; } = new RefToken("subject", Guid.NewGuid().ToString());

        protected Guid AppId { get; } = Guid.NewGuid();

        protected Guid SchemaId { get; } = Guid.NewGuid();

        protected string AppName { get; } = "my-app";

        protected string SchemaName { get; } = "my-schema";

        protected NamedId<Guid> AppNamedId
        {
            get { return new NamedId<Guid>(AppId, AppName); }
        }

        protected NamedId<Guid> SchemaNamedId
        {
            get { return new NamedId<Guid>(SchemaId, SchemaName); }
        }

        protected IAggregateHandler Handler
        {
            get { return handler; }
        }

        protected CommandContext CreateContextForCommand<TCommand>(TCommand command) where TCommand : SquidexCommand
        {
            return new CommandContext(CreateCommand(command));
        }

        protected async Task TestCreate(T domainObject, Func<T, Task> action, bool shouldCreate = true)
        {
            handler.Init(domainObject);

            await action(domainObject);

            if (!handler.IsCreated && shouldCreate)
            {
                throw new InvalidOperationException("Create not called");
            }
        }

        protected async Task TestUpdate(T domainObject, Func<T, Task> action, bool shouldUpdate = true)
        {
            handler.Init(domainObject);

            await action(domainObject);

            if (!handler.IsUpdated && shouldUpdate)
            {
                throw new InvalidOperationException("Create not called");
            }
        }

        protected TCommand CreateCommand<TCommand>(TCommand command) where TCommand : SquidexCommand
        {
            command.Actor = User;

            var appCommand = command as AppCommand;

            if (appCommand != null)
            {
                appCommand.AppId = AppNamedId;
            }

            var schemaCommand = command as SchemaCommand;

            if (schemaCommand != null)
            {
                schemaCommand.SchemaId = SchemaNamedId;
            }

            return command;
        }

        protected TEvent CreateEvent<TEvent>(TEvent @event) where TEvent : SquidexEvent
        {
            @event.Actor = User;

            var appEvent = @event as AppEvent;

            if (appEvent != null)
            {
                appEvent.AppId = AppNamedId;
            }

            var schemaEvent = @event as SchemaEvent;

            if (schemaEvent != null)
            {
                schemaEvent.SchemaId = SchemaNamedId;
            }

            return @event;
        }
    }
}


#pragma warning restore IDE0019