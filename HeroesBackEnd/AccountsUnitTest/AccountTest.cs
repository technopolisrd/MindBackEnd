namespace AccountsUnitTest;

using Mind.Business.Log;

public class AccountTest
{

    [Fact]
    public void TestLoginUser()
    {
        //arrange
        double a = 2;
        double b = 3;
        double expected = 6;

        //act
        double result = Errors.AddforTestPorposes(a, b);

        //assert
        Assert.Equal(expected, result);

    }
}