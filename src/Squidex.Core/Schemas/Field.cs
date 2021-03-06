// ==========================================================================
//  Field.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using Squidex.Core.Schemas.Validators;
using Squidex.Infrastructure;

// ReSharper disable InvertIf
// ReSharper disable ConvertIfStatementToReturnStatement
// ReSharper disable ConvertIfStatementToConditionalTernaryExpression

namespace Squidex.Core.Schemas
{
    public abstract class Field : CloneableBase
    {
        private readonly Lazy<List<IValidator>> validators;
        private readonly long id;
        private string name;
        private bool isDisabled;
        private bool isHidden;

        public long Id
        {
            get { return id; }
        }

        public string Name
        {
            get { return name; }
        }

        public bool IsHidden
        {
            get { return isHidden; }
        }

        public bool IsDisabled
        {
            get { return isDisabled; }
        }

        public IReadOnlyList<IValidator> Validators
        {
            get { return validators.Value; }
        }

        public abstract FieldProperties RawProperties { get; }

        protected Field(long id, string name)
        {
            Guard.ValidPropertyName(name, nameof(name));
            Guard.GreaterThan(id, 0, nameof(id));

            this.id = id;

            this.name = name;

            validators = new Lazy<List<IValidator>>(() => new List<IValidator>(CreateValidators()));
        }

        public abstract Field Update(FieldProperties newProperties);

        public abstract object ConvertValue(JToken value);

        public Field Hide()
        {
            return Clone<Field>(clone => clone.isHidden = true);
        }

        public Field Show()
        {
            return Clone<Field>(clone => clone.isHidden = false);
        }

        public Field Disable()
        {
            return Clone<Field>(clone => clone.isDisabled = true);
        }

        public Field Enable()
        {
            return Clone<Field>(clone => clone.isDisabled = false);
        }

        public Field Rename(string newName)
        {
            if (!newName.IsSlug())
            {
                var error = new ValidationError("Name must be a valid slug", "Name");

                throw new ValidationException($"Cannot rename the field '{name}' ({id})", error);
            }

            return Clone<Field>(clone => clone.name = newName);
        }

        public void AddToEdmType(EdmStructuredType edmType, IEnumerable<Language> languages, string schemaName, Func<EdmComplexType, EdmComplexType> typeResolver)
        {
            Guard.NotNull(edmType, nameof(edmType));
            Guard.NotNull(languages, nameof(languages));
            Guard.NotNull(typeResolver, nameof(typeResolver));

            if (!RawProperties.IsLocalizable)
            {
                languages = new[] { Language.Invariant };
            }

            var edmValueType = CreateEdmType();

            if (edmValueType == null)
            {
                return;
            }

            var languageType = typeResolver(new EdmComplexType("Squidex", $"{schemaName}{Name.ToPascalCase()}Property"));

            foreach (var language in languages)
            {
                languageType.AddStructuralProperty(language.Iso2Code, edmValueType);
            }

            edmType.AddStructuralProperty(Name, new EdmComplexTypeReference(languageType, false));
        }

        public void AddToJsonSchema(JsonSchema4 schema, IEnumerable<Language> languages, string schemaName, Func<string, JsonSchema4, JsonSchema4> schemaResolver)
        {
            Guard.NotNull(schema, nameof(schema));
            Guard.NotNull(languages, nameof(languages));
            Guard.NotNull(schemaResolver, nameof(schemaResolver));

            if (!RawProperties.IsLocalizable)
            {
                languages = new[] { Language.Invariant };
            }

            var languagesProperty = CreateProperty();
            var languagesObject = new JsonSchema4 { Type = JsonObjectType.Object, AllowAdditionalProperties = false };

            foreach (var language in languages)
            {
                var languageProperty = new JsonProperty { Description = language.EnglishName, IsRequired = RawProperties.IsRequired };

                PrepareJsonSchema(languageProperty, schemaResolver);

                languagesObject.Properties.Add(language.Iso2Code, languageProperty);
            }

            languagesProperty.SchemaReference = schemaResolver($"{schemaName}{Name.ToPascalCase()}Property", languagesObject);

            schema.Properties.Add(Name, languagesProperty);
        }

        public JsonProperty CreateProperty()
        {
            var jsonProperty = new JsonProperty { IsRequired = RawProperties.IsRequired, Type = JsonObjectType.Object };

            if (!string.IsNullOrWhiteSpace(RawProperties.Hints))
            {
                jsonProperty.Description = RawProperties.Hints;
            }
            else
            {
                jsonProperty.Description = Name;
            }

            if (!string.IsNullOrWhiteSpace(RawProperties.Hints))
            {
                jsonProperty.Description += $" ({RawProperties.Hints}).";
            }

            return jsonProperty;
        }

        protected abstract IEnumerable<IValidator> CreateValidators();

        protected abstract IEdmTypeReference CreateEdmType();

        protected abstract void PrepareJsonSchema(JsonProperty jsonProperty, Func<string, JsonSchema4, JsonSchema4> schemaResolver);
    }
}