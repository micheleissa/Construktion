﻿namespace Construktion.Tests.Registry
{
    using System.Collections.Generic;
    using Blueprints.Simple;
    using Shouldly;

    public class BlueprintTests
    {
        private readonly ConstruktionRegistry registry;
        private readonly Construktion construktion;

        public BlueprintTests()
        {
            registry = new ConstruktionRegistry();
            construktion = new Construktion();
        }

        public void should_register_a_custom_blueprint()
        {
            registry.AddBlueprint(new StringOneBlueprint());

            var result = construktion.Apply(registry).Construct<string>();

            result.ShouldBe("StringOne");
        }

        public void should_register_via_generic_parameter()
        {
            registry.AddBlueprint<StringOneBlueprint>();

            var result = construktion.Apply(registry).Construct<string>();

            result.ShouldBe("StringOne");
        }

        public void blueprints_registered_first_are_chosen_first()
        {
            registry.AddBlueprint(new StringTwoBlueprint());
            registry.AddBlueprint(new StringOneBlueprint());

            var result = construktion.Apply(registry).Construct<string>();

            result.ShouldBe("StringTwo");
        }

        public void registries_registered_first_should_have_their_blueprints_used_first()
        {
            construktion
                .Apply(new StringTwoRegistry())
                .Apply(new StringOneRegistry());

            var result = construktion.Construct<string>();

            result.ShouldBe("StringTwo");
        }

        public void should_be_linq_enabled()
        {
            var reg = new ConstruktionRegistry(x => x.AddBlueprint<StringOneBlueprint>());

            var result = construktion.Apply(reg).Construct<string>();

            result.ShouldBe("StringOne");
        }

        public void should_add_multiple_blueprints()
        {
            var blueprints = new List<Blueprint>
            {
                new CustomPropertyValueBlueprint(x => x.PropertyType == typeof(string), () => "x"),
                new CustomPropertyValueBlueprint(x => x.PropertyType == typeof(int), () => 1)
            };

            var result = construktion.Apply(blueprints).Construct<Foo>();

            result.String.ShouldBe("x");
            result.Int.ShouldBe(1);
        }

        public void registry_should_add_multiple_blueprints()
        {
            var stringBlueprint = new CustomPropertyValueBlueprint(x => x.PropertyType == typeof(string), () => "x");
            var intBlueprint = new CustomPropertyValueBlueprint(x => x.PropertyType == typeof(int), () => 1);

            var result = construktion.Apply(x => x.AddBlueprints(new List<Blueprint>{stringBlueprint, intBlueprint })).Construct<Foo>();

            result.String.ShouldBe("x");
            result.Int.ShouldBe(1);
        }

        public class StringOneBlueprint : AbstractBlueprint<string>
        {
            public override string Construct(ConstruktionContext context, ConstruktionPipeline pipeline) => "StringOne";
        }

        public class StringTwoBlueprint : AbstractBlueprint<string>
        {
            public override string Construct(ConstruktionContext context, ConstruktionPipeline pipeline) => "StringTwo";
        }

        public class StringOneRegistry : ConstruktionRegistry
        {
            public StringOneRegistry()
            {
                AddBlueprint(new StringOneBlueprint());
            }
        }

        public class StringTwoRegistry : ConstruktionRegistry
        {
            public StringTwoRegistry()
            {
                AddBlueprint(new StringTwoBlueprint());
            }
        }

        public class Foo
        {
            public string String { get; set; }
            public int Int { get; set; }
        }
    }
}