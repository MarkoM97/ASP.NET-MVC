using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Youtube.Models;
using Youtube.Models.View_Models;

namespace Youtube.Controllers
{
    public class UserController : Controller
    {
        private treca_aplikacija_model db = new treca_aplikacija_model();

        // GET: User
        public ActionResult Index()
        {
            return View(db.users.ToList());
        }

        // GET: User/Details/5
        //public ActionResult Details(byte? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    user user = db.users.Find(id);
        //    if (user == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(user);
        //}

        // GET : User/Details?Username=
        public ActionResult Details(string UserName)
        {
            Debug.WriteLine(UserName);
            //user user = db.users.Distinct(u => u.user_username.Equals(UserName));
            foreach (user u in db.users)
            {
                if (u.user_username.Equals(UserName))
                {
                    return View(u);
                }
            }
            return HttpNotFound();

        }



        //public ActionResult Login(string username, string password)
        //{
        //    Debug.WriteLine(username);
        //    Debug.WriteLine(password);
        //    foreach (var user in db.USERS)
        //    {
        //        if (user.USER_USERNAME.Equals(username) && user.USER_PASSWORD.Equals(password))
        //        {
        //            return RedirectToAction("Index", "VIDEOs");
        //        }
        //        else
        //        {
        //            return RedirectToAction("Index", "COMMENTs");
        //        }
        //    }
        //    return null;
        //}

        public ActionResult Ban(string userName)
        {
            if(!User.IsInRole("Admin"))
            {
                return RedirectToAction("Unauthorized", "Account");
            }

            using (treca_aplikacija_model db = new treca_aplikacija_model())
            {
                foreach (var x in db.users.ToList())
                {
                    if (x.user_username.Equals(userName))
                    {
                        return View(ApplicationUtils.CreateUserViewModelDTO(x));
                    }
                }
                return View();
            }
        }

        [HttpPost]
        public ActionResult BanConfirmed(string userName)
        {
            using (treca_aplikacija_model db = new treca_aplikacija_model())
            {
                foreach (var x in db.users.ToList())
                {
                    if (x.user_username.Equals(userName))
                    {
                        x.user_banned = true;
                        db.SaveChanges();
                        return RedirectToAction("Index", "Video");
                    }
                }
                return View();
            }
        }

        public ActionResult Unban(string userName)
        {
            using (treca_aplikacija_model db = new treca_aplikacija_model())
            {
                foreach (var x in db.users.ToList())
                {
                    if (x.user_username.Equals(userName))
                    {
                        x.user_banned = false;
                        db.SaveChanges();
                        return Json(new { message = "success" }, JsonRequestBehavior.AllowGet);
                    }
                }
                return null;
            }
        }



        //[Authorize(Roles = "Admin")]
        public ActionResult Administrator()
        {
            if(!User.IsInRole("Admin")){
                return RedirectToAction("Unauthorized", "Account");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Administrator(string query, bool isUser)
        {
            List<Object> retVal = new List<Object>();
            using (treca_aplikacija_model db = new treca_aplikacija_model())
            {
                if (isUser)
                {
                    foreach (var x in db.users.Where(x => x.user_username.Contains(query)).ToList())
                    {
                        retVal.Add(ApplicationUtils.CreateUserViewModelDTO(x));
                    }
                }
                else
                {
                    foreach (var x in db.videos.Where(x => x.video_name.Contains(query)).ToList())
                    {
                        retVal.Add(ApplicationUtils.CreateVideoViewModel(x));
                    }
                }

                return Json(new { returnValues = retVal }, JsonRequestBehavior.AllowGet);
            }
        }



        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "users_id,user_username,user_password,user_icon,user_created,user_banned")] user user)
        {
            if (ModelState.IsValid)
            {
                db.users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: User/Edit/5
        public ActionResult Edit(byte? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "users_id,user_username,user_password,user_icon,user_created,user_banned")] user user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: User/Delete/5
        public ActionResult Delete(byte? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(byte id)
        {
            user user = db.users.Find(id);
            db.users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
