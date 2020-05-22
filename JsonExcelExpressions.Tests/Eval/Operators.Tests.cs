using Newtonsoft.Json.Linq;
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

            AssertExpression("=1+SUM(a+b)", "42", JObject.Parse("{a: [7, 11, 13], b: [ 2, 3, 5]}"));
            AssertExpression("=1+SUM(a+2)", "38", JObject.Parse("{a: [7, 11, 13], b: [ 2, 3, 5]}"));
            AssertExpression("=1+SUM(3+b)", "20", JObject.Parse("{a: [7, 11, 13], b: [ 2, 3, 5]}"));
            AssertExpression("=SUM(SUM(a+b)+b)", "133", JObject.Parse("{a: [7, 11, 13], b: [ 2, 3, 5]}"));
        }

        [Fact]
        public void MINUS()
        {
            AssertExpression("=1-1", "0");
            AssertExpression("=1.1-1.1", "0");
            AssertExpression("=2*SUM(a-b)", "42", JObject.Parse("{a: [7, 11, 13], b: [ 2, 3, 5]}"));
        }

        [Fact]
        public void TIMES()
        {
            AssertExpression("=2*4", "8");
            AssertExpression("=1.5*4.2", "6,3");
            AssertExpression("=SUM(a*b)", "112", JObject.Parse("{a: [7, 11, 13], b: [ 2, 3, 5]}"));
        }

        [Fact]
        public void DIV()
        {
            AssertExpression("=2/4", "0,5");
            AssertExpression("=1.5/4.2", "0,3571428571");
            AssertExpression("=2/0", "#DIV/0!");
            AssertExpression("=SUM(a/b)", "9", JObject.Parse("{a: [8, 6, 12], b: [ 2, 3, 4]}"));
            AssertExpression("=a/b", "['4','#DIV/0!','3']", JObject.Parse("{a: [8, 6, 12], b: [ 2, 0, 4]}"));
        }

        [Fact]
        public void NEGATE()
        {
            AssertExpression("=-1", "-1");
            AssertExpression("=-4.2", "-4,2");
            AssertExpression("=-a", "['-8','-6','-12']", JObject.Parse("{a: [8, 6, 12]}"));
        }

        [Fact]
        public void POW()
        {
            AssertExpression("=2^3", "8");
            AssertExpression("=3^2", "9");
            AssertExpression("=2^3.1", "8,5741877");
            AssertExpression("=SUM(a ^ b)", "68", JObject.Parse("{a: [5, 3, 2], b: [ 2, 3, 4]}"));
        }

        [Fact]
        public void CONCAT()
        {
            AssertExpression("=\"A\"&\"B\"", "AB");
            AssertExpression("=a & b", "['52','33','24']", JObject.Parse("{a: [5, 3, 2], b: [ 2, 3, 4]}"));
        }
    }
}
