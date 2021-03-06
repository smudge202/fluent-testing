﻿using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace FluentGwt.Tests
{
    public partial class GivenTests
    {
        private Foo? _foo;

        private sealed class Foo
        {
            public int? Bar { get; set; }
            public int? Baz { get; set; }
        }
        
        private Func<Given<GivenTests>> SelfGiven => 
            () => this.Given(x => x._foo = new Foo());
        
        private Action SelfAssertion =>
            () => _foo.Should().NotBeNull();

        private int ExpectedBar { get; } = Randomizer.Int();
        private int ExpectedBaz { get; } = Randomizer.Int();
        
        [Fact]
        public void CanInstantiateBlankSelfGiven()
        {
            Action act = () => this.Given();
            act.Should().NotThrow();
        }
        
        [Fact]
        public void CanInstantiateBlankTargetedGiven()
        {
            Action act = () => new Foo().Given();
            act.Should().NotThrow();
        }
        
        [Fact]
        public void CanInstantiateGivenWithSelfDeferredState()
        {
            Action act = () => SelfGiven();
            act.Should().NotThrow();
        }

        [Fact]
        public async Task CanExecuteGivenWithSelfDeferredState()
        {
            var given = SelfGiven();
            await given.Execute();
            SelfAssertion();
        }

        [Fact]
        public void CanAddDeferredStateToSelfGiven()
        {
            Action act = () => SelfGiven().Given(x => x._foo!.Bar = ExpectedBar);
            act.Should().NotThrow();
        }

        [Fact]
        public async Task CanExecuteAddedDeferredStateToSelfGiven()
        {
            var given = SelfGiven().Given(x => x._foo!.Bar = ExpectedBar);
            await given.Execute();
            _foo!.Bar.Should().Be(ExpectedBar);
        }

        private Func<Given<Foo>> TargetedGiven =>
            () => (_foo = new Foo()).Given(x => x.Bar = ExpectedBar);

        private Action TargetedAssertion =>
            () => _foo!.Bar.Should().Be(ExpectedBar);

        [Fact]
        public void CanInstantiateGivenWithTargetedDeferredState()
        {
            Action act = () => TargetedGiven();
            act.Should().NotThrow();
        }

        [Fact]
        public async Task CanExecuteGivenWithTargetedDeferredState()
        {
            var given = TargetedGiven();
            await given.Execute();
            TargetedAssertion();
        }

        [Fact]
        public void CanAddDeferredStateToTargetedGiven()
        {
            Action act = () => TargetedGiven().Given(x => x.Baz = ExpectedBaz);
            act.Should().NotThrow();
        }

        [Fact]
        public async Task CanExecuteAddedDeferredStateToTargetedGiven()
        {
            var given = TargetedGiven().Given(x => x.Baz = ExpectedBaz);
            await given.Execute();
            _foo!.Baz.Should().Be(ExpectedBaz);
        }
    }
}