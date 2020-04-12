using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentCreator.Model
{
    public class EvaluationRequest
    {
        public string TemplateName { get; set; }
        public IEnumerable<TemplateFieldExpression> Expressions { get; set; }
        public IEnumerable<EvaluationSource> Sources { get; set; }
    }
}
