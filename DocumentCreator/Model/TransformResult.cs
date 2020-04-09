using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace DocumentCreator.Model
{
    public class TransformResult
    {
        public TransformResult()
        {
            Rows = new List<string>();
        }
        public long TargetId { get; set; }
        public string Expression { get; set; }
        public string Value { get; set; }
        public string Error { get; set; }
        public List<string> Rows { get; set; }

        public int ChildRows { get; set; }

        public override string ToString()
        {
            return $"{Expression}: {(Error == null ? (Value ?? "null") : " ------- ERROR " + Error)}";
        }
    }
}
