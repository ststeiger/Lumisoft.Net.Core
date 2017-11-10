using System;

namespace LumiSoft.MailServer
{
    /// <summary>
    /// 
    /// </summary>
	public enum CompareSource
	{
        /// <summary>
        /// 
        /// </summary>
		HeaderField,

        /// <summary>
        /// 
        /// </summary>
		BodyText,

        /// <summary>
        /// 
        /// </summary>
		BodyHtml,

        /// <summary>
        /// 
        /// </summary>
		MimeEntry,
	}

    /// <summary>
    /// 
    /// </summary>
	public enum CompareType
	{
        /// <summary>
        /// 
        /// </summary>
		AstericPattern,

        /// <summary>
        /// 
        /// </summary>
		Equal,

        /// <summary>
        /// 
        /// </summary>
		NotEqual,

        /// <summary>
        /// 
        /// </summary>
		Regex,

        /// <summary>
        /// 
        /// </summary>
		Md5,
	}

    /// <summary>
    /// 
    /// </summary>
	public enum MatchAction
	{
        /// <summary>
        /// 
        /// </summary>
		AutoResponse,

        /// <summary>
        /// 
        /// </summary>
		Forward,

        /// <summary>
        /// 
        /// </summary>
		StoreToFolder,
	}
}
