using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Youtube;
using Youtube.Models;
using Youtube.Models.Model_Extensions;
using Youtube.Models.View_Models;

namespace Youtube.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        public ActionResult Unauthorized()
        { return View(); }
        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            string s = Request.QueryString["email"];
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, false, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:

                    //if (User.Identity.IsAuthenticated)
                    //{
                    //    var user = User.Identity;
                    //    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                    //    //var s = userManager.GetRoles(user.GetUserId());
                    //    userManager.AddToRole(user.GetUserId(), "Admin");
                    //    //if (s[0].ToString().Equals("Admin"))
                    //    //{
                    //    //    Debug.WriteLine("Its admin");
                    //    //}
                    //}


                    return RedirectToAction("Index", "Video");
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //[Authorize(Roles="Admin")]
        [HttpGet]
        public ActionResult Administrator()
        {
            return View();
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                //Model 3
                treca_aplikacija_model db = new treca_aplikacija_model();
                var newUser = new user();
                newUser.user_email = model.Email;
                newUser.user_username = model.Username;
                newUser.user_password = model.Password;
                newUser.user_created = DateTime.Now;
                db.users.Add(newUser);
                db.SaveChanges();


                //Model 3 auth
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, ApplicationUserUsername = model.Username };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {

                    var user1 = User.Identity;
                    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                    userManager.AddToRole(user.Id, "Regular");

                    //userManager.AddToRole(user1.GetUserId(), "Admin");


                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    return RedirectToAction("Index", "Video");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        public ActionResult Edit(string username)
        {
            using (treca_aplikacija_model db = new treca_aplikacija_model())
            {
                //string loggedInUser = User.Identity.GetApplicationUserUsername();
                foreach (var x in db.users.ToList())
                {
                    if (x.user_username.Equals(username))
                    {
                        EditAccoutViewModel eavm = new EditAccoutViewModel();
                        var user = UserManager.FindByEmail(x.user_email);
                        eavm.Id = user.Id;
                        eavm.Username = x.user_username;
                        eavm.Password = x.user_password;
                        eavm.ConfirmPassword = x.user_password;
                        eavm.OldPassword = x.user_password;
                        eavm.Email = x.user_email;

                        return View(eavm);
                    }
                }
            }
            return View();
        }




        [HttpPost]
        public ActionResult Edit(EditAccoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //ApplicationUser
            var user = UserManager.FindById(model.Id);
            Debug.WriteLine("Korisnik" + user.ToString());
            //for database user
            var oldUsername = user.ApplicationUserUsername;
            Debug.WriteLine("Staro korisnicko : " + oldUsername);

            //Database user
            using (treca_aplikacija_model db = new treca_aplikacija_model())
            {
                foreach (var x in db.users.ToList())
                {
                    if (x.user_username.Equals(oldUsername))
                    {
                        x.user_username = model.Username;
                        x.user_email = model.Email;
                        x.user_password = model.Password;

                        //Image sex
                        if(model.ImageFile != null)
                        {
                            string Extension = Path.GetExtension(model.ImageFile.FileName);
                            string FileName = x.users_id.ToString() + Extension;

                            string rootPath = Server.MapPath("~");
                            if (x.user_icon != null) System.IO.File.Delete(rootPath + "/Content/UserPhotos/" + x.user_icon);

                            //Assigning video icon
                            x.user_icon = FileName;
                            //Saving video thumbnail
                            FileName = Path.Combine(Server.MapPath("~/Content/UserPhotos/"), FileName);
                            model.ImageFile.SaveAs(FileName);
                        }
                    }
                }
                db.SaveChanges();
            }



            //Applicationuser
            user.ApplicationUserUsername = model.Username;
            UserManager.SetEmail(model.Id, model.Email);
            user.UserName = model.Email;
            UserManager.ChangePassword(model.Id, model.OldPassword, model.Password);
            if (model.Role != null)
            {
                if (model.Role.Equals("Admin"))
                {
                    UserManager.AddToRole(model.Id, "Admin");
                }
                else
                {
                    UserManager.RemoveFromRole(model.Id, "Admin");
                }
            }
            else
            {
                UserManager.AddToRole(model.Id, "Regular");
            }


            UserManager.Update(user);


            if (User.Identity.GetUserId().Equals(model.Id))
            {
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Login", "Account", new { returnUrl = "/Video/Index" });
            }
            else
            {
                return View();
            }



        }


        [HttpGet]
        public ActionResult Delete(string username)
        {
            if(!User.IsInRole("Admin"))
            {
                return RedirectToAction("Unauthorized", "Account");
            }

            using (treca_aplikacija_model db = new treca_aplikacija_model())
            {
                foreach (var x in db.users.ToList())
                {

                    if (x.user_username.Equals(username))
                    {
                        return View(ApplicationUtils.CreateUserViewModelDTO(x));
                    }
                }
            }
            return null;
        }


        [HttpPost]
        public ActionResult DeleteConfirmed(string username)
        {
            //Delete applicationUser

            //Delete databaserUser
            Debug.WriteLine(username);
            int userId = ApplicationUtils.FindUserByUsername(username).users_id;
            string loggedInUserUsername = User.Identity.GetApplicationUserUsername();
            if (loggedInUserUsername.Equals(username))
            {
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                UserManager.Delete(UserManager.FindByEmail((ApplicationUtils.FindUserByUsername(username)).user_email));
            }



            using (treca_aplikacija_model db = new treca_aplikacija_model())
            {
                db.Database.ExecuteSqlCommand("DELETE FROM comment_like_dislike WHERE users_id = {0}", userId);
                db.Database.ExecuteSqlCommand("DELETE FROM video_like_dislike WHERE users_id = {0}", userId);
                db.Database.ExecuteSqlCommand("DELETE FROM users_role WHERE users_id = {0}", userId);
                db.Database.ExecuteSqlCommand("DELETE FROM users_following WHERE user_following = {0} or user_followed = {1}", userId, userId);
                db.Database.ExecuteSqlCommand("DELETE FROM comment WHERE comment_user_id = {0}", userId);
                db.Database.ExecuteSqlCommand("DELETE FROM comment WHERE comment_video_id IN (SELECT video_id FROM video WHERE video_user_id = {0})", userId);
                //db.Database.ExecuteSqlCommand("DELETE FROM comment WHERE video_id = SELEC", userId);
                db.Database.ExecuteSqlCommand("DELETE FROM video WHERE video_user_id = {0}", userId);
                db.Database.ExecuteSqlCommand("DELETE FROM users WHERE users_id = {0}", userId);
            }
            return RedirectToAction("Index", "Video");

        }


        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Video");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}