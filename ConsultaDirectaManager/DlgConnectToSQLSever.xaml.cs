using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace ConsultaDirectaManager
{
    /// <summary>
    /// Lógica de interacción para ConnectSQLSever.xaml
    /// </summary>
    public partial class ConnectSQLSever : Window
    {

        private bool _EsWinAut = false;

        public Configuracion Cfg { get; set; }

        public ConnectSQLSever()
        {
            InitializeComponent();

            Cfg = new Configuracion();
            Cfg = Cfg.CargarCfg();

            if (Cfg.CnxInfo != null)
            {
                SetScr(Cfg.CnxInfo);
            }
        }
        public void SetScr(SQLConexionInfo cnInfo)
        {
            txtServidor.Text = cnInfo.Servidor;
            txtBasedeDatos.Text = cnInfo.NombreBasedeDatos;
            chkEsWindowsAutentication.IsChecked = cnInfo.EsWinAut;
            if (!cnInfo.EsWinAut)
            {
                txtUsr.Text = cnInfo.Usuario;
                txtPsw.Password = cnInfo.Password;
            }
        }

        public SQLConexionInfo SQLCnxInfo { get; set; }

        private void chkEsWindowsAutentication_Checked(object sender, RoutedEventArgs e)
        {
            _EsWinAut = true;
        }

        private void chkEsWindowsAutentication_Unchecked(object sender, RoutedEventArgs e)
        {
            _EsWinAut = false;
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void cmdOk_Click(object sender, RoutedEventArgs e)
        {

            SQLCnxInfo = _EsWinAut
                ? new SQLConexionInfo(txtServidor.Text, txtBasedeDatos.Text)
                : new SQLConexionInfo(txtServidor.Text, txtBasedeDatos.Text, txtUsr.Text, txtPsw.Password);
            DialogResult = true;
            Cfg.GuardarCfg(SQLCnxInfo);
        }

        private void cmdTest_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;

            SQLCnxInfo = _EsWinAut
                ? new SQLConexionInfo(txtServidor.Text, txtBasedeDatos.Text)
                : new SQLConexionInfo(txtServidor.Text, txtBasedeDatos.Text, txtUsr.Text, txtPsw.Password);

            if (TestConection(SQLCnxInfo) == true)
            {
                MessageBox.Show($"Conexión Exitosa\n Servidor: {txtServidor.Text}", "Prueba de Conexión", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Error Conexión \n Servidor: {txtServidor.Text}", "Prueba de Conexión", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            this.Cursor = Cursors.Arrow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Cfg is null) return;
            if (Cfg.CnxInfo == null) return;

            txtBasedeDatos.Text = Cfg.CnxInfo.NombreBasedeDatos;
            txtServidor.Text = Cfg.CnxInfo.Servidor;
            chkEsWindowsAutentication.IsChecked = Cfg.CnxInfo.EsWinAut;
            txtUsr.Text = Cfg.CnxInfo.Usuario;
            txtPsw.Password = Cfg.CnxInfo.Password;

        }

        private bool TestConection(SQLConexionInfo sqlCnxInfo)
        {

            using (SqlConnection lxCnx = new SqlConnection(sqlCnxInfo.ObtenerStringDeConexion()))
            {
                try
                {
                    lxCnx.Open();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Cfg.GuardarCfg(SQLCnxInfo);
        }
    }
}
