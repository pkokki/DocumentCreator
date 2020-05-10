using JsonExcelExpressions.Lang;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace JsonExcelExpressions.Eval
{
    public class ExpressionScope
    {
        private readonly IEnumerable<EvaluationSource> sources;
        private readonly IDictionary<string, ExcelValue> sourceValues;
        private readonly IDictionary<string, ExcelValue> values;

        public ExpressionScope(CultureInfo culture, IEnumerable<EvaluationSource> sources) 
            : this(Language.Create(culture), sources)
        {
        }
        internal ExpressionScope(Language outLanguage, IEnumerable<EvaluationSource> sources)
        {
            sourceValues = new Dictionary<string, ExcelValue>();
            values = new Dictionary<string, ExcelValue>();
            OutLanguage = outLanguage;
            this.sources = sources ?? new List<EvaluationSource>();
        }

        internal Language OutLanguage { get; }
        public string ParentName { get; set; }

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
            if (key.Contains(':'))
                return GetRangeValues(key);
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

        private ExcelValue GetRangeValues(string key)
        {
            var rangeCells = key.Split(':');
            var startCell = new CellAddress(rangeCells[0]);
            var endCell = new CellAddress(rangeCells[1]);
            if (startCell.Column != endCell.Column)
                throw new InvalidOperationException($"GetRangeValues does not support multi-column range {key}.");
            var values = new List<ExcelValue>();
            for (var row = startCell.Row; row <= endCell.Row; row++)
            {
                values.Add(Get($"{startCell.Column}{row}"));
            }
            return new ExcelValue.ArrayValue(values, OutLanguage);
        }
    }
}
