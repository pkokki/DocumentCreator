using Xunit;
using Xunit.Abstractions;

namespace DocumentCreator.ExcelFormulaParser
{
    public class Logical : BaseTest
    {
        public Logical(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void TRUE()
        {
            AssertExpression("=TRUE", "TRUE");
        }

        [Fact]
        public void FALSE()
        {
            AssertExpression("=FALSE", "FALSE");
        }

        [Fact]
        public void AND()
        {
            AssertExpression("=AND(FALSE, FALSE)", "FALSE");
            AssertExpression("=AND(FALSE, TRUE)", "FALSE");
            AssertExpression("=AND(TRUE, FALSE)", "FALSE");
            AssertExpression("=AND(TRUE, TRUE)", "TRUE");
            AssertExpression("=AND(TRUE, TRUE, TRUE)", "TRUE");
            AssertExpression("=AND(TRUE, TRUE, FALSE)", "FALSE");
        }

        [Fact]
        public void OR()
        {
            AssertExpression("=OR(FALSE, FALSE)", "FALSE");
            AssertExpression("=OR(FALSE, TRUE)", "TRUE");
            AssertExpression("=OR(TRUE, FALSE)", "TRUE");
            AssertExpression("=OR(TRUE, TRUE)", "TRUE");
            AssertExpression("=OR(FALSE, FALSE, FALSE)", "FALSE");
            AssertExpression("=OR(FALSE, FALSE, TRUE)", "TRUE");
        }

        [Fact]
        public void XOR()
        {
            AssertExpression("=XOR(FALSE, FALSE)", "FALSE");
            AssertExpression("=XOR(FALSE, TRUE)", "TRUE");
            AssertExpression("=XOR(TRUE, FALSE)", "TRUE");
            AssertExpression("=XOR(TRUE, TRUE)", "FALSE");
            AssertExpression("=XOR(FALSE, FALSE, TRUE)", "TRUE");
        }

        [Fact]
        public void NOT()
        {
            AssertExpression("=NOT(TRUE)", "FALSE");
            AssertExpression("=NOT(FALSE)", "TRUE");
        }

        [Fact]
        public void IF()
        {
            AssertExpression("=IF(TRUE, 1, 2)", "1");
            AssertExpression("=IF(FALSE, 1, 2)", "2");
        }

        [Fact]
        public void IFERROR()
        {
            AssertExpression("=IFERROR(1/2, 2)", "0,5");
            AssertExpression("=IFERROR(1/0, 2)", "2");
        }

        [Fact]
        public void IFNA()
        {
            AssertExpression("=IFNA(NA(), 1)", "1");
        }
    }
}
