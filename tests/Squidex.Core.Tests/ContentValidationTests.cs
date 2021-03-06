﻿// ==========================================================================
//  ContentValidationTests.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Squidex.Core.Contents;
using Squidex.Core.Schemas;
using Squidex.Infrastructure;
using Xunit;

namespace Squidex.Core
{
    public class ContentValidationTests
    {
        private readonly HashSet<Language> languages = new HashSet<Language>(new[] { Language.DE, Language.EN });
        private readonly List<ValidationError> errors = new List<ValidationError>();
        private Schema schema = Schema.Create("my-name", new SchemaProperties());

        [Fact]
        public async Task Should_add_error_if_validating_data_with_unknown_field()
        {
            var data =
                new ContentData()
                    .AddField("unknown",
                        new ContentFieldData());

            await data.ValidateAsync(schema, languages, errors);

            errors.ShouldBeEquivalentTo(
                new List<ValidationError>
                {
                    new ValidationError("unknown is not a known field", "unknown")
                });
        }

        [Fact]
        public async Task Should_add_error_if_validating_data_with_invalid_field()
        {
            schema = schema.AddOrUpdateField(new NumberField(1, "my-field", new NumberFieldProperties { MaxValue = 100 }));

            var data =
                new ContentData()
                    .AddField("my-field",
                        new ContentFieldData()
                            .SetValue(1000));

            await data.ValidateAsync(schema, languages, errors);

            errors.ShouldBeEquivalentTo(
                new List<ValidationError>
                {
                    new ValidationError("my-field must be less than '100'", "my-field")
                });
        }

        [Fact]
        public async Task Should_add_error_non_localizable_data_field_contains_language()
        {
            schema = schema.AddOrUpdateField(new NumberField(1, "my-field", new NumberFieldProperties()));

            var data =
                new ContentData()
                    .AddField("my-field",
                        new ContentFieldData()
                            .AddValue("es", 1)
                            .AddValue("it", 1));

            await data.ValidateAsync(schema, languages, errors);

            errors.ShouldBeEquivalentTo(
                new List<ValidationError>
                {
                    new ValidationError("my-field can only contain a single entry for invariant language (iv)", "my-field")
                });
        }

        [Fact]
        public async Task Should_add_error_if_validating_data_with_invalid_localizable_field()
        {
            schema = schema.AddOrUpdateField(new NumberField(1, "my-field", new NumberFieldProperties { IsRequired = true, IsLocalizable = true }));

            var data =
                new ContentData();

            await data.ValidateAsync(schema, languages, errors);

            errors.ShouldBeEquivalentTo(
                new List<ValidationError>
                {
                    new ValidationError("my-field (de) is required", "my-field"),
                    new ValidationError("my-field (en) is required", "my-field")
                });
        }

        [Fact]
        public async Task Should_add_error_if_required_data_field_is_not_in_bag()
        {
            schema = schema.AddOrUpdateField(new NumberField(1, "my-field", new NumberFieldProperties { IsRequired = true }));

            var data =
                new ContentData();

            await data.ValidateAsync(schema, languages, errors);

            errors.ShouldBeEquivalentTo(
                new List<ValidationError>
                {
                    new ValidationError("my-field is required", "my-field")
                });
        }

        [Fact]
        public async Task Should_add_error_if_data_contains_invalid_language()
        {
            schema = schema.AddOrUpdateField(new NumberField(1, "my-field", new NumberFieldProperties { IsLocalizable = true }));

            var data =
                new ContentData()
                    .AddField("my-field",
                        new ContentFieldData()
                            .AddValue("de", 1)
                            .AddValue("xx", 1));

            await data.ValidateAsync(schema, languages, errors);

            errors.ShouldBeEquivalentTo(
                new List<ValidationError>
                {
                    new ValidationError("my-field has an invalid language 'xx'", "my-field")
                });
        }

