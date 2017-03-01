using Sitecore.Diagnostics;
using Sitecore.Pipelines.LoggingIn;
using Sitecore.Security.Accounts;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Web.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitecore.Support.Pipelines.LoggingIn
{
    public class ClearSession
    {
        public void Process(LoggingInArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            User user = User.FromName(args.Username, false);

            if (user != null)
            {
                DomainAccessGuard.Sessions.RemoveAll(p => p.UserName == user.Name);
            }
        }
    }
}
