using System.Threading.Tasks;

namespace RollYourOwnCryptoWPF.ViewModels
{
    class HashViewModel : Observable
    {
        private string _userPassword;
        private string _key;
        private bool _key128;
        private bool _key256;
        private bool _key512;
        private bool _salted;
        private Hash _hash;

        public string UserPassword
        {
            get
            {
                return _userPassword;
            }

            set
            {
                _userPassword = value;
                OnPropertyChanged("UserPassword");
            }
        }

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

        public bool Key128
        {
            get { return _key128; }
            set
            {
                _key128 = value;
                if (value)
                {
                    _key256 = false;
                    _key512 = false;
                }
                OnPropertyChanged("Key128");
            }
        }

        public bool Key256
        {
            get { return _key256; }
            set
            {
                _key256 = value;
                if (value)
                {
                    _key128 = false;
                    _key512 = false;
                }
                OnPropertyChanged("Key256");
            }
        }

        public bool Key512
        {
            get { return _key512; }
            set
            {
                _key512 = value;
                if (value)
                {
                    _key128 = false;
                    _key256 = false;
                }
                OnPropertyChanged("Key512");
            }
        }

        public bool Salted
        {
            get
            {
                return _salted;
            }
            set
            {
                _salted = value;
                OnPropertyChanged("Salted");
            }
        }

        public void GetKey()
        {
            if (_userPassword != "")
            {
                Task.Run(async () =>
                {
                    _hash = new Hash(_userPassword, _key128, _key256, _key512, _salted);
                    await Task.Run(() => Key = _hash.GenerateKey());
                });
            }
        }
    }
}
