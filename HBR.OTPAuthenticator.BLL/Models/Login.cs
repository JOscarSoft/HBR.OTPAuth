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
            get => SecretBytes != null ? getASCIIString(Encoding.ASCII.GetString(SecretBytes)) : null;
            set => SecretBytes = Encoding.ASCII.GetBytes(value.ToCharArray());
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

        private string getASCIIString(string code)
        {
            int i = code.IndexOf("\0");
            if (i >= 0)
                code = code.Substring(0, i);
            return code;
        }
    }
}
