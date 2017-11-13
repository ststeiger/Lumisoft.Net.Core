
// Copyright 2017-+infinity Stefan Steiger
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.


// Simulates CmsSigner, SignedCms for .NET Core 



#define DOTNETCORE_LEGACY_COMPATIBILITY 
#if DOTNETCORE_LEGACY_COMPATIBILITY

// MIME/MIME_b_ApplicationPkcs7Mime.cs
// MIME/MIME_b_MultipartSigned.cs


using Org.BouncyCastle.Cms;


// CMS: https://en.wikipedia.org/wiki/Cryptographic_Message_Syntax
// PKCS: https://en.wikipedia.org/wiki/PKCS
namespace System.Security.Cryptography.Pkcs
{


    // https://github.com/mono/mono/blob/master/mcs/class/System.Security/System.Security.Cryptography.Pkcs/CmsSigner.cs
    // https://referencesource.microsoft.com/#System.Security/system/security/cryptography/pkcs/pkcs7signer.cs

    public class CmsSigner
    {

        public System.Security.Cryptography.X509Certificates.X509Certificate2 Certificate;
        
        public CmsSigner(System.Security.Cryptography.X509Certificates.X509Certificate2 cert)
        {
            this.Certificate = cert;
        }

    }


    // https://github.com/mono/mono/blob/master/mcs/class/System.Security/System.Security.Cryptography.Pkcs/SignedCms.cs
    // https://referencesource.microsoft.com/#System.Security/system/security/cryptography/pkcs/signedpkcs7.cs
    public class SignedCms
    {

        private System.Security.Cryptography.Pkcs.ContentInfo m_contentInfo;
        private CmsSigner m_signer;
        private CmsSignedData m_signedData;
        

        public ContentInfo ContentInfo
        {
            get
            {
                return this.m_contentInfo;
            }
            set
            {
                this.m_contentInfo = value;
            }
        }



        public SignedCms()
        { }

        // SignedCms(new ContentInfo(tmpDataEntityStream.ToArray()),true);
        public SignedCms(ContentInfo contentInfo, bool detached)
        {
            this.m_contentInfo = contentInfo;
        }


        // signedCms.ComputeSignature(new CmsSigner(m_pSignerCert));
        // byte[] pkcs7 = signedCms.Encode();
        public byte[] Encode()
        {
            return this.m_signedData.GetEncoded();
        }
        

        public void Decode(byte[] data)
        {
            CmsSignedData sig = new CmsSignedData(data);
            byte[] content = sig.SignedContent.GetContent() as byte[];

            //this.m_contentInfo = new System.Security.Cryptography.Pkcs.ContentInfo(contentType, content);
            this.m_contentInfo = new System.Security.Cryptography.Pkcs.ContentInfo(content);
        }


        public void CheckSignature(bool verifySignatureOnly)
        {
            bool signaturesValid = VerifySignatures(new CmsSignedData(this.m_contentInfo.Content));
            if (!signaturesValid)
                throw new System.Exception("Invalid signature");
        }


        // taken from bouncy castle SignedDataTest.cs
        private static bool VerifySignatures(CmsSignedData sp)
        {
            var signaturesValid = true;
            Org.BouncyCastle.X509.Store.IX509Store x509Certs = sp.GetCertificates("Collection");
            SignerInformationStore signers = sp.GetSignerInfos();

            foreach (SignerInformation signer in signers.GetSigners())
            {
                Collections.ICollection certCollection = x509Certs.GetMatches(signer.SignerID);

                Collections.IEnumerator certEnum = certCollection.GetEnumerator();
                certEnum.MoveNext();
                Org.BouncyCastle.X509.X509Certificate cert = (Org.BouncyCastle.X509.X509Certificate)certEnum.Current;

                signaturesValid &= signer.Verify(cert);
            }
            
            return signaturesValid;
        }
        

