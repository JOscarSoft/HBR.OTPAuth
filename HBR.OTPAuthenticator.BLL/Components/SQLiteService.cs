using HBR.OTPAuthenticator.BLL.Models;
using Plugin.FileSystem;
using Plugin.SecureStorage;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HBR.OTPAuthenticator.BLL
{
    public class SQLiteService
    {
        private SemaphoreSlim ConnectionMutex { get; } = new SemaphoreSlim(1, 1);
        internal const string AppKeychainId = "Token";
        internal const string DbFileName = "Data.db3";
        internal const string PasswordSalt = "bz77KNXdP,Bc4Acg";

        private readonly Lazy<byte[]> EncryptionKey;
        internal FileInfo DbFile { get; }
        private SQLiteAsyncConnection Connection { get; set; }

        public SQLiteService()
        {
            DbFile = new FileInfo(Path.Combine(CrossFileSystem.Current.LocalStorage.FullName, DbFileName));
            EncryptionKey = new Lazy<byte[]>(GetEncryptionKey);
        }

        public byte[] GetEncryptionKey()
        {
            var output = default(byte[]);
            var keyString = CrossSecureStorage.Current.GetValue(AppKeychainId);

            if (keyString == null)
            {
                using (var algorithm = Aes.Create())
                {
                    algorithm.GenerateKey();
                    output = algorithm.Key;
                }

                keyString = Convert.ToBase64String(output);
                CrossSecureStorage.Current.SetValue(AppKeychainId, keyString);
            }
            else
            {
                output = Convert.FromBase64String(keyString);
            }

            return output;
        }

        public byte[] Encrypt(byte[] clearText, out byte[] IV)
        {
            using (var algorithm = Aes.Create())
            {
                algorithm.Padding = PaddingMode.Zeros;
                algorithm.Key = EncryptionKey.Value;
                algorithm.GenerateIV();

                using (var encryptor = algorithm.CreateEncryptor())
                using (var memStream = new MemoryStream())
                using (var cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(clearText, 0, clearText.Length);
                    cryptoStream.FlushFinalBlock();

                    var output = memStream.ToArray();
                    IV = algorithm.IV;
                    return output;
                }
            }
        }

        public byte[] Decrypt(byte[] cypherText, byte[] IV)
        {
            using (var algorithm = Aes.Create())
            {
                algorithm.Padding = PaddingMode.Zeros;
                algorithm.Key = EncryptionKey.Value;
                algorithm.IV = IV;

                using (var decryptor = algorithm.CreateDecryptor())
                using (var memStream = new MemoryStream(cypherText))
                using (var cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read))
                using (var outStream = new MemoryStream())
                {
                    cryptoStream.CopyTo(outStream);
                    var output = outStream.ToArray();
                    return output;
                }
            }
        }

        public async Task<SQLiteAsyncConnection> GetConnectionAsync()
        {
            if (Connection == null)
            {
                Connection = new SQLiteAsyncConnection(DbFile.FullName);
                await Connection.CreateTableAsync<OTPGeneratorInner>();
                await Connection.CreateTableAsync<Login>();
            }

            return Connection;
        }

        public async Task CloseConnectionAsync()
        {
            await Connection.CloseAsync();
            Connection = null;
        }
        public async Task ClearAsync()
        {
            await ConnectionMutex.WaitAsync();
            var connection = await GetConnectionAsync();
            await connection.DropTableAsync<OTPGeneratorInner>();
            await connection.CreateTableAsync<OTPGeneratorInner>();
            await connection.DropTableAsync<Login>();
            await connection.CreateTableAsync<Login>();
            ConnectionMutex.Release();
        }
    }
}
