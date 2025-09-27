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
            1.Should().Be(1);
        }
    }
}