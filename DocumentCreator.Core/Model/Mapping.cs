using JsonExcelExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace DocumentCreator.Core.Model
{
    /// <summary>
    /// Represents the user-defined information of a mapping
    /// </summary>
    public class MappingData
    {
        /// <summary>
        /// The mapping name
        /// </summary>
        [Required]
        public string MappingName { get; set; }
       
        /// <summary>
        /// The template name
        /// </summary>
        [Required]
        public string TemplateName { get; set; }
    }

    /// <summary>
    /// Represents a mapping (an Excel workbook with mapping expression)
    /// </summary>
    public class Mapping : MappingData
    {
        /// <summary>
        /// The template version
        /// </summary>
        [Required]
        public string TemplateVersion { get; set; }

        /// <summary>
        /// The mapping version
        /// </summary>
        [Required]
        public string MappingVersion { get; set; }

        /// <summary>
        /// The creation date
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The size of the mapping in bytes
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// The filename of the mapping
        /// </summary>
        public string FileName { get; set; }
    }

    /// <summary>
    /// Represents a mapping (an Excel workbook with mapping expression) with details
    /// </summary>
    public class MappingDetails : Mapping
    {
        /// <summary>
        /// The stream with the contents of the mapping
        /// </summary>
        [JsonIgnore]
        public Stream Buffer { get; set; }

        /// <summary>
        /// The expressions (Excel formulas) of the mapping
        /// </summary>
        [Required]
        public IEnumerable<MappingExpression> Expressions { get; set; }

        /// <summary>
        /// The JSON sources of the mapping
        /// </summary>
        [Required]
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

    /// <summary>
    /// Aggreagated information for a mapping or template
    /// </summary>
    public class MappingStats
    {
        /// <summary>
        /// The mapping name
        /// </summary>
        public string MappingName { get; set; }
        /// <summary>
        /// The template name
        /// </summary>
        public string TemplateName { get; set; }
        /// <summary>
        /// The timestamp of latest version
        /// </summary>
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// The total number of templates associated with the mapping.
        /// It is 1 if template name is not null.
        /// </summary>
        public int Templates { get; set; }
        /// <summary>
        /// The total number of documents associated with the mapping and the template (if defined).
        /// </summary>
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

    /// <summary>
    /// Information to fill a mapping Excel workbook
    /// </summary>
    public class FillMappingPayload
    {
        /// <summary>
        /// A collection of JSON sources
        /// </summary>
        public IEnumerable<EvaluationSource> Sources { get; set; }
    }
}
