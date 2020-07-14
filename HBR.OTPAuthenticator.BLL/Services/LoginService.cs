using HBR.OTPAuthenticator.BLL.Components;
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
        private static string secretIV = "20AuthOTPHbrSoft20";

        public LoginService()
        {
            SqliteService = new SQLiteService();
        }

        public async Task<Login> GetLoginInformationAsync()
        {
            Login loginInfo = null;
            await ConnectionMutex.WaitAsync();
            var connection = await SqliteService.GetConnectionAsync();
            var LoginInformation = await connection.Table<Login>().FirstOrDefaultAsync();

            if (LoginInformation != null)
            {
                LoginInformation.SecretCode = Encryptor.SimpleDecryptWithPassword(LoginInformation.DbEncryptedSecret, secretIV);

                loginInfo = LoginInformation;
            }

            ConnectionMutex.Release();
            return loginInfo;
        }

        public async Task<int> InsertOrReplaceAsync(Login input)
        {
            input.DbEncryptedSecret = Encryptor.SimpleEncryptWithPassword(input.SecretCode, secretIV);

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
