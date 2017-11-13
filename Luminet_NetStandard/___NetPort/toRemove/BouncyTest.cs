
using Org.BouncyCastle.Cms;


namespace Luminet_NetStandard.___NetPort.toRemove
{


    class BouncyTest
    {

        //static void SignTestData()
        //{
        //    // make some pkcs7 signedCms to work on
        //    SignedCms p7 = new SignedCms(new System.Security.Cryptography.Pkcs.ContentInfo(
        //        new byte[] { 0x01, 0x02 })
        //    );
        //    p7.ComputeSignature(new CmsSigner(), false);

        //    // encode to get signedCms byte[] representation
        //    var signedCms = p7.Encode();

        //    // load using bouncyCastle
        //    CmsSignedData sig = new CmsSignedData(signedCms);

        //    var allSigsValid = VerifySignatures(sig);

        //    byte[] content = sig.SignedContent.GetContent() as byte[];
        //}



        // taken from bouncy castle SignedDataTest.cs
        private static bool VerifySignatures(CmsSignedData sp)
        {
            var signaturesValid = true;
            Org.BouncyCastle.X509.Store.IX509Store x509Certs = sp.GetCertificates("Collection");
            SignerInformationStore signers = sp.GetSignerInfos();

            foreach (SignerInformation signer in signers.GetSigners())
            {
                System.Collections.ICollection certCollection = x509Certs.GetMatches(signer.SignerID);

                System.Collections.IEnumerator certEnum = certCollection.GetEnumerator();
                certEnum.MoveNext();
                Org.BouncyCastle.X509.X509Certificate cert = (Org.BouncyCastle.X509.X509Certificate)certEnum.Current;

                signaturesValid &= signer.Verify(cert);
            }

            return signaturesValid;
        }


        // https://stackoverflow.com/questions/22658501/is-there-an-alternative-of-signedcms-in-winrt
        private byte[] CheckAndRemoveSignature(string data, out bool isValid)
        {
            isValid = false;

            // using bouncyCastle
            try
            {
                var bytes = System.Convert.FromBase64String(data);

                // assign data to CmsSignedData                  
                CmsSignedData sig = new CmsSignedData(bytes);

                // check if signature is valid
                var allSigsValid = VerifySignatures(sig);
                if (allSigsValid.Equals(true)) { isValid = true; }

                // get signature from cms
                byte[] content = sig.SignedContent.GetContent() as byte[];

                return content;
            }
            catch (System.Exception ex)
            {
                // cryptOutput.Text += "Error in 'BouncyCastle unsign' " + ex; 
            }

            return null;
        }


        // https://github.com/joschi/cryptoworkshop-bouncycastle/blob/master/src/main/java/cwguide/BcSignedDataExample.java
        public static void foo()
        {
            byte[] data = null;
            Org.BouncyCastle.Cms.CmsSignedData signedData = new CmsSignedData(data);

            byte[] content = signedData.SignedContent.GetContent() as byte[];

            Org.BouncyCastle.X509.Store.IX509Store certs = signedData.GetCertificates("type");

        }


    }


}
