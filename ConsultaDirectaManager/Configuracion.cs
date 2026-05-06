using Newtonsoft.Json;
using System.IO;

namespace ConsultaDirectaManager;

public class Configuracion
{
    public string AppDataPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ConsultaDirectaCreator");
    public string CfgPth { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ConsultaDirectaCreator", "Configuration");
    public SQLConexionInfo CnxInfo { get; set; }

    public string ArchCfg { get; set; }
    public Configuracion()
    {
        ArchCfg = Path.Combine(CfgPth, "cfg.json");
    }
    public Configuracion(SQLConexionInfo cnxInfo)
    {
        ArchCfg = Path.Combine(CfgPth, "cfg.json");
        CnxInfo = cnxInfo;
    }

    public Configuracion CargarCfg()
    {
        if (File.Exists(ArchCfg))
        {
            string lxJsonText = File.ReadAllText(ArchCfg);
            try
            {
                var lxCfg = JsonConvert.DeserializeObject<Configuracion>(lxJsonText);
                return lxCfg;
            }
            catch (Exception)
            {
                return new Configuracion();
            }
        }
        else
        {
            return new Configuracion();
        }
    }

    public void GuardarCfg(SQLConexionInfo sqlCnxInfo)
    {
        CnxInfo = sqlCnxInfo;
        if (!Directory.Exists(CfgPth))
        {
            Directory.CreateDirectory(CfgPth);
        }

        File.WriteAllText(ArchCfg, JsonConvert.SerializeObject(this));
    }
}
