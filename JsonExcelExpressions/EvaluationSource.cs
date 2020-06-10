using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExcelExpressions
{
    /// <summary>
    /// The source for an evaluation
    /// </summary>
    public class EvaluationSource
    {
        /// <summary>
        /// The name of the source
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The cell address of the source - can be used for reference
        /// </summary>
        public string Cell { get; set; }

        /// <summary>
        /// The content of the source
        /// </summary>
        public JObject Payload { get; set; }
    }
}
