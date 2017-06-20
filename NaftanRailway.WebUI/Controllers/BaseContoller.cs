﻿using log4net;
using NaftanRailway.BLL.DTO.Admin;
using System;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Web.Hosting;
using System.Web.Mvc;

namespace NaftanRailway.WebUI.Controllers {
    /// <summary>
    /// Use BaseController
    //  It is recommended to use Base Controller with our Controller and this Base Controller will inherit Controller class directly.
    /// It provides isolation space between our Controller[InterviewController] and Controller.
    /// Using Base Controller, we can write common logic which could be shared by all Controllers.
    /// (Auth, exception logic)
    /// Although in basic controller we have properties for access to httpContextObjec
    /// </summary>
    //[AllowAnonymous]
    public abstract class BaseController : Controller {
        private readonly object _threadLock = new object();
        public static ILog Log { get; private set; }

        /// <summary>
        /// current AD user
        /// </summary>
        public ADUserDTO CurrentADUser {
            get {
                string identity = User.Identity.Name;

                Func<PrincipalContext, ADUserDTO> func = delegate (PrincipalContext ctx) {
                    var userPrinc = UserPrincipal.FindByIdentity(ctx, identity);
                    if (userPrinc != null) {
                        return new ADUserDTO {
                            FullName = userPrinc.Name,
                            Domain = userPrinc.Name,
                            Name = userPrinc.DisplayName,
                            EmailAddress = userPrinc.EmailAddress,
                            Phone = userPrinc.VoiceTelephoneNumber,
                            Sam = userPrinc.SamAccountName,
                            PrincipalName = userPrinc.UserPrincipalName,
                            Groups = userPrinc.GetGroups().Select(gr => new ADGroupDTO { Name = gr.Name }).ToList()
                        };
                    }
                    return new ADUserDTO { Name = "Anonymous", Description = "Information not found" };
                };

                //Home station without AD
                if (Request.IsLocal) {
                    using (var ctx = new PrincipalContext(ContextType.Machine) { }) {
                        return func(ctx);
                    }
                }

                //On work
                var domainHost = identity.Substring(0, 7).ToLower() == "polymir" ? "POLYMIR.NET" : "lan.naftan.by";
                using (var ctx = new PrincipalContext(ContextType.Domain, domainHost)) {
                    return func(ctx);
                }
            }
        }

        public string BrowserInfo {
            get { return GetBrowserInfo(); }
        }

        public BaseController(ILog logger) {
            Log = logger;
        }

        //Summarize information about user and environment
        private string GetBrowserInfo() {
            var browser = Request.Browser;
            var userName = User.Identity.Name;
            var totalOnlineUsers = (int)@HttpContext.Application["TotalOnlineUsers"];

            var result = String.Format(
                 "Browser: {0} {1},<br />EcmaScript: {2},<br />JavaScript: {3},<br />Platform: {4}," +
                 "<br />Cookies: {5},<br />ActiveXControls: {6},<br />JavaApplets {7},<br />Frames: {8}," +
                 "<br />IsMobile: {9},<br />Manufacture: {10},<br />Model: {11}," +
                 "<br />User Name: {12}{13},<br />Online: {14}",
             browser.Browser,
             browser.Version,
             browser.EcmaScriptVersion,
             browser["JavaScriptVersion"],
             browser.Platform,
             browser.Cookies,
             browser.ActiveXControls,
             browser.JavaApplets,
             browser.Frames,
             browser.IsMobileDevice,
             browser.MobileDeviceManufacturer,
             browser.MobileDeviceModel,
             string.Format("{0} ({1})", CurrentADUser.Name, CurrentADUser.EmailAddress),
             userName.Length == 0 ? "" : string.Format("({0})", userName.Replace(@"\", "&#92;")),
             totalOnlineUsers
            );

            return result;
        }

        //return log file
        public virtual FileContentResult GetLog() {
            var txt = String.Empty;

            var serverPath = Server.MapPath("~/") ?? HostingEnvironment.ApplicationPhysicalPath;
            var logpath = Path.Combine(serverPath, @"logs\log.txt");

            try {
                lock (_threadLock) {
                    //CreateDirectory create all needed subfolders
                    if (!System.IO.File.Exists(logpath)) {
                        Directory.CreateDirectory(Path.Combine(serverPath, @"logs\"));
                        using (var stream = System.IO.File.Create(logpath)) {
                            stream.Close();
                        }
                    }

                    using (var fileStream = new FileStream(logpath, FileMode.Open, FileAccess.Read))
                    using (var streamReader = new StreamReader(fileStream, Encoding.UTF8)) {
                        txt = streamReader.ReadToEnd();
                    }
                }
            } catch (Exception ex) {
                txt = String.Format("Возникло исключение: {3}{0}{3}Server.MapPath(~/): \"{1}\"{3}HostingEnvironment.ApplicationPhysicalPath: \"{2}\"{3}",
                    ex.Message, Server.MapPath("~/"),
                    HostingEnvironment.ApplicationPhysicalPath,
                    Environment.NewLine
                );

                Log.Debug(ex.Message);
            }

            var cd = new ContentDisposition {
                FileName = String.Format("Лог_{0}.txt", DateTime.Now),
                // always prompt the user for downloading, set to true if you want the browser to try to show the file inline
                Inline = false,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(Encoding.UTF8.GetBytes(txt), @"text/plain"/*, "Лог.txt"*/);
        }
    }
}