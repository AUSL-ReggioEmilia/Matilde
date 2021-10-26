﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using UnicodeSrl.NotSvc;

namespace NotificationWebSvc
{
    /// <summary>
    /// Summary description for TestSvc
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class TestSvc : System.Web.Services.WebService
    {

        [WebMethod]
        public void HelloWorld()
        {
            Exception e = new Exception("Test");

            ErrorHandler.AddException(e);
        }
    }
}
