﻿using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExcelExpressions
{
    public class EvaluationResult
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public string Text { get; set; }
        public string Error { get; set; }
    }
}