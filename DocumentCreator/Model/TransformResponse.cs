using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentCreator.Model
{
    public class TransformResponse
    {
        public TransformResponse(string name)
        {
            Name = name;
            Results = new List<TransformResult>();
        }

        public string Name { get; }
        public List<TransformResult> Results { get; }

        public void AddResult(TransformResult result)
        {
            Results.Add(result);
        }
    }
}
