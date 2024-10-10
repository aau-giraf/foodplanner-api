namespace testing;

public class TestExample
{
    // Assert.True() to test if a boolean expression is true
    // Assert.False() to test if a boolean expression is false
    // Assert.Equal() to test an expected value against actual value, this warks similarly to object.equals()
    // There are more Assert methods, which can be found here: https://gist.github.com/jonesandy/f622874e78d9d9f356896c4ac63c0ac3
    //                                                       & https://textbooks.cs.ksu.edu/cis400/1-object-orientation/04-testing/05-xunit-assertions/ 
    // But these are the most important and most test variation can be implemented using these.

    // Use [Fact] to test a single variation of inputs or changing expected values
    [Fact]
    public void Example1() {
        bool actual = true;

        Assert.True(actual);
    }

    // Use [Theory] to test multiple variations of input
    [Theory]
    [InlineData(true, "this will pass")]
    [InlineData(false, "this will fail")]
    public void Example2(bool expected, string input) {
        Assert.NotNull(input);
        var actual = input == "this will pass";

        Assert.Equal(expected, actual);
    }
}