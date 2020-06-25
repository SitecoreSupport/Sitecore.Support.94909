namespace Sitecore.Support
{
    using System;
    using Sitecore.Pipelines.LoggedIn;
    using Sitecore.Web.Authentication;
    using System.Web;
    using Sitecore.Diagnostics;

    public class RemoveMultipleSession
    {
        public void Process(LoggedInArgs args)
        {
            try
            {
                //Get all of the ticket IDs
                var ticketIDs = TicketManager.GetTicketIDs();
                if (ticketIDs != null)
                {
                    foreach (var ticketID in ticketIDs)
                    {
                        Web.Authentication.Ticket ticket = TicketManager.GetTicket(ticketID);

                        // Remove session and ticket for user entries with the same user name, but different session ID
                        if ((ticket != null) && !string.IsNullOrEmpty(ticket.ClientId) && string.Equals(Context.GetUserName(), ticket.UserName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (!string.Equals(HttpContext.Current.Session.SessionID, ticket.ClientId, StringComparison.InvariantCultureIgnoreCase))
                            {
                                DomainAccessGuard.Kick(ticket.ClientId);
                                TicketManager.RemoveTicket(ticket.Id);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //If anything goes wrong, avoid crashing but log the error
                Log.Error("Exception occured in the Sitecore.Support.RemoveMultipleSession Processor", ex, this);
            }
        }
    }
}