
using System;
using System.Net;
using System.Net.Sockets;

using LumiSoft.Net;
using LumiSoft.Net.TCP;


namespace LumiSoft.MailServer.Monitoring
{


    /// <summary>
    /// LumiSoft mailserver monitoring server.
    /// </summary>
    public class MonitoringServer
        : TCP_Server<MonitoringServerSession>
    {
        private int m_MaxBadCommands = 30;
        private Server m_pServer = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MonitoringServer(Server server) : base()
        {
            m_pServer = server;

            if (Socket.OSSupportsIPv6)
            {
                this.Bindings = new IPBindInfo[]{
                    new IPBindInfo(Dns.GetHostName(),BindInfoProtocol.TCP,IPAddress.Any,5252),
                    new IPBindInfo(Dns.GetHostName(),BindInfoProtocol.TCP,IPAddress.IPv6Any,5252)
                };
            }
            else
            {
                this.Bindings = new IPBindInfo[] { new IPBindInfo(Dns.GetHostName(), BindInfoProtocol.TCP, IPAddress.Any, 5252) };
            }
            this.SessionIdleTimeout = 30 * 60; // Session idle timeout after 30 min
        }


        /// <summary>
        /// Gets or sets how many bad commands session can have before it's terminated. Value 0 means unlimited.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <exception cref="ArgumentException">Is raised when invalid value is passed.</exception>
        public int MaxBadCommands
        {
            get
            {
                if (this.IsDisposed)
                {
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                return m_MaxBadCommands;
            }

            set
            {
                if (this.IsDisposed)
                {
                    throw new ObjectDisposedException(this.GetType().Name);
                }
                if (value < 0)
                {
                    throw new ArgumentException("Property 'MaxBadCommands' value must be >= 0.");
                }

                m_MaxBadCommands = value;
            }
        }

        /// <summary>
		/// Gets reference to MailServer.
		/// </summary>
		public Server MailServer
        {
            get { return m_pServer; }
        }


    }
}
