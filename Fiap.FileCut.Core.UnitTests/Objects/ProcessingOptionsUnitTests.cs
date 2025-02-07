using Fiap.FileCut.Core.Objects;

namespace Fiap.FileCut.Core.UnitTests.Objects
{
    public class ProcessingOptionsUnitTests
    {
        [Fact]
        public void ProcessingOptions_Should_Have_Default_Values()
        {
            // Arrange & Act
            var options = new ProcessingOptions();

            // Assert
            Assert.Equal(20, options.FrameIntervalSeconds);
            Assert.Equal(1920, options.FrameWidth);
            Assert.Equal(1080, options.FrameHeight);
        }

        [Fact]
        public void ProcessingOptions_Should_Allow_Setting_Properties()
        {
            // Arrange
            var options = new ProcessingOptions
            {
                // Act
                FrameIntervalSeconds = 10,
                FrameWidth = 1280,
                FrameHeight = 720
            };

            // Assert
            Assert.Equal(10, options.FrameIntervalSeconds);
            Assert.Equal(1280, options.FrameWidth);
            Assert.Equal(720, options.FrameHeight);
        }
    }
}