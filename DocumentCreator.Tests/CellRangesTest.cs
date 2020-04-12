using DocumentCreator.ExcelFormulaParser.Languages;
using DocumentCreator.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DocumentCreator
{
    public class CellRangesTest
    {
        [Fact] 
        public void CanUseBackwardOwnCellValues()
        {
            var expressions = new List<TemplateFieldExpression>
            {
                new TemplateFieldExpression() { Name = "F01", Cell = "J13", Expression = "=3" },
                new TemplateFieldExpression() { Name = "F02", Cell = "J14", Expression = "=4" },
                new TemplateFieldExpression() { Name = "F03", Cell = "J15", Expression = "=J13+J14" }
            };

            var processor = new ExpressionEvaluator(Language.Invariant, Language.ElGr);
            processor.Evaluate(expressions, null);
            
            Assert.NotNull(expressions[2].Result);
            Assert.Null(expressions[2].Result.Error);
            Assert.Equal("7", expressions[2].Result.Text);
            Assert.Equal(7M, expressions[2].Result.Value);
        }
    }
}
