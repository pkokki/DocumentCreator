using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace DocumentCreator.Core.Model
{
    /// <summary>
    /// Represents the user-defined information of a template
    /// </summary>
    public class TemplateData
    {
        /// <summary>
        /// The name of the template
        /// </summary>
        [Required]
        public string TemplateName { get; set; }
    }

    /// <summary>
    /// Represents a template (a Word document with content controls)
    /// </summary>
    public class Template : TemplateData
    {
        /// <summary>
        /// The filename of the template
        /// </summary>
        [Required]
        public string FileName { get; set; }

        /// <summary>
        /// The version identifier
        /// </summary>
        [Required]
        public string Version { get; set; }

        /// <summary>
        /// The creation date
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The size of the template in bytes
        /// </summary>
        public long Size { get; set; }
    }

    /// <summary>
    /// Represents a template (a Word document with content controls) with details
    /// </summary>
    public class TemplateDetails : Template
    {
        /// <summary>
        /// The fields of the template
        /// </summary>
        [Required]
        public IEnumerable<TemplateField> Fields { get; set; }

        /// <summary>
        /// The stream with the contents of the template
        /// </summary>
        [JsonIgnore]
        public Stream Buffer { get; set; }
    }

    /// <summary>
    /// Represents a content control of the template
    /// </summary>
    public class TemplateField
    {
        /// <summary>
        /// The name of the template field
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// The content of the template field
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// True if the field is a repeating content control
        /// </summary>
        public bool IsCollection { get; set; }
        /// <summary>
        /// The template field name that contains the field or null
        /// </summary>
        public string Parent { get; set; }
    }
}
