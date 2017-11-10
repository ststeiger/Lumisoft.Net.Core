using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LumiSoft.Net.Mime.vCard
{
    /// <summary>
    /// Provides vCard related methods.
    /// </summary>
    internal class vCard_Utils
    {
        #region static method Encode

        /// <summary>
        /// Encodes vcard value.
        /// </summary>
        /// <param name="version">VCard version.</param>
        /// <param name="value">Value to encode.</param>
        /// <returns>Returns encoded value.</returns>
        public static string Encode(string version,string value)
        {
            return Encode(version,Encoding.UTF8,value);
        }

        /// <summary>
        /// Encodes vcard value.
        /// </summary>
        /// <param name="version">VCard version.</param>
        /// <param name="charset">Charset used to encode.</param>
        /// <param name="value">Value to encode.</param>
        /// <returns>Returns encoded value.</returns>
        public static string Encode(string version,Encoding charset,string value)
        {
            if(charset == null){
                throw new ArgumentNullException("charset");
            }

            if(version.StartsWith("3")){
                // We need to escape CR LF COMMA SEMICOLON
                //value = value.Replace("\r","").Replace("\n","\\n").Replace(",","\\,").Replace(";","\\;");
                value = value.Replace("\r","").Replace("\n","\\n").Replace(",","\\,");
            }
            else{
                value = QPEncode(charset.GetBytes(value));
            }

            return value;
        }

        #endregion

        #region static method QPEncode

        /// <summary>
        /// Encodes data with quoted-printable encoding.
        /// </summary>
        /// <param name="data">Data to encode.</param>
        /// <returns>Returns encoded data.</returns>
        public static string QPEncode(byte[] data)
        {
            if(data == null){
                return null;
            }

            StringBuilder retVal = new StringBuilder();
            foreach(byte b in data){
                string val = null;
                if(b > 127 || b == '=' || b == '?' || b == '_' || char.IsControl((char)b)){
                    val = "=" + b.ToString("X2");
                }
                else{
                    val = ((char)b).ToString();
                }

                retVal.Append(val);
            }

            return retVal.ToString();
        }

        #endregion
    }
}
