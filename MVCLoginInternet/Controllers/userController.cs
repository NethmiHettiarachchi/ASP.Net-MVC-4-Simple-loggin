using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MVCLoginInternet.Controllers;
using MVCLoginInternet.Models;
using System.Security.Principal;

using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using MVCLoginInternet.Filters;
using System.Net;
using System.Data;

namespace MVCLoginInternet.Controllers
{

    [InitializeSimpleMembership]
    public class userController : Controller
    {
        //
        // GET: /user/

       

        public ActionResult Index()
        {
            ViewBag.DataExists = false;
            return View();
        }



        [HttpGet]
        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(MVCLoginInternet.Models.Login user)
        {
            // if(ModelState.IsValid)
            // {

            if (isValid(user.email, user.password))
            {
                FormsAuthentication.SetAuthCookie(user.email, false);

                return RedirectToAction("List");
                //return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Login Data is Incorrect");
            }
            //}

            return View(user);
        }

        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registration(MVCLoginInternet.Models.userModel user)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new MainDBEntities())
                    {
                        var sysUser = db.users.Create();

                        sysUser.userId = Guid.NewGuid();
                        sysUser.email = user.email;
                        sysUser.password = user.password;
                        sysUser.userName = user.userName;
                        sysUser.firstName = user.firstName;
                        sysUser.lastName = user.lastName;
                        //sysUser.twitterName = user.twitterName;
                        // sysUser.tw
                        //   sysUser.twitterName = user.twitterName;

                        db.users.Add(sysUser);
                        db.SaveChanges();


                        FormsAuthentication.SetAuthCookie(user.email, false);

                        return RedirectToAction("List");
                        // return RedirectToAction("Index", "Home");
                    }

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Add unique values to username and email, this user already exists");
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("", "Registration data is incorrect");
            }
              return View();
        }



        [HttpPost]
        public ActionResult LogOut()
        {
            return View();
        }

        [HttpGet]
        public ActionResult LogOut(MVCLoginInternet.Models.userModel user)
        {


            FormsAuthentication.SignOut();
            //  HttpContext.CurrentHandler
            HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
            Session.Abandon();

            return RedirectToAction("Index", "Home");
        }

        private bool isValid(String email, String password)
        {
            bool isvalid = false;

            using (var db = new MainDBEntities())
            {
                var user = db.users.FirstOrDefault(u => u.email == email);

                if (user != null)
                {
                    if (user.password == password)
                    {
                        isvalid = true;
                    }
                }
            }
            return isvalid;
        }


        public ActionResult List()
        {
            var user = System.Web.HttpContext.Current.User.Identity.Name;


            var finalResult = string.Empty;
            using (var dc = new MainDBEntities())
            {

                finalResult = dc.users.FirstOrDefault(u => u.email == user).userName;

            }

            ViewBag.MyURL = finalResult;

            Guid id;
            String userl = System.Web.HttpContext.Current.User.Identity.Name;
            using (var db = new MainDBEntities())
            {
                id = db.users.FirstOrDefault(u => u.email == userl).userId;
            }

            ViewBag.CurrentUserGuid=id.ToString(); 


            /////////////////////////////////////////


            List<user> person = new List<user>();

            using (var dc = new MainDBEntities())
            {
                person = dc.users.ToList();
            }

            return View(person.AsEnumerable());
        }


        public ActionResult DeleteSelected(String[] ids)
        {
            if (ids == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Guid[] id = null;

            //getting the guid of the current user;
            String user = System.Web.HttpContext.Current.User.Identity.Name;
            Guid currentGuid;
            using (var dc = new MainDBEntities())
            {

                currentGuid = dc.users.FirstOrDefault(u => u.email == user).userId;

            }

            //ViewBag.CurrentUserGuid = currentGuid.ToString();

            if (ids != null)
            {
                id = new Guid[ids.Length];
                int j = 0;
                foreach (String i in ids)
                {
                    Guid.TryParse(i, out id[j++]);
                }
            }

            foreach (Guid i in id)
            {
                if (i == currentGuid)
                {
                    //return JavaScript(alert('Hello this is an alert'));
                    //ViewBag.Message = "Mymessage";
                    return Content("<script language='javascript' type='text/javascript'>alert('You Cant Delete Yourself');</script>");
                   // ViewBag.DataExists = true;
                   // return RedirectToAction("List");

                }
            }


            if (id != null && id.Length > 0)
            {
                List<user> personInfo = new List<user>();
                using (var dc = new MainDBEntities())
                {
                    personInfo = dc.users.Where(u => id.Contains(u.userId)).ToList();
                    foreach (var i in personInfo)
                    {
                        dc.users.Remove(i);
                    }
                    dc.SaveChanges();
                }

            }

            return RedirectToAction("List");
        }


        // GET: /Movies/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            String userLogged = System.Web.HttpContext.Current.User.Identity.Name;
            user userEdit = null;
            //Guid currentGuid;
            using (var dc = new MainDBEntities())
            {
                userEdit = dc.users.Find(id);
            }

            if (userEdit == null)
            {
                return HttpNotFound();
            } 

            return View(userEdit);
        }

        [HttpPost]
        public ActionResult Edit([Bind(Include = "userId,email,userName,password,firstName,lastName")] user userE)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var dc = new MainDBEntities())
                    {
                        //user userEdit = dc.users.Find(userId);
                        dc.Entry(userE).State = EntityState.Modified;
                        dc.SaveChanges();
                        return RedirectToAction("List");
                    }
                }
                catch 
                {
                    ModelState.AddModelError("", "Add value to the password field");
                    return View();

                }
            }
            return View(userE);
        }

        /////////////////////////////////////////////google login////////////

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }


        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            // Rewrite request before it gets passed on to the OAuth Web Security classes
            GooglePlusClient.RewriteRequest();

            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            /////////////////////////////////////

            user userEdit = null;
            using (var dc = new MainDBEntities())
            {
                userEdit = dc.users.FirstOrDefault(u => u.email == result.UserName);
            }

            if (userEdit!=null)
            {

                // If the current user is logged in add the new account
               // OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, result.UserName);
                FormsAuthentication.SetAuthCookie(result.UserName, false);

                return RedirectToAction("List");
                //return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }


        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (MainDBEntities db = new MainDBEntities())
                {     
                    user userEx = db.users.FirstOrDefault(u => u.email == model.UserName);
                    // Check if user already exists

                    if (userEx != null)
                    {
                        userEx.userName = model.twitterName;
                        userEx.firstName = model.firstName;
                        userEx.lastName = model.lastName;
                        db.Entry(userEx).State = EntityState.Modified;
                        db.SaveChanges();
                        FormsAuthentication.SetAuthCookie(userEx.email, false);

                        return RedirectToAction("List");
                    }
                    if (userEx == null)
                    {
                        
                        /////////////////////////////////////////

                        try
                        {

                            using (var df = new MainDBEntities())
                            {
                                var sysUser = df.users.Create();

                                sysUser.userId = Guid.NewGuid();
                                sysUser.email = model.UserName;
                                sysUser.password = "1234567";
                                sysUser.userName = model.twitterName;
                                sysUser.firstName = model.firstName;
                                sysUser.lastName = model.lastName;

                                df.users.Add(sysUser);
                                df.SaveChanges();


                                //return RedirectToAction("List");
                                FormsAuthentication.SetAuthCookie(model.UserName, false);

                                return RedirectToAction("List");
                            }
                        }
                        catch
                        {
                            ModelState.AddModelError("UserName", "User name or twitter name already exists. Please enter a different user name or twitter name.");

                        }

                        /////////////////////////////////

                        // Insert name into the profile table
                       // db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                     //   db.SaveChanges();

                        //OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        //OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        //return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }



        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        #endregion



    }

}