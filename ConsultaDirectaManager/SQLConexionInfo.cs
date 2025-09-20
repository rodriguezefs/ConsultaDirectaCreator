namespace ConsultaDirectaManager;

public class SQLConexionInfo
{
    public SQLConexionInfo(string servidor, string nombreBasedeDatos)
    {
        Servidor = servidor;
        NombreBasedeDatos = nombreBasedeDatos;
        EsWinAut = true;
        Usuario = "";
        Password = "";
    }

    public SQLConexionInfo(string servidor, string nombreBasedeDatos, string usuario, string password)
    {
        Servidor = servidor;
        NombreBasedeDatos = nombreBasedeDatos;
        EsWinAut = false;
        Usuario = usuario;
        Password = password;
    }

    public string ObtenerStringDeConexion()
    {
        if (EsWinAut)
        {
            return $"Server={Servidor};Database={NombreBasedeDatos};Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;";
        }
        else
        {
            return $"Server={Servidor};Database={NombreBasedeDatos};User Id={Usuario};Password={Password};TrustServerCertificate=True;Encrypt=False;";
        }
    }

    public bool EsWinAut { get; set; }
    public string NombreBasedeDatos { get; set; }
    public string Password { get; set; }
    public string Servidor { get; set; }
    public string Usuario { get; set; }
}
