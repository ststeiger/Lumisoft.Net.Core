
namespace LumiSoft.MailServer
{


    /// <summary>
    /// Route actions.
    /// </summary>
    public enum RouteAction_enum
    {
        /// <summary>
        /// Routes message to local server user mailbox.
        /// </summary>
        RouteToMailbox = 1,

        /// <summary>
        /// Routes message to email address.
        /// </summary>
        RouteToEmail = 2,

        /// <summary>
        /// Routes message to destination host.
        /// </summary>
        RouteToHost = 3,
    }


}
