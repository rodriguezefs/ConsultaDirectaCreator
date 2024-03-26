using System.Data.SqlClient;
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
        }

        private void cmdTest_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;

            SQLCnxInfo = _EsWinAut
                ? new SQLConexionInfo(txtServidor.Text, txtBasedeDatos.Text)
                : new SQLConexionInfo(txtServidor.Text, txtBasedeDatos.Text, txtUsr.Text, txtPsw.Password);

            if (TestConection(SQLCnxInfo) == true)
            {
                MessageBox.Show("Conexión Exitosa", "Prueba de Conexión", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Error Conexión", "Prueba de Conexión", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            this.Cursor = Cursors.Arrow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Cfg is null) return;
            if (Cfg.Cnx == null) return;

            txtBasedeDatos.Text = Cfg.Cnx.NombreBasedeDatos;
            txtServidor.Text = Cfg.Cnx.Servidor;
            chkEsWindowsAutentication.IsChecked = Cfg.Cnx.EsWinAut;
            txtUsr.Text = Cfg.Cnx.Usuario;
            txtPsw.Password = Cfg.Cnx.Password;

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
            Cfg.GuardarCfg();
        }
    }
}
