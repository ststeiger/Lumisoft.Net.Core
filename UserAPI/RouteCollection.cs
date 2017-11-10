using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

using LumiSoft.Net;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// The RouteCollection object represents route groups in LumiSoft Mail Server virtual server.
    /// </summary>
    public class RouteCollection : IEnumerable
    {
        private VirtualServer m_pVirtualServer = null;
        private List<Route>   m_pRoutes        = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="virtualServer">Owner virtual server.</param>
        internal RouteCollection(VirtualServer virtualServer)
        {
            m_pVirtualServer = virtualServer;
            m_pRoutes        = new List<Route>();

            Bind();
        }


        #region method Add

        /// <summary>
        /// Creates and adds new route to route collection.
        /// </summary>
        /// <param name="description">Route description text.</param>
        /// <param name="pattern">Route pattern.</param>
        /// <param name="enabled">Specifies if route is enabled.</param>
        /// <param name="action">Route action.</param>
        /// <returns></returns>
        public Route Add(string description,string pattern,bool enabled,RouteActionBase action)
        {
            /* AddRoute <virtualServerID> "<routeID>" <cost> "<description>" "<pattern>" <enabled> <actionType> "<actionData>:base64"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */
            
            string id   = Guid.NewGuid().ToString();
            long   cost = DateTime.Now.Ticks;

            // Call TCP AddRoute
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("AddRoute " + 
                m_pVirtualServer.VirtualServerID + " " + 
                TextUtils.QuoteString(id) + " " + 
                cost + " " +
                TextUtils.QuoteString(description) + " " + 
                TextUtils.QuoteString(pattern) + " " + 
                enabled + " " +
                (int)action.ActionType + " " +
                Convert.ToBase64String(action.Serialize())
            );
                        
            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }
            
            Route route = new Route(this,id,cost,description,pattern,enabled,action);
            m_pRoutes.Add(route);
            return route;
        }

        #endregion

        #region method Remove

        /// <summary>
        /// Deletes and removes specified route from route collection.
        /// </summary>
        /// <param name="route">Route to delete.</param>
        public void Remove(Route route)
        {
            /* DeleteRoute <virtualServerID> "<routeID>"
                  Responses:
                    +OK                     
                    -ERR <errorText>
            */
            
            string id = Guid.NewGuid().ToString();

            // Call TCP DeleteRoute
            m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("DeleteRoute " + m_pVirtualServer.VirtualServerID + " " + TextUtils.QuoteString(route.ID));
                        
            string response = m_pVirtualServer.Server.ReadLine();
            if(!response.ToUpper().StartsWith("+OK")){
                throw new Exception(response);
            }

            m_pRoutes.Remove(route);
        }

        #endregion

        #region method Contains

        /// <summary>
        /// Check if collection contains route with specified ID.
        /// </summary>
        /// <param name="routeID">Route ID.</param>
        /// <returns></returns>
        public bool Contains(string routeID)
        {
            foreach(Route route in m_pRoutes){
                if(route.ID.ToLower() == routeID.ToLower()){
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region method ContainsPattern

        /// <summary>
        /// Check if collection contains route with route pattern.
        /// </summary>
        /// <param name="pattern">Route pettern.</param>
        /// <returns></returns>
        public bool ContainsPattern(string pattern)
        {
            foreach(Route route in m_pRoutes){
                if(route.Pattern.ToLower() == pattern.ToLower()){
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region method GetRouteByPattern

        /// <summary>
        /// Gets a Route object in the collection by route pattern.
        /// </summary>
        /// <param name="pattern">Route pattern.</param>
        /// <returns>A Route object value that represents the route in virtual server.</returns>
        public Route GetRouteByPattern(string pattern)
        {
            foreach(Route route in m_pRoutes){
                if(route.Pattern.ToLower() == pattern.ToLower()){
                    return route;
                }
            }

            throw new Exception("Route with specified pattern '" + pattern + "' doesn't exist !");
        }

        #endregion

        #region method Refresh

        /// <summary>
        /// Refreshes routes.
        /// </summary>
        public void Refresh()
        {
            m_pRoutes.Clear();
            Bind();
        }

        #endregion


        #region method Bind

        /// <summary>
        /// Gets server groups and binds them to this.
        /// </summary>
        private void Bind()
        {
            /* GetRoutes <virtualServerID>
                  Responses:
                    +OK <sizeOfData>
                    <data>
                    
                    -ERR <errorText>
            */
            
            lock(m_pVirtualServer.Server.LockSynchronizer){
                // Call TCP GetRoutes
                m_pVirtualServer.Server.TcpClient.TcpStream.WriteLine("GetRoutes " + m_pVirtualServer.VirtualServerID);

                string response = m_pVirtualServer.Server.ReadLine();
                if(!response.ToUpper().StartsWith("+OK")){
                    throw new Exception(response);
                }

                int sizeOfData = Convert.ToInt32(response.Split(new char[]{' '},2)[1]);
                MemoryStream ms = new MemoryStream();
                m_pVirtualServer.Server.TcpClient.TcpStream.ReadFixedCount(ms,sizeOfData);
                
                // Decompress dataset
                DataSet ds = Utils.DecompressDataSet(ms);

                if(ds.Tables.Contains("Routing")){
                    foreach(DataRow dr in ds.Tables["Routing"].Rows){
                        RouteAction_enum actionType = (RouteAction_enum)Convert.ToInt32(dr["Action"]);
                        RouteActionBase action = null;
                        if(actionType == RouteAction_enum.RouteToEmail){
                            action = new RouteAction_RouteToEmail(Convert.FromBase64String(dr["ActionData"].ToString()));
                        }
                        else if(actionType == RouteAction_enum.RouteToHost){
                            action = new RouteAction_RouteToHost(Convert.FromBase64String(dr["ActionData"].ToString()));
                        }
                        else if(actionType == RouteAction_enum.RouteToMailbox){
                            action = new RouteAction_RouteToMailbox(Convert.FromBase64String(dr["ActionData"].ToString()));
                        }
                      
                        m_pRoutes.Add(new Route(
                            this,
                            dr["RouteID"].ToString(),
                            Convert.ToInt64(dr["Cost"]),
                            dr["Description"].ToString(),
                            dr["Pattern"].ToString(),
                            Convert.ToBoolean(dr["Enabled"].ToString()),
                            action
                        ));
                    }
                }
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
			return m_pRoutes.GetEnumerator();
		}

		#endregion


        #region Properties Implementaion
        
        /// <summary>
        /// Gets the VirtualServer object that is the owner of this collection.
        /// </summary>
        public VirtualServer VirtualServer
        {
            get{ return m_pVirtualServer; }
        }

        /// <summary>
        /// Gets number of routes in virtual server.
        /// </summary>
        public int Count
        {
            get{ return m_pRoutes.Count; }
        }

        /// <summary>
        /// Gets a Route object in the collection by index number.
        /// </summary>
        /// <param name="index">An Int32 value that specifies the position of the Route object in the RouteCollection collection.</param>
        /// <returns>A Route object value that represents the route in virtual server.</returns>
        public Route this[int index]
        {
            get{ return m_pRoutes[index]; }
        }

        /// <summary>
        /// Gets a Route object in the collection by group ID.
        /// </summary>
        /// <param name="routeID">A String value that specifies the group ID of the Route object in the RouteCollection collection.</param>
        /// <returns>A Route object value that represents the route in virtual server.</returns>
        public Route this[string routeID]
        {
            get{ 
                foreach(Route route in m_pRoutes){
                    if(route.ID.ToLower() == routeID.ToLower()){
                        return route;
                    }
                }

                throw new Exception("Route with specified ID '" + routeID + "' doesn't exist !"); 
            }
        }
        
        #endregion

    }
}
