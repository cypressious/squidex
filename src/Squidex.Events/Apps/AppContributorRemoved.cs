﻿// ==========================================================================
//  AppContributorRemoved.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using Squidex.Infrastructure;

namespace Squidex.Events.Apps
{
    [TypeName("AppContributorRemovedEvent")]
    public class AppContributorRemoved : AppEvent
    {
        public string ContributorId { get; set; }
    }
}
