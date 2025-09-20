using ConsultaDirectaManager.Util;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Microsoft.Win32;
using SharpCompress.Common;
using SharpCompress.Readers;
using SharpCompress.Writers;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Path = System.IO.Path;

namespace ConsultaDirectaManager;

/// <summary>
/// Lógica de interacción para MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const string CFG_INI = "cfg.ini";
    public MainWindow()
    {
        InitializeComponent();
        Cfg = new Configuracion();
        SQLCnx = Cfg.Cnx;
    }

    private Configuracion Cfg { get; set; }
    private SQLConexionInfo SQLCnx { get; set; }
    private void ArchivoAbrir()
    {
        OpenFileDialog lxDlg = new OpenFileDialog();
        lxDlg.Filter = "Todos (*.*)|*.*|Latis (*.latis)|*.latis|Zip (*.zip)|*.zip";
        lxDlg.DefaultExt = ".latis";

        if (lxDlg.ShowDialog() == true)
        {
            string lxNomArch = lxDlg.FileName;
            txtArchLatis.Text = lxNomArch;

            string lxFldrDst = Path.Combine(@"C:\temp\", Path.GetRandomFileName());

            ExtraerTodoslosArchivos(lxNomArch, lxFldrDst);

            string lxCfgFile = Path.Combine(lxFldrDst, CFG_INI);
            string lxCfgText = "";
            if (File.Exists(lxCfgFile))
            {
                lxCfgText = File.ReadAllText(lxCfgFile, Encoding.GetEncoding(1252));
                txtCfg.Text = lxCfgText;
            }

            string lxNomScript = lxCfgText.Split('\n')
                                          .FirstOrDefault(x => x.Contains("script", StringComparison.CurrentCultureIgnoreCase))
                                          .ToString()
                                          .Split('=')[1];

            string lxNomScriptFile = Path.Combine(lxFldrDst, lxNomScript);
            if (!File.Exists(lxNomScriptFile))
            {
                lxNomScriptFile = Directory.GetFiles(lxFldrDst, "*.sql").FirstOrDefault();
            }

            if (File.Exists(lxNomScriptFile))
            {
                string lxScriptText = File.ReadAllText(lxNomScriptFile, Encoding.GetEncoding(1252));
                txtSQL.Text = lxScriptText;
            }

            try
            {
                //Eliminar directorio temporal
                Directory.Delete(lxFldrDst, true);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.Message, "Abrir", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void ArchivoGuardar(string NomArchLatis)
    {
        //Crear carpteta temporal
        string lxNomCsl = Path.GetFileNameWithoutExtension(NomArchLatis);
        string lxBaseDir = (Path.GetDirectoryName(NomArchLatis));

        string lxDir = Path.Combine(lxBaseDir, lxNomCsl + "_Qry");
        if (!Directory.Exists(lxDir)) { Directory.CreateDirectory(lxDir); }

        //Guardar cfg.ini
        string lxArchCfgIni = Path.Combine(lxDir, CFG_INI);
        txtCfg.Save(lxArchCfgIni);

        //Guardar script.sql
        string lxScriptSQL = IniRead.ValorObtener(lxArchCfgIni, "CFG", "Script");
        if (string.IsNullOrEmpty(lxScriptSQL))
        {
            lxScriptSQL = Path.Combine(lxDir, lxNomCsl + ".sql");
        }
        else
        {
            lxScriptSQL = Path.Combine(lxDir, lxScriptSQL);
        }

        txtSQL.Save(lxScriptSQL);

        //Comprimir carpeta temporal en formato .zip con extensión .latis
        using (var zip = File.OpenWrite(NomArchLatis))
        {
            using (var zipWriter = WriterFactory.Open(zip, ArchiveType.Zip, CompressionType.Deflate))
            {
                string[] lxFileList = Directory.GetFiles(lxDir);
                foreach (var filePath in lxFileList)
                {
                    zipWriter.Write(Path.GetFileName(filePath), filePath);
                }
            }
        }
        StatusBarSet($"Archivo '{NomArchLatis}' Guardado.");

        try
        {
            //Eliminar directorio temporal
            Directory.Delete(lxDir, true);
        }
        catch (Exception ex)
        {
            _ = MessageBox.Show(ex.Message, "Guardar", MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }

    private void ArchivoNuevo()
    {


        SaveFileDialog lxDlg = new SaveFileDialog
        {
            DefaultExt = ".latis",
            AddExtension = true,
            Filter = "Todos (*.*)|*.*|Latis (*.latis)|*.latis|Zip (*.zip)|*.zip"
        };

        if (lxDlg.ShowDialog() == true)
        {
            string lxNomArch = lxDlg.FileName;
            txtArchLatis.Text = lxNomArch;

            string lxNomScript = Path.GetFileNameWithoutExtension(lxNomArch);

            string lxTemplate =
            "[CFG]\n" +
            "Ver=2\n" +
            "Dsc=\n" +
            "Script=\n" +
            "RtnReg=1\n" +
            "\n" +
            "[Ifo]\n" +
            ";3000 = Compras, 3200 = Inventario, 3300 = Ventas \n" +
            "Mdl=3200\n" +
            "NomQry=\n" +
            "IdQry=" + lxNomScript.Truncate(10) + "\n" +
            "\n" +
            "[Pmt]\n" +
            ";Pmtn=Nombre Publico|Nombre Parametro|Tipo|Default\n" +
            ";Tipo: Ver cosntantes de ADO\n" +
            ";      7 = adDate\n" +
            ";    200 = adVarChar\n" +
            "Pmt1=Fecha desde|FchDes|7|01/01/2019\n" +
            "Pmt2=Fecha hasta|FchHas|7|01/01/2019";

            txtCfg.Text = lxTemplate;

            txtSQL.Text =
                "SET NOCOUNT ON; \n\n" +
                "-- Escribir Query aquí\n\n" +
                "SET NOCOUNT OFF; \n ";
        }
    }

    private void Attach_Events()
    {
        txtSQL.TextArea.DocumentChanged += TextAreaDocumentChanged;
    }

    private void Detach_Events()
    {
        txtSQL.TextArea.DocumentChanged -= TextAreaDocumentChanged;
    }

    private void ExtraerTodoslosArchivos(string NomArch, string FldrDst)
    {

        if (!Directory.Exists(FldrDst)) { Directory.CreateDirectory(FldrDst); }

        using (Stream stream = File.OpenRead(NomArch))
        {
            var reader = ReaderFactory.Open(stream);
            while (reader.MoveToNextEntry())
            {
                if (!reader.Entry.IsDirectory)
                {
                    Console.WriteLine(reader.Entry.Key);

                    reader.WriteEntryToDirectory(FldrDst, new ExtractionOptions() { ExtractFullPath = false, Overwrite = true });
                }
            }
        }
    }

    private void IniCnx()
    {
        if (Cfg is null)
        {
            Cfg = new Configuracion();
        }

        if (Cfg.Cnx is null && SQLCnx.Servidor != null) Cfg.Cnx = new SQLConexionInfo(SQLCnx.Servidor, SQLCnx.NombreBasedeDatos);

        Cfg.Cnx.EsWinAut = SQLCnx.EsWinAut;
        Cfg.Cnx.Servidor = SQLCnx.Servidor;
        Cfg.Cnx.NombreBasedeDatos = SQLCnx.NombreBasedeDatos;
        Cfg.Cnx.Usuario = SQLCnx.Usuario;
        Cfg.Cnx.Password = SQLCnx.Password;
    }

    void ShowDataTable(DataTable dt)
    {
        StringBuilder lxSB = new();
        if (dt == null) return;

        string lxPfj = "";
        foreach (DataColumn lxCol in dt.Columns)
        {
            lxSB.Append($"{lxPfj}{lxCol.ColumnName}");
            lxPfj = "\t";
        }
        lxSB.Append('\n');

        lxPfj = "";
        foreach (DataRow lxRow in dt.Rows)
        {

            foreach (DataColumn lxCol in lxRow.Table.Columns)
            {
                string lxVlr = "";
                switch (lxCol.DataType.ToString())
                {
                    case "System.DBNull":
                        lxVlr = "NULL";
                        break;
                    case "System.String":
                    case "System.Guid":
                        lxVlr = lxRow[lxCol.ColumnName] == DBNull.Value
                            ? "NULL"
                            : $"'{lxRow[lxCol.ColumnName].ToString().Replace("'", "''").Trim()}'";
                        break;
                    case "System.Int32":
                        lxVlr = lxRow[lxCol.ColumnName] == DBNull.Value
                            ? "NULL"
                            : $"{lxRow[lxCol.ColumnName]}";
                        break;
                    case "System.Boolean":
                        lxVlr = lxRow[lxCol.ColumnName] == DBNull.Value
                            ? "NULL"
                            : $"{((bool)lxRow[lxCol.ColumnName] ? 1 : 0)}";
                        break;
                    case "System.DateTime":
                        lxVlr = lxRow[lxCol.ColumnName] == DBNull.Value
                            ? "NULL"
                            : $"'{lxRow[lxCol.ColumnName]:yyyyMMdd hh:mm:ss}'";
                        break;
                    case "System.Decimal":
                        lxVlr = lxRow[lxCol.ColumnName] == DBNull.Value
                              ? "NULL"
                              : string.Format(new CultureInfo("en-US"), "{0:###0.0000}", lxRow[lxCol.ColumnName]);
                        break;
                    default:
                        lxVlr = lxRow[lxCol.ColumnName] == DBNull.Value
                            ? "NULL"
                            : $"{lxRow[lxCol.ColumnName]}";
                        break;
                };

                lxSB.Append($"{lxPfj}{lxVlr}");
                lxPfj = "\t";

            }
            lxSB.Append('\n');
        }

        if (dt.Rows.Count > 0)

        {
            txtRslt.Text = lxSB.ToString();
        }
    }

    private void SQLServerConectar()
    {
        ConnectSQLSever lxDlgCnx = new ConnectSQLSever();

        lxDlgCnx.Cfg = Cfg;
        lxDlgCnx.ShowDialog();

        if (lxDlgCnx.DialogResult.HasValue && lxDlgCnx.DialogResult.Value)
        {
            //OK
            Cursor = Cursors.Wait;
            SQLCnx = lxDlgCnx.SQLCnxInfo;
            try
            {
                IniCnx();
            }
            catch (Exception ex)
            {
                MessageBoxResult r = MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Cursor = Cursors.Arrow;
            }
        }
    }

    private void SQLServerEjecutarSQL()
    {
        if (SQLCnx == null)
        {
            txtRslt.Text = "No hay conexión.";
            return;
        }

        string lxQry = txtSQL.Text;
        try
        {
            var starTime = DateTime.Now;
            DataTable lxDT = new();
            using (SqlConnection lxCnx = new(SQLCnx.ObtenerStringDeConexion()))
            {
                SqlCommand lxCmd = new(lxQry, lxCnx)
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = 300
                };

                using SqlDataAdapter lxDA = new(lxCmd);
                lxDA.Fill(lxDT);
            }
            var endTime = DateTime.Now;

            StatusBarDurationSet(endTime - starTime);
            
            ShowDataTable(lxDT);
            gridRslt.DataContext = lxDT;
        }
        catch (Exception ex)
        {
            txtRslt.Text = ex.Message;
            StatusBarSet(ex.Message);
        }
    }

    private void StatusBarLocationSet()
    {
        int lxOffset = txtSQL.CaretOffset;
        TextLocation lxTxtLocation = txtSQL.Document.GetLocation(lxOffset);

        txtLocation.Text = $"Lin: {lxTxtLocation.Line} Col: {lxTxtLocation.Column}";
    }

    private void StatusBarSet(string msg = "")
    {
        txtStatus.Text = msg;
    }
    private void StatusBarDurationSet(TimeSpan duration)
    {
        txtDuration.Text = $"{duration:hh\\:mm\\:ss}";
        //FrameworkContentElement.ToolTip tt = new();
        //tt.Content = txtDuration.Text;
        //txtDuration.ToolTip = tt;
    }
    #region "Eventos de la forma"
    private void CmdAbir_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = true;
    }

    private void CmdAbir_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        ArchivoAbrir();
    }

    private void CmdConectarASQLServer_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = true;
    }

    private void CmdConectarASQLServer_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        SQLServerConectar();
    }

    private void CmdEjecutarSQL_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = true;
    }

    private void CmdEjecutarSQL_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        SQLServerEjecutarSQL();
    }

    private void CmdGuardar_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = txtCfg.Text.Length > 0 && txtSQL.Text.Length > 0 && txtArchLatis.Text.Length > 0;
    }

    private void CmdGuardar_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        ArchivoGuardar(txtArchLatis.Text);
    }

    private void CmdNuevo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = true;
    }

    private void CmdNuevo_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        ArchivoNuevo();
    }

    private void CmdSalir_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = true;
    }

    private void CmdSalir_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
    private void TxtSQL_KeyUp(object sender, KeyEventArgs e)
    {
        StatusBarLocationSet();
    }

    private void TxtSQL_MouseUp(object sender, MouseButtonEventArgs e)
    {
        StatusBarLocationSet();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        Detach_Events();
    }
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {

        Attach_Events();

        //var resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();

        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ConsultaDirectaManager.Resources.sql.xshd"))
        {
            using (var reader = new System.Xml.XmlTextReader(stream))
            {
                txtSQL.SyntaxHighlighting =
                    HighlightingLoader.Load(reader,
                    HighlightingManager.Instance);
            }
        }
        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ConsultaDirectaManager.Resources.ini.xshd"))
        {
            using (var reader = new System.Xml.XmlTextReader(stream))
            {
                txtCfg.SyntaxHighlighting =
                    HighlightingLoader.Load(reader,
                    HighlightingManager.Instance);
            }
        }
    }
    #endregion
    void TextAreaDocumentChanged(object sender, EventArgs e)
    {
        TabConsulta.Header = "_2 Consulta (x)";
    }
}
