using WordleWinForms;
using WordleWinForms.Enums;

namespace WordleWinFormsTests;

public class SquareTests
{
    [Fact]
    public void DefaultConstructor_ShouldSetDefaultValues()
    {
        // Arrange & Act
        var square = new Square();

        // Assert
        Assert.Equal(Status.Inactive, square.Status);
        Assert.Equal(' ', square.Letter);
    }

    [Fact]
    public void ParameterizedConstructor_ShouldSetValues()
    {
        // Arrange
        var expectedStatus = Status.Correct;
        var expectedLetter = 'A';

        // Act
        var square = new Square(expectedStatus, expectedLetter);

        // Assert
        Assert.Equal(expectedStatus, square.Status);
        Assert.Equal(expectedLetter, square.Letter);
    }

    [Fact]
    public void SetStatus_ShouldUpdateStatus()
    {
        // Arrange
        var square = new Square();
        var newStatus = Status.Correct;

        // Act
        square.Status = newStatus;

        // Assert
        Assert.Equal(newStatus, square.Status);
    }

    [Fact]
    public void SetLetter_ShouldUpdateLetter()
    {
        // Arrange
        var square = new Square();
        var newLetter = 'B';

        // Act
        square.Letter = newLetter;

        // Assert
        Assert.Equal(newLetter, square.Letter);
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var square = new Square(Status.Correct, 'C');

        // Act
        var result = square.ToString();

        // Assert
        Assert.Equal("Square: C, Status: Correct", result);
    }
}
