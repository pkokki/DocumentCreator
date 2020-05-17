using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace JsonExcelExpressions
{
    public class Info : BaseTest
    {
        public Info(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void ISEVEN()
        {
            AssertExpression("=ISEVEN(0)", "TRUE");
            AssertExpression("=ISEVEN(1)", "FALSE");
            AssertExpression("=ISEVEN(2)", "TRUE");
            AssertExpression("=ISEVEN(-1)", "FALSE");
            AssertExpression("=ISEVEN(1,5)", "FALSE");
            AssertExpression("=ISEVEN(2,5)", "TRUE");
            AssertExpression("=ISEVEN(-2,9)", "TRUE");
            AssertExpression("=ISEVEN(\"10\")", "TRUE");
            AssertExpression("=ISEVEN(\"10A\")", "#VALUE!");
            AssertExpression("=ISEVEN(NA())", "#N/A");
            AssertExpression("=ISEVEN(DATE(2020,5,16))", "FALSE");
            AssertExpression("=ISEVEN(TRUE)", "#VALUE!");
            AssertExpression("=ISEVEN(FALSE)", "#VALUE!");
        }

        [Fact]
        public void ISBLANK()
        {
            AssertExpression("=ISBLANK(\"\")", "FALSE");
            AssertExpression("=ISBLANK(NA())", "FALSE");
            AssertExpression("=ISBLANK(D232)", "FALSE");
            AssertExpression("=ISBLANK(D233:D234)", "FALSE");
            AssertExpression("=ISBLANK(0)", "FALSE");
            AssertExpression("=ISBLANK(TRUE)", "FALSE");
            AssertExpression("=ISBLANK(NOW())", "FALSE");
        }

        [Fact]
        public void ISERR()
        {
            AssertExpression("=ISERR(\"\")", "FALSE");
            AssertExpression("=ISERR(NA())", "FALSE");
            AssertExpression("=ISERR(1/0)", "TRUE");
            AssertExpression("=ISERR(ISEVEN(TRUE))", "TRUE");
            AssertExpression("=ISERR(0)", "FALSE");
            AssertExpression("=ISERR(0)", "FALSE");
        }

        [Fact]
        public void ISERROR()
        {
            AssertExpression("=ISERROR(\"\")", "FALSE");
            AssertExpression("=ISERROR(NA())", "TRUE");
            AssertExpression("=ISERROR(1/0)", "TRUE");
            AssertExpression("=ISERROR(ISEVEN(TRUE))", "TRUE");
            AssertExpression("=ISERROR(0)", "FALSE");
            AssertExpression("=ISERROR(0)", "FALSE");
        }

        [Fact]
        public void ISLOGICAL()
        {
            //AssertExpression("=ISLOGICAL(TRUE)", "TRUE");
            //AssertExpression("=ISLOGICAL(FALSE)", "TRUE");
            //AssertExpression("=ISLOGICAL(1)", "FALSE");
            //AssertExpression("=ISLOGICAL(\"abc\")", "FALSE");
            //AssertExpression("=ISLOGICAL(1/0)", "FALSE");
            //AssertExpression("=ISLOGICAL(NA())", "FALSE");
            //AssertExpression("=ISLOGICAL(ISERR(123))", "TRUE");
            AssertExpression("=ISLOGICAL(10/\"a\")", "FALSE");
        }

        [Fact]
        public void ISNA()
        {
            AssertExpression("=ISNA(NA())", "TRUE");
            AssertExpression("=ISNA(1/0)", "FALSE");
            AssertExpression("=ISNA(1)", "FALSE");
            AssertExpression("=ISNA(FALSE)", "FALSE");
            AssertExpression("=ISNA(10/\"a\")", "FALSE");
        }

        [Fact]
        public void ISNUMBER()
        {
            AssertExpression("=ISNUMBER(1)", "TRUE");
            AssertExpression("=ISNUMBER(10.1)", "TRUE");
            AssertExpression("=ISNUMBER(\"10\")", "FALSE");
            AssertExpression("=ISNUMBER(TRUE)", "FALSE");
            AssertExpression("=ISNUMBER(NA())", "FALSE");
            AssertExpression("=ISNUMBER(1/0)", "FALSE");
            AssertExpression("=ISNUMBER(NOW())", "TRUE");
            AssertExpression("=ISNUMBER(TIME(10,10,10))", "TRUE");
        }

        [Fact]
        public void ISTEXT()
        {
            AssertExpression("=ISTEXT(\"\")", "TRUE");
            AssertExpression("=ISTEXT(\"10\")", "TRUE");
            AssertExpression("=ISTEXT(7 & \"A\")", "TRUE");
            AssertExpression("=ISTEXT(1)", "FALSE");
            AssertExpression("=ISTEXT(TRUE)", "FALSE");
            AssertExpression("=ISTEXT(NA())", "FALSE");
            AssertExpression("=ISTEXT(1/0)", "FALSE");
            AssertExpression("=ISTEXT(NOW())", "FALSE");
        }

        [Fact]
        public void ISNONTEXT()
        {
            AssertExpression("=ISNONTEXT(\"\")", "FALSE");
            AssertExpression("=ISNONTEXT(\"10\")", "FALSE");
            AssertExpression("=ISNONTEXT(7 & \"A\")", "FALSE");
            AssertExpression("=ISNONTEXT(1)", "TRUE");
            AssertExpression("=ISNONTEXT(TRUE)", "TRUE");
            AssertExpression("=ISNONTEXT(NA())", "TRUE");
            AssertExpression("=ISNONTEXT(1/0)", "TRUE");
            AssertExpression("=ISNONTEXT(NOW())", "TRUE");
        }

        [Fact]
        public void TYPE()
        {
            AssertExpression("=TYPE(1)", "1");
            AssertExpression("=TYPE(FALSE)", "4");
            AssertExpression("=TYPE(1.23)", "1");
            AssertExpression("=TYPE(NOW())", "1");
            AssertExpression("=TYPE(\"\")", "2");
            AssertExpression("=TYPE(NA())", "16");
            AssertExpression("=TYPE(1/0)", "16");
            AssertExpression("=TYPE(2+\"a\")", "16");
            //AssertExpression("=TYPE({1,2,3,4})", "64");
        }

        [Fact]
        public void N()
        {
            AssertExpression("=N(2)", "2");
            AssertExpression("=N(3.14)", "3,14");
            AssertExpression("=N(DATE(2020,5,16) + TIME(11,32,23))", "43967,48082");
            AssertExpression("=N(\"12/3/2020\")", "0");
            AssertExpression("=N(TRUE)", "1");
            AssertExpression("=N(FALSE)", "0");
            AssertExpression("=N(\"1\")", "0");
            AssertExpression("=N(NA())", "#N/A");
            AssertExpression("=N(1/0)", "#DIV/0!");
            AssertExpression("=N(\"\")", "0");
            AssertExpression("=N(\"10\")", "0"); 
            AssertExpression("=N(1+\"a\")", "#VALUE!");
        }
    }
}