        // https://stackoverflow.com/questions/1182612/what-is-the-difference-between-x509certificate2-and-x509certificate-in-net
        private static Org.BouncyCastle.X509.X509Certificate FromX509Certificate(System.Security.Cryptography.X509Certificates.X509Certificate2 x509Cert)
        {
            // https://stackoverflow.com/questions/8136651/how-can-i-convert-a-bouncycastle-x509certificate-to-an-x509certificate2
            // https://stackoverflow.com/questions/1182612/what-is-the-difference-between-x509certificate2-and-x509certificate-in-net
            return new Org.BouncyCastle.X509.X509CertificateParser().ReadCertificate(x509Cert.GetRawCertData());
        }
        

        private static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GetRsaKeyPair(RSAParameters rp)
        {
            Org.BouncyCastle.Math.BigInteger modulus = new Org.BouncyCastle.Math.BigInteger(1, rp.Modulus);
            Org.BouncyCastle.Math.BigInteger pubExp = new Org.BouncyCastle.Math.BigInteger(1, rp.Exponent);

            Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters pubKey =
                new Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters(
                false,
                modulus,
                pubExp);

            Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters privKey =
                new Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters(
                modulus,
                pubExp,
                new Org.BouncyCastle.Math.BigInteger(1, rp.D),
                new Org.BouncyCastle.Math.BigInteger(1, rp.P),
                new Org.BouncyCastle.Math.BigInteger(1, rp.Q),
                new Org.BouncyCastle.Math.BigInteger(1, rp.DP),
                new Org.BouncyCastle.Math.BigInteger(1, rp.DQ),
                new Org.BouncyCastle.Math.BigInteger(1, rp.InverseQ));

            return new Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair(pubKey, privKey);
        }



        private static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GetDsaKeyPair(DSAParameters dp)
        {
            Org.BouncyCastle.Crypto.Parameters.DsaValidationParameters validationParameters = (dp.Seed != null)
                ? new Org.BouncyCastle.Crypto.Parameters.DsaValidationParameters(dp.Seed, dp.Counter)
                : null;

            Org.BouncyCastle.Crypto.Parameters.DsaParameters parameters =
                new Org.BouncyCastle.Crypto.Parameters.DsaParameters(
                new Org.BouncyCastle.Math.BigInteger(1, dp.P),
                new Org.BouncyCastle.Math.BigInteger(1, dp.Q),
                new Org.BouncyCastle.Math.BigInteger(1, dp.G),
                validationParameters);

            Org.BouncyCastle.Crypto.Parameters.DsaPublicKeyParameters pubKey =
                new Org.BouncyCastle.Crypto.Parameters.DsaPublicKeyParameters(
                new Org.BouncyCastle.Math.BigInteger(1, dp.Y),
                parameters);

            Org.BouncyCastle.Crypto.Parameters.DsaPrivateKeyParameters privKey =
                new Org.BouncyCastle.Crypto.Parameters.DsaPrivateKeyParameters(
                new Org.BouncyCastle.Math.BigInteger(1, dp.X),
                parameters);

            return new Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair(pubKey, privKey);
        }


        // https://gist.github.com/Jargon64/5b172c452827e15b21882f1d76a94be4
        private static Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair GetKeyPair(AsymmetricAlgorithm privateKey)
        {
            if (privateKey is DSA)
            {
                DSAParameters parameters = default(DSAParameters);

                // return GetDsaKeyPair((DSA)privateKey);
                using (DSACryptoServiceProvider prov = (DSACryptoServiceProvider)privateKey)
                {
                    parameters = prov.ExportParameters(true);
                }
                    
                return GetDsaKeyPair(parameters);
            }

            if (privateKey is RSA)
            {
                // return GetRsaKeyPair((RSA)privateKey);
                RSAParameters parameters = default(RSAParameters);

                using (RSACryptoServiceProvider prov = (RSACryptoServiceProvider)privateKey)
                {
                    parameters = prov.ExportParameters(true);
                }

                return GetRsaKeyPair(parameters);
            }

            throw new ArgumentException("Unsupported algorithm specified", "privateKey");
        }


