﻿// ==========================================================================
//  SchemaDomainObject.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using Squidex.Core.Schemas;
using Squidex.Events.Schemas;
using Squidex.Events.Schemas.Utils;
using Squidex.Infrastructure;
using Squidex.Infrastructure.CQRS;
using Squidex.Infrastructure.CQRS.Events;
using Squidex.Infrastructure.Dispatching;
using Squidex.Infrastructure.Reflection;
using Squidex.Write.Schemas.Commands;

namespace Squidex.Write.Schemas
{
    public class SchemaDomainObject : DomainObjectBase
    {
        private readonly FieldRegistry registry;
        private bool isDeleted;
        private long totalFields;
        private Schema schema;

        public Schema Schema
        {
            get { return schema; }
        }

        public bool IsDeleted
        {
            get { return isDeleted; }
        }

        public SchemaDomainObject(Guid id, int version, FieldRegistry registry)
            : base(id, version)
        {
            Guard.NotNull(registry, nameof(registry));

            this.registry = registry;
        }

        public void On(FieldAdded @event)
        {
            totalFields++;

            schema = SchemaEventDispatcher.Dispatch(@event, schema, registry);
        }

        protected void On(SchemaCreated @event)
        {
            schema = SchemaEventDispatcher.Dispatch(@event);
        }

        protected void On(FieldUpdated @event)
        {
            schema = SchemaEventDispatcher.Dispatch(@event, schema);
        }

        protected void On(FieldHidden @event)
        {
            schema = SchemaEventDispatcher.Dispatch(@event, schema);
        }

        protected void On(FieldShown @event)
        {
            schema = SchemaEventDispatcher.Dispatch(@event, schema);
        }

        protected void On(FieldDisabled @event)
        {
            schema = SchemaEventDispatcher.Dispatch(@event, schema);
        }

        protected void On(FieldEnabled @event)
        {
            schema = SchemaEventDispatcher.Dispatch(@event, schema);
        }

        protected void On(SchemaUpdated @event)
        {
            schema = SchemaEventDispatcher.Dispatch(@event, schema);
        }

        protected void On(FieldDeleted @event)
        {
            schema = SchemaEventDispatcher.Dispatch(@event, schema);
        }

        protected void On(SchemaPublished @event)
        {
            schema = SchemaEventDispatcher.Dispatch(@event, schema);
        }

        protected void On(SchemaUnpublished @event)
        {
            schema = SchemaEventDispatcher.Dispatch(@event, schema);
        }

        protected void On(SchemaDeleted @event)
        {
            isDeleted = true;
        }

        public SchemaDomainObject AddField(AddField command)
        {
            Guard.Valid(command, nameof(command), () => $"Cannot add field to schema {Id}");

            VerifyCreatedAndNotDeleted();

            RaiseEvent(SimpleMapper.Map(command, new FieldAdded { FieldId = new NamedId<long>(++totalFields, command.Name) }));

            return this;
        }

        public SchemaDomainObject UpdateField(UpdateField command)
        {
            Guard.Valid(command, nameof(command), () => $"Cannot update schema '{Id}'");

            VerifyCreatedAndNotDeleted();

            RaiseEvent(command, SimpleMapper.Map(command, new FieldUpdated()));

            return this;
        }

        public SchemaDomainObject Create(CreateSchema command)
        {
            Guard.Valid(command, nameof(command), () => "Cannot create schema");

            VerifyNotCreated();

            RaiseEvent(SimpleMapper.Map(command, new SchemaCreated { SchemaId = new NamedId<Guid>(Id, command.Name) }));

            return this;
        }

        public SchemaDomainObject Update(UpdateSchema command)
        {
            Guard.Valid(command, nameof(command), () => $"Cannot update schema '{Id}'");

            VerifyCreatedAndNotDeleted();

            RaiseEvent(SimpleMapper.Map(command, new SchemaUpdated()));

            return this;
        }

        public SchemaDomainObject HideField(HideField command)
        {
            Guard.NotNull(command, nameof(command));

            VerifyCreatedAndNotDeleted();
            
            RaiseEvent(command, new FieldHidden());

            return this;
        }

        public SchemaDomainObject ShowField(ShowField command)
        {
            Guard.NotNull(command, nameof(command));

            VerifyCreatedAndNotDeleted();
            
            RaiseEvent(command, new FieldShown());

            return this;
        }

        public SchemaDomainObject DisableField(DisableField command)
        {
            Guard.NotNull(command, nameof(command));

            VerifyCreatedAndNotDeleted();

            RaiseEvent(command, new FieldDisabled());

            return this;
        }

        public SchemaDomainObject EnableField(EnableField command)
        {
            Guard.NotNull(command, nameof(command));

            VerifyCreatedAndNotDeleted();
            
            RaiseEvent(command, new FieldEnabled());

            return this;
        }

        public SchemaDomainObject DeleteField(DeleteField command)
        {
            Guard.NotNull(command, nameof(command));

            VerifyCreatedAndNotDeleted();
            
            RaiseEvent(command, new FieldDeleted());

            return this;
        }

        public SchemaDomainObject Publish(PublishSchema command)
        {
            Guard.NotNull(command, nameof(command));

            VerifyCreatedAndNotDeleted();

            RaiseEvent(SimpleMapper.Map(command, new SchemaPublished()));

            return this;
        }

        public SchemaDomainObject Unpublish(UnpublishSchema command)
        {
            Guard.NotNull(command, nameof(command));

            VerifyCreatedAndNotDeleted();

            RaiseEvent(SimpleMapper.Map(command, new SchemaUnpublished()));

            return this;
        }

        public SchemaDomainObject Delete(DeleteSchema command)
        {
            VerifyCreatedAndNotDeleted();

            RaiseEvent(SimpleMapper.Map(command, new SchemaDeleted()));

            return this;
        }

        protected void RaiseEvent(FieldCommand fieldCommand, FieldEvent @event)
        {
            SimpleMapper.Map(fieldCommand, @event);

            if (schema.Fields.TryGetValue(fieldCommand.FieldId, out Field field))
            {
                @event.FieldId = new NamedId<long>(field.Id, field.Name);
            }
            else
            {
                throw new DomainObjectNotFoundException(fieldCommand.FieldId.ToString(), "Fields", typeof(Field));
            }

            RaiseEvent(@event);
        }

        private void VerifyNotCreated()
        {
            if (schema != null)
            {
                throw new DomainException("Schema has already been created.");
            }
        }

        private void VerifyCreatedAndNotDeleted()
        {
            if (isDeleted || schema == null)
            {
                throw new DomainException("Schema has already been deleted or not created yet.");
            }
        }

        protected override void DispatchEvent(Envelope<IEvent> @event)
        {
            this.DispatchAction(@event.Payload);
        }
    }
}