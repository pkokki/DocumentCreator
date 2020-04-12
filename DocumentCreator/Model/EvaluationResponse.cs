using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentCreator.Model
{
    public class EvaluationResponse
    {
        public int Total { get; set; }
        public int Errors { get; set; }

        public IEnumerable<EvaluationResult> Results { get; set; }
    }
}
