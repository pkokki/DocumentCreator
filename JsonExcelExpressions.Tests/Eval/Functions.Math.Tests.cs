using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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
            AssertExpression("=SUM(42,NA())", "#VALUE!");
            AssertExpression("=SUM(1.2,1.02,1.002)", "3,222");
            AssertExpression("=SUM(-5,-3)", "-8");
        }

        [Fact]
        public void CEILING()
        {
            AssertExpression("=CEILING(4, 2)", "4");
            AssertExpression("=CEILING(2.5, 1)", "3");
            AssertExpression("=CEILING(-2.5, -2)", "-4");
            AssertExpression("=CEILING(-2.5, 2)", "-2");
            AssertExpression("=CEILING(1.5, 0.1)", "1,5");
            AssertExpression("=CEILING(-5.5, 2)", "-4");
            AssertExpression("=CEILING(0.234, 0.01)", "0,24");
            AssertExpression("=CEILING(1723, 1000)", "2000");
            AssertExpression("=CEILING(12345, 10000)", "20000");
            AssertExpression("=CEILING(1234, 0)", "0");
        }

        [Fact]
        public void CEILING_MATH()
        {
            AssertExpression("=CEILING.MATH(24.3, 5)", "25");
            AssertExpression("=CEILING.MATH(6.7)", "7");
            AssertExpression("=CEILING.MATH(-8.1, 2)", "-8");
            AssertExpression("=CEILING.MATH(8.1, 2)", "10");
            AssertExpression("=CEILING.MATH(-5.5, 2, -1)", "-6");
            AssertExpression("=CEILING.MATH(-5.5, 2, 0)", "-4");
            AssertExpression("=CEILING.MATH(9.99, 10, 0)", "10");

            AssertExpression("=CEILING.MATH(5.2)", "6");
            AssertExpression("=CEILING.MATH(5.2,2)", "6");
            AssertExpression("=CEILING.MATH(5.2,-2)", "6");
            AssertExpression("=CEILING.MATH(5.2,2,0)", "6");
            AssertExpression("=CEILING.MATH(5.2,2,1)", "6");
            AssertExpression("=CEILING.MATH(5.2,-2,0)", "6");
            AssertExpression("=CEILING.MATH(5.2,-2,1)", "6");
            AssertExpression("=CEILING.MATH(-5.2)", "-5");
            AssertExpression("=CEILING.MATH(-5.2,2)", "-4");
            AssertExpression("=CEILING.MATH(-5.2,-2)", "-4");
            AssertExpression("=CEILING.MATH(-5.2,2,0)", "-4");
            AssertExpression("=CEILING.MATH(-5.2,2,1)", "-6");
            AssertExpression("=CEILING.MATH(-5.2,-2,0)", "-4");
            AssertExpression("=CEILING.MATH(-5.2,-2,1)", "-6");
        }

        [Fact]
        public void CEILING_PRECISE()
        {
            AssertExpression("=CEILING.PRECISE(4.3)", "5");
            AssertExpression("=CEILING.PRECISE(-4.3)", "-4");
            AssertExpression("=CEILING.PRECISE(4.3)", "5");
            AssertExpression("=CEILING.PRECISE(4.3, 2)", "6");
            AssertExpression("=CEILING.PRECISE(4.3, -2)", "6");
            AssertExpression("=CEILING.PRECISE(-4.3, 2)", "-4");
            AssertExpression("=CEILING.PRECISE(-4.3, 10)", "0");
        }

        [Fact]
        public void FLOOR()
        {
            AssertExpression("=FLOOR(3.7,2)", "2");
            AssertExpression("=FLOOR(-2.5,-2)", "-2");
            AssertExpression("=FLOOR(2.5,-2)", "#VALUE!");
            AssertExpression("=FLOOR(-2.5,2)", "-4");
            AssertExpression("=FLOOR(2.5,2)", "2");
            AssertExpression("=FLOOR(1.58,0.1)", "1,5");
            AssertExpression("=FLOOR(-1.58,0.1)", "-1,6");
            AssertExpression("=FLOOR(0.234,0.01)", "0,23");
            AssertExpression("=FLOOR(2,0)", "#VALUE!");
        }
        [Fact]
        public void FLOOR_MATH()
        {
            AssertExpression("=FLOOR.MATH(24.3,5)", "20");
            AssertExpression("=FLOOR.MATH(6.7)", "6");
            AssertExpression("=FLOOR.MATH(-8.1,2)", "-10");
            AssertExpression("=FLOOR.MATH(-5.5,2,-1)", "-4");
            AssertExpression("=FLOOR.MATH(5.2)", "5");
            AssertExpression("=FLOOR.MATH(5.2,2)", "4");
            AssertExpression("=FLOOR.MATH(5.2,-2)", "4");
            AssertExpression("=FLOOR.MATH(5.2,2,0)", "4");
            AssertExpression("=FLOOR.MATH(5.2,2,1)", "4");
            AssertExpression("=FLOOR.MATH(5.2,-2,0)", "4");
            AssertExpression("=FLOOR.MATH(5.2,-2,1)", "4");
            AssertExpression("=FLOOR.MATH(-5.2)", "-6");
            AssertExpression("=FLOOR.MATH(-5.2,2)", "-6");
            AssertExpression("=FLOOR.MATH(-5.2,-2)", "-6");
            AssertExpression("=FLOOR.MATH(-5.2,2,0)", "-6");
            AssertExpression("=FLOOR.MATH(-5.2,2,1)", "-4");
            AssertExpression("=FLOOR.MATH(-5.2,-2,0)", "-6");
            AssertExpression("=FLOOR.MATH(-5.2,-2,1)", "-4");
        }
        [Fact]
        public void FLOOR_PRECISE()
        {
            AssertExpression("=CEILING.PRECISE(5.2)", "6");
            AssertExpression("=CEILING.PRECISE(5.2,2)", "6");
            AssertExpression("=CEILING.PRECISE(5.2,-2)", "6");
            AssertExpression("=CEILING.PRECISE(-5.2)", "-5");
            AssertExpression("=CEILING.PRECISE(-5.2,2)", "-4");
            AssertExpression("=CEILING.PRECISE(-5.2,-2)", "-4");
            AssertExpression("=CEILING.PRECISE(0,-2)", "0");
            AssertExpression("=CEILING.PRECISE(4,0)", "0");
            AssertExpression("=CEILING.PRECISE(0,0)", "0");
        }

        [Fact]
        public void ROUND()
        {
            AssertExpression("=ROUND(0, 0)", "0");
            AssertExpression("=ROUND(10, 0)", "10");
            AssertExpression("=ROUND(10.001, 2.9)", "10");
            AssertExpression("=ROUND(9.99, 2)", "9,99");
            AssertExpression("=ROUND(9.999, 2)", "10");
            AssertExpression("=ROUND(-10, 0)", "-10");
            AssertExpression("=ROUND(-10, 2)", "-10");
            AssertExpression("=ROUND(-9.99, 2)", "-9,99");
            AssertExpression("=ROUND(-9.999, 2)", "-10");
            AssertExpression("=ROUND(-9.001, 2)", "-9");
            AssertExpression("=ROUND(-9.5, 2)", "-9,5");
            AssertExpression("=ROUND(-9.5, 0)", "-10");
            AssertExpression("=ROUND(9.5, 0)", "10");
            AssertExpression("=ROUND(2.15, 1)", "2,2");
            AssertExpression("=ROUND(2.149, 1)", "2,1");
            AssertExpression("=ROUND(-1.475, 2)", "-1,48");
            AssertExpression("=ROUND(21.5, -1)", "20");
            AssertExpression("=ROUND(626.3, -3)", "1000");
            AssertExpression("=ROUND(1.98, -1)", "0");
            AssertExpression("=ROUND(-50.55, -2)", "-100");
        }
        [Fact]
        public void ROUNDDOWN()
        {
            AssertExpression("=ROUNDDOWN(0, 0)", "0");
            AssertExpression("=ROUNDDOWN(10, 0)", "10");
            AssertExpression("=ROUNDDOWN(10.001, 2.9)", "10");
            AssertExpression("=ROUNDDOWN(9.99, 2)", "9,99");
            AssertExpression("=ROUNDDOWN(9.999, 2)", "9,99");
            AssertExpression("=ROUNDDOWN(-10, 0)", "-10");
            AssertExpression("=ROUNDDOWN(-10, 2)", "-10");
            AssertExpression("=ROUNDDOWN(-9.99, 2)", "-9,99");
            AssertExpression("=ROUNDDOWN(-9.999, 2)", "-9,99");
            AssertExpression("=ROUNDDOWN(-9.001, 2)", "-9");
            AssertExpression("=ROUNDDOWN(-9.5, 2)", "-9,5");
            AssertExpression("=ROUNDDOWN(-9.5, 0)", "-9");
            AssertExpression("=ROUNDDOWN(9.5, 0)", "9");
            AssertExpression("=ROUNDDOWN(2.15, 1)", "2,1");
            AssertExpression("=ROUNDDOWN(2.149, 1)", "2,1");
            AssertExpression("=ROUNDDOWN(-1.475, 2)", "-1,47");
            AssertExpression("=ROUNDDOWN(21.5, -1)", "20");
            AssertExpression("=ROUNDDOWN(626.3, -3)", "0");
            AssertExpression("=ROUNDDOWN(1.98, -1)", "0");
            AssertExpression("=ROUNDDOWN(-50.55, -2)", "0");
        }
        [Fact]
        public void ROUNDUP()
        {
            AssertExpression("=ROUNDUP(0, 0)", "0");
            AssertExpression("=ROUNDUP(10, 0)", "10");
            AssertExpression("=ROUNDUP(10.001, 2.9)", "10,01");
            AssertExpression("=ROUNDUP(9.99, 2)", "9,99");
            AssertExpression("=ROUNDUP(9.999, 2)", "10");
            AssertExpression("=ROUNDUP(-10, 0)", "-10");
            AssertExpression("=ROUNDUP(-10, 2)", "-10");
            AssertExpression("=ROUNDUP(-9.99, 2)", "-9,99");
            AssertExpression("=ROUNDUP(-9.999, 2)", "-10");
            AssertExpression("=ROUNDUP(-9.001, 2)", "-9,01");
            AssertExpression("=ROUNDUP(-9.5, 2)", "-9,5");
            AssertExpression("=ROUNDUP(-9.5, 0)", "-10");
            AssertExpression("=ROUNDUP(9.5, 0)", "10");
            AssertExpression("=ROUNDUP(2.15, 1)", "2,2");
            AssertExpression("=ROUNDUP(2.149, 1)", "2,2");
            AssertExpression("=ROUNDUP(-1.475, 2)", "-1,48");
            AssertExpression("=ROUNDUP(21.5, -1)", "30");
            AssertExpression("=ROUNDUP(626.3, -3)", "1000");
            AssertExpression("=ROUNDUP(1.98, -1)", "10");
            AssertExpression("=ROUNDUP(-50.55, -2)", "-100");
        }

        [Fact]
        public void TRUNC()
        {
            AssertExpression("=TRUNC(8.9)", "8");
            AssertExpression("=TRUNC(-8.9)", "-8");
            AssertExpression("=TRUNC(0.45)", "0");
            AssertExpression("=TRUNC(0.55)", "0");
            AssertExpression("=TRUNC(-0.55)", "0");
            AssertExpression("=TRUNC(0.55, 0)", "0");
            AssertExpression("=TRUNC(-0.55, 0)", "0");
            AssertExpression("=TRUNC(555, -1)", "550");
            AssertExpression("=TRUNC(555, 1)", "555");
            AssertExpression("=TRUNC(0.55, 1)", "0,5");
            AssertExpression("=TRUNC(-0.55, 1)", "-0,5");
        }

        [Fact]
        public void EXP()
        {
            AssertExpression("=EXP(0)", "1");
            AssertExpression("=EXP(1)", "2,718281828");
            AssertExpression("=EXP(-1.2)", "0,3011942119");
            AssertExpression("=EXP(1.234)", "3,434941861");
        }

        [Fact]
        public void SIGN()
        {
            AssertExpression("=SIGN(0)", "0");
            AssertExpression("=SIGN(13)", "1");
            AssertExpression("=SIGN(-21.2)", "-1");
            AssertExpression("=SIGN(12.234)", "1");
        }

        [Fact]
        public void INT()
        {
            AssertExpression("=INT(0)", "0");
            AssertExpression("=INT(1.99)", "1");
            AssertExpression("=INT(-1.99)", "-2");
            AssertExpression("=INT(-1.5)", "-2");
            AssertExpression("=INT(1.5)", "1");
            AssertExpression("=INT(TRUE)", "1");
            AssertExpression("=INT(\"10,123\")", "10");
        }

        [Fact]
        public void LN()
        {
            AssertExpression("=LN(0)", "#VALUE!");
            AssertExpression("=LN(1)", "0");
            AssertExpression("=LN(-1.2)", "#VALUE!");
            AssertExpression("=LN(1.234)", "0,2102609255");
        }

        [Fact]
        public void LOG()
        {
            AssertExpression("=LOG(0, 2)", "#VALUE!");
            AssertExpression("=LOG(1, 2)", "0");
            AssertExpression("=LOG(-1.2, 2)", "#VALUE!");
            AssertExpression("=LOG(1.234, 2)", "0,3033423945");
            AssertExpression("=LOG(5)", "0,6989700043");
            AssertExpression("=LOG(0.599999)", "-0,2218494734");
            AssertExpression("=LOG(111111111.9999)", "8,045757494");
            AssertExpression("=LOG(1.234, 3.2)", "0,1807684126");
        }

        [Fact]
        public void LOG10()
        {
            AssertExpression("=LOG10(0)", "#VALUE!");
            AssertExpression("=LOG10(1)", "0");
            AssertExpression("=LOG10(-1,2)", "#VALUE!");
            AssertExpression("=LOG10(1.234)", "0,0913151597");
        }

        [Fact]
        public void MOD()
        {
            AssertExpression("=MOD(0, 5)", "0");
            AssertExpression("=MOD(5, 0)", "#VALUE!");
            AssertExpression("=MOD(5, 2)", "1");
            AssertExpression("=MOD(10, 1.345)", "0,585");
            AssertExpression("=MOD(23.456, 2.432)", "1,568");
            AssertExpression("=MOD(-23.456, -2.432)", "-1,568");
            AssertExpression("=MOD(23.456, -2.432)", "-0,864");
            AssertExpression("=MOD(-23.456, 2.432)", "0,864");
        }

        [Fact]
        public void POWER()
        {
            AssertExpression("=POWER(0, 2)", "0");
            AssertExpression("=POWER(1, 2)", "1");
            AssertExpression("=POWER(-1.2, 2)", "1,44");
            AssertExpression("=POWER(1.234, 2)", "1,522756");
            AssertExpression("=POWER(5, -2.34)", "0,0231424956");
            AssertExpression("=POWER(0.599999, -1)", "1,666669444");
            AssertExpression("=POWER(111111111.9999, 1.02)", "160943204,4");
            AssertExpression("=POWER(1.234, 3.2)", "1,959785369");
        }

        [Fact]
        public void PRODUCT()
        {
            AssertExpression("=PRODUCT(5,15,30)", "2250");
            AssertExpression("=PRODUCT({5;15;30}, 2)", "4500");
            AssertExpression("=PRODUCT({5,15,30}, 2)", "4500");
        }

        [Fact]
        public void RAND()
        {
            for (var i = 0; i < 30; i++)
                AssertExpression("=RAND()", v => double.Parse(v, culture) >= 0 && double.Parse(v, culture) < 1);
        }

        [Fact]
        public void RANDBETWEEN()
        {
            AssertExpression("=RANDBETWEEN(0, 0)", "0");
            AssertExpression("=RANDBETWEEN(1, 1)", "1");
            AssertExpression("=RANDBETWEEN(10, 5)", "#VALUE!");

            var ints = new List<double>();
            for (var i = 0; i < 30; i++)
                AssertExpression("=RANDBETWEEN(1, 5)", v => { ints.Add(double.Parse(v, culture)); return true; });
            Assert.True(ints.All(x => x == 1 || x == 2 || x == 3 || x == 4), "unexpected integer values");
            Assert.True(ints.Contains(1), "missing 1");
            Assert.True(ints.Contains(2), "missing 2");
            Assert.True(ints.Contains(3), "missing 3");
            Assert.True(ints.Contains(4), "missing 4");

            AssertExpression("=RANDBETWEEN(1.34, 5.3)", v => double.Parse(v, culture) >= 2 && double.Parse(v, culture) < 5);
            var randoms = new List<double>();
            for (var i = 0; i < 30; i++)
                AssertExpression("=RANDBETWEEN(-1.9, 1.9)", v => { randoms.Add(double.Parse(v, culture)); return true; });
            Assert.True(randoms.All(x => x == -1 || x ==0 || x == 1), "unexpected random values");
            Assert.True(randoms.Contains(-1), "missing -1");
            Assert.True(randoms.Contains(0), "missing 0");
            Assert.True(randoms.Contains(1), "missing 1");
        }

    }
}
