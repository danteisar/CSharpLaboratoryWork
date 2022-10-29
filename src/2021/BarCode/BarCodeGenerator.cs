using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace LaboratoryWorks.BarCode
{
    public class BarCode
    {
        #region ctor

        public BarCode(int code)
        {
            Code = code;
        }

        public BarCode(string bar)
        {
            Bar = bar;
        }

        #endregion

        private string _bar;
        private int _code;

        public int Code
        {
            get => _code; 
            set
            {
                _code = value;
                _bar = BarCodeGenerator.Generate(value, _bar);
            }
        }
        public string Bar
        {
            get => _bar; set
            {
                _bar = value;
                _code = BarCodeGenerator.Generate(value);
            }
        }


        #region implicit cast

        public static implicit operator int(BarCode value) => value.Code;

        public static implicit operator string(BarCode value) => value.Bar;

        public static implicit operator BarCode(int value) => new BarCode(value);

        public static implicit operator BarCode(string value) => new BarCode(value);

        #endregion
    }

    public static class BarCodeGenerator
    {
        public const char Spliter = ':';

        public static char Build(string bar) => bar switch
        {
            "00" => ' ',
            "01" => '│',
            "10" => '║',
            "11" => '▌',
            _ => throw new NotImplementedException()
        };

        public static string Generate(int code)
        {
            var bar = Convert.ToString(code, 2);
            if (bar.Length % 2 > 0) bar = $"0{bar}";

            //var regular = new Regex(".{0,2}");
            //var matches = regular.Matches(bar);
            //var result = string.Empty;
            //foreach (Match match in matches)
            //{
            //    result += Match(match);
            //}

            var result = string.Empty;
            for (int i = 0; i < bar.Length - 1; i++)
            {
                result += Build(bar.Substring(i, 2));
            }
            return $"{Spliter}{result}{Spliter}";
        }

        public static string Generate(int code, string oldCode)
        {
            var newCode = Generate(code);

            if (string.IsNullOrEmpty(oldCode)) return newCode;

            for (int i = 1; i < oldCode.Length; i++)
            {
                if (oldCode[i] == Spliter && i != oldCode.Length - 1)
                {
                    return newCode + oldCode.Substring(i + 1, oldCode.Length - 1);
                }
            }

            return newCode;
        }

        public static int Generate(string bar)
        {
            //var regular = new Regex(@$"{Spliter}[^{Spliter}]*{Spliter}");
            //var result = regular.Matches(bar)[0].Value.Replace(Spliter, '');

            bar = bar.Substring(1, bar.Length - 1);
            var count = bar.IndexOf(Spliter);

            var result = bar.Substring(0, count);

            result = result.Replace(" ", "00").Replace("│", "01").Replace("║", "10").Replace("▌", "11");           
            return Convert.ToInt32(result, 2);
        }       
    }
}   
