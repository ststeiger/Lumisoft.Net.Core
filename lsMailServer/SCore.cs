using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Text;

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


        #region static method RtfToText

        /// <summary>
        /// Converts RTF text to plain text.
        /// </summary>
        /// <param name="rtfText">RTF text to convert.</param>
        /// <returns></returns>
        public static string RtfToText(string rtfText)
        {
            using(System.Windows.Forms.RichTextBox t = new System.Windows.Forms.RichTextBox()){
                t.Rtf = rtfText;

                // Ensure that all line-feeds are CRLF.
                string retVal = t.Text.Replace("\r\n","\n").Replace("\n","\r\n");

                return retVal;
            }
        }

        #endregion

        #region static method RtfToHtml

        /// <summary>
        /// Converts RTF text to HTML.
        /// </summary>
        /// <param name="rtfText">RTF text to convert.</param>
        /// <returns></returns>
        public static string RtfToHtml(string rtfText)
        {
            using(RichTextBox textBox = new RichTextBox()){
                textBox.Rtf = rtfText;

                StringBuilder retVal = new StringBuilder();
                retVal.Append("<html>\r\n");
                        
                // Go to text start.
                textBox.SelectionStart  = 0;
                textBox.SelectionLength = 1;

                Font  currentFont           = textBox.SelectionFont;
                Color currentSelectionColor = textBox.SelectionColor;
                Color currentBackColor      = textBox.SelectionBackColor;

                int startPos = 0;
                while(textBox.Text.Length > textBox.SelectionStart){  
                    textBox.SelectionStart++;
                    textBox.SelectionLength = 1;
                                
                    // Font style or size or color or back color changed
                    if(textBox.Text.Length == textBox.SelectionStart || (currentFont.Name != textBox.SelectionFont.Name || currentFont.Size != textBox.SelectionFont.Size || currentFont.Style != textBox.SelectionFont.Style || currentSelectionColor != textBox.SelectionColor || currentBackColor != textBox.SelectionBackColor)){
                        string currentTextBlock = textBox.Text.Substring(startPos,textBox.SelectionStart - startPos).Replace("\r","").Replace("<","&lt;").Replace(">","&gt;").Replace("\n","<br/>");
       
                        //--- Construct text block html -----------------------------------------------------------------//
                        // Make colors to html color syntax: #hex(r)hex(g)hex(b)
                        string htmlSelectionColor = "#" + currentSelectionColor.R.ToString("X2") + currentSelectionColor.G.ToString("X2") + currentSelectionColor.B.ToString("X2");
                        string htmlBackColor      = "#" + currentBackColor.R.ToString("X2") + currentBackColor.G.ToString("X2") + currentBackColor.B.ToString("X2");
                        string textStyleStartTags = "";
                        string textStyleEndTags   = "";
                        if(currentFont.Bold){
                            textStyleStartTags += "<b>";
                            textStyleEndTags   += "</b>";
                        }
                        if(currentFont.Italic){
                            textStyleStartTags += "<i>";
                            textStyleEndTags   += "</i>";
                        }
                        if(currentFont.Underline){
                            textStyleStartTags += "<u>";
                            textStyleEndTags   += "</u>";
                        }           
                        retVal.Append("<span style=\"color:" + htmlSelectionColor + "; font-size:" + currentFont.Size + "pt; font-family:" + currentFont.FontFamily.Name + "; background-color:" + htmlBackColor + ";\">" + textStyleStartTags + currentTextBlock + textStyleEndTags);
                        //-----------------------------------------------------------------------------------------------//

                        startPos              = textBox.SelectionStart;
                        currentFont           = textBox.SelectionFont;
                        currentSelectionColor = textBox.SelectionColor;
                        currentBackColor      = textBox.SelectionBackColor;

                        retVal.Append("</span>\r\n");
                    }
                } 
    
                retVal.Append("</html>\r\n");

                return retVal.ToString();
            }
        }

        #endregion
    }
}
