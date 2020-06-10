using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExcelExpressions
{
    /// <summary>
    /// The evaluation result of an expression
    /// </summary>
    public class EvaluationResult
    {
        /// <summary>
        /// The name of the expression
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The cell address of the expression
        /// </summary>
        public string Cell { get; set; }

        /// <summary>
        /// The result of the expression
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// The text representation of the result of the expression
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The error (if any) of the evaluation
        /// </summary>
        public string Error { get; set; }
    }
}
