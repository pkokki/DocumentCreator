using JsonExcelExpressions.Eval;
using JsonExcelExpressions.Lang;
using System;
using System.Globalization;
using Xunit;
using Xunit.Abstractions;

namespace JsonExcelExpressions
{
    public class Dates : BaseTest
    {
        public Dates(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void DateValue_ToDateSerial()
        {
            Assert.Null(ExcelValue.ToDateSerial(new DateTime(1821, 3, 25)));
            Assert.Null(ExcelValue.ToDateSerial(new DateTime(1899, 12, 31)));
            Assert.Equal(1, ExcelValue.ToDateSerial(new DateTime(1900, 1, 1)));
            Assert.Equal(2, ExcelValue.ToDateSerial(new DateTime(1900, 1, 2)));
            Assert.Equal(59, ExcelValue.ToDateSerial(new DateTime(1900, 2, 28)));
            Assert.Equal(61, ExcelValue.ToDateSerial(new DateTime(1900, 3, 1)));
            Assert.Equal(39448, ExcelValue.ToDateSerial(new DateTime(2008, 1, 1)));
            Assert.Equal(43967, ExcelValue.ToDateSerial(new DateTime(2020, 5, 16)));

            Assert.Null(ExcelValue.ToDateSerial(0, 0, 0, 0, 0, -1));
            Assert.Equal(0M, ExcelValue.ToDateSerial(0, 0, 0, 0, 0, 0));
            Assert.Equal(0.0000115741M, ExcelValue.ToDateSerial(0, 0, 0, 0, 0, 1));
            Assert.Equal(0.4668171296M, ExcelValue.ToDateSerial(0, 0, 0, 11, 12, 13));
            Assert.Equal(0.9999884259M, ExcelValue.ToDateSerial(0, 0, 0, 23, 59, 59));
        }

        [Fact]
        public void DateValue_FromDateSerial()
        {
            Assert.Null(ExcelValue.FromDateSerial(-1));
            Assert.Null(ExcelValue.FromDateSerial(0));
            Assert.Equal(new DateTime(1900, 1, 1, 6, 0, 0), ExcelValue.FromDateSerial(0.25M));
            Assert.Equal(new DateTime(1900, 1, 1, 12, 0, 0), ExcelValue.FromDateSerial(0.5M));
            Assert.Equal(new DateTime(1900, 1, 1, 18, 0, 0), ExcelValue.FromDateSerial(0.75M));
            Assert.Equal(new DateTime(1900, 1, 1), ExcelValue.FromDateSerial(1));
            Assert.Equal(new DateTime(1900, 1, 2), ExcelValue.FromDateSerial(2));
            Assert.Equal(new DateTime(1900, 2, 28), ExcelValue.FromDateSerial(59));
            Assert.Equal(new DateTime(1900, 3, 1), ExcelValue.FromDateSerial(61));
            Assert.Equal(new DateTime(2008, 1, 1), ExcelValue.FromDateSerial(39448));
            Assert.Equal(new DateTime(2020, 5, 16), ExcelValue.FromDateSerial(43967));
        }

        [Fact]
        public void DATE()
        {
            AssertExpression("=DATE(2020,4,18)", "18/4/2020");
            AssertExpression("=DATE(2020,4,18)+60", "17/6/2020");
            AssertExpression("=DATE(2020,4,18)-60", "18/2/2020");
            AssertExpression("=DATE(2020,4,18)-0.42", "17/4/2020");
            AssertExpression("=DATE(2020,4,18)+0.42", "18/4/2020");
            AssertExpression("=DATE(2020,4,18)*0.42", "10/7/1950");

            var enProcessor = new ExpressionEvaluator(CultureInfo.GetCultureInfo("en-US"));
            var result = enProcessor.Evaluate("=DATE(2020,4,18)");
            Assert.Null(result.Error);
            Assert.Equal("4/18/2020", result.Text);
        }

        [Fact]
        public void TIME()
        {
            AssertExpression("=TIME(11,6,43)", "11:06 πμ");
            AssertExpression("=TIME(23,6,43)", "11:06 μμ");
            AssertExpression("=TIME(11,6,43)+0.3", "6:18 μμ");
            AssertExpression("=TIME(11,6,43)-0.3", "3:54 πμ");
            AssertExpression("=TIME(11,6,43)*0.3", "3:20 πμ");
            AssertExpression("=TIME(11,6,43)*4.32", "12:00 πμ");
            AssertExpression("=TIME(11,6,43)+4.32", "6:47 μμ");

            var enProcessor = new ExpressionEvaluator(CultureInfo.GetCultureInfo("en-US"));
            var result = enProcessor.Evaluate("=TIME(11,6,43)");
            Assert.Null(result.Error);
            Assert.Equal("11:06 AM", result.Text);
        }

        [Fact]
        public void NOW()
        {
            AssertExpression("=NOW()", DateTime.Now.ToString("d", CultureInfo.GetCultureInfo("el-GR")));
            AssertExpression("=NOW()+123", (DateTime.Now.AddDays(123)).ToString("d", CultureInfo.GetCultureInfo("el-GR")));
        }

        [Fact]
        public void DAY()
        {
            AssertExpression("=DAY(1)", "1");
            AssertExpression("=DAY(39448)", "1");
            AssertExpression("=DAY(39448.999)", "1");
            AssertExpression("=DAY(0)", "0");
            AssertExpression("=DAY(-1)", "#VALUE!");
            AssertExpression("=DAY(\"18/4/2020\")", "18");
            AssertExpression("=DAY(\"18/4/2020 10:10:20\")", "18");
        }

        [Fact]
        public void MONTH()
        {
            AssertExpression("=MONTH(1)", "1");
            AssertExpression("=MONTH(0)", "1");
            AssertExpression("=MONTH(-1)", "#VALUE!");
            AssertExpression("=MONTH(39448)", "1");
            AssertExpression("=MONTH(39448.999)", "1");
            AssertExpression("=MONTH(\"18/4/2020\")", "4");
            AssertExpression("=MONTH(\"18/4/2020 10:10:20\")", "4");
        }

        [Fact]
        public void YEAR()
        {
            AssertExpression("=YEAR(1)", "1900");
            AssertExpression("=YEAR(0)", "1900");
            AssertExpression("=YEAR(-1)", "#VALUE!");
            AssertExpression("=YEAR(39448)", "2008");
            AssertExpression("=YEAR(40178.99999)", "2009");
            AssertExpression("=YEAR(40179)", "2010");
            AssertExpression("=YEAR(\"18/4/2020\")", "2020");
            AssertExpression("=YEAR(\"18/4/2020 10:10:20\")", "2020");
        }

        [Fact]
        public void DAYS()
        {
            AssertExpression("=DAYS(0,0)", "0");
            AssertExpression("=DAYS(1,0)", "1");
            AssertExpression("=DAYS(2,1)", "1");
            AssertExpression("=DAYS(1,2)", "-1");
            AssertExpression("=DAYS(40002.999,40000.222)", "2");
            AssertExpression("=DAYS(\"20/4/2020\",\"18/4/2020\")", "2");
            AssertExpression("=DAYS(\"23/5/2020\",43967)", "7");
            AssertExpression("=DAYS(43967,\"23/5/2020\")", "-7");
            AssertExpression("=DAYS(43967.99999,43967)", "0");
            AssertExpression("=DAYS(43967,43967.99999)", "0");
            AssertExpression("=DAYS(43967,-1)", "#VALUE!");
            AssertExpression("=DAYS(\"32/4/2020\",\"18/4/2020\")", "#VALUE!");
        }

        [Fact]
        public void DATEDIF()
        {
            AssertExpression("=DATEDIF(\"1/6/2001\", \"15/8/2002\", \"Y\")", "1");
            AssertExpression("=DATEDIF(\"1/6/2001\", \"15/8/2002\", \"M\")", "14");
            AssertExpression("=DATEDIF(\"1/6/2001\", \"15/8/2002\", \"D\")", "440");
            AssertExpression("=DATEDIF(\"1/6/2001\", \"15/8/2002\", \"MD\")", "14");
            AssertExpression("=DATEDIF(\"1/6/2001\", \"15/8/2002\", \"YM\")", "2");
            AssertExpression("=DATEDIF(\"1/6/2001\", \"15/8/2002\", \"YD\")", "75");
            AssertExpression("=DATEDIF(\"1/6/2003\", \"15/8/2002\", \"YD\")", "#VALUE!");
            AssertExpression("=DATEDIF(\"1/1/2001\", \"1/1/2003\", \"Y\")", "2");

            AssertExpression("=DATEDIF(\"1/6/2019\", \"31/5/2020\", \"Y\")", "0");
            AssertExpression("=DATEDIF(\"1/6/2019\", \"1/6/2020\", \"Y\")", "1");
            AssertExpression("=DATEDIF(\"1/6/2020\", \"31/5/2021\", \"Y\")", "0");
            AssertExpression("=DATEDIF(\"1/6/2020\", \"1/6/2021\", \"Y\")", "1");

            AssertExpression("=DATEDIF(\"1/6/2019\", \"31/5/2020\", \"M\")", "11");
            AssertExpression("=DATEDIF(\"1/6/2019\", \"1/6/2020\", \"m\")", "12");
            AssertExpression("=DATEDIF(\"1/6/2020\", \"31/5/2021\", \"m\")", "11");
            AssertExpression("=DATEDIF(\"1/6/2020\", \"1/6/2021\", \"m\")", "12");

            AssertExpression("=DATEDIF(\"1/6/2019\", \"31/5/2020\", \"D\")", "365");
            AssertExpression("=DATEDIF(\"1/6/2019\", \"1/6/2020\", \"D\")", "366");
            AssertExpression("=DATEDIF(\"1/6/2020\", \"31/5/2021\", \"D\")", "364");
            AssertExpression("=DATEDIF(\"1/6/2020\", \"1/6/2021\", \"D\")", "365");

            AssertExpression("=DATEDIF(\"1/6/2019\", \"31/5/2020\", \"MD\")", "30");
            AssertExpression("=DATEDIF(\"1/6/2019\", \"1/6/2020\", \"MD\")", "0");
            AssertExpression("=DATEDIF(\"1/6/2020\", \"31/5/2021\", \"MD\")", "30");
            AssertExpression("=DATEDIF(\"1/6/2020\", \"1/6/2021\", \"MD\")", "0");

            AssertExpression("=DATEDIF(\"1/6/2019\", \"31/5/2020\", \"YM\")", "11");
            AssertExpression("=DATEDIF(\"1/6/2019\", \"1/6/2020\", \"YM\")", "0");
            AssertExpression("=DATEDIF(\"1/6/2020\", \"31/5/2021\", \"YM\")", "11");
            AssertExpression("=DATEDIF(\"1/6/2020\", \"1/6/2021\", \"YM\")", "0");

            AssertExpression("=DATEDIF(\"1/6/2019\", \"31/5/2020\", \"YD\")", "365");
            AssertExpression("=DATEDIF(\"1/6/2019\", \"1/6/2020\", \"YD\")", "0");
            AssertExpression("=DATEDIF(\"1/6/2020\", \"31/5/2021\", \"YD\")", "364");
            AssertExpression("=DATEDIF(\"1/6/2020\", \"1/6/2021\", \"YD\")", "0");
        }
    }
}
