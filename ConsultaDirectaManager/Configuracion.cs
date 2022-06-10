using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace ConsultaDirectaManager
{
    public class Configuracion
    {
        public string AppDataPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ConsultaDirectaCreator");
        public string CfgPth { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ConsultaDirectaCreator", "Configuration");

        public string ArchCfg { get; set; }
        public Configuracion()
        {
            ArchCfg = Path.Combine(CfgPth, "cfg.json");
        }

        public SQLConexionInfo Cnx { get; set; }

        public Configuracion CargarCfg()
        {
            if (File.Exists(ArchCfg))
            {
                string lxJsonText = File.ReadAllText(ArchCfg);
                return JsonConvert.DeserializeObject<Configuracion>(lxJsonText);
            }
            else
            {
                return new Configuracion();
            }
        }

        public void GuardarCfg()
        {
            if (!Directory.Exists(CfgPth))
            {
                Directory.CreateDirectory(CfgPth);
            }

            File.WriteAllText(ArchCfg, JsonConvert.SerializeObject(this));
        }
    }
}
