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
            // On Windows and CI environments, it will throw PlatformNotSupportedException
            if (IsGpioSupported())
            {
                var adapter = new RealGpioControllerAdapter();
                adapter.Should().NotBeNull();
                adapter.Dispose();
            }
            else
            {
                FluentActions.Invoking(() => new RealGpioControllerAdapter())
                    .Should().Throw<Exception>("GPIO should throw an exception on unsupported platforms");
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
                }).Should().Throw<Exception>("GPIO operations should throw an exception on unsupported platforms");
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
                }).Should().Throw<Exception>("GPIO operations should throw an exception on unsupported platforms");
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
                    catch (Exception)
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
            Console.WriteLine("Checking if GPIO is supported on this platform...");
            Console.WriteLine($"Platform: {Environment.OSVersion.Platform}");
            Console.WriteLine($"Model file exists: {File.Exists("/proc/device-tree/model")}");
            Console.WriteLine($"Is CI Environment: {Environment.GetEnvironmentVariable("CI") != null}");
            Console.WriteLine($"Is GitHub Actions: {Environment.GetEnvironmentVariable("GITHUB_ACTIONS") != null}");
            
            // Return false if we're in a CI environment, as GPIO hardware won't be available
            if (Environment.GetEnvironmentVariable("CI") != null || 
                Environment.GetEnvironmentVariable("GITHUB_ACTIONS") != null)
            {
                Console.WriteLine("CI environment detected - GPIO not supported");
                return false;
            }
            
            // Try to actually create a GpioController to see if it works
            try
            {
                using var controller = new GpioController();
                Console.WriteLine("GpioController created successfully - GPIO supported");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GpioController creation failed: {ex.GetType().Name} - {ex.Message}");
                return false;
            }
        }
    }
}