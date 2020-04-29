using Xunit;
using Xunit.Abstractions;

namespace JsonExcelExpressions
{
    public class Operators : BaseTest
    {
        public Operators(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void PLUS()
        {
            AssertExpression("=1+1", "2");
            AssertExpression("=1.1+1.1", "2,2");
        }

        [Fact]
        public void MINUS()
        {
            AssertExpression("=1-1", "0");
            AssertExpression("=1.1-1.1", "0,0");
        }

        [Fact]
        public void TIMES()
        {
            AssertExpression("=2*4", "8");
            AssertExpression("=1.5*4.2", "6,30");

        }

        [Fact]
        public void DIV()
        {
            AssertExpression("=2/4", "0,5");
            AssertExpression("=1.5/4.2", "0,3571428571428571428571428571");
            AssertExpression("=2/0", "#DIV/0!");
        }

        [Fact]
        public void NEGATE()
        {
            AssertExpression("=-1", "-1");
            AssertExpression("=-4.2", "-4,2");
        }

        [Fact]
        public void POW()
        {
            AssertExpression("=2^3", "8");
            AssertExpression("=3^2", "9");
            AssertExpression("=2^3.1", "8,57418770029034");
        }

        [Fact]
        public void CONCAT()
        {
            AssertExpression("=\"A\"&\"B\"", "AB");
        }
    }
}
