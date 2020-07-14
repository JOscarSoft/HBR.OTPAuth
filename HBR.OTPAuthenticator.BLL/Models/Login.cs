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
        public virtual string SecretCode { get; set; }
        public bool UseBiometricAuth { get; set; }

        [JsonIgnore]
        public string DbEncryptedSecret { get; set; }

        public Login()
        {
            Uid = Guid.NewGuid().ToString();
        }
    }
}
