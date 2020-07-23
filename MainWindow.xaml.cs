using Microsoft.Win32;
using RollYourOwnCryptoWPF.ViewModels;
using System.Windows;


namespace RollYourOwnCryptoWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel _main = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _main;
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                tbxFile.Text = ofd.FileName;
            }
        }

        private void btnGetKey_Click(object sender, RoutedEventArgs e)
        {
            _main.HashVM.GetKey();
        }

        private void tbxKey_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            _main.EncDecVM.Key = tbxKey.Text;
        }

        private async void btnEncDec_Click(object sender, RoutedEventArgs e)
        {
            await _main.EncDecVM.EncryptDecrypt();
        }
    }
}
