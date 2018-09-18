using Sitecore.Pipelines.LoggedIn;
using System;
using System.Web;

namespace Sitecore.Support
{
    public class RemoveMultipleSession
    {
          public void Process(LoggedInArgs args)
          {
                var coreDb = Data.Database.GetDatabase("core");
                var tickets = coreDb.PropertyStore.GetPropertyKeys("SC_TICKET"); // take all the user login tickets
                if (tickets != null)
                {
                    foreach (var ticket in tickets)
                    {
                        Web.Authentication.Ticket ticketByKey = Web.Authentication.Ticket.Parse(coreDb.PropertyStore.GetStringValue(ticket));
                        if (string.Equals(Context.GetUserName(), ticketByKey.UserName) && !string.Equals(HttpContext.Current.Session.SessionID, ticketByKey.ClientId)) // remove the tickets for the same user name, but different ASP.Net session ID
                        {
                            Web.Authentication.DomainAccessGuard.Kick(ticketByKey.ClientId);
                            Web.Authentication.TicketManager.RemoveTicket(ticketByKey.Id);
                        }
                    }
                }
          }
    }
}