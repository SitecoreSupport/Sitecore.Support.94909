namespace Sitecore.Support
{
    using System;
    using Sitecore.Pipelines.LoggedIn;
    using Sitecore.Web.Authentication;
    using System.Web;
    using Sitecore.Diagnostics;
    using Sitecore.Pipelines.Login;

    public class RemoveMultipleSession
    {
        public void Process(LoggedInArgs args)
        {
            try
            {
                var ticketIDs = TicketManager.GetTicketIDs();

                if (ticketIDs != null)
                {
                    foreach (string ticketID in ticketIDs)
                    {
                        //Substring removes the leading underscore in the ticket ID
                        string id = ticketID.Substring(1);
                        Log.Debug(string.Format("Checking for duplicate session associated with ticket with ID: {0}", id), this);
                        Web.Authentication.Ticket ticket = TicketManager.GetTicket(id);

                        // Remove session and ticket for user entries with the same user name, but different session ID
                        if ((ticket != null) && !string.IsNullOrEmpty(ticket?.ClientId) &&
                            string.Equals(Context.GetUserName(), ticket?.UserName, StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(HttpContext.Current.Session.SessionID, ticket.ClientId, StringComparison.OrdinalIgnoreCase))
                        {
                            TicketManager.RemoveTicket(ticket.Id);
                            Log.Debug(string.Format("Removed ticket with ID: {0}", ticket.Id), this);

                            DomainAccessGuard.Kick(ticket.ClientId);
                            Log.Debug(string.Format("Kicked session with ID: {0}", ticket.ClientId), this);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception occured in the Sitecore.Support.RemoveMultipleSession Processor", ex, this);
            }
        }
    }
}