﻿using Construktion.Blueprints.Simple;
using Shouldly;
using Xunit;

namespace Construktion.Tests.SimpleBlueprints
{
    public class EnumBlueprintTests
    {
        [Fact]
        public void should_construct()
        {
            var blueprint = new EnumBlueprint();

            var result = (Gender)blueprint.Construct(new ConstruktionContext(typeof(Gender)), new DefaultConstruktionPipeline());

            result.ShouldBeOneOf(Gender.F, Gender.M);
        }

        public enum Gender
        {
            F,
            M
        }
    }
}