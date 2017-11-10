using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Data;

using LumiSoft.Net.MIME;
using LumiSoft.Net.Mail;
using LumiSoft.Net.SMTP.Server;
using LumiSoft.Net.FTP.Client;

namespace LumiSoft.MailServer
{
    /// <summary>
    /// Global messages rule engine.
    /// </summary>
    public class GlobalMessageRuleProcessor
    {        
        private enum PossibleClauseItem
        {
            AND = 2,
            OR = 4,
            NOT = 8,
            Parenthesizes = 16,
            Matcher = 32,
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GlobalMessageRuleProcessor()
        {
        }


        #region static method CheckMatchExpressionSyntax

        /// <summary>
        /// Checks match expression syntax. Throws exception if syntax isn't valid.
        /// </summary>
        /// <param name="matchExpression">Match expression string.</param>
        public static void CheckMatchExpressionSyntax(string matchExpression)
        {
            GlobalMessageRuleProcessor p = new GlobalMessageRuleProcessor();
            p.Match(true,new LumiSoft.Net.StringReader(matchExpression),null,null,null,null,0);
        }

        #endregion


        #region method Match

        /// <summary>
        /// Checks if specified message matches to specified criteria.
        /// </summary>
        /// <param name="matchExpression">Match expression.</param>
        /// <param name="mailFrom">SMTP MAIL FROM: command email value.</param>
        /// <param name="rcptTo">SMTP RCPT TO: command email values.</param>
        /// <param name="smtpSession">SMTP current session.</param>
        /// <param name="mime">Message to match.</param>
        /// <param name="messageSize">Message size in bytes.</param>
        /// <returns>Returns true if message matches to specified criteria.</returns>
        public bool Match(string matchExpression,string mailFrom,string[] rcptTo,SMTP_Session smtpSession,Mail_Message mime,int messageSize)
        {
            LumiSoft.Net.StringReader r = new LumiSoft.Net.StringReader(matchExpression);
            return Match(false,r,mailFrom,rcptTo,smtpSession,mime,messageSize);
        }

