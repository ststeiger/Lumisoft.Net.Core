using System;
using System.IO;

namespace LumiSoft.MailServer
{
	/// <summary>
	/// Server utility functions.
	/// </summary>
	internal class SCore
	{
	//	public SCore()
	//	{			
	//	}
        		

		#region static IsMatch

		/// <summary>
		/// Checks if text matches to search pattern.
		/// </summary>
		/// <param name="pattern"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		public static bool IsMatch(string pattern,string text)
		{
			if(pattern.IndexOf("*") > -1){
				if(pattern == "*"){
					return true;
				}
				else if(pattern.StartsWith("*") && pattern.EndsWith("*") && text.IndexOf(pattern.Substring(1,pattern.Length - 2)) > -1){
					return true;
				}
				else if(pattern.IndexOf("*") == -1 && text.ToLower() == pattern.ToLower()){
					return true;
				}
				else if(pattern.StartsWith("*") && text.ToLower().EndsWith(pattern.Substring(1).ToLower())){
					return true;
				}
				else if(pattern.EndsWith("*") && text.ToLower().StartsWith(pattern.Substring(0,pattern.Length - 1).ToLower())){
					return true;
				}
			}
			else if(pattern.ToLower() == text.ToLower()){
				return true;
			}

			return false;
		}

		#endregion

        #region static method IsAstericMatch

        /// <summary>
		/// Checks if text matches to search pattern.
		/// </summary>
		/// <param name="pattern"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		public static bool IsAstericMatch(string pattern,string text)
		{
            pattern = pattern.ToLower();
			text = text.ToLower();

			if(pattern == ""){
				pattern = "*";
			}

			while(pattern.Length > 0){
				// *xxx[*xxx...]
				if(pattern.StartsWith("*")){
					// *xxx*xxx
					if(pattern.IndexOf("*",1) > -1){
						string indexOfPart = pattern.Substring(1,pattern.IndexOf("*",1) - 1);
						if(text.IndexOf(indexOfPart) == -1){
							return false;
						}

                        text = text.Substring(text.IndexOf(indexOfPart) + indexOfPart.Length);
                        pattern = pattern.Substring(pattern.IndexOf("*", 1));
					}
					// *xxx   This is last pattern	
					else{				
						return text.EndsWith(pattern.Substring(1));
					}
				}
				// xxx*[xxx...]
				else if(pattern.IndexOfAny(new char[]{'*'}) > -1){
					string startPart = pattern.Substring(0,pattern.IndexOfAny(new char[]{'*'}));
		
					// Text must startwith
					if(!text.StartsWith(startPart)){
						return false;
					}

					text = text.Substring(text.IndexOf(startPart) + startPart.Length);
					pattern = pattern.Substring(pattern.IndexOfAny(new char[]{'*'}));
				}
				// xxx
				else{
					return text == pattern;
				}
			}

            return true;
		}

		#endregion


        #region method PathFix

        /// <summary>
        /// Fixes path separator, replaces / \ with platform separator char.
        /// </summary>
        /// <param name="path">Path to fix.</param>
        /// <returns></returns>
        public static string PathFix(string path)
        {
            return path.Replace('\\',Path.DirectorySeparatorChar).Replace('/',Path.DirectorySeparatorChar);
        }

        #endregion


        #region static method StreamCopy

        /// <summary>
        /// Copies all data from source stream to destination stream.
        /// Copy starts from source stream current position and will be copied to the end of source stream.
        /// </summary>
        /// <param name="source">Source stream.</param>
        /// <param name="destination">Destination stream.</param>
        public static void StreamCopy(Stream source,Stream destination)
        {
            byte[] buffer = new byte[8000];
            int readedCount = source.Read(buffer,0,buffer.Length);
            while(readedCount > 0){
                destination.Write(buffer,0,readedCount);

                readedCount = source.Read(buffer,0,buffer.Length);
            }
        }

        #endregion

    }
}
