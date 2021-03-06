﻿namespace Construktion.Tests.Registry
{
    using System;
    using Shouldly;

    public class ConstructUsingTests
    {
        public void should_construct_using_supplied_function()
        {
            var registry =
                new ConstruktionRegistry().ConstructPropertyUsing(p => p.Name.Equals("Credits"),
                    () => new Random().Next(1, 5));
            var construktion = new Construktion().Apply(registry);

            var foo = construktion.Construct<Foo>();

            foo.Credits.ShouldBeInRange(1, 4);
        }
    }

    public class Foo
    {
        public int Credits { get; set; }
    }
}