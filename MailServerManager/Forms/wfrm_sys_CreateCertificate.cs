using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using LumiSoft.MailServer.UI.Resources;

using Org.BouncyCastle.X509;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Pkcs;

namespace LumiSoft.MailServer.UI
{
    /// <summary>
    /// Create SSL certificate window.
    /// </summary>
    public class wfrm_sys_CreateCertificate : Form
    {
        private Label   mt_Name   = null;
        private TextBox m_pName   = null;
        private Button  m_pCancel = null;
        private Button  m_pCreate = null;

        private byte[] m_pCertificate = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="hostName">Host name.</param>
        public wfrm_sys_CreateCertificate(string hostName)
        {
            InitUI();

            if(string.IsNullOrEmpty(hostName)){
                m_pName.Text = "mail.domain.com";
            }
            else{
                m_pName.Text = hostName;
            }
        }

        #region method InitUI

        /// <summary>
        /// Creates and initializes UI.
        /// </summary>
        private void InitUI()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(350,130);
            this.Icon = ResManager.GetIcon("ssl.ico");
            this.Text = "Create new SSL certificate.";

            mt_Name = new Label();
            mt_Name.Size = new Size(120,20);
            mt_Name.Location = new Point(5,30);
            mt_Name.TextAlign = ContentAlignment.MiddleLeft;
            mt_Name.Text = "Certificate name:";
            this.Controls.Add(mt_Name);

            m_pName = new TextBox();
            m_pName.Size = new Size(200,20);
            m_pName.Location = new Point(130,30);
            this.Controls.Add(m_pName);

            m_pCancel = new Button();
            m_pCancel.Size = new Size(70,20);
            m_pCancel.Location = new Point(175,60);
            m_pCancel.Text = "Cancel";
            m_pCancel.Click += new EventHandler(m_pCancel_Click);
            this.Controls.Add(m_pCancel);

            m_pCreate = new Button();
            m_pCreate.Size = new Size(70,20);
            m_pCreate.Location = new Point(255,60);
            m_pCreate.Text = "Create";
            m_pCreate.Click += new EventHandler(m_pCreate_Click);
            this.Controls.Add(m_pCreate);
        }
                                
        #endregion

        #region Events handling

        #region method m_pCancel_Click

        private void m_pCancel_Click(object sender,EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        #endregion

        #region method m_pCreate_Click

        private void m_pCreate_Click(object sender,EventArgs e)
        {
            m_pCertificate = CreateCertificate(m_pName.Text,"");

            this.DialogResult = DialogResult.OK;
        }

        #endregion

        #endregion


        #region method CreateCertificate

        /// <summary>
        /// Creates X509 v3 certificate in PKCS12 file format.
        /// </summary>
        /// <param name="cn">Common name "CN" value.</param>
        /// <param name="password">Private key password.</param>
        /// <returns>Returns new X509 v3 certificate in PKCS12 file(.p12 or .pfx) format.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>cn</b> or <b>password</b> is null reference.</exception>
        public static byte[] CreateCertificate(string cn,string password)
        {   
            if(cn == null){
                throw new ArgumentNullException("cn");
            }
            if(password == null){
                throw new ArgumentNullException("password");
            }
                 
            RsaKeyPairGenerator kpgen = new RsaKeyPairGenerator(); 
            kpgen.Init(new KeyGenerationParameters(new SecureRandom(new CryptoApiRandomGenerator()),1024)); 
            AsymmetricCipherKeyPair kp = kpgen.GenerateKeyPair(); 

            BigInteger serial = BigInteger.ProbablePrime(120,new Random());
            X509Name certName = new X509Name("CN=" + cn);

            X509V3CertificateGenerator gen = new X509V3CertificateGenerator(); 
            gen.SetSerialNumber(serial);
            gen.SetSubjectDN(certName); 
            gen.SetIssuerDN(certName); 
            gen.SetNotBefore(DateTime.UtcNow.AddDays(-2));
			gen.SetNotAfter(DateTime.UtcNow.AddYears(5));
            gen.SetSignatureAlgorithm("MD5WithRSAEncryption"); 
            gen.SetPublicKey(kp.Public);  
            /*
            gen.AddExtension( 
                X509Extensions.AuthorityKeyIdentifier.Id, 
                false, 
                new AuthorityKeyIdentifier( 
                    SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(kp.Public), 
                    new GeneralNames(new GeneralName(certName)), 
                serial 
            ));*/
            gen.AddExtension(X509Extensions.ExtendedKeyUsage,false,new ExtendedKeyUsage(KeyPurposeID.IdKPServerAuth,KeyPurposeID.AnyExtendedKeyUsage));
                               
            X509Certificate cert = gen.Generate(kp.Private);
                        
            X509CertificateEntry certEntry = new X509CertificateEntry(cert); 

            Pkcs12Store newStore = new Pkcs12Store(); 
            newStore.SetCertificateEntry(cn,certEntry);
            newStore.SetKeyEntry(cn,new AsymmetricKeyEntry(kp.Private),new[]{new X509CertificateEntry(cert)}); 

            MemoryStream retVal = new MemoryStream();
            newStore.Save(retVal,password.ToCharArray(),new SecureRandom(new CryptoApiRandomGenerator()));
            
            return retVal.ToArray();
        }

        #endregion


        #region Properties implementation

        /// <summary>
        /// Gets certificate.
        /// </summary>
        public byte[] Certificate
        {
            get{ return m_pCertificate; }
        }

        #endregion
    }
}
