using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using Youtube.Models;
using Youtube.Models.Model_Extensions;
using Youtube.Models.View_Models;

namespace Youtube.Controllers
{
    public class CommentController : Controller
    {
        private treca_aplikacija_model db = new treca_aplikacija_model();

        // GET: Comment
        public ActionResult Index()
        {
            var comments = db.comments.Include(c => c.user).Include(c => c.video);
            return View(comments.ToList());
        }

        // GET: Comment/Details/5
        public ActionResult Details(byte? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            comment comment = db.comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Comment/Create
        //public ActionResult Create()
        //{
        //    ViewBag.comment_user_id = new SelectList(db.users, "users_id", "user_username");
        //    ViewBag.comment_video_id = new SelectList(db.videos, "video_id", "video_name");
        //    return View();
        //}

        //POST: Comment/Create
        //To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "comment_id,comment_content,comment_created,comment_user_id,comment_video_id")] comment comment)
        //{
        //    if (ModelState.IsValid)
        //    {

        //        String s = Request.Url.AbsolutePath;

        //        db.comments.Add(comment);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.comment_user_id = new SelectList(db.users, "users_id", "user_username", comment.comment_user_id);
        //    ViewBag.comment_video_id = new SelectList(db.videos, "video_id", "video_name", comment.comment_video_id);
        //    return View(comment);
        //}

        [HttpPost]
        public ActionResult Create(string Content, int videoId)
        {

            if(User.Identity.IsAuthenticated == false) {
                return Json(new { message = "redirect" }, JsonRequestBehavior.AllowGet);
            }

            treca_aplikacija_model db = new treca_aplikacija_model();
            comment comment = new comment();
            comment.comment_video_id = (byte)videoId;

            string LoggedInUserName = User.Identity.GetApplicationUserUsername();

            foreach (var x in db.users)
            {
                if (x.user_username.Equals(LoggedInUserName))
                {

                    comment.comment_user_id = x.users_id;
                }
            }

            comment.comment_created = DateTime.Now;
            comment.comment_content = Content;

            db.comments.Add(comment);
            db.SaveChanges();
            //?


            UserViewModel CommentOwner = ApplicationUtils.CreateUserViewModelDTO(db.users.Find(comment.comment_user_id));
            CommentViewModel Comment = ApplicationUtils.CreateCommentViewModelDTO(db.comments.Find(comment.comment_id));

            return Json(new { User = CommentOwner, Comment = Comment }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SubmitLikeDislike(int commentId, bool isLike)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { message = "redirect" });
            }

            using (treca_aplikacija_model db = new treca_aplikacija_model())
            {
                UserViewModel user = ApplicationUtils.FindUserByUsername(User.Identity.GetApplicationUserUsername());
                try
                {
                    ApplicationUtils.RemoveLikeDislike(false, commentId, user.users_id);
                }
                catch (Exception)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                comment_like_dislike newCld = new comment_like_dislike();
                newCld.users_id = (byte)user.users_id;
                newCld.comment_id = (byte)commentId;
                newCld.is_like = isLike == true ? true : false;
                db.comment_like_dislike.Add(newCld);
                db.SaveChanges();

            }
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RemoveLikeDislike(int commentId)
        {
            using (treca_aplikacija_model db = new treca_aplikacija_model())
            {
                UserViewModel user = ApplicationUtils.FindUserByUsername(User.Identity.GetApplicationUserUsername());
                try
                {
                    ApplicationUtils.RemoveLikeDislike(false, commentId, user.users_id);
                }
                catch (Exception)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }


        // GET: Comment/Edit/5
        public ActionResult Edit(byte? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            comment comment = db.comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.comment_user_id = new SelectList(db.users, "users_id", "user_username", comment.comment_user_id);
            ViewBag.comment_video_id = new SelectList(db.videos, "video_id", "video_name", comment.comment_video_id);
            return View(comment);
        }

        // POST: Comment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "comment_id,comment_content,comment_created,comment_user_id,comment_video_id")] comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.comment_user_id = new SelectList(db.users, "users_id", "user_username", comment.comment_user_id);
            ViewBag.comment_video_id = new SelectList(db.videos, "video_id", "video_name", comment.comment_video_id);
            return View(comment);
        }

        // GET: Comment/Delete/5
        public ActionResult Delete(byte? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            comment comment = db.comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(byte id)
        {
            comment comment = db.comments.Find(id);
            db.comments.Remove(comment);
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
