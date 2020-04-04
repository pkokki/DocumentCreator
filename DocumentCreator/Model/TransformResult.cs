using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentCreator.Model
{
    public class TransformResult
    {
        public long TargetId { get; set; }
        public string Expression { get; set; }
        public string Value { get; set; }
        public string Error { get; set; }

        public override string ToString()
        {
            return $"{Expression}: {(Error == null ? (Value ?? "null") : " ------- ERROR " + Error)}";
        }
    }
}
