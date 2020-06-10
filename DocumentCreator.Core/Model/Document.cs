using JsonExcelExpressions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Json.Serialization;

namespace DocumentCreator.Core.Model
{
    /// <summary>
    /// Represents a document (a Word document with content controls replaced by expression results)
    /// </summary>
    public class Document
    {
        /// <summary>
        /// The identifier of the document
        /// </summary>
        [Required]
        public string DocumentId { get; set; }
        /// <summary>
        /// The template name of the document
        /// </summary>
        [Required]
        public string TemplateName { get; set; }
        /// <summary>
        /// The template version of the document
        /// </summary>
        [Required]
        public string TemplateVersion { get; set; }
        /// <summary>
        /// The mapping name of the document
        /// </summary>
        [Required]
        public string MappingName { get; set; }
        /// <summary>
        /// The mapping version of the document
        /// </summary>
        [Required]
        public string MappingVersion { get; set; }
        /// <summary>
        /// The creation date
        /// </summary>
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// The size of the document in bytes
        /// </summary>
        public long Size { get; set; }
        /// <summary>
        /// The filename of the document
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// The link to the HTML version of the document    
        /// </summary>
        public string Url { get; set; }
    }

    /// <summary>
    /// Represents a document (a Word document with content controls replaced by expression results) with details
    /// </summary>
    public class DocumentDetails : Document
    {
        /// <summary>
        /// The stream with the contents of the document
        /// </summary>
        [JsonIgnore]
        public Stream Buffer { get; set; }
    }

    /// <summary>
    /// Criteria for querying documents
    /// </summary>
    public class DocumentQuery : PagingParams
    {
        /// <summary>
        /// The template name
        /// </summary>
        public string TemplateName { get; set; }
        /// <summary>
        /// The template version
        /// </summary>
        public string TemplateVersion { get; set; }
        /// <summary>
        /// The mapping name
        /// </summary>
        public string MappingsName { get; set; }
        /// <summary>
        /// The mapping version
        /// </summary>
        public string MappingsVersion { get; set; }
    }

    /// <summary>
    /// Information used to create a document
    /// </summary>
    public class DocumentPayload
    {
        /// <summary>
        /// A collection of JSON sources
        /// </summary>
        public IEnumerable<EvaluationSource> Sources { get; set; }
    }
}
