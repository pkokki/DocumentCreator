using Xunit;
using Xunit.Abstractions;

namespace JsonExcelExpressions
{
    public class Math : BaseTest
    {
        public Math(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void ABS()
        {
            AssertExpression("=ABS(2.5)", "2,5");
            AssertExpression("=ABS(0)", "0");
            AssertExpression("=ABS(-2.5)", "2,5");
            AssertExpression("=ABS(-TRUE)", "1");
            AssertExpression("=ABS(-TIME(23,59,59))", "0,9999884259");
            AssertExpression("=ABS(\"-12,2\")", "12,2");
            AssertExpression("=ABS(\"abc\")", "#VALUE!");
        }

        [Fact]
        public void ACOS()
        {
            AssertExpression("=ACOS(0)", "1,570796327");
            AssertExpression("=ACOS(-1)", "3,141592654");
            AssertExpression("=ACOS(1)", "0");
            AssertExpression("=ACOS(1.8)", "#VALUE!");
            AssertExpression("=ACOS(-1.5)", "#VALUE!");
        }

        [Fact]
        public void ACOSH()
        {
            AssertExpression("=ACOSH(-1)", "#VALUE!");
            AssertExpression("=ACOSH(0)", "#VALUE!");
            AssertExpression("=ACOSH(1)", "0");
            AssertExpression("=ACOSH(1111)", "7,706162768");
            AssertExpression("=ACOSH(1111111.111)", "14,61401825");
        }

        [Fact]
        public void ASIN()
        {
            AssertExpression("=ASIN(-1)", "-1,570796327");
            AssertExpression("=ASIN(0)", "0");
            AssertExpression("=ASIN(1)", "1,570796327");
            AssertExpression("=ASIN(-1.1)", "#VALUE!");
            AssertExpression("=ASIN(1.1)", "#VALUE!");
        }

        [Fact]
        public void ASINH()
        {
            AssertExpression("=ASINH(-1)", "-0,881373587");
            AssertExpression("=ASINH(0)", "0");
            AssertExpression("=ASINH(1)", "0,881373587");
            AssertExpression("=ASINH(1111)", "7,706163173");
            AssertExpression("=ASINH(1111111.111)", "14,61401825");
        }

        [Fact]
        public void ACOT()
        {
            AssertExpression("=ACOT(0)", "1,570796327");
            AssertExpression("=ACOT(-1)", "2,35619449");
            AssertExpression("=ACOT(-2)", "2,677945045");
            AssertExpression("=ACOT(1)", "0,7853981634");
            AssertExpression("=ACOT(2)", "0,463647609");
        }

        [Fact]
        public void ACOTH()
        {
            AssertExpression("=ACOTH(0)", "#VALUE!");
            AssertExpression("=ACOTH(-1)", "#VALUE!");
            AssertExpression("=ACOTH(-2)", "-0,5493061443");
            AssertExpression("=ACOTH(1)", "#VALUE!");
            AssertExpression("=ACOTH(2)", "0,5493061443");
            AssertExpression("=ACOTH(1112)", "0,000899280818");
            AssertExpression("=ACOTH(3)", "0,3465735903");
            AssertExpression("=ACOTH(-PI())", "-0,329765315");
            AssertExpression("=ACOTH(PI())", "0,329765315");
        }

        [Fact]
        public void ATAN()
        {
            AssertExpression("=ATAN(-444)", "-1,568544078");
            AssertExpression("=ATAN(0)", "0");
            AssertExpression("=ATAN(1234)", "1,569985954");
            AssertExpression("=ATAN(PI()/2)", "1,003884822");
            AssertExpression("=ATAN(PI())", "1,262627256");
            AssertExpression("=ATAN(-PI()/3)", "-0,8084487926");
        }

        [Fact]
        public void ATAN2()
        {
            AssertExpression("=ATAN2(-2, -2)", "-2,35619449");
            AssertExpression("=ATAN2(-1, 0)", "3,141592654");
            AssertExpression("=ATAN2(0, 0)", "#VALUE!");
            AssertExpression("=ATAN2(0, 1)", "1,570796327");
            AssertExpression("=ATAN2(1, 2)", "1,107148718");
        }

        [Fact]
        public void ATANH()
        {
            AssertExpression("=ATANH(0)", "0");
            AssertExpression("=ATANH(0.233)", "0,2373593509");
            AssertExpression("=ATANH(-0.99999)", "-6,103033823");
            AssertExpression("=ATANH(0.99999)", "6,103033823");
            AssertExpression("=ATANH(-1)", "#VALUE!");
            AssertExpression("=ATANH(1)", "#VALUE!");
        }

        [Fact]
        public void COS()
        {
            AssertExpression("=COS(0)", "1");
            AssertExpression("=COS(1)", "0,5403023059");
            AssertExpression("=COS(2)", "-0,4161468365");
            AssertExpression("=COS(3)", "-0,9899924966");
            AssertExpression("=COS(4)", "-0,6536436209");
            AssertExpression("=COS(-1)", "0,5403023059");
        }

        [Fact]
        public void COSH()
        {
            AssertExpression("=COSH(0)", "1");
            AssertExpression("=COSH(1)", "1,543080635");
            AssertExpression("=COSH(3)", "10,067662");
            AssertExpression("=COSH(51)", "7,046745412E+21");
            AssertExpression("=COSH(0.0001)", "1,000000005");
            AssertExpression("=COS(-12)", "0,8438539587");
            AssertExpression("=COSH(-1111111)", "#VALUE!");
            AssertExpression("=COSH(1111111)", "#VALUE!");
        }

        [Fact]
        public void COT()
        {
            AssertExpression("=COT(0)", "#VALUE!");
            AssertExpression("=COT(-1)", "-0,6420926159");
            AssertExpression("=COT(1)", "0,6420926159");
            AssertExpression("=COT(21)", "-0,6546651155");
            AssertExpression("=COT(0.0001)", "9999,999967");
        }

        [Fact]
        public void COTH()
        {
            AssertExpression("=COTH(0)", "#VALUE!");
            AssertExpression("=COTH(1)", "1,313035285");
            AssertExpression("=COTH(-10)", "-1,000000004");
            AssertExpression("=COTH(11)", "1,000000001");
            AssertExpression("=COTH(15)", "1");
        }

        [Fact]
        public void CSC()
        {
            AssertExpression("=CSC(0)", "#VALUE!");
            AssertExpression("=CSC(1)", "1,188395106");
            AssertExpression("=CSC(-10)", "1,838163961");
            AssertExpression("=CSC(11)", "-1,000009794");
            AssertExpression("=CSC(15)", "1,537780562");
        }

        [Fact]
        public void CSCH()
        {
            AssertExpression("=CSCH(0)", "#VALUE!");
            AssertExpression("=CSCH(1)", "0,8509181282");
            AssertExpression("=CSCH(-10)", "-9,079985971E-05");
            AssertExpression("=CSCH(11)", "3,340340159E-05");
            AssertExpression("=CSCH(15)", "6,11804641E-07");
        }

        [Fact]
        public void SEC()
        {
            AssertExpression("=SEC(0)", "1");
            AssertExpression("=SEC(1)", "1,850815718");
            AssertExpression("=SEC(-10)", "-1,191793507");
            AssertExpression("=SEC(11)", "225,9530593");
            AssertExpression("=SEC(15)", "-1,316330013");
        }
        [Fact]
        public void SECH()
        {
            AssertExpression("=SECH(0)", "1");
            AssertExpression("=SECH(1)", "0,6480542737");
            AssertExpression("=SECH(-10)", "9,079985934E-05");
            AssertExpression("=SECH(11)", "3,340340157E-05");
            AssertExpression("=SECH(15)", "6,11804641E-07");
        }
        [Fact]
        public void SIN()
        {
            AssertExpression("=SIN(0)", "0");
            AssertExpression("=SIN(1)", "0,8414709848");
            AssertExpression("=SIN(-10)", "0,5440211109");
            AssertExpression("=SIN(11)", "-0,9999902066");
            AssertExpression("=SIN(15)", "0,6502878402");
        }
        [Fact]
        public void SINH()
        {
            AssertExpression("=SINH(0)", "0");
            AssertExpression("=SINH(1)", "1,175201194");
            AssertExpression("=SINH(-10)", "-11013,23287");
            AssertExpression("=SINH(11)", "29937,07085");
            AssertExpression("=SINH(15)", "1634508,686");
        }
        [Fact]
        public void TAN()
        {
            AssertExpression("=TAN(0)", "0");
            AssertExpression("=TAN(1)", "1,557407725");
            AssertExpression("=TAN(-10)", "-0,6483608275");
            AssertExpression("=TAN(11)", "-225,9508465");
        }
        [Fact]
        public void TANH()
        {
            AssertExpression("=TANH(0)", "0");
            AssertExpression("=TANH(1)", "0,761594156");
            AssertExpression("=TANH(-10)", "-0,9999999959");
            AssertExpression("=TANH(11)", "0,9999999994");
            AssertExpression("=TANH(PI())", "0,9962720762");
            AssertExpression("=TANH(PI()/2)", "0,9171523357");
        }
        [Fact]
        public void SUM()
        {
            /* Joins several text items into one text item */
            AssertExpression("=SUM(42)", "42");
            AssertExpression("=SUM(1,1)", "2");
            AssertExpression("=SUM(1,2,3,4,5,6)", "21");
            AssertExpression("=SUM(42,FALSE)", "42");
            AssertExpression("=SUM(42,TRUE)", "43");
            AssertExpression("=SUM(42,\"A\")", "#VALUE!");
            AssertExpression("=SUM(42,\"1\")", "43");
            AssertExpression("=SUM(42,NA())", "#N/A");
            AssertExpression("=SUM(1.2,1.02,1.002)", "3,222");
            AssertExpression("=SUM(-5,-3)", "-8");

        }
    }
}
