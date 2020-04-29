﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExcelExpressions
{
    public class EvaluationSource
    {
        public string Name { get; set; }
        public string Cell { get; set; }
        public JObject Payload { get; set; }
    }
}
