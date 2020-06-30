using HBR.OTPAuthenticator.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HBR.OTPAuthenticator.BLL.Services
{
    public class OTPStorageService
    {
        private readonly SQLiteService SqliteService;
        private SemaphoreSlim ConnectionMutex { get; } = new SemaphoreSlim(1, 1);

        public OTPStorageService()
        {
            SqliteService = new SQLiteService();
        }

        public async Task<List<OTPGenerator>> GetAllAsync()
        {
            await ConnectionMutex.WaitAsync();
            var connection = await SqliteService.GetConnectionAsync();
            var items = await connection.Table<OTPGeneratorInner>().ToListAsync();
            foreach (var i in items)
            {
                var secret = Convert.FromBase64String(i.DbEncryptedSecret);
                var iv = Convert.FromBase64String(i.DbEncryptedSecretIV);
                i.Secret = SqliteService.Decrypt(secret, iv);
            }

            var output = items.OrderBy(d => d.CreationDate).ThenBy(d => d.Label).Cast<OTPGenerator>().ToList();
            ConnectionMutex.Release();
            return output;
        }

        public async Task<int> InsertOrReplaceAsync(OTPGenerator input)
        {
            var data = new OTPGeneratorInner(input);
            var secret = SqliteService.Encrypt(data.Secret, out var iv);
            data.DbEncryptedSecret = Convert.ToBase64String(secret);
            data.DbEncryptedSecretIV = Convert.ToBase64String(iv);

            await ConnectionMutex.WaitAsync();
            var connection = await SqliteService.GetConnectionAsync();
            var result = await connection.InsertOrReplaceAsync(data);
            ConnectionMutex.Release();
            return result;
        }

        public async Task<int> DeleteAsync(OTPGenerator input)
        {
            await ConnectionMutex.WaitAsync();
            var connection = await SqliteService.GetConnectionAsync();
            var result = await connection.DeleteAsync(new OTPGeneratorInner(input));
            ConnectionMutex.Release();
            return result;
        }
    }
}
