using DocumentCreator.Core.Model;
using DocumentCreator.Properties;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Xunit;

namespace DocumentCreator
{
    public class OpenXmlSpreadsheetTests
    {
        [Fact]
        public void CanGetTemplateFieldExpressions()
        {
            var bytes = new MemoryStream(Resources.open_xml_spreadsheet_tests001_xlsm);
            var info = OpenXmlSpreadsheet.GetMappingInfo(bytes, null);
            Assert.NotEmpty(info.Expressions);
        }

        [Fact]
        public void CanUseForwardOwnCellValues()
        {
            var bytes = new MemoryStream(Resources.use_forward_own_cell_values_xlsm);

            var info = OpenXmlSpreadsheet.GetMappingInfo(bytes, null);
            var processor = new MappingExpressionEvaluator();
            var results = processor.Evaluate(info.Expressions, info.Sources);

            Assert.True(results.All(r => r.Error == null));
            Assert.Equal("22", results.First(o => o.Name == "F01").Text);
            Assert.Equal("2", results.First(o => o.Name == "F02").Text);
            Assert.Equal("6", results.First(o => o.Name == "F03").Text);
            Assert.Equal("16", results.First(o => o.Name == "F04").Text);
            Assert.Equal("8", results.First(o => o.Name == "F05").Text);
        }

        [Fact]
        public void CanSumRangeOwnCellValues()
        {
            var info = new MappingInfo()
            {
                Expressions = new List<MappingExpression>()
                {
                    new MappingExpression() { Name = "x", Cell = "F3", Expression = "=1" },
                    new MappingExpression() { Name = "y", Cell = "F4", Expression = "=2" },
                    new MappingExpression() { Name = "z", Cell = "F5", Expression = "=SUM(F3:F4)" },
                }
            };

            var processor = new MappingExpressionEvaluator();
            var results = processor.Evaluate(info.Expressions, info.Sources);

            var z = results.First(o => o.Name == "z");
            Assert.Null(z.Error);
            Assert.Equal(3M, z.Value);
        }

        [Fact]
        public void CanFormatResultValues()
        {
            var input = new EvaluationInput()
            {
                Expressions = new List<MappingExpression>()
                {
                    new MappingExpression() { Name = "quantity", Cell = "F3", Expression = "=10/3" },
                    new MappingExpression() { Name = "price", Cell = "F4", Expression = "=1.3" },
                    new MappingExpression() { Name = "subtotal", Cell = "F5", Expression = "=F3 * F4", NumFormatId = 2 },
                    new MappingExpression() { Name = "tax", Cell = "F6", Expression = "=F5 * 10%", NumFormatId = 2 },
                    new MappingExpression() { Name = "total", Cell = "F7", Expression = "=F5 + F6", NumFormatId = 2 },
                }
            };

            var processor = new MappingExpressionEvaluator(CultureInfo.GetCultureInfo("el-GR"));
            var results = processor.Evaluate(input).Results;

            var subtotal = results.First(o => o.Name == "subtotal");
            Assert.Equal("4,33", subtotal.Text);
            var tax = results.First(o => o.Name == "tax");
            Assert.Equal("0,43", tax.Text);
            var total = results.First(o => o.Name == "total");
            Assert.Equal("4,77", total.Text);
        }

