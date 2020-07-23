using System;
using System.Collections.Generic;

namespace RollYourOwnCryptoWPF
{
    class Hash
    {
        private string _userPassword;
        private string _key;
        private readonly bool _key128;
        private readonly bool _key256;
        private readonly bool _key512;
        private readonly bool _salted;

        public Hash(string pass, bool is128, bool is256, bool is512, bool isSalted)
        {
            _userPassword = pass;
            _key128 = is128;
            _key256 = is256;
            _key512 = is512;
            _salted = isSalted;
        }

        // Bitwise right rotation for 32-bit unsigned values
        private UInt32 BitwiseRight(UInt32 number, UInt32 amount)
        {
            int bits = (int)(amount % 32);
            return (number >> bits) | (number << 32 - bits);
        }

        // Bitwise left rotation for 32-bit unsigned values
        private UInt32 BitwiseLeft(UInt32 number, UInt32 amount)
        {
            int bits = (int)(amount % 32);
            return (number << bits) | (number >> 32 - bits);
        }

        // Convert _userPassword to array of UInt32, salt with Unix-style systime
        private void SaltedConversion(UInt32[] outputArray)
        {
            for (UInt16 i = 0; i < (UInt16)(_userPassword.Length / 4); i++)
            {
                TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1);
                UInt32 CURRENT_TIME = (UInt32)ts.TotalSeconds;
                outputArray[i] =
                BitwiseLeft(((UInt32)_userPassword[4 * i] << (int)(CURRENT_TIME % 11)) + 
                ((UInt32)_userPassword[(4 * i) + 2] << (int)(CURRENT_TIME % 19)), 24) + 
                BitwiseLeft(((UInt32)_userPassword[(4 * i) + 1] << (int)(CURRENT_TIME % 13)) + 
                ((UInt32)_userPassword[(4 * i) + 3] << (int)(CURRENT_TIME % 17)), 16) + 
                BitwiseLeft(((UInt32)_userPassword[(4 * i) + 3] << (int)(CURRENT_TIME % 17)) + 
                ((UInt32)_userPassword[4 * i] << (int)(CURRENT_TIME % 11)), 8) + 
                (((UInt32)_userPassword[(4 * i) + 2]) << (int)(CURRENT_TIME % 19)) + 
                (((UInt32)_userPassword[(4 * i) + 1]) << (int)(CURRENT_TIME % 13));
            }
        }

        // Convert _userPassword to array of UInt32
        private void UnsaltedConversion(UInt32[] outputArray)
        {
            for (UInt16 i = 0; i < (UInt16)(_userPassword.Length / 4); i++)
            {
                outputArray[i] =
                BitwiseLeft(((UInt32)_userPassword[4 * i] << 11) + 
                ((UInt32)_userPassword[(4 * i) + 2] << 7), 24) + 
                BitwiseLeft(((UInt32)_userPassword[(4 * i) + 1] << 13) + 
                ((UInt32)_userPassword[(4 * i) + 3] << 11), 16) + 
                BitwiseLeft(((UInt32)_userPassword[(4 * i) + 3] << 17) + 
                ((UInt32)_userPassword[4 * i] << 13), 8) + 
                (((UInt32)_userPassword[(4 * i) + 2] << 19) + 
                ((UInt32)_userPassword[(4 * i) + 1] << 17));
            }
        }

