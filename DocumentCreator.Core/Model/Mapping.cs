﻿using JsonExcelExpressions;
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

    public class MappingExpression
    {
        public string Name { get; set; }
        public string Cell { get; set; }
        public string Expression { get; set; }
        public string Parent { get; set; }
        public bool IsCollection { get; set; }
        public string Content { get; set; }
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
