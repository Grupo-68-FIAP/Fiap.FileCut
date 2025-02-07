using Fiap.FileCut.Core.Objects;
using Xunit;

namespace Fiap.FileCut.Core.UnitTests
{
    public class ProcessingOptionsUnitTests
    {
        [Fact]
        public void ProcessingOptions_Should_Have_Default_Values()
        {
            // Arrange & Act
            var options = new ProcessingOptions();

            // Assert
            Assert.Equal("processing", options.WorkingDirectory);
            Assert.Equal(20, options.FrameIntervalSeconds);
            Assert.Equal(1920, options.FrameWidth);
            Assert.Equal(1080, options.FrameHeight);
        }

        [Fact]
        public void ProcessingOptions_Should_Allow_Setting_Properties()
        {
            // Arrange
            var options = new ProcessingOptions();

            // Act
            options.WorkingDirectory = "custom_directory";
            options.FrameIntervalSeconds = 10;
            options.FrameWidth = 1280;
            options.FrameHeight = 720;

            // Assert
            Assert.Equal("custom_directory", options.WorkingDirectory);
            Assert.Equal(10, options.FrameIntervalSeconds);
            Assert.Equal(1280, options.FrameWidth);
            Assert.Equal(720, options.FrameHeight);
        }
    }
}