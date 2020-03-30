﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace DocumentCreator.Model
{
    public class TemplateMapping
    {
        private readonly JObject transformations = new JObject();

        public TemplateMapping(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
        public Dictionary<string, object> Transformations { get { return transformations.ToObject<Dictionary<string, object>>(); } }

        internal void Upsert(JObject transformations)
        {
            this.transformations.Merge(transformations);
        }
    }
}
