using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace DocumentCreator.ExcelFormula
{
    public class Math : BaseTest
    {
        public Math(ITestOutputHelper output) : base(output) { }

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