        [Fact]
        public async Task Should_add_error_if_data_contains_unsupported_language()
        {
            schema = schema.AddOrUpdateField(new NumberField(1, "my-field", new NumberFieldProperties { IsLocalizable = true }));

            var data =
                new ContentData()
                    .AddField("my-field",
                        new ContentFieldData()
                            .AddValue("es", 1)
                            .AddValue("it", 1));

            await data.ValidateAsync(schema, languages, errors);

            errors.ShouldBeEquivalentTo(
                new List<ValidationError>
                {
                    new ValidationError("my-field has an unsupported language 'es'", "my-field"),
                    new ValidationError("my-field has an unsupported language 'it'", "my-field")
                });
        }

        [Fact]
        public async Task Should_add_error_if_validating_partial_data_with_unknown_field()
        {
            var data =
                new ContentData()
                    .AddField("unknown",
                        new ContentFieldData());

            await data.ValidatePartialAsync(schema, languages, errors);

            errors.ShouldBeEquivalentTo(
                new List<ValidationError>
                {
                    new ValidationError("unknown is not a known field", "unknown")
                });
        }

        [Fact]
        public async Task Should_add_error_if_validating_partial_data_with_invalid_field()
        {
            schema = schema.AddOrUpdateField(new NumberField(1, "my-field", new NumberFieldProperties { MaxValue = 100 }));

            var data =
                new ContentData()
                    .AddField("my-field",
                        new ContentFieldData()
                            .SetValue(1000));

            await data.ValidatePartialAsync(schema, languages, errors);

            errors.ShouldBeEquivalentTo(
                new List<ValidationError>
                {
                    new ValidationError("my-field must be less than '100'", "my-field")
                });
        }

        [Fact]
        public async Task Should_add_error_non_localizable_partial_data_field_contains_language()
        {
            schema = schema.AddOrUpdateField(new NumberField(1, "my-field", new NumberFieldProperties()));

            var data =
                new ContentData()
                    .AddField("my-field",
                        new ContentFieldData()
                            .AddValue("es", 1)
                            .AddValue("it", 1));

            await data.ValidatePartialAsync(schema, languages, errors);

            errors.ShouldBeEquivalentTo(
                new List<ValidationError>
                {
                    new ValidationError("my-field can only contain a single entry for invariant language (iv)", "my-field")
                });
        }

        [Fact]
        public async Task Should_not_add_error_if_validating_partial_data_with_invalid_localizable_field()
        {
            schema = schema.AddOrUpdateField(new NumberField(1, "my-field", new NumberFieldProperties { IsRequired = true, IsLocalizable = true }));

            var data =
                new ContentData();

            await data.ValidatePartialAsync(schema, languages, errors);

            Assert.Equal(0, errors.Count);
        }

        [Fact]
        public async Task Should_not_add_error_if_required_partial_data_field_is_not_in_bag()
        {
            schema = schema.AddOrUpdateField(new NumberField(1, "my-field", new NumberFieldProperties { IsRequired = true }));

            var data =
                new ContentData();

            await data.ValidatePartialAsync(schema, languages, errors);

            Assert.Equal(0, errors.Count);
        }

        [Fact]
        public async Task Should_add_error_if_partial_data_contains_invalid_language()
        {
            schema = schema.AddOrUpdateField(new NumberField(1, "my-field", new NumberFieldProperties { IsLocalizable = true }));

            var data =
                new ContentData()
                    .AddField("my-field",
                        new ContentFieldData()
                            .AddValue("de", 1)
                            .AddValue("xx", 1));

            await data.ValidatePartialAsync(schema, languages, errors);

            errors.ShouldBeEquivalentTo(
                new List<ValidationError>
                {
                    new ValidationError("my-field has an invalid language 'xx'", "my-field")
                });
        }

        [Fact]
        public async Task Should_add_error_if_partial_data_contains_unsupported_language()
        {
            schema = schema.AddOrUpdateField(new NumberField(1, "my-field", new NumberFieldProperties { IsLocalizable = true }));

            var data =
                new ContentData()
                    .AddField("my-field",
                        new ContentFieldData()
                            .AddValue("es", 1)
                            .AddValue("it", 1));

            await data.ValidatePartialAsync(schema, languages, errors);

            errors.ShouldBeEquivalentTo(
                new List<ValidationError>
                {
                    new ValidationError("my-field has an unsupported language 'es'", "my-field"),
                    new ValidationError("my-field has an unsupported language 'it'", "my-field")
                });
        }
    }
}
