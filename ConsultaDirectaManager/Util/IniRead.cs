using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultaDirectaManager.Util {
    public static class IniRead {
        public static string ValorObtener(string Archivo, string Seccion, string Key) {
            var lins = File.ReadLines(Archivo, Encoding.GetEncoding(1252));

            var Sec = lins.SkipWhile(lin => !lin.StartsWith($"[{Seccion}]"))
                           .Skip(1)
                           .TakeWhile(lin => !string.IsNullOrEmpty(lin));

            var r = from l in Sec
                    where l.StartsWith(Key)
                    select new
                    {
                        Key = l.Substring(0, l.IndexOf('=')),
                        Value = l.Substring(l.IndexOf('=') + 1)
                    };

            return r.FirstOrDefault().Value;
        }

        /// <summary>
        /// Get a substring of the first N characters.
        /// </summary>
        public static string Truncate(this string source, int length) {
            if (source.Length > length) {
                source = source.Substring(0, length);
            }
            return source;
        }

        /// <summary>
        /// Get a substring of the first N characters. [Slow]
        /// </summary>
        public static string Truncate2(this string source, int length) {
            return source.Substring(0, Math.Min(length, source.Length));
        }
    }
}
