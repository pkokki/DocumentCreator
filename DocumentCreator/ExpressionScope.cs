using DocumentCreator.Core.Model;
using DocumentCreator.ExcelFormulaParser;
using DocumentCreator.ExcelFormulaParser.Languages;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentCreator
{
    public class ExpressionScope
    {
        private readonly IEnumerable<MappingSource> sources;
        private readonly IDictionary<string, ExcelValue> sourceValues;
        private readonly IDictionary<string, ExcelValue> values;

        public ExpressionScope(Language inLanguage, Language outLanguage, IEnumerable<MappingSource> sources)
        {
            sourceValues = new Dictionary<string, ExcelValue>();
            values = new Dictionary<string, ExcelValue>();
            OutLanguage = outLanguage;
            InLanguage = inLanguage;
            this.sources = sources ?? new List<MappingSource>();
        }

        public Language OutLanguage { get; }
        public Language InLanguage { get; }
        public string ParentName { get; internal set; }

        public void Set(string key, ExcelValue value)
        {
            values[key] = value;
        }

        public ExcelValue Get(string key)
        {
            if (sourceValues.ContainsKey(key))
                return sourceValues[key];
            if (sources.Any(o => o.Name == key || o.Cell == key))
                return ExcelValue.Create(sources.First(o => o.Name == key || o.Cell == key).Payload, OutLanguage);
            if (values.ContainsKey(key))
                return values[key];
            throw new InvalidOperationException($"Name or cell {key} not found in scope.");
        }
        public ExcelValue Get(ExcelValue key, ExcelValue path)
        {
            JObject target;
            if (key is ExcelValue.JsonObjectValue)
                target = (JObject)key.InnerValue;
            else
                target = (JObject)Get(key.Text).InnerValue;
            return ExcelValue.Create(target.SelectToken(path.Text), OutLanguage);
        }

        public ExcelValue GetFromParent(ExcelValue path)
        {
            var parentName = ParentName;
            if (parentName != null)
            {
                var parent = (IEnumerable<ExcelValue>)Get(parentName).InnerValue;
                var values = parent.Select(item => Get(item, path));
                return new ExcelValue.ArrayValue(values, OutLanguage);
            }
            throw new InvalidOperationException();
        }
    }
}
