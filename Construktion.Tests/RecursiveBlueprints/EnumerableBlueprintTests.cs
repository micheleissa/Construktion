﻿// ReSharper disable PossibleMultipleEnumeration

namespace Construktion.Tests.RecursiveBlueprints
{
    using System.Collections.Generic;
    using System.Linq;
    using Blueprints.Recursive;
    using Internal;
    using Shouldly;

    public class EnumerableBlueprintTests
    {
        public void should_match_enumerables_of_t()
        {
            var blueprint = new EnumerableBlueprint();

            var matches = blueprint.Matches(new ConstruktionContext(typeof(IEnumerable<>)));

            matches.ShouldBe(true);
        }

        public void should_build_simple_enumerable()
        {
            var blueprint = new EnumerableBlueprint();

            var result = (IEnumerable<int>)blueprint.Construct(new ConstruktionContext(typeof(IEnumerable<int>)),
                new DefaultConstruktionPipeline());

            result.Count().ShouldBe(3);
            result.ShouldAllBe(x => x != 0);
        }

        public void should_build_complex_enumerable()
        {
            var blueprint = new EnumerableBlueprint();

            var result = (IEnumerable<Foo>)blueprint.Construct(new ConstruktionContext(typeof(IEnumerable<Foo>)),
                new DefaultConstruktionPipeline());

            result.ShouldAllBe(x => !string.IsNullOrWhiteSpace(x.Bar));
            result.ShouldAllBe(x => x.Baz != 0);
        }

        public void should_build_nested_enumerables()
        {
            var blueprint = new EnumerableBlueprint();

            var result = (IEnumerable<Bar>)blueprint.Construct(new ConstruktionContext(typeof(IEnumerable<Bar>)),
                new DefaultConstruktionPipeline());

            result.ShouldAllBe(x => x != null);

            var foos = result.SelectMany(x => x.Foo);
            foos.ShouldAllBe(x => !string.IsNullOrWhiteSpace(x.Bar));
            foos.ShouldAllBe(x => x.Baz != 0);
        }

        public void should_build_a_list()
        {
            var blueprint = new EnumerableBlueprint();

            var result = (List<int>)blueprint.Construct(new ConstruktionContext(typeof(List<int>)),
                new DefaultConstruktionPipeline());

            result.ShouldNotBe(null);
            result.ShouldAllBe(x => x != default(int));
        }

        public void should_build_icollections()
        {
            var blueprint = new EnumerableBlueprint();

            var result = (ICollection<Foo>)blueprint.Construct(new ConstruktionContext(typeof(ICollection<Foo>)),
                new DefaultConstruktionPipeline());

            result.ShouldAllBe(x => !string.IsNullOrWhiteSpace(x.Bar));
            result.ShouldAllBe(x => x.Baz != 0);
        }

        public class Foo
        {
            public string Bar { get; set; }
            public int Baz { get; set; }
        }

        public class Bar
        {
            public IEnumerable<Foo> Foo { get; set; }
        }
    }
}