using DocumentCreator.Core.Model;
using DocumentCreator.ExcelFormulaParser;
using DocumentCreator.ExcelFormulaParser.Languages;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace DocumentCreator
{
    public class JsonExpressionHelper
    {
        public IEnumerable<ExcelFormulaToken> Parse(JObject sourcePayload, string expression)
        {
            if (!expression.StartsWith("="))
                expression = "=" + expression;
            var sourceTokens = new ExcelFormulaParser.ExcelFormula(expression, Language.ElGr).ToList();
            var tokens = new List<ExcelFormulaToken>();
            foreach (var sourceToken in sourceTokens)
            {
                    var sourcePath = sourceToken.Value;
                if (sourceToken.Type == ExcelFormulaTokenType.Operand 
                    && sourceToken.Subtype == ExcelFormulaTokenSubtype.Range 
                    && !Regex.IsMatch(sourcePath, "__[Aa][0-9]+"))
                {
                    var jToken = sourcePayload.SelectToken(sourcePath);
                    if (jToken != null)
                    {
                        if (jToken.Type == JTokenType.Array)
                            tokens.AddRange(PrepareMapValueCall("N3", sourcePath));
                        else
                            tokens.Add(CreateExcelOperandToken(jToken));
                    }
                    else
                    {
                        // Try to parse and split array expression
                        if (TrySplitArrayExpression(sourcePayload, sourcePath, out IEnumerable<ExcelFormulaToken> exprTokens))
                            tokens.AddRange(exprTokens);
                        else
                        {
                            // Not found
                            tokens.Add(new ExcelFormulaToken("NA", ExcelFormulaTokenType.Function, ExcelFormulaTokenSubtype.Start));
                            tokens.Add(new ExcelFormulaToken(null, ExcelFormulaTokenType.Function, ExcelFormulaTokenSubtype.Stop));
                        }
                    }
                }
                else
                {
                    tokens.Add(sourceToken);
                }
            }
            return tokens;
        }

        private bool TrySplitArrayExpression(JToken source, string sourcePath, out IEnumerable<ExcelFormulaToken> tokens)
        {
            tokens = new List<ExcelFormulaToken>();
            var partsQueue = new Queue<string>(sourcePath.Split('.'));
            var arrayPathQueue = new Queue<string>();
            var currentToken = source;
            do
            {
                var pathPart = partsQueue.Peek();
                if (!currentToken.HasValues) break;
                currentToken = currentToken[pathPart];
                if (currentToken == null) break;

                partsQueue.Dequeue();
                arrayPathQueue.Enqueue(pathPart);
                if (currentToken is JArray)
                {
                    var arrayPath = string.Join('.', arrayPathQueue);
                    var objectPath = string.Join('.', partsQueue);
                    tokens = PrepareMapItemCall(PrepareMapValueCall("N3", arrayPath), objectPath);
                    return true;
                }
            } while (partsQueue.Any());
            return false;
        }

        private IEnumerable<ExcelFormulaToken> PrepareMapValueCall(string sourceCell, string sourcePath)
        {
            var tokens = new List<ExcelFormulaToken>();
            tokens.Add(new ExcelFormulaToken("MAPVALUE", ExcelFormulaTokenType.Function, ExcelFormulaTokenSubtype.Start));
            tokens.Add(new ExcelFormulaToken(sourceCell, ExcelFormulaTokenType.Operand, ExcelFormulaTokenSubtype.Range));
            tokens.Add(new ExcelFormulaToken(",", ExcelFormulaTokenType.Argument));
            tokens.Add(new ExcelFormulaToken(sourcePath, ExcelFormulaTokenType.Operand, ExcelFormulaTokenSubtype.Text));
            tokens.Add(new ExcelFormulaToken(null, ExcelFormulaTokenType.Function, ExcelFormulaTokenSubtype.Stop));
            return tokens;
        }

        private IEnumerable<ExcelFormulaToken> PrepareMapItemCall(IEnumerable<ExcelFormulaToken> parentTokens, string sourcePath)
        {
            var tokens = new List<ExcelFormulaToken>();
            tokens.Add(new ExcelFormulaToken("MAPITEM", ExcelFormulaTokenType.Function, ExcelFormulaTokenSubtype.Start));
            tokens.AddRange(parentTokens);
            tokens.Add(new ExcelFormulaToken(",", ExcelFormulaTokenType.Argument));
            tokens.Add(new ExcelFormulaToken(sourcePath, ExcelFormulaTokenType.Operand, ExcelFormulaTokenSubtype.Text));
            tokens.Add(new ExcelFormulaToken(null, ExcelFormulaTokenType.Function, ExcelFormulaTokenSubtype.Stop));
            return tokens;
        }

        private ExcelFormulaToken CreateExcelOperandToken(JToken value)
        {
            string tokenValue;
            ExcelFormulaTokenSubtype subtype;
            switch (value.Type)
            {
                case JTokenType.Object:
                    throw new NotImplementedException();
                case JTokenType.Integer:
                    tokenValue = ((long)value).ToString(CultureInfo.InvariantCulture.NumberFormat);
                    subtype = ExcelFormulaTokenSubtype.Number;
                    break;
                case JTokenType.Float:
                    tokenValue = ((double)value).ToString(CultureInfo.InvariantCulture.NumberFormat);
                    subtype = ExcelFormulaTokenSubtype.Number;
                    break;
                case JTokenType.String:
                    tokenValue = (string)value;
                    subtype = ExcelFormulaTokenSubtype.Text;
                    break;
                case JTokenType.Boolean:
                    tokenValue = (bool)value ? "1" : "0";
                    subtype = ExcelFormulaTokenSubtype.Number;
                    break;
                default:
                    throw new NotSupportedException();
            }
            return new ExcelFormulaToken(tokenValue, ExcelFormulaTokenType.Operand, subtype);
        }

        public EvaluationResult TranslateResult(EvaluationResult result)
        {
            if (result.Value is IEnumerable<ExcelValue> values)
            {
                result.Value = new JArray(values.Select(o => o.InnerValue).ToArray());
            }
            else if (result.Value is decimal d)
            {
                if (d >= int.MinValue && d <= int.MaxValue && (d % 1) == 0)
                    result.Value = Convert.ToInt32(d);
            }
            return result;
        }
    }
}
