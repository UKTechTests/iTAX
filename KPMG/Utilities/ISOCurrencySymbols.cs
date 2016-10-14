using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace KPMG.Utilities
{
    public class ISOCurrencySymbols
    {
        private HashSet<string> symbols;

        public const string CURRENCY_ERROR = "Currency Code must be a valid ISO 4217";

        private static ISOCurrencySymbols instance;

        // The valid symbols are loaded only once using a private constructor
        private ISOCurrencySymbols()
        {
            symbols = new HashSet<string>();

            foreach (CultureInfo cultureInfo in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                RegionInfo regionInfo = new RegionInfo(cultureInfo.LCID);
                symbols.Add(regionInfo.ISOCurrencySymbol);
            }   
        }

        public static ISOCurrencySymbols Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ISOCurrencySymbols();
                }
                return instance;
            }
        }

        public bool IsISO4217(string symbol)
        {
            return symbols.Contains(symbol);
        }

    }
}