
namespace RSACryptoServiceProviderExtensions
{


    public static class RSACryptoServiceProviderExtensions
    {


        public static void FromXmlString(
            this System.Security.Cryptography.RSACryptoServiceProvider rsa
            , string xmlString
        )
        {
            System.Security.Cryptography.RSAParameters parameters = 
                new System.Security.Cryptography.RSAParameters();

            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.LoadXml(xmlString);

            if (xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
            {
                foreach (System.Xml.XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "Modulus": parameters.Modulus = System.Convert.FromBase64String(node.InnerText); break;
                        case "Exponent": parameters.Exponent = System.Convert.FromBase64String(node.InnerText); break;
                        case "P": parameters.P = System.Convert.FromBase64String(node.InnerText); break;
                        case "Q": parameters.Q = System.Convert.FromBase64String(node.InnerText); break;
                        case "DP": parameters.DP = System.Convert.FromBase64String(node.InnerText); break;
                        case "DQ": parameters.DQ = System.Convert.FromBase64String(node.InnerText); break;
                        case "InverseQ": parameters.InverseQ = System.Convert.FromBase64String(node.InnerText); break;
                        case "D": parameters.D = System.Convert.FromBase64String(node.InnerText); break;
                    }
                }
            }
            else
            {
                throw new System.Exception("Invalid XML RSA key.");
            }

            rsa.ImportParameters(parameters);
        }


        public static string ToXmlString(this System.Security.Cryptography.RSACryptoServiceProvider rsa)
        {
            System.Security.Cryptography.RSAParameters parameters = rsa.ExportParameters(true);

            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
                System.Convert.ToBase64String(parameters.Modulus),
                System.Convert.ToBase64String(parameters.Exponent),
                System.Convert.ToBase64String(parameters.P),
                System.Convert.ToBase64String(parameters.Q),
                System.Convert.ToBase64String(parameters.DP),
                System.Convert.ToBase64String(parameters.DQ),
                System.Convert.ToBase64String(parameters.InverseQ),
                System.Convert.ToBase64String(parameters.D));
        }


    }


}
