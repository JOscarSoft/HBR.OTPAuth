using Newtonsoft.Json;
using OtpNet;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace HBR.OTPAuthenticator.BLL.Models
{
    [Table("Login")]
    public class Login
    {
        [PrimaryKey]
        public string Uid { get; set; }

        [Ignore]
        [JsonIgnore]
        public virtual string SecretCode
        {
            get => SecretBytes != null ? Encoding.UTF8.GetString(SecretBytes) : null;
            set => SecretBytes = Encoding.UTF8.GetBytes(value.ToCharArray());
        }
        public bool UseBiometricAuth { get; set; }

        [Ignore]
        public byte[] SecretBytes { get; set; }

        [JsonIgnore]
        public string DbEncryptedSecret { get; set; }

        [JsonIgnore]
        public string DbEncryptedSecretIV { get; set; }

        public Login()
        {
            Uid = Guid.NewGuid().ToString();
        }
    }
}
