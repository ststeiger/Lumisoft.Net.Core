
namespace CoreMail
{


    internal class RtfFixer
    {


        // TODO: Check if this is correct...
        internal static bool LdapAuth(string userName, string password, string m_Auth_LDAP_Server, string m_Auth_LDAP_DN)
        {
            // https://www.novell.com/support/kb/doc.php?id=3449660
            if (string.IsNullOrEmpty(password))
                return false;

            try
            {
                string dn = m_Auth_LDAP_DN.Replace("%user", userName);

                int num = 389; // LDAP: 389, LDAPS: 636 

                if (m_Auth_LDAP_Server.StartsWith("LDAPS://", System.StringComparison.InvariantCultureIgnoreCase))
                { 
                    num = 636;
                    m_Auth_LDAP_Server = m_Auth_LDAP_Server.Substring(8);
                }
                else if (m_Auth_LDAP_Server.StartsWith("LDAP://", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    num = 389;
                    m_Auth_LDAP_Server = m_Auth_LDAP_Server.Substring(7);
                }

                int pos = m_Auth_LDAP_Server.IndexOf(':');
                if (pos != -1 && (m_Auth_LDAP_Server.Length > pos))
                {
                    string strNum = m_Auth_LDAP_Server.Substring(pos + 1);
                    int temp = 0;

                    if (int.TryParse(strNum, out temp))
                        num = temp;

                    m_Auth_LDAP_Server = m_Auth_LDAP_Server.Substring(0, pos);
                }


                using (Novell.Directory.Ldap.LdapConnection conn = 
                    new Novell.Directory.Ldap.LdapConnection() )
                {
                    // System.Console.WriteLine("Connecting to:" + ldapHost);
                    conn.Connect(m_Auth_LDAP_Server, num);
                    conn.Bind(3, dn, password);

                    // System.Console.WriteLine(" Bind Successfull");
                    conn.Disconnect();
                }

                return true;
            }
            catch (Novell.Directory.Ldap.LdapException e)
            {
                // System.Console.WriteLine("Error:" + e.LdapErrorMessage);
            }
            catch (System.Exception e)
            {
                // System.Console.WriteLine("Error:" + e.Message);
            }

            return false;



            /*
            // using System.DirectoryServices.Protocols;
            bool validated = false;

            try
            {
                string dn = m_Auth_LDAP_DN.Replace("%user", userName);

                using (LdapConnection ldap = new LdapConnection(new LdapDirectoryIdentifier(m_Auth_LDAP_Server), new System.Net.NetworkCredential(dn, password), System.DirectoryServices.Protocols.AuthType.Basic))
                {
                    ldap.SessionOptions.ProtocolVersion = 3;
                    ldap.Bind();
                }



                validated = true;
            }
            catch
            {
            }

            return validated;
            */
        }


        internal static string RtfToHtml(string rtfText)
        {
            return RtfPipe.Rtf.ToHtml(rtfText);
        }


        // https://stackoverflow.com/questions/36936114/how-to-convert-html-to-rtf-and-vice-versa-in-windows-10-uwp
        internal static string RtfToText(string rtfText)
        {
            string text = null;

            using (BracketPipe.HtmlReader reader = new  BracketPipe.HtmlReader(RtfToHtml(rtfText)))
            {
                text = BracketPipe.Html.ToPlainText(reader);
            }

            return text;
            
            /*
            string retValue = null;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            // using HtmlAgilityPack;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(RtfToHtml(rtfText));

            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//text()"))
            {
                string text = node.InnerText;
                if (!string.IsNullOrEmpty(text))
                    sb.AppendLine(text.Trim());
            }

            retValue = sb.ToString();
            sb.Clear();
            sb = null;

            return retValue;
            */
        }


    }


}
