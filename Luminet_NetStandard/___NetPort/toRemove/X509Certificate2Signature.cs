
using System.Security.Cryptography;


namespace LumiSoft.Net.X509
{


    // https://sourceforge.net/p/itextsharp/itextsharp/ci/e167cfdf90bd5a3a2318d31613ac39d8aeb32e15/tree/src/core/iTextSharp/text/pdf/security/X509Certificate2Signature.cs#l43
    public class X509Certificate2Signature 
    {


        private System.Security.Cryptography.X509Certificates.X509Certificate2 certificate;
        private string hashAlgorithm;
        private string encryptionAlgorithm; // from private-key


        public X509Certificate2Signature(
            System.Security.Cryptography.X509Certificates.X509Certificate2 certificate
            , string hashAlgorithm
        )
        {
            if (!certificate.HasPrivateKey)
                throw new System.ArgumentException("No private key.");

            this.certificate = certificate;
            this.hashAlgorithm = DigestAlgorithms.GetDigest(DigestAlgorithms.GetAllowedDigests(hashAlgorithm));

            if (certificate.PrivateKey is RSACryptoServiceProvider)
                encryptionAlgorithm = "RSA";
            else if (certificate.PrivateKey is DSACryptoServiceProvider)
                encryptionAlgorithm = "DSA";
            else
                throw new System.ArgumentException("Unknown encryption algorithm " + certificate.PrivateKey);
        }


        public byte[] Sign(byte[] message)
        {
            if (certificate.PrivateKey is RSACryptoServiceProvider)
            {
                RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)certificate.PrivateKey;
                return rsa.SignData(message, hashAlgorithm);
            }
            else
            {
                DSACryptoServiceProvider dsa = (DSACryptoServiceProvider)certificate.PrivateKey;
                return dsa.SignData(message);
            }
        }

        
        public string GetHashAlgorithm()
        {
            return hashAlgorithm;
        }


        public string GetEncryptionAlgorithm()
        {
            return encryptionAlgorithm;
        }


    }


}
