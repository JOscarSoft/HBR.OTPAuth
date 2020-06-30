using HBR.OTPAuthenticator.BLL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HBR.OTPAuthenticator.BLL.Services
{
    public class LoginService
    {
        private readonly SQLiteService SqliteService;
        private SemaphoreSlim ConnectionMutex { get; } = new SemaphoreSlim(1, 1);

        public LoginService()
        {
            SqliteService = new SQLiteService();
        }

        public async Task<bool> Login(string Code)
        {
            bool result = false;
            await ConnectionMutex.WaitAsync();
            var connection = await SqliteService.GetConnectionAsync();
            var LoginInformation = await connection.Table<Login>().FirstOrDefaultAsync();

            if(LoginInformation != null)
            {
                var secret = Convert.FromBase64String(LoginInformation.DbEncryptedSecret);
                var iv = Convert.FromBase64String(LoginInformation.DbEncryptedSecretIV);
                LoginInformation.SecretBytes = SqliteService.Decrypt(secret, iv);

                result = LoginInformation.SecretCode == Code;
            }

            ConnectionMutex.Release();
            return result;
        }

        public async Task<Login> GetLoginInformationAsync()
        {
            Login loginInfo = null;
            await ConnectionMutex.WaitAsync();
            var connection = await SqliteService.GetConnectionAsync();
            var LoginInformation = await connection.Table<Login>().FirstOrDefaultAsync();

            if (LoginInformation != null)
            {
                var secret = Convert.FromBase64String(LoginInformation.DbEncryptedSecret);
                var iv = Convert.FromBase64String(LoginInformation.DbEncryptedSecretIV);
                LoginInformation.SecretBytes = SqliteService.Decrypt(secret, iv);

                loginInfo = LoginInformation;
            }

            ConnectionMutex.Release();
            return loginInfo;
        }

        public async Task<int> InsertOrReplaceAsync(Login input)
        {
            var secret = SqliteService.Encrypt(input.SecretBytes, out var iv);
            input.DbEncryptedSecret = Convert.ToBase64String(secret);
            input.DbEncryptedSecretIV = Convert.ToBase64String(iv);

            await ConnectionMutex.WaitAsync();
            var connection = await SqliteService.GetConnectionAsync();
            var result = await connection.InsertOrReplaceAsync(input);
            ConnectionMutex.Release();
            return result;
        }

        public async Task<int> DeleteAsync(Login input)
        {
            await ConnectionMutex.WaitAsync();
            var connection = await SqliteService.GetConnectionAsync();
            var result = await connection.DeleteAsync(input);
            ConnectionMutex.Release();
            return result;
        }
    }
}
