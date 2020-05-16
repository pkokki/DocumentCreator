using System;
using System.Collections.Generic;

namespace JsonExcelExpressions.Eval
{
    internal partial class Functions
    {
        public static readonly Functions INSTANCE = new Functions();

        private readonly Dictionary<string, Func<List<ExcelValue>, ExpressionScope, ExcelValue>> Registry
            = new Dictionary<string, Func<List<ExcelValue>, ExpressionScope, ExcelValue>>();

        private Functions()
        {
            Registry.Add("NA", NA);
            
            Registry.Add("CONCATENATE", CONCATENATE);
            Registry.Add("EXACT", EXACT);
            Registry.Add("FIND", FIND);
            Registry.Add("FIXED", FIXED);
            Registry.Add("LEFT", LEFT);
            Registry.Add("LEN", LEN);
            Registry.Add("LOWER", LOWER);
            Registry.Add("MID", MID);
            Registry.Add("PROPER", PROPER);
            Registry.Add("REPLACE", REPLACE);
            Registry.Add("RIGHT", RIGHT);
            Registry.Add("SEARCH", SEARCH);
            Registry.Add("SUBSTITUTE", SUBSTITUTE);
            Registry.Add("T", T);
            Registry.Add("TEXT", TEXT);
            Registry.Add("TRIM", TRIM);
            Registry.Add("UPPER", UPPER);
            Registry.Add("VALUE", VALUE);

            Registry.Add("AND", AND);
            Registry.Add("IF", IF);
            Registry.Add("IFERROR", IFERROR);
            Registry.Add("IFNA", IFNA);
            Registry.Add("NOT", NOT);
            Registry.Add("OR", OR);
            Registry.Add("XOR", XOR);

            Registry.Add("PI", PI);
            Registry.Add("SUM", SUM);

            Registry.Add("NOW", NOW);
            Registry.Add("TODAY", TODAY);
            Registry.Add("DATE", DATE);
            Registry.Add("TIME", TIME);
            Registry.Add("DATEDIF", DATEDIF);
            Registry.Add("DATEVALUE", DATEVALUE);
            Registry.Add("TIMEVALUE", TIMEVALUE);
            Registry.Add("DAYS", DAYS);
            Registry.Add("DAY", DAY);
            Registry.Add("MONTH", MONTH);
            Registry.Add("YEAR", YEAR);
            Registry.Add("HOUR", HOUR);
            Registry.Add("MINUTE", MINUTE);
            Registry.Add("SECOND", SECOND);
            //Registry.Add("DAYS360", DAYS360);
            //Registry.Add("EDATE", EDATE);
            //Registry.Add("EOMONTH", EOMONTH);
            //Registry.Add("ISOWEEKNUM", ISOWEEKNUM);
            //Registry.Add("NETWORKDAYS", NETWORKDAYS);
            //Registry.Add("WEEKDAY", WEEKDAY);
            //Registry.Add("WEEKNUM", WEEKNUM);
            //Registry.Add("WORKDAY", WORKDAY);
            //Registry.Add("WORKDAY.INTL", WORKDAY.INTL);
            //Registry.Add("YEARFRAC", YEARFRAC);

            Registry.Add("SYSDATE", SYSDATE);
            Registry.Add("SOURCE", SOURCE);
            Registry.Add("RQD", RQD);
            Registry.Add("RQL", RQL);
            Registry.Add("RQR", RQR);
            Registry.Add("CONTENT", CONTENT);
            Registry.Add("MAPVALUE", MAPVALUE);
            Registry.Add("MAPITEM", MAPITEM);
            Registry.Add("GETITEM", GETITEM);
            Registry.Add("GETLIST", GETLIST);
        }

        public ExcelValue Evaluate(string name, List<ExcelValue> args, ExpressionScope scope)
        {
            if (Registry.TryGetValue(name, out var function))
                return function(args, scope);
            else
                throw new InvalidOperationException($"Unknown function name: {name}");
        }

        public ExcelValue NA(List<ExcelValue> args, ExpressionScope scope)
        {
            return ExcelValue.NA;
        }
        
    }
}
