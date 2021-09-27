
using System;


namespace LumiSoft.MailServer
{


    /// <summary>
    /// This exception is thrown when not enough permissions to complete operation.
    /// </summary>
    public class InsufficientPermissionsException 
        : Exception
    {     
        

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="errorText">Error text.</param>
        public InsufficientPermissionsException(string errorText) : base(errorText)
        { }


    }


}
