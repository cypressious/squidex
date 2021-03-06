﻿// ==========================================================================
//  MongoAppRepository_EventHandling.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System.Threading.Tasks;
using Squidex.Events.Apps;
using Squidex.Infrastructure;
using Squidex.Infrastructure.CQRS.Events;
using Squidex.Infrastructure.Dispatching;
using Squidex.Infrastructure.Reflection;
using Squidex.Read.MongoDb.Utils;

namespace Squidex.Read.MongoDb.Apps
{
    public partial class MongoAppRepository
    {
        public string Name
        {
            get { return GetType().Name; }
        }

        public Task On(Envelope<IEvent> @event)
        {
            return this.DispatchActionAsync(@event.Payload, @event.Headers);
        }

        protected Task On(AppCreated @event, EnvelopeHeaders headers)
        {
            return Collection.CreateAsync(@event, headers, a =>
            {
                SimpleMapper.Map(@event, a);
            });
        }

        protected Task On(AppContributorRemoved @event, EnvelopeHeaders headers)
        {
            return Collection.UpdateAsync(@event, headers, a =>
            {
                a.Contributors.Remove(@event.ContributorId);
            });
        }

        protected Task On(AppClientAttached @event, EnvelopeHeaders headers)
        {
            return Collection.UpdateAsync(@event, headers, a =>
            {
                a.Clients[@event.Id] = SimpleMapper.Map(@event, new MongoAppClientEntity());
            });
        }

        protected Task On(AppClientRevoked @event, EnvelopeHeaders headers)
        {
            return Collection.UpdateAsync(@event, headers, a =>
            {
                a.Clients.Remove(@event.Id);
            });
        }

        protected Task On(AppClientRenamed @event, EnvelopeHeaders headers)
        {
            return Collection.UpdateAsync(@event, headers, a =>
            {
                a.Clients[@event.Id].Name = @event.Name;
            });
        }

        protected Task On(AppLanguageAdded @event, EnvelopeHeaders headers)
        {
            return Collection.UpdateAsync(@event, headers, a =>
            {
                a.Languages.Add(@event.Language.Iso2Code);
            });
        }

        protected Task On(AppLanguageRemoved @event, EnvelopeHeaders headers)
        {
            return Collection.UpdateAsync(@event, headers, a =>
            {
                a.Languages.Remove(@event.Language.Iso2Code);
            });
        }

        protected Task On(AppMasterLanguageSet @event, EnvelopeHeaders headers)
        {
            return Collection.UpdateAsync(@event, headers, a =>
            {
                a.MasterLanguage = @event.Language.Iso2Code;
            });
        }

        protected Task On(AppContributorAssigned @event, EnvelopeHeaders headers)
        {
            return Collection.UpdateAsync(@event, headers, a =>
            {
                var contributor = a.Contributors.GetOrAddNew(@event.ContributorId);

                SimpleMapper.Map(@event, contributor);
            });
        }
    }
}
