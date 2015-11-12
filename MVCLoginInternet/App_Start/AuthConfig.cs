using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using MVCLoginInternet.Models;

namespace MVCLoginInternet
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            OAuthWebSecurity.RegisterClient(new GooglePlusClient("719594808492-29iqj1rkt2fpfq8s70ve75ag0qn0al26.apps.googleusercontent.com", "zGpyWCYWbMDJp8cI1oWAnK9r"), "Google+", null);
        }
    }
}