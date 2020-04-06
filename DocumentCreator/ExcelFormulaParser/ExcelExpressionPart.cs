﻿using System;
using System.Linq;
using System.Globalization;

namespace DocumentCreator.ExcelFormulaParser
{
    public class ExcelExpressionPart
    {
        private readonly ExcelFormulaToken originalToken;
        private readonly CultureInfo culture;
        private ExcelValue value;

        public ExcelExpressionPart(ExcelFormulaToken token, CultureInfo culture)
        {
            this.originalToken = token;
            this.TokenType = token.Type;
            this.culture = culture;
            if (token.Type == ExcelFormulaTokenType.OperatorInfix 
                || token.Type == ExcelFormulaTokenType.OperatorPrefix
                || token.Type == ExcelFormulaTokenType.OperatorPostfix
                )
                this.Operator = token.Value;
        }
        public ExcelExpressionPart(ExcelValue value)
        {
            this.value = value;
            this.TokenType = ExcelFormulaTokenType.Operand;
            this.Operator = null;
        }

        public ExcelFormulaTokenType TokenType { get; }

        public string Operator { get; }

        public bool IsPrefixOperator(params string[] opers)
        {
            return TokenType == ExcelFormulaTokenType.OperatorPrefix && opers.Contains(originalToken.Value);
        }
        public bool IsPostfixOperator(params string[] opers)
        {
            return TokenType == ExcelFormulaTokenType.OperatorPostfix && opers.Contains(originalToken.Value);
        }
        public bool IsBinaryOperator(params string[] opers)
        {
            return TokenType == ExcelFormulaTokenType.OperatorInfix && opers.Contains(originalToken.Value);
        }
        public bool IsComparisonOperator => 
            TokenType == ExcelFormulaTokenType.OperatorInfix && originalToken.Subtype == ExcelFormulaTokenSubtype.Logical;
        
        public ExcelValue Value
        {
            get
            {
                if (value == null)
                {
                    if (originalToken != null && originalToken.Type != ExcelFormulaTokenType.Operand)
                        throw new InvalidOperationException($"Token of type {originalToken.Type} do not have Value");
                    value = ExcelValue.Create(originalToken, culture);
                }
                return value;
            }
        }
    }
}