using Microsoft.Win32;
using RollYourOwnCryptoWPF.ViewModels;
using System;
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
            try
            {
                byte[] keyList = new byte[tbxKey.Text.Length / 2];
                for (UInt16 i = 0; i < tbxKey.Text.Length / 2; i++)
                {
                    keyList[i] = Convert.ToByte(tbxKey.Text.Substring(2 * i, 2), 16);
                }
                if (tbxKey.Text != "" && tbxKey.Text.Length % 32 == 0)
                {
                    await _main.EncDecVM.EncryptDecrypt();
                    MessageBox.Show("Operation completed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    _main.EncDecVM.Progress = 0;
                }
                else
                {
                    MessageBox.Show("Key length is invalid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Key format is invalid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