        /// <summary>
        /// Checks if specified message matches to specified criteria.
        /// </summary>
        /// <param name="syntaxCheckOnly">Specifies if syntax check is only done. If true no matching is done.</param>
        /// <param name="r">Match expression reader what contains match expression.</param>
        /// <param name="mailFrom">SMTP MAIL FROM: command email value.</param>
        /// <param name="rcptTo">SMTP RCPT TO: command email values.</param>
        /// <param name="smtpSession">SMTP current session.</param>
        /// <param name="mime">Message to match.</param>
        /// <param name="messageSize">Message size in bytes.</param>
        /// <returns>Returns true if message matches to specified criteria.</returns>
        private bool Match(bool syntaxCheckOnly,LumiSoft.Net.StringReader r,string mailFrom,string[] rcptTo,SMTP_Session smtpSession,Mail_Message mime,int messageSize)
        {             
            /* Possible keywords order
                At first there can be NOT,parethesized or matcher
                    After NOT, parethesized or matcher
                    After matcher, AND or OR
                    After OR, NOT,parethesized or matcher
                    After AND, NOT,parethesized or matcher
                    After parethesized, NOT or matcher
            */
 
            PossibleClauseItem possibleClauseItems = PossibleClauseItem.Parenthesizes | PossibleClauseItem.NOT | PossibleClauseItem.Matcher;
            bool lastMatchValue = false;

            // Empty string passed 
            r.ReadToFirstChar();
            if(r.Available == 0){
                throw new Exception("Invalid syntax: '" + ClauseItemsToString(possibleClauseItems) + "' expected !");
            }

            // Parse while there are expressions or get error
            while(r.Available > 0){
                r.ReadToFirstChar();

                // Syntax check must consider that there is alwas match !!!
                if(syntaxCheckOnly){
                    lastMatchValue = true;
                }

                #region () Groupped matchers

                // () Groupped matchers
                if(r.StartsWith("(")){
                    lastMatchValue = Match(syntaxCheckOnly,new LumiSoft.Net.StringReader(r.ReadParenthesized()),mailFrom,rcptTo,smtpSession,mime,messageSize);

                    possibleClauseItems = PossibleClauseItem.Parenthesizes | PossibleClauseItem.Matcher | PossibleClauseItem.NOT;
                }

                #endregion

                #region AND clause

                // AND clause
                else if(r.StartsWith("and",false)){
                    // See if AND allowed
                    if((possibleClauseItems & PossibleClauseItem.AND) == 0){
                        throw new Exception("Invalid syntax: '" + ClauseItemsToString(possibleClauseItems) + "' expected !");
                    }
                    
                    // Last match value is false, no need to check next conditions
                    if(!lastMatchValue){
                        return false;
                    }

                    // Remove AND
                    r.ReadWord();
                    r.ReadToFirstChar();

                    lastMatchValue = Match(syntaxCheckOnly,r,mailFrom,rcptTo,smtpSession,mime,messageSize);

                    possibleClauseItems = PossibleClauseItem.Parenthesizes | PossibleClauseItem.Matcher | PossibleClauseItem.NOT;
                }

                #endregion

                #region OR clause

                // OR clause
                else if(r.StartsWith("or",false)){
                    // See if OR allowed
                    if((possibleClauseItems & PossibleClauseItem.OR) == 0){
                        throw new Exception("Invalid syntax: '" + ClauseItemsToString(possibleClauseItems) + "' expected !");
                    }

                    // Remove OR
                    r.ReadWord();
                    r.ReadToFirstChar();

                    // Last match value is false, then we need to check next condition.
                    // Otherwise OR is matched already, just eat next matcher.
                    if(lastMatchValue){
                        // Skip next clause
                        Match(syntaxCheckOnly,r,mailFrom,rcptTo,smtpSession,mime,messageSize);
                    }
                    else{
                        lastMatchValue = Match(syntaxCheckOnly,r,mailFrom,rcptTo,smtpSession,mime,messageSize);
                    }

                    possibleClauseItems = PossibleClauseItem.Parenthesizes | PossibleClauseItem.Matcher | PossibleClauseItem.NOT;
                }

                #endregion

                #region NOT clause

                // NOT clause
                else if(r.StartsWith("not",false)){
                    // See if NOT allowed
                    if((possibleClauseItems & PossibleClauseItem.NOT) == 0){
                        throw new Exception("Invalid syntax: '" + ClauseItemsToString(possibleClauseItems) + "' expected !");
                    }

                    // Remove NOT
                    r.ReadWord();
                    r.ReadToFirstChar();

                    // Just reverse match result value
                    lastMatchValue = !Match(syntaxCheckOnly,r,mailFrom,rcptTo,smtpSession,mime,messageSize);

                    possibleClauseItems = PossibleClauseItem.Parenthesizes | PossibleClauseItem.Matcher;
                }

                #endregion

                else{
                    // See if matcher allowed
                    if((possibleClauseItems & PossibleClauseItem.Matcher) == 0){
                        throw new Exception("Invalid syntax: '" + ClauseItemsToString(possibleClauseItems) + "' expected ! \r\n\r\n Near: '" + r.OriginalString.Substring(0,r.Position) + "'");
                    }

                    // 1) matchsource
                    // 2) keyword
                
                    // Read match source
                    string word = r.ReadWord();
                    if(word == null){
                        throw new Exception("Invalid syntax: matcher is missing !");
                    }
                    word = word.ToLower();
                    string[] matchSourceValues = new string[]{};


                    #region smtp.mail_from
                    
                    // SMTP command MAIL FROM: value.
                    //  smtp.mail_from
                    if(word == "smtp.mail_from"){
                        if(!syntaxCheckOnly){
                            matchSourceValues = new string[]{mailFrom};
                        }
                    }

                    #endregion

                    #region smtp.rcpt_to

                    // SMTP command RCPT TO: values.
                    //  smtp.mail_to
                    else if(word == "smtp.rcpt_to"){
                        if(!syntaxCheckOnly){
                            matchSourceValues = rcptTo;
                        }
                    }

                    #endregion

                    #region smtp.ehlo

                    // SMTP command EHLO/HELO: value.
                    //  smtp.ehlo
                    else if(word == "smtp.ehlo"){
                        if(!syntaxCheckOnly){
                            matchSourceValues = new string[]{smtpSession.EhloHost};
                        }
                    }

                    #endregion

                    #region smtp.authenticated

                    // Specifies if SMTP session is authenticated.
                    //  smtp.authenticated
                    else if(word == "smtp.authenticated"){
                        if(!syntaxCheckOnly){
                            if(smtpSession != null){
                                matchSourceValues = new string[]{smtpSession.IsAuthenticated.ToString()};
                            }
                        }
                    }

                    #endregion

                    #region smtp.user

                    // SMTP authenticated user name. Empy string "" if not authenticated.
                    //  smtp.user
                    else if(word == "smtp.user"){
                        if(!syntaxCheckOnly){
                            if(smtpSession != null && smtpSession.AuthenticatedUserIdentity != null){
                                matchSourceValues = new string[]{smtpSession.AuthenticatedUserIdentity.Name};
                            }
                        }
                    }

                    #endregion

                    #region smtp.remote_ip

                    // SMTP session connected client IP address.
                    //  smtp.remote_ip
                    else if(word == "smtp.remote_ip"){
                        if(!syntaxCheckOnly){
                            if(smtpSession != null){
                                matchSourceValues = new string[]{smtpSession.RemoteEndPoint.Address.ToString()};
                            }
                        }
                    }

                    #endregion

                    
                    #region message.size

                    // Message size in bytes.
                    //  message.size
                    else if(word == "message.size"){
                        if(!syntaxCheckOnly){
                            matchSourceValues = new string[]{messageSize.ToString()};
                        }
                    }

                    #endregion

                    #region message.header <SP> "HeaderFieldName:"

                    // Message main header header field. If multiple header fields, then all are checked.
                    //  message.header <SP> "HeaderFieldName:"
                    else if(word == "message.header"){
                        string headerFieldName = r.ReadWord();
                        if(headerFieldName == null){
                            throw new Exception("Match source MainHeaderField HeaderFieldName is missing ! Syntax:{MainHeaderField <SP> \"HeaderFieldName:\"}");
                        }
                    
                        if(!syntaxCheckOnly){
                            if(mime.Header.Contains(headerFieldName)){
                                MIME_h[] fields = mime.Header[headerFieldName];
                                matchSourceValues = new string[fields.Length];
                                for(int i=0;i<matchSourceValues.Length;i++){
                                    matchSourceValues[i] = fields[i].ValueToString();
                                }
                            }
                        }
                    }

                    #endregion

                    #region message.all_headers <SP> "HeaderFieldName:"

                    // Any mime entity header header field. If multiple header fields, then all are checked.
                    //  message.all_headers <SP> "HeaderFieldName:"
                    else if(word == "message.all_headers"){
                        string headerFieldName = r.ReadWord();
                        if(headerFieldName == null){
                            throw new Exception("Match source MainHeaderField HeaderFieldName is missing ! Syntax:{MainHeaderField <SP> \"HeaderFieldName:\"}");
                        }

                        if(!syntaxCheckOnly){
                            List<string> values = new List<string>();
                            foreach(MIME_Entity entity in mime.AllEntities){
                                if(entity.Header.Contains(headerFieldName)){
                                    MIME_h[] fields = entity.Header[headerFieldName];
                                    for(int i=0;i<fields.Length;i++){
                                        values.Add(fields[i].ValueToString());
                                    }
                                }
                            }
                            matchSourceValues = values.ToArray();
                        }
                    }

                    #endregion

                    #region message.body_text

                    // Message body text.
                    //  message.body_text
                    else if(word == "message.body_text"){
                        if(!syntaxCheckOnly){
                            matchSourceValues = new string[]{mime.BodyText};
                        }
                    }

                    #endregion

                    #region message.body_html

                    // Message body html.
                    //  message.body_html
                    else if(word == "message.body_html"){
                        if(!syntaxCheckOnly){
                            matchSourceValues = new string[]{mime.BodyHtmlText};
                        }
                    }

                    #endregion

                    #region message.content_md5

                    // Message any mime entity decoded data MD5 hash.
                    //  message.content_md5
                    else if(word == "message.content_md5"){
                        if(!syntaxCheckOnly){
                            List<string> values = new List<string>();
                            foreach(MIME_Entity entity in mime.AllEntities){
                                try{
                                    if(entity.Body is MIME_b_SinglepartBase){
                                        byte[] data = ((MIME_b_SinglepartBase)entity.Body).Data;
                                        if(data != null){
                                            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                                            values.Add(System.Text.Encoding.Default.GetString(md5.ComputeHash(data)));
                                        }
                                    }
                                }
                                catch{
                                    // Message data parsing failed, just skip that entity md5
                                }
                            }
                            matchSourceValues = values.ToArray();
                        }
                    }

                    #endregion


                    #region sys.date_time

                    // System current date time. Format: yyyy.MM.dd HH:mm:ss.
                    //  sys.date_time
                    else if(word == "sys.date_time"){
                        if(!syntaxCheckOnly){
                            matchSourceValues = new string[]{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")};
                        }
                    }

                    #endregion

                    #region sys.date

                    // System current date. Format: yyyy.MM.dd.
                    //  sys.date
                    else if(word == "sys.date"){
                        if(!syntaxCheckOnly){
                            matchSourceValues = new string[]{DateTime.Today.ToString("dd.MM.yyyy")};
                        }
                    }

                    #endregion

                    #region sys.time

                    // System current time. Format: HH:mm:ss.
                    //  sys.time
                    else if(word == "sys.time"){
                        if(!syntaxCheckOnly){
                            matchSourceValues = new string[]{DateTime.Now.ToString("HH:mm:ss")};
                        }
                    }

                    #endregion

                    #region sys.day_of_week

                    // Day of week. Days: sunday,monday,tuesday,wednesday,thursday,friday,saturday.
                    //  sys.day_of_week
                    else if(word == "sys.day_of_week"){
                        if(!syntaxCheckOnly){
                            matchSourceValues = new string[]{DateTime.Today.DayOfWeek.ToString()};
                        }
                    }

                    #endregion

                    /*
                    // Day of month. Format: 1 - 31. If no so much days in month, then replaced with month max days.
                    // sys.day_of_month
                    else if(word == "sys.day_of_month"){
                    }
*/
                    #region sys.day_of_year

                    // Month of year. Format: 1 - 12.
                    // sys.day_of_year
                    else if(word == "sys.day_of_year"){
                        if(!syntaxCheckOnly){
                            matchSourceValues = new string[]{DateTime.Today.ToString("M")};
                        }
                    }

                    #endregion

                    #region Unknown

                    // Unknown
                    else{
                        throw new Exception("Unknown match source '" + word + "' !");
                    }

                    #endregion

                    
                    /* If we reach so far, then we have valid match sorce and compare value.
                       Just do compare.
                    */

                    // Reset lastMatch result
                    lastMatchValue = false;

                    // Read matcher
                    word = r.ReadWord(true,new char[]{' '},true);
                    if(word == null){
                        throw new Exception("Invalid syntax: operator is missing ! \r\n\r\n Near: '" + r.OriginalString.Substring(0,r.Position) + "'");
                    }
                    word = word.ToLower();

                    #region * <SP> "astericPattern"

                    // * <SP> "astericPattern"
                    if(word == "*"){
                        string val = r.ReadWord();
                        if(val == null){
                            throw new Exception("Invalid syntax: <SP> \"value\" is missing !");
                        }
                        val = val.ToLower();
                        
                        if(!syntaxCheckOnly){
                            // We check matchSourceValues when first is found 
                            foreach(string matchSourceValue in matchSourceValues){                            
                                if(SCore.IsAstericMatch(val,matchSourceValue.ToLower())){
                                    lastMatchValue = true;
                                    break;
                                }
                            }
                        }
                    }

                    #endregion

                    #region !* <SP> "astericPattern"

                    // !* <SP> "astericPattern"
                    else if(word == "!*"){
                        string val = r.ReadWord();
                        if(val == null){
                            throw new Exception("Invalid syntax: <SP> \"value\" is missing !");
                        }
                        val = val.ToLower();
                        
                        if(!syntaxCheckOnly){
                            // We check matchSourceValues when first is found 
                            foreach(string matchSourceValue in matchSourceValues){                            
                                if(SCore.IsAstericMatch(val,matchSourceValue.ToLower())){
                                    lastMatchValue = false;
                                    break;
                                }
                            }
                        }
                    }

                    #endregion

                    #region == <SP> "value"

                    // == <SP> "value"
                    else if(word == "=="){
                        string val = r.ReadWord();
                        if(val == null){
                            throw new Exception("Invalid syntax: <SP> \"value\" is missing !");
                        }
                        val = val.ToLower();

                        if(!syntaxCheckOnly){
                            // We check matchSourceValues when first is found 
                            foreach(string matchSourceValue in matchSourceValues){
                                if(val == matchSourceValue.ToLower()){
                                    lastMatchValue = true;
                                    break;
                                }
                            }
                        }
                    }

                    #endregion

                    #region != <SP> "value"

                    // != <SP> "value"
                    else if(word == "!="){
                        string val = r.ReadWord();
                        if(val == null){
                            throw new Exception("Invalid syntax: <SP> \"value\" is missing !");
                        }
                        val = val.ToLower();

                        if(!syntaxCheckOnly){
                            // We check matchSourceValues when first is found, then already value equals 
                            foreach(string matchSourceValue in matchSourceValues){
                                if(val == matchSourceValue.ToLower()){
                                    lastMatchValue = false;
                                    break;
                                }
                                lastMatchValue = true;
                            }
                        }
                    }

                    #endregion

                    #region >= <SP> "value"

                    // >= <SP> "value"
                    else if(word == ">="){
                        string val = r.ReadWord();
                        if(val == null){
                            throw new Exception("Invalid syntax: <SP> \"value\" is missing !");
                        }
                        val = val.ToLower();

                        if(!syntaxCheckOnly){
                            // We check matchSourceValues when first is found 
                            foreach(string matchSourceValue in matchSourceValues){
                                if(matchSourceValue.ToLower().CompareTo(val) >= 0){
                                    lastMatchValue = true;
                                    break;
                                }
                            }
                        }
                    }

                    #endregion

                    #region <= <SP> "value"

                    // <= <SP> "value"
                    else if(word == "<="){
                        string val = r.ReadWord();
                        if(val == null){
                            throw new Exception("Invalid syntax: <SP> \"value\" is missing !");
                        }
                        val = val.ToLower();

                        if(!syntaxCheckOnly){
                            // We check matchSourceValues when first is found 
                            foreach(string matchSourceValue in matchSourceValues){
                                if(matchSourceValue.ToLower().CompareTo(val) <= 0){
                                    lastMatchValue = true;
                                    break;
                                }
                            }
                        }
                    }

                    #endregion

                    #region > <SP> "value"

                    // > <SP> "value"
                    else if(word == ">"){
                        string val = r.ReadWord();
                        if(val == null){
                            throw new Exception("Invalid syntax: <SP> \"value\" is missing !");
                        }
                        val = val.ToLower();

                        if(!syntaxCheckOnly){
                            // We check matchSourceValues when first is found 
                            foreach(string matchSourceValue in matchSourceValues){
                                if(matchSourceValue.ToLower().CompareTo(val) > 0){
                                    lastMatchValue = true;
                                    break;
                                }
                            }
                        }
                    }

                    #endregion

                    #region < <SP> "value"

                    // < <SP> "value"
                    else if(word == "<"){
                        string val = r.ReadWord();
                        if(val == null){
                            throw new Exception("Invalid syntax: <SP> \"value\" is missing !");
                        }
                        val = val.ToLower();

                        if(!syntaxCheckOnly){
                            // We check matchSourceValues when first is found 
                            foreach(string matchSourceValue in matchSourceValues){
                                if(matchSourceValue.ToLower().CompareTo(val) < 0){
                                    lastMatchValue = true;
                                    break;
                                }
                            }
                        }
                    }

                    #endregion

                    #region regex <SP> "value"

                    // Regex <SP> "value"
                    else if(word == "regex"){
                        string val = r.ReadWord();
                        if(val == null){
                            throw new Exception("Invalid syntax: <SP> \"value\" is missing !");
                        }
                        val = val.ToLower();

                        if(!syntaxCheckOnly){
                            // We check matchSourceValues when first is found 
                            foreach(string matchSourceValue in matchSourceValues){                            
                                if(Regex.IsMatch(val,matchSourceValue.ToLower())){
                                    lastMatchValue = true;
                                    break;
                                }
                            }
                        }
                    }

                    #endregion

                    #region Unknown

                    // Unknown
                    else{
                        throw new Exception("Unknown keword '" + word + "' !");
                    }

                    #endregion

                    possibleClauseItems = PossibleClauseItem.AND | PossibleClauseItem.OR;
                }
            }

            return lastMatchValue;
        }

        #endregion


        #region method ClauseItemsToString

        /// <summary>
        /// Converts PossibleClauseItem flags to human readable string.
        /// </summary>
        /// <param name="clauseItems">Cause item falgs to convert.</param>
        /// <returns></returns>
        private string ClauseItemsToString(PossibleClauseItem clauseItems)
        {
            string retVal = "";
            if((clauseItems & PossibleClauseItem.AND) != 0){
                retVal += "AND,";
            }
            if((clauseItems & PossibleClauseItem.Matcher) != 0){
                retVal += "Matcher,";
            }
            if((clauseItems & PossibleClauseItem.NOT) != 0){
                retVal += "NOT,";
            }
            if((clauseItems & PossibleClauseItem.OR) != 0){
                retVal += "OR,";
            }
            if((clauseItems & PossibleClauseItem.Parenthesizes) != 0){
                retVal += "Parenthesizes,";
            }
            if(retVal.EndsWith(",")){
                retVal = retVal.Substring(0,retVal.Length - 1);
            }

            return retVal.Trim();
        }

        #endregion
                        
    }
}