        // https://www.bouncycastle.org/docs/pkixdocs1.5on/org/bouncycastle/cms/CMSSignedDataGenerator.html
        private void SignInternal(
            System.Security.Cryptography.X509Certificates.X509Certificate2 certificate
            , byte[] data)
        {

            // https://stackoverflow.com/questions/3240222/get-private-key-from-bouncycastle-x509-certificate-c-sharp
            // Org.BouncyCastle.X509.X509Certificate c = FromX509Certificate(certificate);
            //var algorithm = LumiSoft.Net.X509.DigestAlgorithms.GetDigest(c.SigAlgOid);
            //var signature = new LumiSoft.Net.X509.X509Certificate2Signature(this.m_certificate, algorithm);
            //signature.Sign(data);


            CmsProcessable msg = new Org.BouncyCastle.Cms.CmsProcessableByteArray(data);

            CmsSignedDataGenerator gen = new CmsSignedDataGenerator();

            Org.BouncyCastle.Security.SecureRandom random = new Org.BouncyCastle.Security.SecureRandom();

            Org.BouncyCastle.X509.X509Certificate c = FromX509Certificate(certificate);
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair issuerKeys = GetKeyPair(certificate.PrivateKey);
            string signatureAlgo = certificate.PrivateKey is RSA ? "SHA1withRSA" : certificate.PrivateKey is DSA ? "SHA1withDSA" : null;
            // var sha1Signer = Org.BouncyCastle.Security.SignerUtilities.GetSigner("SHA1withRSA");



            // https://stackoverflow.com/questions/3240222/get-private-key-from-bouncycastle-x509-certificate-c-sharp

            // https://stackoverflow.com/questions/36712679/bouncy-castles-x509v3certificategenerator-setsignaturealgorithm-marked-obsolete
            Org.BouncyCastle.Crypto.ISignatureFactory signatureFactory =
                new Org.BouncyCastle.Crypto.Operators.Asn1SignatureFactory
                (signatureAlgo, issuerKeys.Private, random);

            // gen.AddCertificates(certs);
            gen.AddSignerInfoGenerator(new SignerInfoGeneratorBuilder()
                .Build(signatureFactory, c)
            );

            this.m_signedData = gen.Generate(msg, false);
        }



        public void ComputeSignature(CmsSigner signer)
        {
            this.m_signer = signer;
            SignInternal(signer.Certificate, this.m_contentInfo.Content);
        }


        private static System.Security.Cryptography.X509Certificates.X509Certificate2 
            BouncyCertToNetCert(Org.BouncyCastle.X509.X509Certificate bouncyCert)
        {
            byte[] derEncoded = bouncyCert.GetEncoded();
            return new System.Security.Cryptography.X509Certificates.X509Certificate2(derEncoded);
        }


        public System.Security.Cryptography.X509Certificates.X509Certificate2Collection Certificates
        {
            get
            {
                CmsSignedData sp = new CmsSignedData(this.m_contentInfo.Content);

                System.Security.Cryptography.X509Certificates.X509Certificate2Collection coll =
                    new X509Certificates.X509Certificate2Collection();

                Org.BouncyCastle.X509.Store.IX509Store x509Certs = sp.GetCertificates("Collection");
                SignerInformationStore signers = sp.GetSignerInfos();

                foreach (SignerInformation signer in signers.GetSigners())
                {
                    System.Collections.ICollection certCollection = x509Certs.GetMatches(signer.SignerID);
                    System.Collections.IEnumerator certEnum = certCollection.GetEnumerator();

                    certEnum.MoveNext();
                    Org.BouncyCastle.X509.X509Certificate cert = 
                        (Org.BouncyCastle.X509.X509Certificate)certEnum.Current;

                    coll.Add(BouncyCertToNetCert(cert));
                }
                
                return coll;
            }
        }


    }


}

#endif 