        [Fact]
        public void CanFormatNumbersWithStandardNumberFormats()
        {
            var expressions = new List<MappingExpression>()
            {
                new MappingExpression() { Name = "A1", Cell = "A1", Expression = "=PI()*10^4" }
            };
            for (var i = 1; i <= 12; i++)
                expressions.Add(new MappingExpression() { Name = $"B{i}", Cell = $"B{i}", Expression = "=A1", NumFormatId = i });
            for (var i = 37; i <= 44; i++)
                expressions.Add(new MappingExpression() { Name = $"B{i}", Cell = $"B{i}", Expression = "=A1", NumFormatId = i });
            expressions.Add(new MappingExpression() { Name = $"B49", Cell = $"B49", Expression = "=A1", NumFormatId = 49 });
            var input = new EvaluationInput()
            {
                Expressions = expressions
            };

            var processor = new MappingExpressionEvaluator(CultureInfo.GetCultureInfo("el-GR"));
            var results = processor.Evaluate(input).Results;

            Assert.Equal("31416", results.First(o => o.Name == "B1").Text);
            Assert.Equal("31415,93", results.First(o => o.Name == "B2").Text);
            Assert.Equal("31.416", results.First(o => o.Name == "B3").Text);
            Assert.Equal("31.415,93", results.First(o => o.Name == "B4").Text);
            Assert.Equal("31.416 €", results.First(o => o.Name == "B5").Text);
            Assert.Equal("31.416 €", results.First(o => o.Name == "B6").Text);
            Assert.Equal("31.415,93 €", results.First(o => o.Name == "B7").Text);
            Assert.Equal("31.415,93 €", results.First(o => o.Name == "B8").Text);
            Assert.Equal("3.141.593%", results.First(o => o.Name == "B9").Text);
            Assert.Equal("3.141.592,65%", results.First(o => o.Name == "B10").Text);
            Assert.Equal("3,14E+004", results.First(o => o.Name == "B11").Text);
            Assert.Equal("31416    ", results.First(o => o.Name == "B12").Text);
            //Assert.Equal("31415 63/68", results.First(o => o.Name == "B13").Text);


            Assert.Equal("31.416  ", results.First(o => o.Name == "B37").Text);
            Assert.Equal("31.416  ", results.First(o => o.Name == "B38").Text);
            Assert.Equal("31.415,93  ", results.First(o => o.Name == "B39").Text);
            Assert.Equal("31.415,93  ", results.First(o => o.Name == "B40").Text);

            Assert.Equal(" 31.416   ", results.First(o => o.Name == "B41").Text);
            Assert.Equal(" 31.416 € ", results.First(o => o.Name == "B42").Text);
            Assert.Equal(" 31.415,93   ", results.First(o => o.Name == "B43").Text);
            Assert.Equal(" 31.415,93 € ", results.First(o => o.Name == "B44").Text);

            Assert.Equal("31415,92654", results.First(o => o.Name == "B49").Text);

        }

        [Fact]
        public void CanFormatNumbersWithStandardDateFormats()
        {
            var expressions = new List<MappingExpression>()
            {
                new MappingExpression() { Name = "A1", Cell = "A1", Expression = "=PI()*10^4" }
            };
            for (var i = 14; i <= 22; i++)
                expressions.Add(new MappingExpression() { Name = $"B{i}", Cell = $"B{i}", Expression = "=A1", NumFormatId = i });
            for (var i = 45; i <= 48; i++)
                expressions.Add(new MappingExpression() { Name = $"B{i}", Cell = $"B{i}", Expression = "=A1", NumFormatId = i });
            var input = new EvaluationInput()
            {
                Expressions = expressions
            };

            var processor = new MappingExpressionEvaluator(CultureInfo.GetCultureInfo("el-GR"));
            var results = processor.Evaluate(input).Results;

            Assert.Equal("3/1/1986", results.First(o => o.Name == "B14").Text);
            Assert.Equal("3-Ιαν-86", results.First(o => o.Name == "B15").Text);
            Assert.Equal("3-Ιαν", results.First(o => o.Name == "B16").Text);
            Assert.Equal("Ιαν-86", results.First(o => o.Name == "B17").Text);
            Assert.Equal("10:14 μμ", results.First(o => o.Name == "B18").Text);
            Assert.Equal("10:14:13 μμ", results.First(o => o.Name == "B19").Text);
            Assert.Equal("22:14", results.First(o => o.Name == "B20").Text);
            Assert.Equal("22:14:13", results.First(o => o.Name == "B21").Text);
            Assert.Equal("3/1/1986 22:14", results.First(o => o.Name == "B22").Text);

            Assert.Equal("14:13", results.First(o => o.Name == "B45").Text);
            //Assert.Equal("753982:14:13", results.First(o => o.Name == "B46").Text);
            //Assert.Equal("14:12,7", results.First(o => o.Name == "B47").Text);
            Assert.Equal("31,4E+3", results.First(o => o.Name == "B48").Text);
        }

