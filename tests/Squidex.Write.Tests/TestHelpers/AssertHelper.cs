﻿// ==========================================================================
//  AssertHelper.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Squidex.Infrastructure.CQRS.Events;

namespace Squidex.Write.TestHelpers
{
    public static class AssertHelper
    {
        public static void ShouldHaveSameEvents(this IEnumerable<Envelope<IEvent>> events, params IEvent[] others)
        {
            var source = events.Select(x => x.Payload).ToArray();

            source.Should().HaveSameCount(others);

            for (var i = 0; i < source.Length; i++)
            {
                ((object)source[i]).ShouldBeEquivalentTo(others[i], o => o.IncludingAllDeclaredProperties());
            }
        }
    }
}
