using NUnit.Framework;
using ADOBuildLight.Services;
using System.Device.Gpio;
using FluentAssertions;
using Moq;
using ADOBuildLight.Interfaces;

namespace ADOBuildLight.Tests.ServiceTests
{
    [TestFixture]
    public class GpioControllerWrapperTests
    {
        [Test]
        public void Can_Instantiate_GpioControllerWrapper_WithMock()
        {
            var mockController = new Mock<IGpioControllerInternal>();
            var wrapper = new GpioControllerWrapper(mockController.Object);
            wrapper.Should().NotBeNull();
        }

        [Test]
        public void OpenPin_CallsUnderlyingController()
        {
            var mockController = new Mock<IGpioControllerInternal>();
            var wrapper = new GpioControllerWrapper(mockController.Object);
            wrapper.OpenPin(27, PinMode.Output);
            mockController.Verify(c => c.OpenPin(27, PinMode.Output), Times.Once);
        }

        [Test]
        public void Write_CallsUnderlyingController()
        {
            var mockController = new Mock<IGpioControllerInternal>();
            var wrapper = new GpioControllerWrapper(mockController.Object);
            wrapper.Write(27, PinValue.High);
            mockController.Verify(c => c.Write(27, PinValue.High), Times.Once);
        }

        [Test]
        public void Dispose_CallsUnderlyingController()
        {
            var mockController = new Mock<IGpioControllerInternal>();
            var wrapper = new GpioControllerWrapper(mockController.Object);
            wrapper.Dispose();
            mockController.Verify(c => c.Dispose(), Times.Once);
        }
    }
}
