using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentCreator.Model
{
    public class Transformation
    {
        public string Name { get; internal set; }
        public string Expression { get; internal set; }
        public TransformResult Result { get; internal set; }
    }
}
