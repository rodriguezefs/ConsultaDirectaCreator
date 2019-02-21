using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultaDirectaManager.Util {
    public static class IniRead {
        public static string ValorObtener(string Archivo, string Seccion, string Key) {
            var vals = File.ReadLines(Archivo, Encoding.GetEncoding(1252))
                           .SkipWhile(lin => !lin.StartsWith($"[{Seccion}]"))
                           .Skip(1)
                           .TakeWhile(lin => !string.IsNullOrEmpty(lin))
                           .Select(lin => new {
                                Key = lin.Substring(0, lin.IndexOf('=')),
                                Value = lin.Substring(lin.IndexOf('=') + 2)
                            })
                            //.FirstOrDefault(lin => lin.Contains<string>(Key))
                            ;

            return vals.ToString();
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
