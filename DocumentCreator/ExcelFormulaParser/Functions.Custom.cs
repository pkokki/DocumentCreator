using DocumentCreator.ExcelFormulaParser.Languages;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentCreator.ExcelFormulaParser
{
    public partial class Functions
    {
        public ExcelValue SYSDATE(List<ExcelValue> args, ExpressionScope scope)
        {
            return new ExcelValue.TextValue(DateTime.Today.ToString("d/M/yyyy"), scope.OutLanguage);
        }
        public ExcelValue SOURCE(List<ExcelValue> args, ExpressionScope scope)
        {
            return scope.Get(args[0], args[1]);
        }
        public ExcelValue RQD(List<ExcelValue> args, ExpressionScope scope)
        {
            return scope.Get(scope.Get("RQ"), new ExcelValue.TextValue("RequestData." + args[0].Text, scope.OutLanguage));
        }
        public ExcelValue RQL(List<ExcelValue> args, ExpressionScope scope)
        {
            return scope.Get(scope.Get("RQ"), new ExcelValue.TextValue("LogHeader." + args[0].Text, scope.OutLanguage));
        }
        public ExcelValue RQR(List<ExcelValue> args, ExpressionScope scope)
        {
            return scope.GetFromParent(args[0]);
        }
        public ExcelValue CONTENT(List<ExcelValue> args, ExpressionScope scope)
        {
            return new ExcelValue.TextValue((args[0].AsBoolean() ?? false) ? "#SHOW_CONTENT#" : "#HIDE_CONTENT#", scope.OutLanguage);
        }

        public ExcelValue MAPVALUE(List<ExcelValue> args, ExpressionScope scope)
        {
            var value = scope.Get(args[0], args[1]);
            if (args.Count > 2 && !(args[2] is ExcelValue.NullValue))
            {
                var args2 = new List<ExcelValue> { args[2], value };
                value = MAPVALUE(args2, scope);
            }
            return value;
        }

        public ExcelValue MAPITEM(List<ExcelValue> args, ExpressionScope scope)
        {
            var values = ((IEnumerable<ExcelValue>)args[0].InnerValue).Select(item => scope.Get(item, args[1]));
            return new ExcelValue.ArrayValue(values, scope.OutLanguage);
        }

        public ExcelValue GETITEM(List<ExcelValue> args, ExpressionScope scope)
        {
            var index = args.Count > 3 ? ((int)args[3].AsDecimal().Value) - 1 : 0;

            var target = ((IEnumerable<ExcelValue>)args[0].InnerValue).ElementAt(index);
            var result = scope.Get(target, args[1]);
            if (args.Count > 2 && !(args[2] is ExcelValue.NullValue))
            {
                var args2 = new List<ExcelValue> { args[2], result };
                result = MAPVALUE(args2, scope);
            }
            return result;
        }
        public ExcelValue GETLIST(List<ExcelValue> args, ExpressionScope scope)
        {
            var values = ((IEnumerable<ExcelValue>)args[0].InnerValue).Select(item => scope.Get(item, args[1]));
            return new ExcelValue.ArrayValue(values, scope.OutLanguage);
        }
    }
}