        // Create message digest using constants and converted password
        private UInt32[] Digest(UInt32[] convertedPassword, UInt32[] result)
        {
            for (UInt16 i = 0; i < (UInt16)(_userPassword.Length / 4) / 16; i++)
            {
                // Loop though each group of 512 bits, compounding changes with each round
                result[0] += BitwiseLeft(result[0], convertedPassword[16 * i] * convertedPassword[16 * i]);
                result[1] += (convertedPassword[(16 * i) + 1] ^ result[0]) * result[0];
                result[2] += (result[0] & convertedPassword[(16 * i) + 2]) + result[1];
                result[3] += ((result[1] ^ result[2]) | convertedPassword[(16 * i) + 3]) - result[2];
                result[4] += BitwiseLeft(result[3] & convertedPassword[(16 * i) + 4], result[4]) + result[3];
                result[5] += BitwiseRight(convertedPassword[(16 * i) + 5], result[4]) + result[4];
                result[6] += (~convertedPassword[(16 * i) + 6] ^ result[5]) * result[5];
                result[7] += BitwiseRight(result[6] ^ result[5], convertedPassword[(16 * i) + 7]) + result[6];
                result[8] += BitwiseLeft(result[5] + convertedPassword[(16 * i) + 8], result[3]) - result[7];
                result[9] += (convertedPassword[(16 * i) + 9] ^ ~result[7]) * result[8];
                result[10] += ((result[7] + convertedPassword[(16 * i) + 10]) - result[8]) & result[9];
                result[11] += BitwiseRight(convertedPassword[(16 * i) + 11], result[3]) + result[10];
                result[12] += (convertedPassword[(16 * i) + 12] + result[2]) * result[11];
                result[13] += (convertedPassword[(16 * i) + 13] & result[1]) - ~result[12];
                result[14] += (result[12] ^ result[0]) + convertedPassword[(16 * i) + 14];
                result[15] += (result[13] - convertedPassword[(16 * i) + 15]) * (result[14] ^ result[12]);
            }
            return result;
        }

        // Convert digest to 512-bit key
        private string Get512(UInt32[] digest)
        {
            string key = "";
            for (UInt16 i = 0; i < 16; i++) { key += digest[i].ToString("X8"); }
            return key;
        }

        // Convert digest to 256-bit key
        private string Get256(UInt32[] digest)
        {
            List<UInt32> to_256 = new List<UInt32>() {
                digest[0] + digest[8],
                digest[1] + digest[9],
                digest[2] + digest[10],
                digest[3] + digest[11],
                digest[4] + digest[12],
                digest[5] + digest[13],
                digest[6] + digest[14],
                digest[7] + digest[15]
            };
            string key = "";
            for (UInt16 i = 0; i < 8; i++) { key += to_256[i].ToString("X8"); }
            return key;
        }

        // Convert digest to 128-bit key
        private string Get128(UInt32[] digest)
        {
            List<UInt32> to_128 = new List<UInt32>() {
                (digest[0] & digest[8]) + (digest[4] ^ digest[12]),
                (digest[1] + digest[9]) * (digest[5] - digest[13]),
                (digest[2] ^ digest[10]) + (digest[6] & digest[14]),
                (digest[3] - digest[11]) * (digest[7] + digest[15])
            };
            string key = "";
            for (UInt16 i = 0; i < 4; i++) { key += to_128[i].ToString("X8"); }
            return key;
        }

        // Combine and order methods to generate key
        public string GenerateKey()
        {
            if (_userPassword != "")
            {
                // Initialized array to hold result of hashing process
                // Digits pulled from the decimal portion of pi
                UInt32[] constants =
                {
                    1415926535, 897932384, 4264338327, 2884197169,
                    3993751058, 2097494459, 2307816406, 2862089986,
                    2803482534, 2117067982, 1480865132, 823066470,
                    3844609550, 582231725, 3594081284, 811174502
                };

                UInt32 passLength = (UInt32)_userPassword.Length;
                _userPassword += passLength.ToString();

                while (_userPassword.Length % 64 != 0)
                {
                    _userPassword += "0";
                }

                UInt32[] passwordList = new UInt32[(UInt16)(_userPassword.Length / 4)];

                if (_salted)
                {
                    SaltedConversion(passwordList);
                }
                else 
                {
                    UnsaltedConversion(passwordList);
                }

                if (_key128)
                {
                    _key = Get128(Digest(passwordList, constants));
                }
                else if (_key256)
                {
                    _key = Get256(Digest(passwordList, constants));
                }
                else if (_key512)
                {
                    _key = Get512(Digest(passwordList, constants));
                }

                return _key;
            }
            else { return ""; }
        }
    }
}
