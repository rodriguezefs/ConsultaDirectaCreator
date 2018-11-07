using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultaDirectaManager.Util {
    public static class IniRead {
        public static string ValorObtener(string Archivo, string Seccion, string Key) {
            var vals = File.ReadLines(Archivo)
                           .SkipWhile(lin => !lin.StartsWith($"[{Seccion}]"))
                           .Skip(1)
                           .TakeWhile(lin => !string.IsNullOrEmpty(lin))
                           .Select(lin => new {
                                Key = lin.Substring(0, lin.IndexOf('=')),
                                Value = lin.Substring(lin.IndexOf('=') + 2)
                            })
                            .FirstOrDefault(lin => lin.Contains<string>(Key));

            return vals;
        }
    }
}