        [Fact]
        public void CanFormatDatesAndTimes()
        {
            var expressions = new List<MappingExpression>()
            {
                new MappingExpression() { Name = "A1", Expression = "=1", NumFormatId = 14 },
                new MappingExpression() { Name = "A2", Expression = "=1", NumFormatId = 19 },
                new MappingExpression() { Name = "B1", Expression = "=2", NumFormatId = 14 },
                new MappingExpression() { Name = "B2", Expression = "=2", NumFormatId = 19 },
                new MappingExpression() { Name = "A3", Expression = "=PI()", NumFormatId = 14 },
                new MappingExpression() { Name = "A4", Expression = "=PI()", NumFormatId = 19 },
                new MappingExpression() { Name = "A5", Expression = "=10000.00001", NumFormatId = 14 },
                new MappingExpression() { Name = "A6", Expression = "=10000.00001", NumFormatId = 19 },
                new MappingExpression() { Name = "B5", Expression = "=10000.000001", NumFormatId = 14 },
                new MappingExpression() { Name = "B6", Expression = "=10000.000001", NumFormatId = 19 },
                new MappingExpression() { Name = "A7", Expression = "=10000.99999", NumFormatId = 14 },
                new MappingExpression() { Name = "A8", Expression = "=10000.99999", NumFormatId = 19 },
                new MappingExpression() { Name = "A9", Expression = "=10000.999999", NumFormatId = 14 },
                new MappingExpression() { Name = "A10", Expression = "=10000.999999", NumFormatId = 19 },
                new MappingExpression() { Name = "A11", Expression = "=0.5787", NumFormatId = 14 },
                new MappingExpression() { Name = "A12", Expression = "=0.5787", NumFormatId = 19 },

                new MappingExpression() { Name = "C5", Expression = "=A5", NumFormatId = 0 },
                new MappingExpression() { Name = "C6", Expression = "=A6", NumFormatId = 0 },
                new MappingExpression() { Name = "C7", Expression = "=B5", NumFormatId = 0 },
                new MappingExpression() { Name = "C8", Expression = "=B6", NumFormatId = 0 },
                new MappingExpression() { Name = "C9", Expression = "=A7", NumFormatId = 0 },
                new MappingExpression() { Name = "C10", Expression = "=A8", NumFormatId = 0 },
            };

            var input = new EvaluationInput()
            {
                Expressions = expressions
            };

            var processor = new MappingExpressionEvaluator(CultureInfo.GetCultureInfo("el-GR"));
            var results = processor.Evaluate(input).Results;
            Assert.Equal("1/1/1900", results.First(o => o.Name == "A1").Text);
            Assert.Equal("12:00:00 πμ", results.First(o => o.Name == "A2").Text);
            Assert.Equal("2/1/1900", results.First(o => o.Name == "B1").Text);
            Assert.Equal("12:00:00 πμ", results.First(o => o.Name == "B2").Text);
            Assert.Equal("3/1/1900", results.First(o => o.Name == "A3").Text);
            Assert.Equal("3:23:54 πμ", results.First(o => o.Name == "A4").Text);
            Assert.Equal("18/5/1927", results.First(o => o.Name == "A5").Text);
            Assert.Equal("12:00:01 πμ", results.First(o => o.Name == "A6").Text);
            Assert.Equal("18/5/1927", results.First(o => o.Name == "B5").Text);
            Assert.Equal("12:00:00 πμ", results.First(o => o.Name == "B6").Text);
            Assert.Equal("18/5/1927", results.First(o => o.Name == "A7").Text);
            Assert.Equal("11:59:59 μμ", results.First(o => o.Name == "A8").Text);
            Assert.Equal("19/5/1927", results.First(o => o.Name == "A9").Text);
            Assert.Equal("12:00:00 πμ", results.First(o => o.Name == "A10").Text);
            Assert.Equal("1/1/1900", results.First(o => o.Name == "A11").Text);
            Assert.Equal("1:53:20 μμ", results.First(o => o.Name == "A12").Text);

            Assert.Equal("10000,00001", results.First(o => o.Name == "C5").Text);
            Assert.Equal("10000,00001", results.First(o => o.Name == "C6").Text);
            Assert.Equal("10000", results.First(o => o.Name == "C7").Text);
            Assert.Equal("10000", results.First(o => o.Name == "C8").Text);
            Assert.Equal("10000,99999", results.First(o => o.Name == "C9").Text);
            Assert.Equal("10000,99999", results.First(o => o.Name == "C10").Text);
        }
    }
}
