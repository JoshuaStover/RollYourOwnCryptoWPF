using System;
using System.IO;

namespace RollYourOwnCryptoWPF
{
    class EncDec
    {
        private readonly string _key;
        private readonly string _path;
        private readonly byte[] _keyList;
        private double _progress;

        public EncDec(string keyString, string pathString)
        {
            _key = keyString;
            _path = pathString;
            _keyList = KeyToByteArray();
            _progress = 0;
        }
        
        private byte[] KeyToByteArray()
        {
            byte[] keyList = new byte[_key.Length / 2];
            for (UInt16 i = 0; i < _key.Length / 2; i++)
            {
                keyList[i] = Convert.ToByte(_key.Substring(2 * i, 2), 16);
            }
            return keyList;
        }


        public void FileEncryptDecrypt()
        {
            if ((_key.Length > 0 && _key.Length % 32 == 0) && (_path != ""))
            {
                byte[] fileContents = File.ReadAllBytes(_path);

                for (UInt32 i = 0; i < fileContents.Length; i++)
                {
                    for (UInt16 j = 0; j < _keyList.Length; j++)
                    {
                        fileContents[i] ^= _keyList[j];
                    }
                    _progress = 50 * ((double)i / (double)(fileContents.Length - 1));
                }

                FileStream f = new FileStream(_path, FileMode.Open, FileAccess.Write);

                for (UInt32 i = 0; i < fileContents.Length; i++)
                {
                    f.WriteByte(fileContents[i]);
                    _progress = 50 + (50 * ((double)i / (double)(fileContents.Length - 1)));
                }

                f.Close();
            }
        }

        public double GetProgress()
        {
            return _progress;
        }
    }
}
