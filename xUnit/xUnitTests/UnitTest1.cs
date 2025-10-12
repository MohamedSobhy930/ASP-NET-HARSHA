using FluentAssertions;

namespace xUnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            // arrange 
            MathOperations mathOperations = new MathOperations();
            int num1 = 3 , num2 = 4 ;
            int output = 7 ;
            // act 
            int result = mathOperations.Add(num1, num2);
            // assert
            result.Should().Be(output);
        }
    }
}