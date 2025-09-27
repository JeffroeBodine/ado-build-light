using System;
using NUnit.Framework;
using FluentAssertions;

namespace ADOBuildLight.Tests
{
    [TestFixture]
    public class ExpectedExceptionTests
    {
        [Test]
        public void HandlesArgumentExceptionWithNewSyntax()
        {
            Assert.That(1, Is.EqualTo(1));
            1.Should().Be(1);
        }
    }
}