using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
    /// <summary>
	/// SIP header fields collection.
	/// </summary>
	public class SIP_HeaderFieldCollection : IEnumerable
	{
		private List<SIP_HeaderField> m_pHeaderFields = null;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public SIP_HeaderFieldCollection()
		{
			m_pHeaderFields = new List<SIP_HeaderField>();
		}


		#region method Add
        
		/// <summary>
		/// Adds a new header field with specified name and value to the end of the collection.
		/// </summary>
		/// <param name="fieldName">Header field name.</param>
		/// <param name="value">Header field value.</param>
		public void Add(string fieldName,string value)
		{
            Add(GetheaderField(fieldName,value));
		}

		/// <summary>
		/// Adds specified header field to the end of the collection.
		/// </summary>
		/// <param name="headerField">Header field.</param>
		public void Add(SIP_HeaderField headerField)
		{
			m_pHeaderFields.Add(headerField);
		}

		#endregion

		#region method Insert

		/// <summary>
		/// Inserts a new header field into the collection at the specified location.
		/// </summary>
		/// <param name="index">The location in the collection where you want to add the header field.</param>
		/// <param name="fieldName">Header field name.</param>
		/// <param name="value">Header field value.</param>
		public void Insert(int index,string fieldName,string value)
		{
            m_pHeaderFields.Insert(index,GetheaderField(fieldName,value));
		}

		#endregion

        #region method Set

        /// <summary>
        /// Sets specified header field value. If header field existst, first found value is set.
        /// If field doesn't exist, it will be added.
        /// </summary>
        /// <param name="fieldName">Header field name.</param>
        /// <param name="value">Header field value.</param>
        public void Set(string fieldName,string value)
        {
            SIP_HeaderField h = this.GetFirst(fieldName);
            if(h != null){
                h.Value = value;
            }
            else{
                this.Add(GetheaderField(fieldName,value));
            }
        }

        #endregion


        #region method Remove

        /// <summary>
		/// Removes header field at the specified index from the collection.
		/// </summary>
		/// <param name="index">The index of the header field to remove.</param>
		public void Remove(int index)
		{
			m_pHeaderFields.RemoveAt(index);
		}

		/// <summary>
		/// Removes specified header field from the collection.
		/// </summary>
		/// <param name="field">Header field to remove.</param>
		public void Remove(SIP_HeaderField field)
		{
			m_pHeaderFields.Remove(field);
		}

		#endregion

        #region method RemoveFirst

        /// <summary>
        /// Removes first header field with specified name.
        /// </summary>
        /// <param name="name">Header fields name.</param>
        public void RemoveFirst(string name)
        {
            foreach(SIP_HeaderField h in m_pHeaderFields){
                if(h.Name.ToLower() == name.ToLower()){
                    m_pHeaderFields.Remove(h);
                    break;
                }
            }
        }

        #endregion

        #region method RemoveAll

        /// <summary>
		/// Removes all header fields with specified name from the collection.
		/// </summary>
		/// <param name="fieldName">Header field name.</param>
		public void RemoveAll(string fieldName)
		{
			for(int i=0;i<m_pHeaderFields.Count;i++){
				SIP_HeaderField h = (SIP_HeaderField)m_pHeaderFields[i];
				if(h.Name.ToLower() == fieldName.ToLower()){
					m_pHeaderFields.Remove(h);
					i--;
				}
			}
		}

		#endregion
		
		#region method Clear

		/// <summary>
		/// Clears the collection of all header fields.
		/// </summary>
		public void Clear()
		{
			m_pHeaderFields.Clear();
		}

		#endregion


		#region method Contains

		/// <summary>
		/// Gets if collection contains specified header field.
		/// </summary>
		/// <param name="fieldName">Header field name.</param>
		/// <returns></returns>
		public bool Contains(string fieldName)
		{
			foreach(SIP_HeaderField h in m_pHeaderFields){
				if(h.Name.ToLower() == fieldName.ToLower()){
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Gets if collection contains specified header field.
		/// </summary>
		/// <param name="headerField">Header field.</param>
		/// <returns></returns>
		public bool Contains(SIP_HeaderField headerField)
		{
			return m_pHeaderFields.Contains(headerField);
		}

		#endregion


		#region method GetFirst

		/// <summary>
		/// Gets first header field with specified name, returns null if specified field doesn't exist.
		/// </summary>
		/// <param name="fieldName">Header field name.</param>
		/// <returns></returns>
		public SIP_HeaderField GetFirst(string fieldName)
		{
			foreach(SIP_HeaderField h in m_pHeaderFields){
				if(h.Name.ToLower() == fieldName.ToLower()){
					return h;
				}
			}

			return null;
		}

		#endregion

		#region method Get

		/// <summary>
		/// Gets header fields with specified name.
		/// </summary>
		/// <param name="fieldName">Header field name.</param>
		/// <returns></returns>
		public SIP_HeaderField[] Get(string fieldName)
		{
            List<SIP_HeaderField> fields = new List<SIP_HeaderField>();
			foreach(SIP_HeaderField h in m_pHeaderFields){
				if(h.Name.ToLower() == fieldName.ToLower()){
					fields.Add(h);
				}
			}

            return fields.ToArray();
		}

		#endregion


        #region method Parse
	
		/// <summary>
		/// Parses header fields from string.
		/// </summary>
		/// <param name="headerString">Header string.</param>
		public void Parse(string headerString)
		{
			Parse(new MemoryStream(Encoding.Default.GetBytes(headerString)));
		}

		/// <summary>
		/// Parses header fields from stream. Stream position stays where header reading ends.
		/// </summary>
		/// <param name="stream">Stream from where to parse.</param>
		public void Parse(Stream stream)
		{			
			/* Rfc 2822 2.2 Header Fields
				Header fields are lines composed of a field name, followed by a colon
				(":"), followed by a field body, and terminated by CRLF.  A field
				name MUST be composed of printable US-ASCII characters (i.e.,
				characters that have values between 33 and 126, inclusive), except
				colon.  A field body may be composed of any US-ASCII characters,
				except for CR and LF.  However, a field body may contain CRLF when
				used in header "folding" and  "unfolding" as described in section
				2.2.3.  All field bodies MUST conform to the syntax described in
				sections 3 and 4 of this standard. 
				
			   Rfc 2822 2.2.3 Long Header Fields
				The process of moving from this folded multiple-line representation
				of a header field to its single line representation is called
				"unfolding". Unfolding is accomplished by simply removing any CRLF
				that is immediately followed by WSP.  Each header field should be
				treated in its unfolded form for further syntactic and semantic
				evaluation.
				
				Example:
					Subject: aaaaa<CRLF>
					<TAB or SP>aaaaa<CRLF>
			*/

			m_pHeaderFields.Clear();

			StreamLineReader r = new StreamLineReader(stream);
            r.CRLF_LinesOnly = false;
			string line = r.ReadLineString();
			while(line != null){
				// End of header reached
				if(line == ""){
					break;
				}

				// Store current header line and read next. We need to read 1 header line to ahead,
				// because of multiline header fields.
				string headerField = line; 
				line = r.ReadLineString();

				// See if header field is multiline. See comment above.				
				while(line != null && (line.StartsWith("\t") || line.StartsWith(" "))){
					headerField += line;
					line = r.ReadLineString();
				}

				string[] name_value = headerField.Split(new char[]{':'},2);
				// There must be header field name and value, otherwise invalid header field
				if(name_value.Length == 2){
			        Add(name_value[0] + ":",name_value[1].Trim());
                }
			}
		}
		
		#endregion

        #region method ToHeaderString

		/// <summary>
		/// Converts header fields to SIP message header string.
		/// </summary>
		/// <returns>Returns SIP message header as string.</returns>
		public string ToHeaderString()
		{
			StringBuilder headerString = new StringBuilder();
			foreach(SIP_HeaderField f in this){                
				headerString.Append(f.Name + " " + f.Value + "\r\n");
			}
            headerString.Append("\r\n");

			return headerString.ToString();
		}

		#endregion

        
        #region method GetheaderField

        /// <summary>
        /// Gets right type header field.
        /// </summary>
        /// <param name="name">Header field name.</param>
        /// <param name="value">Header field name.</param>
        /// <returns>Returns right type header field.</returns>
        private SIP_HeaderField GetheaderField(string name,string value)
        {
            name = name.Replace(":","").Trim();

            #region  Replace short names to long 

            if(string.Equals(name,"i",StringComparison.InvariantCultureIgnoreCase)){
                name = "Call-ID";
            }
            else if(string.Equals(name,"m",StringComparison.InvariantCultureIgnoreCase)){
                name = "Contact";
            }
            else if(string.Equals(name,"e",StringComparison.InvariantCultureIgnoreCase)){
                name = "Content-Encoding";
            }
            else if(string.Equals(name,"l",StringComparison.InvariantCultureIgnoreCase)){
                name = "Content-Length";
            }
            else if(string.Equals(name,"c",StringComparison.InvariantCultureIgnoreCase)){
                name = "Content-Type";
            }
            else if(string.Equals(name,"f",StringComparison.InvariantCultureIgnoreCase)){
                name = "From";
            }
            else if(string.Equals(name,"s",StringComparison.InvariantCultureIgnoreCase)){
                name = "Subject";
            }
            else if(string.Equals(name,"k",StringComparison.InvariantCultureIgnoreCase)){
                name = "Supported";
            }
            else if(string.Equals(name,"t",StringComparison.InvariantCultureIgnoreCase)){
                name = "To";
            }
            else if(string.Equals(name,"v",StringComparison.InvariantCultureIgnoreCase)){
                name = "Via";
            }
            else if(string.Equals(name,"u",StringComparison.InvariantCultureIgnoreCase)){
                name = "AllowEevents";
            }
            else if(string.Equals(name,"r",StringComparison.InvariantCultureIgnoreCase)){
                name = "Refer-To";
            }
            else if(string.Equals(name,"d",StringComparison.InvariantCultureIgnoreCase)){
                name = "Request-Disposition";
            }
            else if(string.Equals(name,"x",StringComparison.InvariantCultureIgnoreCase)){
                name = "Session-Expires";
            }
            else if(string.Equals(name,"o",StringComparison.InvariantCultureIgnoreCase)){
                name = "Event";
            }
            else if(string.Equals(name,"b",StringComparison.InvariantCultureIgnoreCase)){
                name = "Referred-By";
            }
            else if(string.Equals(name,"a",StringComparison.InvariantCultureIgnoreCase)){
                name = "Accept-Contact";
            }
            else if(string.Equals(name,"y",StringComparison.InvariantCultureIgnoreCase)){
                name = "Identity";
            }
            else if(string.Equals(name,"n",StringComparison.InvariantCultureIgnoreCase)){
                name = "Identity-Info";
            }
            else if(string.Equals(name,"j",StringComparison.InvariantCultureIgnoreCase)){
                name = "Reject-Contact";
            }

            #endregion

            if(string.Equals(name,"accept",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_AcceptRange>("Accept:",value);
            }
            else if(string.Equals(name,"accept-contact",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_ACValue>("Accept-Contact:",value);
            }
            else if(string.Equals(name,"accept-encoding",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_Encoding>("Accept-Encoding:",value);
            }
            else if(string.Equals(name,"accept-language",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_Language>("Accept-Language:",value);
            }
            else if(string.Equals(name,"accept-resource-priority",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_RValue>("Accept-Resource-Priority:",value);
            }
            else if(string.Equals(name,"alert-info",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_AlertParam>("Alert-Info:",value);
            }
            else if(string.Equals(name,"allow",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_Method>("Allow:",value);
            }
            else if(string.Equals(name,"allow-events",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_EventType>("Allow-Events:",value);
            }
            else if(string.Equals(name,"authentication-info",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_SingleValueHF<SIP_t_AuthenticationInfo>("Authentication-Info:",new SIP_t_AuthenticationInfo(value));
            }
            else if(string.Equals(name,"authorization",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_SingleValueHF<SIP_t_Credentials>("Authorization:",new SIP_t_Credentials(value));
            }
            else if(string.Equals(name,"contact",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_ContactParam>("Contact:",value);
            }
            else if(string.Equals(name,"Content-Disposition",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_SingleValueHF<SIP_t_ContentDisposition>("Content-Disposition:",new SIP_t_ContentDisposition(value));
            }
            else if(string.Equals(name,"cseq",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_SingleValueHF<SIP_t_CSeq>("CSeq:",new SIP_t_CSeq(value));
            }
            else if(string.Equals(name,"content-encoding",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_ContentCoding>("Content-Encoding:",value);
            }
            else if(string.Equals(name,"content-language",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_LanguageTag>("Content-Language:",value);
            }
            else if(string.Equals(name,"error-info",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_ErrorUri>("Error-Info:",value);
            }
            else if(string.Equals(name,"event",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_SingleValueHF<SIP_t_Event>("Event:",new SIP_t_Event(value));
            }
            else if(string.Equals(name,"from",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_SingleValueHF<SIP_t_From>("From:",new SIP_t_From(value));
            }
            else if(string.Equals(name,"history-info",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_HiEntry>("History-Info:",value);
            }
            else if(string.Equals(name,"identity-info",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_SingleValueHF<SIP_t_IdentityInfo>("Identity-Info:",new SIP_t_IdentityInfo(value));
            }
            else if(string.Equals(name,"in-replay-to",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_CallID>("In-Reply-To:",value);
            }
            else if(string.Equals(name,"join",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_SingleValueHF<SIP_t_Join>("Join:",new SIP_t_Join(value));
            }
            else if(string.Equals(name,"min-se",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_SingleValueHF<SIP_t_MinSE>("Min-SE:",new SIP_t_MinSE(value));
            }
            else if(string.Equals(name,"path",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_AddressParam>("Path:",value);
            }
            else if(string.Equals(name,"proxy-authenticate",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_SingleValueHF<SIP_t_Challenge>("Proxy-Authenticate:",new SIP_t_Challenge(value));
            }
            else if(string.Equals(name,"proxy-authorization",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_SingleValueHF<SIP_t_Credentials>("Proxy-Authorization:",new SIP_t_Credentials(value));
            }
            else if(string.Equals(name,"proxy-require",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_OptionTag>("Proxy-Require:",value);
            }
            else if(string.Equals(name,"rack",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_SingleValueHF<SIP_t_RAck>("RAck:",new SIP_t_RAck(value));
            }
            else if(string.Equals(name,"reason",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_ReasonValue>("Reason:",value);
            }
            else if(string.Equals(name,"record-route",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_AddressParam>("Record-Route:",value);
            }
            else if(string.Equals(name,"refer-sub",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_SingleValueHF<SIP_t_ReferSub>("Refer-Sub:",new SIP_t_ReferSub(value));
            }
            else if(string.Equals(name,"refer-to",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_SingleValueHF<SIP_t_AddressParam>("Refer-To:",new SIP_t_AddressParam(value));
            }
            else if(string.Equals(name,"referred-by",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_SingleValueHF<SIP_t_ReferredBy>("Referred-By:",new SIP_t_ReferredBy(value));
            }
            else if(string.Equals(name,"reject-contact",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_RCValue>("Reject-Contact:",value);
            }
            else if(string.Equals(name,"replaces",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_SingleValueHF<SIP_t_SessionExpires>("Replaces:",new SIP_t_SessionExpires(value));
            }
            else if(string.Equals(name,"reply-to",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_AddressParam>("Reply-To:",value);
            }
            else if(string.Equals(name,"request-disposition",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_Directive>("Request-Disposition:",value);
            }
            else if(string.Equals(name,"require",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_OptionTag>("Require:",value);
            }
            else if(string.Equals(name,"resource-priority",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_RValue>("Resource-Priority:",value);
            }
            else if(string.Equals(name,"retry-after",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_SingleValueHF<SIP_t_RetryAfter>("Retry-After:",new SIP_t_RetryAfter(value));
            }
            else if(string.Equals(name,"route",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_AddressParam>("Route:",value);
            }
            else if(string.Equals(name,"security-client",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_SecMechanism>("Security-Client:",value);
            }
            else if(string.Equals(name,"security-server",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_SecMechanism>("Security-Server:",value);
            }
            else if(string.Equals(name,"security-verify",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_SecMechanism>("Security-Verify:",value);
            }
            else if(string.Equals(name,"service-route",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_AddressParam>("Service-Route:",value);
            }
            else if(string.Equals(name,"session-expires",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_SingleValueHF<SIP_t_SessionExpires>("Session-Expires:",new SIP_t_SessionExpires(value));
            }
            else if(string.Equals(name,"subscription-state",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_SingleValueHF<SIP_t_SubscriptionState>("Subscription-State:",new SIP_t_SubscriptionState(value));
            }
            else if(string.Equals(name,"supported",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_OptionTag>("Supported:",value);
            }
            else if(string.Equals(name,"target-dialog",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_SingleValueHF<SIP_t_TargetDialog>("Target-Dialog:",new SIP_t_TargetDialog(value));
            }
            else if(string.Equals(name,"timestamp",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_SingleValueHF<SIP_t_Timestamp>("Timestamp:",new SIP_t_Timestamp(value));
            }
            else if(string.Equals(name,"to",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_SingleValueHF<SIP_t_To>("To:",new SIP_t_To(value));
            }
            else if(string.Equals(name,"unsupported",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_OptionTag>("Unsupported:",value);
            }
            else if(string.Equals(name,"via",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_ViaParm>("Via:",value);
            }
            else if(string.Equals(name,"warning",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_MultiValueHF<SIP_t_WarningValue>("Warning:",value);
            }
            else if(string.Equals(name,"www-authenticate",StringComparison.InvariantCultureIgnoreCase)){
                return new SIP_SingleValueHF<SIP_t_Challenge>("WWW-Authenticate:",new SIP_t_Challenge(value));
            }
            else{
                return new SIP_HeaderField(name + ":",value);
            }
        }

        #endregion


        #region interface IEnumerator

        /// <summary>
		/// Gets enumerator.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return m_pHeaderFields.GetEnumerator();
		}

		#endregion

		#region Properties Implementation
		
		/// <summary>
		/// Gets header field from specified index.
		/// </summary>
		public SIP_HeaderField this[int index]
		{
			get{ return (SIP_HeaderField)m_pHeaderFields[index]; }
		}

		/// <summary>
		/// Gets header fields count in the collection.
		/// </summary>
		public int Count
		{
			get{ return m_pHeaderFields.Count; }
		}

		#endregion

	}
}
