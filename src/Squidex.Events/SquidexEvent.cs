﻿// ==========================================================================
//  SquidexEventBase.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using Squidex.Infrastructure;
using Squidex.Infrastructure.CQRS.Events;

namespace Squidex.Events
{
    public abstract class SquidexEvent : IEvent
    {
        public RefToken Actor { get; set; }
    }
}
