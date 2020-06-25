using System;
using System.Collections.Generic;
using System.Text;
using OtpNet;
using SQLite;
using Newtonsoft.Json;
using HBR.OTPAuthenticator.BLL.Components;

namespace HBR.OTPAuthenticator.BLL.Models
{
    public class OTPGenerator
    {
        internal const string HMacSha1Name = "SHA1";
        internal const string HMacSha256Name = "SHA256";
        internal const string HMacSha512Name = "SHA512";

        internal static readonly IReadOnlyDictionary<string, OtpHashMode> AlgorithmsMapping = new Dictionary<string, OtpHashMode>
        {
            { HMacSha1Name, OtpHashMode.Sha1 },
            { HMacSha256Name, OtpHashMode.Sha256 },
            { HMacSha512Name, OtpHashMode.Sha512 }
        };

        public const int TimeStepSeconds = 30;
        public const int ManualStepSeconds = 5;

        public const int MinNumDigits = 6;
        public const int MaxNumDigits = 8;

        [PrimaryKey]
        public string Uid { get; set; }

        public string Label { get; set; }

        public string Issuer { get; set; }

        public bool TimeBased { get; set; }
        public long Counter { get; set; }
        public string AlgorithmName { get; set; }

        public virtual byte[] Secret { get; set; }

        [Ignore]
        [JsonIgnore]
        public string SecretBase32
        {
            get => Secret != null ? Base32Encoding.ToString(Secret) : null;
            set => Secret = Base32Encoding.ToBytes(value);
        }

        private int numDigits = MinNumDigits;
        public int NumDigits
        {
            get => numDigits;
            set => numDigits = Math.Max(Math.Min(value, MaxNumDigits), MinNumDigits);
        }

        public static OTPGenerator FromString(string input)
        {
            if (Uri.TryCreate(input, UriKind.Absolute, out var uri))
            {
                return FromUri(uri);
            }

            return null;
        }

        public static OTPGenerator FromUri(Uri input)
        {
            return OTPUriConverter.OTPGeneratorFromUri(input);
        }

        public OTPGenerator()
        {
            Uid = Guid.NewGuid().ToString();
            Label = Issuer = string.Empty;
            AlgorithmName = HMacSha1Name;
            TimeBased = TimeBased;
        }

        public string GenerateOTP(DateTime input)
        {
            if (TimeBased)
            {
                var otp = new Totp(Secret, TimeStepSeconds, AlgorithmsMapping[AlgorithmName], NumDigits);
                return otp.ComputeTotp(input);
            }

            var hotp = new Hotp(Secret, AlgorithmsMapping[AlgorithmName], NumDigits);
            return hotp.ComputeHOTP(Counter++);
        }

        public Uri ToUri()
        {
            return OTPUriConverter.UriFromOTPGenerator(this);
        }
    }
}
