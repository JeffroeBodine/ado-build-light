using NUnit.Framework;
using ADOBuildLight.Services;
using System.Device.Gpio;
using FluentAssertions;

namespace ADOBuildLight.Tests.ServiceTests
{
    [TestFixture]
    public class RealGpioControllerAdapterTests
    {
        [Test]
        public void Can_Instantiate_RealGpioControllerAdapter()
        {
            // This test will only pass on supported platforms (like Raspberry Pi)
            // On Windows, it will throw PlatformNotSupportedException
            if (IsGpioSupported())
            {
                var adapter = new RealGpioControllerAdapter();
                adapter.Should().NotBeNull();
                adapter.Dispose();
            }
            else
            {
                FluentActions.Invoking(() => new RealGpioControllerAdapter())
                    .Should().Throw<PlatformNotSupportedException>();
            }
        }

        [Test]
        public void OpenPin_ThrowsOnUnsupportedPlatform()
        {
            if (!IsGpioSupported())
            {
                FluentActions.Invoking(() => 
                {
                    var adapter = new RealGpioControllerAdapter();
                    adapter.OpenPin(27, PinMode.Output);
                }).Should().Throw<PlatformNotSupportedException>();
            }
            else
            {
                // On supported platforms, verify it doesn't throw
                var adapter = new RealGpioControllerAdapter();
                FluentActions.Invoking(() => adapter.OpenPin(27, PinMode.Output))
                    .Should().NotThrow();
                adapter.Dispose();
            }
        }

        [Test]
        public void Write_ThrowsOnUnsupportedPlatform()
        {
            if (!IsGpioSupported())
            {
                FluentActions.Invoking(() => 
                {
                    var adapter = new RealGpioControllerAdapter();
                    adapter.Write(27, PinValue.High);
                }).Should().Throw<PlatformNotSupportedException>();
            }
            else
            {
                // On supported platforms, verify it doesn't throw
                var adapter = new RealGpioControllerAdapter();
                adapter.OpenPin(27, PinMode.Output);
                FluentActions.Invoking(() => adapter.Write(27, PinValue.High))
                    .Should().NotThrow();
                adapter.Dispose();
            }
        }

        [Test]
        public void Dispose_DoesNotThrow()
        {
            if (IsGpioSupported())
            {
                var adapter = new RealGpioControllerAdapter();
                FluentActions.Invoking(() => adapter.Dispose()).Should().NotThrow();
            }
            else
            {
                // Even on unsupported platforms, Dispose should not throw if constructor threw
                FluentActions.Invoking(() => 
                {
                    try
                    {
                        var adapter = new RealGpioControllerAdapter();
                        adapter.Dispose();
                    }
                    catch (PlatformNotSupportedException)
                    {
                        // Expected on unsupported platforms - constructor throws, so Dispose never gets called
                    }
                }).Should().NotThrow();
            }
        }

        [Test]
        public void OpenPin_SupportsMultiplePinModes()
        {
            if (IsGpioSupported())
            {
                var adapter = new RealGpioControllerAdapter();
                
                // Test different pin modes
                FluentActions.Invoking(() => adapter.OpenPin(18, PinMode.Output)).Should().NotThrow();
                FluentActions.Invoking(() => adapter.OpenPin(19, PinMode.Input)).Should().NotThrow();
                FluentActions.Invoking(() => adapter.OpenPin(20, PinMode.InputPullUp)).Should().NotThrow();
                
                adapter.Dispose();
            }
        }

        [Test]
        public void Write_SupportsMultiplePinValues()
        {
            if (IsGpioSupported())
            {
                var adapter = new RealGpioControllerAdapter();
                adapter.OpenPin(21, PinMode.Output);
                
                // Test different pin values
                FluentActions.Invoking(() => adapter.Write(21, PinValue.High)).Should().NotThrow();
                FluentActions.Invoking(() => adapter.Write(21, PinValue.Low)).Should().NotThrow();
                
                adapter.Dispose();
            }
        }

        [Test]
        public void MultipleOperationsSequence_WorksCorrectly()
        {
            if (IsGpioSupported())
            {
                var adapter = new RealGpioControllerAdapter();
                
                // Test a sequence of operations
                FluentActions.Invoking(() => 
                {
                    adapter.OpenPin(22, PinMode.Output);
                    adapter.Write(22, PinValue.High);
                    adapter.Write(22, PinValue.Low);
                    adapter.OpenPin(23, PinMode.Output);
                    adapter.Write(23, PinValue.High);
                }).Should().NotThrow();
                
                adapter.Dispose();
            }
        }

        [Test]
        public void Dispose_CanBeCalledMultipleTimes()
        {
            if (IsGpioSupported())
            {
                var adapter = new RealGpioControllerAdapter();
                adapter.OpenPin(24, PinMode.Output);
                
                // Dispose should be safe to call multiple times
                FluentActions.Invoking(() => 
                {
                    adapter.Dispose();
                    adapter.Dispose(); // Second call should not throw
                }).Should().NotThrow();
            }
        }

        private static bool IsGpioSupported()
        {
            // Check if we're on a platform that supports GPIO
            // This is a simple heuristic - in practice, you might want more sophisticated detection
            return Environment.OSVersion.Platform == PlatformID.Unix && File.Exists("/proc/device-tree/model");
        }
    }
}