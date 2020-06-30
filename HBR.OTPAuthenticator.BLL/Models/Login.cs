using Newtonsoft.Json;
using OtpNet;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace HBR.OTPAuthenticator.BLL.Models
{
    [Table("OTPGenerator")]
    public class Login
    {
        [PrimaryKey]
        public string Uid { get; set; }

        [Ignore]
        [JsonIgnore]
        public virtual string SecretCode
        {
            get => SecretBytes != null ? Base32Encoding.ToString(SecretBytes) : null;
            set => SecretBytes = Base32Encoding.ToBytes(value);
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
