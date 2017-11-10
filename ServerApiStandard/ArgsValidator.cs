using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.MailServer
{
    /// <summary>
    /// Provides mehtods for validating values.
    /// </summary>
    public class ArgsValidator
    {
        #region method ValidateFolder

        /// <summary>
        /// Validates folder. Folder may contains only printable chars and not:
        /// : * ? " &lt; &gt; | % . Throws Exception if folder isn't valid.
        /// </summary>
        /// <param name="folder">Folder to validate.</param>
        public static void ValidateFolder(string folder) 
        {
            if(folder == null || folder == ""){
                throw new ArgumentException("Invalid folder value, user name may not be '' or null !");
            }

            if(ContainsChars(folder,new char[]{':','*','?','\"','<','>','|','%'})){
                throw new ArgumentException("Invalid folder value, folder may not contain {: * ? \" < > | %} chars ! ");
            }

            // See if all chars are printable chars
            if(!IsPrintableCharsOnly(folder)){
                throw new ArgumentException("Invalid folder value, folder may contain printable chars only !");
            }
        }

        #endregion

        #region method ValidateSharedFolderRoot

        /// <summary>
        /// Validates shared root folder name value.
        /// </summary>
        /// <param name="folder"></param>
        public static void ValidateSharedFolderRoot(string folder)
        {
            if(folder == null || folder == ""){
                throw new ArgumentException("Invalid root folder value, user name may not be '' or null !");
            }

            if(ContainsChars(folder,new char[]{':','*','?','\"','<','>','|','%','\\','/'})){
                throw new ArgumentException("Invalid root folder value, folder may not contain {: * ? \" < > | % \\ /} chars ! ");
            }

            // See if all chars are printable chars
            if(!IsPrintableCharsOnly(folder)){
                throw new ArgumentException("Invalid root folder value, folder may contain printable chars only !");
            }

            // Don't allow known folder names to shared folder root
            if(folder.ToLower() == "inbox"){
                throw new ArgumentException("Invalid root folder value, 'Inbox' is reserved folder name !");
            }
            else if(folder.ToLower() == "drafts"){
                throw new ArgumentException("Invalid root folder value, 'Drafts' is reserved folder name !");
            }
            else if(folder.ToLower() == "trash"){
                throw new ArgumentException("Invalid root folder value, 'Trash' is reserved folder name !");
            }
            else if(folder.ToLower() == "deleted items"){
                throw new ArgumentException("Invalid root folder value, 'Deleted Items' is reserved folder name !");
            }
            else if(folder.ToLower() == "sent"){
                throw new ArgumentException("Invalid root folder value, 'Sent' is reserved folder name !");
            }
            else if(folder.ToLower() == "sent items"){
                throw new ArgumentException("Invalid root folder value, 'Sent Items' is reserved folder name !");
            }
            else if(folder.ToLower() == "outbox"){
                throw new ArgumentException("Invalid root folder value, 'Outbox' is reserved folder name !");
            }
        }

        #endregion

        #region method ValidateUserName

        /// <summary>
        /// Validates user name value, check that doesn't contain invalid chars. User name may contains only 
        /// printable chars and not: : * ? " &lt; &gt; | % . Throws Exception if user name isn't valid.
        /// </summary>
        /// <param name="userName">User name value to validate.</param>
        public static void ValidateUserName(string userName)
        {
            if(userName == null || userName == ""){
                throw new ArgumentException("Invalid user name value, user name may not be '' or null !");
            }

            if(ContainsChars(userName,new char[]{':','*','?','\"','<','>','|','%'})){
                throw new ArgumentException("Invalid user name value, user name may not contain {: * ? \" < > | %} chars ! ");
            }

            // See if all chars are printable chars
            if(!IsPrintableCharsOnly(userName)){
                throw new ArgumentException("Invalid user name value, user name may contain printable chars only !");
            }
        }

        #endregion

        #region method ValidateNotNull

        /// <summary>
        /// Requires value to be not null. Throws Exception if value is null.
        /// </summary>
        /// <param name="val">Value to check.</param>
        public static void ValidateNotNull(object val)
        {
            if(val == null){
                throw new ArgumentNullException("Invalid value, value may not be null !");
            }
        }

        #endregion
        
        #region method ValidateDomainName

        /// <summary>
        /// Validates domain name.
        /// </summary>
        /// <param name="domainName"></param>
        public static void ValidateDomainName(string domainName)
        {
            if(domainName == null){
                throw new ArgumentException("Invalid  domainName value, null values not allowed !");
            }
            if(domainName == ""){
                throw new ArgumentException("Invalid  domainName value, please specify domain name !");
            }
            if(!IsPrintableCharsOnly(domainName)){
                throw new ArgumentException("Invalid  domainName value, domain name may conatin ASCII printable chars only !");
            }
            if(ContainsChars(domainName,new char[]{'@'})){
                throw new ArgumentException("Invalid  domainName value, domain name may not conatin '@' char !");
            }
        }

        #endregion

        #region method ValidateEmail

        /// <summary>
        /// Validates email address, check that won't contain illegal chars. 
        /// Throws Exception if email isn't valid.
        /// </summary>
        /// <param name="email">Email to check.</param>
        public static void ValidateEmail(string email)
        {
            if(email == null){
                throw new ArgumentException("Invalid  email value, null values not allowed !");
            }
            if(email == ""){
                throw new ArgumentException("Invalid  email value, please specify email !");
            }
        }

        #endregion


        #region method ContainsChars

        /// <summary>
        /// Gets if specified text contains any of the specified chars.
        /// </summary>
        /// <param name="text">Text to check.</param>
        /// <param name="chars">Chars to check.</param>
        /// <returns>Returns true if specified text contains any of the chars.</returns>
        private static bool ContainsChars(string text,char[] chars)
        {
            foreach(char c in chars){
                if(text.IndexOf(c) > -1){
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region method IsPrintableCharsOnly

        /// <summary>
        /// Gets if all chars in text are printable chars (Not CR,LF,SP,TAB,...). 
        /// Chars 0 - 31 are ASCII controls characters and not allowed.
        /// </summary>
        /// <param name="text">Text to check.</param>
        /// <returns>Returns true is all chars are printable chars.</returns>
        private static bool IsPrintableCharsOnly(string text)
        {
            foreach(char c in text){
                if((int)c <= 31){
                    return false;
                }
            }

            return true;
        }

        #endregion

    }
}
