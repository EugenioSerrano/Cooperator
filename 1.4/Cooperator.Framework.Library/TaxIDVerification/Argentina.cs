using System;
using System.Collections.Generic;
using System.Text;

namespace Cooperator.Framework.Library.TaxIDVerification
{
    /// <summary>
    /// 
    /// </summary>
    public static class Argentina
    {

        /// <summary>
        /// Funcion de verificacion de Cuit.
        /// </summary>
        public static bool IsValidCuit(string cuitNumber, bool allowNullCuit)
        {
            //El formato de Cuit debe ser: 99-99999999-9
            if (cuitNumber == null || string.IsNullOrEmpty(cuitNumber.Trim()) || cuitNumber == "  -        - " || cuitNumber == "  -        -")
            {
                return allowNullCuit;
            }

            string DgiKey, Result;
            int Counter, Row;
            if (cuitNumber.IndexOf(" ") != -1) return false;
            if (cuitNumber.Length != 13) return false;
            DgiKey = cuitNumber.Substring(0, 2) + cuitNumber.Substring(3, 8);
            Counter = 0;
            Row = 2;
            for (int cc = 10; cc >= 1; cc--)
            {
                Counter = Counter + Convert.ToInt32(DgiKey.Substring(cc - 1, 1)) * Row;
                if (Row == 7)
                    Row = 2;
                else
                    Row = Row + 1;
            }
            Result = Convert.ToString((Counter * 10) % 11).Trim();

            if (Result.Substring(Result.Length - 1) == cuitNumber.Substring(cuitNumber.Length - 1))
                return true;
            else
                return false;
        }
    }
}
