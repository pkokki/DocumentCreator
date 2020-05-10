using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExcelExpressions.Eval
{
    public class CellAddress
    {
        public CellAddress(string address)
        {
            var parts = SplitAddress(address);
            Row = int.Parse(parts[1]);
            Column = parts[0].ToUpperInvariant();
        }

        public int Row { get; }
        public string Column { get; }

        private static string[] SplitAddress(string address)
        {
            int i;
            for (i = 0; i < address.Length; i++)
                if (address[i] >= '0' && address[i] <= '9')
                    break;
            if (i == address.Length)
                throw new InvalidOperationException("Bad cell address.");
            return new[] {
                address.Substring(0, i),
                address.Substring(i)
            };
        }
    }
}
