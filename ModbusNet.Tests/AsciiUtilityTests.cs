using System;
using Xunit;

namespace ModbusNet.Utils.Tests
{
    public class AsciiUtilityTests
    {
        #region ToAsciiBytes Tests

        public class ToAsciiBytesTests
        {
            [Fact]
            public void ToAsciiBytes_WithValidInput_ReturnsCorrectAsciiBytes()
            {
                // Arrange
                byte[] input = { 0xAB, 0xCD, 0xEF };

                // Act
                byte[] result = AsciiUtility.ToAsciiBytes(input);

                // Assert
                byte[] expected = { (byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F' };
                Assert.Equal(expected, result);
            }

            [Fact]
            public void ToAsciiBytes_WithSingleByte_ReturnsTwoAsciiBytes()
            {
                // Arrange
                byte[] input = { 0x1F };

                // Act
                byte[] result = AsciiUtility.ToAsciiBytes(input);

                // Assert
                Assert.Equal(2, result.Length);
                Assert.Equal((byte)'1', result[0]);
                Assert.Equal((byte)'F', result[1]);
            }

            [Fact]
            public void ToAsciiBytes_WithZeroByte_ReturnsZeroAsciiBytes()
            {
                // Arrange
                byte[] input = { 0x00 };

                // Act
                byte[] result = AsciiUtility.ToAsciiBytes(input);

                // Assert
                byte[] expected = { (byte)'0', (byte)'0' };
                Assert.Equal(expected, result);
            }

            [Fact]
            public void ToAsciiBytes_WithMaxByte_ReturnsFFAsciiBytes()
            {
                // Arrange
                byte[] input = { 0xFF };

                // Act
                byte[] result = AsciiUtility.ToAsciiBytes(input);

                // Assert
                byte[] expected = { (byte)'F', (byte)'F' };
                Assert.Equal(expected, result);
            }

            [Fact]
            public void ToAsciiBytes_WithEmptyArray_ReturnsEmptyArray()
            {
                // Arrange
                byte[] input = Array.Empty<byte>();

                // Act
                byte[] result = AsciiUtility.ToAsciiBytes(input);

                // Assert
                Assert.Empty(result);
            }

            [Fact]
            public void ToAsciiBytes_WithMultipleBytes_ReturnsCorrectLength()
            {
                // Arrange
                byte[] input = { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF };

                // Act
                byte[] result = AsciiUtility.ToAsciiBytes(input);

                // Assert
                Assert.Equal(input.Length * 2, result.Length);
            }

            [Fact]
            public void ToAsciiBytes_WithNumericBytes_ReturnsCorrectAscii()
            {
                // Arrange
                byte[] input = { 0x12, 0x34 };

                // Act
                byte[] result = AsciiUtility.ToAsciiBytes(input);

                // Assert
                byte[] expected = { (byte)'1', (byte)'2', (byte)'3', (byte)'4' };
                Assert.Equal(expected, result);
            }
        }

        #endregion

        #region FromAsciiBytes Tests

        public class FromAsciiBytesTests
        {
            [Fact]
            public void FromAsciiBytes_WithValidInput_ReturnsCorrectBytes()
            {
                // Arrange
                byte[] asciiBytes = { (byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F' };

                // Act
                byte[] result = AsciiUtility.FromAsciiBytes(asciiBytes);

                // Assert
                byte[] expected = { 0xAB, 0xCD, 0xEF };
                Assert.Equal(expected, result);
            }

            [Fact]
            public void FromAsciiBytes_WithNumericChars_ReturnsCorrectBytes()
            {
                // Arrange
                byte[] asciiBytes = { (byte)'1', (byte)'2', (byte)'3', (byte)'4' };

                // Act
                byte[] result = AsciiUtility.FromAsciiBytes(asciiBytes);

                // Assert
                byte[] expected = { 0x12, 0x34 };
                Assert.Equal(expected, result);
            }

            [Fact]
            public void FromAsciiBytes_WithMixedChars_ReturnsCorrectBytes()
            {
                // Arrange
                byte[] asciiBytes = { (byte)'1', (byte)'A', (byte)'F', (byte)'2', (byte)'0', (byte)'0' };

                // Act
                byte[] result = AsciiUtility.FromAsciiBytes(asciiBytes);

                // Assert
                byte[] expected = { 0x1A, 0xF2, 0x00 };
                Assert.Equal(expected, result);
            }

            [Fact]
            public void FromAsciiBytes_WithOddLength_ThrowsArgumentException()
            {
                // Arrange
                byte[] asciiBytes = { (byte)'A', (byte)'B', (byte)'C' }; // Odd length

                // Act & Assert
                Assert.Throws<ArgumentException>(() => AsciiUtility.FromAsciiBytes(asciiBytes));
            }

            [Fact]
            public void FromAsciiBytes_WithEmptyArray_ReturnsEmptyArray()
            {
                // Arrange
                byte[] asciiBytes = Array.Empty<byte>();

                // Act
                byte[] result = AsciiUtility.FromAsciiBytes(asciiBytes);

                // Assert
                Assert.Empty(result);
            }

            [Fact]
            public void FromAsciiBytes_WithInvalidHexChar_ThrowsArgumentException()
            {
                // Arrange
                byte[] asciiBytes = { (byte)'G', (byte)'1' }; // 'G' is not a valid hex char

                // Act & Assert
                var exception = Assert.Throws<ArgumentException>(() => AsciiUtility.FromAsciiBytes(asciiBytes));
                Assert.Contains("Invalid hex character", exception.Message);
            }

            [Fact]
            public void FromAsciiBytes_WithLowerCaseChar_ThrowsArgumentException()
            {
                // Arrange
                byte[] asciiBytes = { (byte)'a', (byte)'b' }; // 'a' is lowercase

                // Act & Assert
                var exception = Assert.Throws<ArgumentException>(() => AsciiUtility.FromAsciiBytes(asciiBytes));
                Assert.Contains("Invalid hex character", exception.Message);
            }

            [Fact]
            public void FromAsciiBytes_WithNonHexChar_ThrowsArgumentException()
            {
                // Arrange
                byte[] asciiBytes = { (byte)'$', (byte)'1' }; // '$' is not a valid hex char

                // Act & Assert
                Assert.Throws<ArgumentException>(() => AsciiUtility.FromAsciiBytes(asciiBytes));
            }

            [Fact]
            public void FromAsciiBytes_WithSpecialChar_ThrowsArgumentException()
            {
                // Arrange
                byte[] asciiBytes = { (byte)'-', (byte)'1' }; // '-' is not a valid hex char

                // Act & Assert
                Assert.Throws<ArgumentException>(() => AsciiUtility.FromAsciiBytes(asciiBytes));
            }
        }

        #endregion

        #region RoundTrip Tests

        public class RoundTripTests
        {
            [Theory]
            [InlineData(new byte[] { 0x00 })]
            [InlineData(new byte[] { 0xFF })]
            [InlineData(new byte[] { 0x12, 0x34 })]
            [InlineData(new byte[] { 0xAB, 0xCD, 0xEF })]
            [InlineData(new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89 })]
            [InlineData(new byte[] { 0x10, 0x20, 0x30, 0x40, 0x50 })]
            public void RoundTrip_WithVariousInputs_ReturnsOriginalBytes(byte[] original)
            {
                // Act
                byte[] asciiBytes = AsciiUtility.ToAsciiBytes(original);
                byte[] result = AsciiUtility.FromAsciiBytes(asciiBytes);

                // Assert
                Assert.Equal(original, result);
            }

            [Fact]
            public void RoundTrip_WithAllPossibleNibbles_ReturnsOriginalBytes()
            {
                // Arrange - test all possible nibble combinations
                byte[] original = new byte[16];
                for (int i = 0; i < 16; i++)
                {
                    original[i] = (byte)((i << 4) | i); // 0x00, 0x11, 0x22, ..., 0xFF
                }

                // Act
                byte[] asciiBytes = AsciiUtility.ToAsciiBytes(original);
                byte[] result = AsciiUtility.FromAsciiBytes(asciiBytes);

                // Assert
                Assert.Equal(original, result);
            }

            [Fact]
            public void RoundTrip_WithLargeArray_ReturnsOriginalBytes()
            {
                // Arrange
                byte[] original = new byte[100];
                for (int i = 0; i < original.Length; i++)
                {
                    original[i] = (byte)((i * 17) % 256); // Pattern that uses various hex digits
                }

                // Act
                byte[] asciiBytes = AsciiUtility.ToAsciiBytes(original);
                byte[] result = AsciiUtility.FromAsciiBytes(asciiBytes);

                // Assert
                Assert.Equal(original, result);
            }
        }

        #endregion

        #region Edge Cases

        public class EdgeCasesTests
        {
            [Fact]
            public void FromAsciiBytes_WithNullInput_ThrowsArgumentNullException()
            {
                // Arrange
                byte[] asciiBytes = null;

                // Act & Assert
                Assert.Throws<ArgumentNullException>(() => AsciiUtility.FromAsciiBytes(asciiBytes));
            }

            [Fact]
            public void ToAsciiBytes_WithNullInput_ThrowsArgumentNullException()
            {
                // Arrange
                byte[] bytes = null;

                // Act & Assert
                Assert.Throws<ArgumentNullException>(() => AsciiUtility.ToAsciiBytes(bytes));
            }
        }

        #endregion
    }
}