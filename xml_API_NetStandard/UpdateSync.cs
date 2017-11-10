using System;
using System.Threading;

namespace LumiSoft.MailServer
{
	/// <summary>
	/// Updates synchronizer.
	/// </summary>
	internal class UpdateSync
	{
		private bool   m_BlockReads = false;
		private Thread m_UpdTr      = null;
		private int    m_Updates    = 0;
		public int    m_Reads      = 0;
		private object m_UpdSync    = null;

		public UpdateSync()
		{	
			m_UpdSync = new object();
		}

		#region method AddMethod

		public void AddMethod()
		{
			// Allow only update thread to access this method if update is active 
            while(m_BlockReads && !Thread.CurrentThread.Equals(m_UpdTr)){
				System.Threading.Thread.Sleep(50);
			}
		
			lock(this){
				m_Reads++;
			}
		}

		#endregion

		#region method RemoveMethod

		public void RemoveMethod()
		{
			lock(this){
				m_Reads--;

				if(m_Reads < 0){
					throw new Exception("RemoveMethod < 0, RemoveMethod is called more than AddMethod !");
				}
			}
		}

		#endregion

		#region method BeginUpdate

		public void BeginUpdate()
		{	
			// Enter block all Threads except current thread
			Monitor.Enter(m_UpdSync);
			m_Updates++;

			// Wait while there any method isn't accessing API.
			while(true){
				lock(this){
					if(m_Reads == 0){
						m_BlockReads = true;
						break;
					}

					System.Threading.Thread.Sleep(50);
				}
			}

			m_UpdTr = Thread.CurrentThread;
		}

		#endregion

		#region method EndUpdate

		public void EndUpdate()
		{
			m_Updates--;

			if(m_Updates == 0){
				m_BlockReads = false;
				m_UpdTr      = null;
			}

			Monitor.Exit(m_UpdSync);
		}

		#endregion
	}
}
