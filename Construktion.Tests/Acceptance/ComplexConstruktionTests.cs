﻿// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable ArrangeTypeMemberModifiers

namespace Construktion.Tests.Acceptance
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Shouldly;

    public class ComplexConstruktionTests
    {
        private readonly Construktion construktion;

        public ComplexConstruktionTests()
        {
            construktion = new Construktion();
        }

        public void enums()
        {
            var result = construktion.Construct<TestResult>();

            result.ShouldBeOneOf(TestResult.Pass, TestResult.Fail);
        }

        public void should_build_classes()
        {
            var result = construktion.Construct<Child>();

            result.Name.ShouldNotBeNullOrEmpty();
            result.Age.ShouldNotBe(default(int));
        }

        public void should_build_nested_classes()
        {
            var result = construktion.Construct<Parent>();

            result.Name.ShouldNotBeNullOrEmpty();
            result.Age.ShouldNotBe(default(int));
            result.Child.Name.ShouldNotBeNullOrEmpty();
            result.Child.Age.ShouldNotBe(default(int));
        }

        public void should_prefix_properties_with_property_name()
        {
            var child = construktion.Construct<Child>();

            child.Name.ShouldStartWith("Name-");
        }

        public void should_hardcode_properties()
        {
            var result = construktion.Construct<Parent>(x =>
            {
                x.Name = "Foo";
                x.Child.Name = "Lil Foo";
            });

            result.Name.ShouldBe("Foo");
            result.Age.ShouldNotBe(default(int));
            result.Child.Name.ShouldBe("Lil Foo");
            result.Child.Age.ShouldNotBe(default(int));
        }

        public void should_ignore_private_and_no_setters_by_default()
        {
            var result = construktion.Construct<Private>();

            result.PrivateName.ShouldBe(null);
            result.PrivateAge.ShouldBe(0);
            result.NoSetter.ShouldBe(null);
        }

        public void should_build_any_type_implementing_ienumerable()
        {
            var result = construktion.Construct<IReadOnlyCollection<Child>>();

            result.Count.ShouldBe(3);
            result.ShouldAllBe(x => !string.IsNullOrWhiteSpace(x.Name));
            result.ShouldAllBe(x => x.Age != 0);
        }

        public void should_build_arrays()
        {
            var result = construktion.Construct<string[]>();

            result.ShouldAllBe(x => !string.IsNullOrWhiteSpace(x));
        }

        public void should_build_dictionaries()
        {
            var result = construktion.Construct<Dictionary<string, int>>();

            result.ShouldNotBe(null);
            result.Count.ShouldBe(3);
            result.Keys.ShouldAllBe(x => !string.IsNullOrWhiteSpace(x));
            result.Values.ShouldAllBe(x => x != 0);
        }

        public void should_resolve_runtime_type()
        {
            var result = construktion.Construct(typeof(Child)) as Child;

            result.Name.ShouldNotBeNullOrWhiteSpace();
            result.Age.ShouldNotBe(0);
        }

        public void should_resolve_parameters_from_method()
        {
            var methodInfo = typeof(ComplexConstruktionTests).GetMethod(nameof(TestMethod),
                BindingFlags.NonPublic | BindingFlags.Instance);

            var values = methodInfo.GetParameters()
                                   .Select(pi => construktion.Construct(pi))
                                   .ToList();

            values.Count.ShouldBe(2);
            ((string)values[0]).ShouldNotBeNullOrWhiteSpace();
            ((int)values[1]).ShouldNotBe(0);
        }

        private void TestMethod(string name, int age) { }

        private class Private
        {
            public string PrivateName { get; private set; }
            public int PrivateAge { get; private set; }
            public string NoSetter { get; }
        }

        private class Child
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        private class Parent
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public Child Child { get; set; }
        }

        enum TestResult
        {
            Pass,
            Fail
        }
    }
}