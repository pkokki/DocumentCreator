using JsonExcelExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace DocumentCreator.Core.Model
{
    public class MappingData
    {
        public string MappingName { get; set; }
        public string TemplateName { get; set; }
    }
    public class Mapping : MappingData
    {
        public string TemplateVersion { get; set; }
        public string MappingVersion { get; set; }
        public DateTime Timestamp { get; set; }
        public long Size { get; set; }
        public string FileName { get; set; }
    }

    public class MappingDetails : Mapping
    {
        [JsonIgnore]
        public Stream Buffer { get; set; }
        public IEnumerable<MappingExpression> Expressions { get; set; }
        public IEnumerable<EvaluationSource> Sources { get; set; }
    }

    public class MappingInfo
    {
        public IEnumerable<MappingExpression> Expressions { get; set; }
        public IEnumerable<EvaluationSource> Sources { get; set; }
    }

    /// <summary>
    /// An expression definition
    /// </summary>
    public class MappingExpression
    {
        /// <summary>
        /// The name of the expression
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The cell address of the expression - can be used for reference
        /// </summary>
        public string Cell { get; set; }
        /// <summary>
        /// The excel formula
        /// </summary>
        public string Expression { get; set; }
        /// <summary>
        /// The name of the parent expression (if exists)
        /// </summary>
        public string Parent { get; set; }
        /// <summary>
        /// True if the expression is parent of other expressions
        /// </summary>
        public bool IsCollection { get; set; }
        /// <summary>
        /// The content of the expression in the template (used for hide/show content)
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// The Excel format identifier of the cell for numeric expressions
        /// </summary>
        public int? NumFormatId { get; set; }
        /// <summary>
        /// The Excel format code of the cell for numeric expressions
        /// </summary>
        public string NumFormatCode { get; set; }
    }


    public class MappingStats
    {
        public string MappingName { get; set; }
        public string TemplateName { get; set; }
        public DateTime Timestamp { get; set; }
        public int Templates { get; set; }
        public int Documents { get; set; }
    }

    public class FillMappingInfo
    {
        public string TemplateName { get; set; }
        public string MappingName { get; set; }
        public string TestUrl { get; set; }
        public FillMappingPayload Payload { get; set; }
    }

    public class FillMappingResult
    {
        public string FileName { get; set; }
        public Stream Buffer { get; set; }
    }

    public class FillMappingPayload
    {
        public IEnumerable<EvaluationSource> Sources { get; set; }
    }
}
