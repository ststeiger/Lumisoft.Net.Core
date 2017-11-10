
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


#if true 

using System;
using System.Collections.Generic;
using System.Text;

using Org.BouncyCastle.Cms;

// CMS: https://en.wikipedia.org/wiki/Cryptographic_Message_Syntax
// PKCS: https://en.wikipedia.org/wiki/PKCS
namespace System.Security.Cryptography.Pkcs
{


    // https://github.com/mono/mono/blob/master/mcs/class/System.Security/System.Security.Cryptography.Pkcs/CmsSigner.cs
    // https://referencesource.microsoft.com/#System.Security/system/security/cryptography/pkcs/pkcs7signer.cs
    public class CmsSigner
    {

        public CmsSigner(System.Security.Cryptography.X509Certificates.X509Certificate2 cert)
        {
            byte[] data = null;
            CmsProcessable msg = new Org.BouncyCastle.Cms.CmsProcessableByteArray(data);

            CmsSignedDataGenerator gen = new CmsSignedDataGenerator();
            var sha1Signer = Org.BouncyCastle.Security.SignerUtilities.GetSigner("SHA1withRSA");

            /*
            new 

            gen.AddSignerInfoGenerator(new SignerInfoGeneratorBuilder()
                .Build(sha1Signer, null);
            */
            // gen.AddCertificates(certs);
            gen.Generate(msg, false);


        }

    }


    // https://github.com/mono/mono/blob/master/mcs/class/System.Security/System.Security.Cryptography.Pkcs/SignedCms.cs
    // https://referencesource.microsoft.com/#System.Security/system/security/cryptography/pkcs/signedpkcs7.cs
    public class SignedCms
    {

        // Property ?
        // public ContentInfo ContentInfo;
        protected ContentInfo m_ContentInfo;


        public ContentInfo ContentInfo
        {
            get
            {
                return this.m_ContentInfo;
            }
            set
            {
                this.m_ContentInfo = value;
            }
        }



        public SignedCms()
        { }

        // SignedCms(new ContentInfo(tmpDataEntityStream.ToArray()),true);
        public SignedCms(ContentInfo ci, bool b)
        {
        }



        // signedCms.ComputeSignature(new CmsSigner(m_pSignerCert));

        // byte[] pkcs7 = signedCms.Encode();
        public byte[] Encode()
        {
            return null;
        }


        public void Decode(byte[] data)
        {

        }

        public void CheckSignature(bool b)
        {

        }

        public void ComputeSignature(CmsSigner signer)
        { }


        public System.Security.Cryptography.X509Certificates.X509Certificate2Collection Certificates
        {
            get
            {
                return null;
            }
        }


    }


}

#endif 

#if false

namespace System.Security.Cryptography.Pkcs
{



    public class EnvelopedCms
    {

        public EnvelopedCms()
        { }


        public byte[] Encode()
        {
            return null;
        }


        public void Decode(byte[] data)
        {

        }


        public void Decrypt(System.Security.Cryptography.X509Certificates
            .X509Certificate2Collection certificates)
        {

        }



    }

    public class SignedCms
    {

        // Make property ?
        public ContentInfo ContentInfo;

        public SignedCms()
        { }

        // SignedCms(new ContentInfo(tmpDataEntityStream.ToArray()),true);
        public SignedCms(ContentInfo ci, bool b)
        {
        }



        // signedCms.ComputeSignature(new CmsSigner(m_pSignerCert));

        // byte[] pkcs7 = signedCms.Encode();
        public byte[] Encode()
        {
            return null;
        }


        public void Decode(byte[] data)
        {

        }

        public void CheckSignature(bool b)
        {

        }

        public void ComputeSignature(CmsSigner signer)
        { }


        public System.Security.Cryptography.X509Certificates.X509Certificate2Collection Certificates
        {
            get
            {
                return null;
            }
        }


    }


    class CoreCMSSignedData
    {

        public static void foo()
        {
            byte[] sigBlock = null;
            Org.BouncyCastle.Cms.CmsSignedData c = new Org.BouncyCastle.Cms.CmsSignedData(sigBlock);

        }

    }
}

#endif

#endif 
