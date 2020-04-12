using Newtonsoft.Json;
using System.Collections.Generic;

namespace DocumentCreator.Model
{
    public class ExpressionResult : EvaluationResult
    {
        public ExpressionResult()
        {
            Rows = new List<string>();
        }
        
        public string Expression { get; set; }
        
        [JsonIgnore]
        public List<string> Rows { get; set; }

        [JsonIgnore]
        public int ChildRows { get; set; }

        public override string ToString()
        {
            return $"{Expression}: {(Error == null ? (Value ?? "null") : " ------- ERROR " + Error)}";
        }
    }
}
