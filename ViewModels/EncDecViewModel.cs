using System;
using System.Timers;
using System.Threading.Tasks;

namespace RollYourOwnCryptoWPF.ViewModels
{
    class EncDecViewModel : Observable
    {
        private string _key;
        private string _path;
        private double _progress;
        private EncDec _encDec;
        private Timer clock;
        private const int _timerInterval = 13;

        public string Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
                OnPropertyChanged("Key");
            }
        }

        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
                OnPropertyChanged("Path");
            }
        }

        public double Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                _progress = value;
                OnPropertyChanged("Progress");
            }
        }

        public async Task EncryptDecrypt()
        {
            if ((_key.Length > 0 && _key.Length % 32 == 0) && (_path != ""))
            {
                clock = new Timer(_timerInterval);
                clock.Elapsed += SyncProgress;
                clock.AutoReset = true;
                clock.Start();

                _encDec = new EncDec(_key, _path);
                await Task.Run(() => _encDec.FileEncryptDecrypt());

                clock.Stop();
                clock.Dispose();
            }
        }

        private async void SyncProgress(object sender, EventArgs e)
        {
            Progress = await Task.Run(() => _encDec.GetProgress());
        }
    }
}
