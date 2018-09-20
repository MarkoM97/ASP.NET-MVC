using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Youtube.Models;
using Youtube.Models.Model_Extensions;
using Youtube.Models.View_Models;

namespace Youtube.Controllers
{
    public class VideoController : Controller
    {
        //private treca_aplikacija_model db = new treca_aplikacija_model();

        // GET: Video
        public ActionResult Index(string SearchParam)
        {
            using (treca_aplikacija_model db = new treca_aplikacija_model())
            {
                if (SearchParam == null)
                {
                    var videos = db.videos.Include(v => v.user).Include(v => v.comments);
                    return View(ApplicationUtils.CreateVideoViewModels(videos.ToList()));
                }
                else
                {
                    var videos = db.videos.Include(v => v.user).Include(v => v.comments).Where(x => x.video_name.Contains(SearchParam));
                    return View(ApplicationUtils.CreateVideoViewModels(videos.ToList()));
                }
            }
        }

        [HttpPost]
        public ActionResult Filter(string userFilter, string lowDateFilter, string highDateFilter)
        {
            Debug.WriteLine("UserF: " + userFilter);
            Debug.WriteLine("LowDF: " + lowDateFilter);
            Debug.WriteLine("HighF: " + highDateFilter);
            treca_aplikacija_model db = new treca_aplikacija_model();

            List<VideoViewModel> videosDTO = new List<VideoViewModel>();
            if (userFilter == null && lowDateFilter == null & highDateFilter == null)
            {
                return Json(new { message = "Everyting is null" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<VideoViewModel> videos = new List<VideoViewModel>();

                if (userFilter != null)
                {
                    foreach (var x in db.videos.Include(v => v.user).Where(x => x.user.user_username.Equals(userFilter)).ToList())
                    {
                        videos.Add(ApplicationUtils.CreateVideoViewModel(x));
                    }
                    //videos += db.videos.Include(v => v.user).Include(v => v.comments).Where(x => x.user.user_username.Contains(userFilter)).ToList();

                }
                if (lowDateFilter != null && highDateFilter != null)
                {
                    foreach (var x in db.videos.Include(v => v.user).ToList())
                    {
                        if (x.video_created > DateTime.Parse(lowDateFilter) && x.video_created < DateTime.Parse(highDateFilter))
                        {
                            videos.Add(ApplicationUtils.CreateVideoViewModel(x));
                        }

                    }
                    //videos += db.videos.Include(v => v.user).Include(v => v.comments).Where(x => x.video_created > Convert.ToDateTime(lowDateFilter) && x.video_created < Convert.ToDateTime(highDateFilter) ).ToList();
                }
                else
                {
                    if (lowDateFilter != null)
                    {
                        foreach (var x in db.videos.Include(v => v.user).ToList())
                        {
                            if (x.video_created > DateTime.Parse(lowDateFilter))
                            {
                                videos.Add(ApplicationUtils.CreateVideoViewModel(x));
                            }

                        }
                        //List<video> videos = db.videos.Include(v => v.user).Include(v => v.comments).Where(x => x.video_created > Convert.ToDateTime(lowDateFilter)).ToList();
                    }
                    else if (highDateFilter != null)
                    {
                        foreach (var x in db.videos.Include(v => v.user).ToList())
                        {
                            if (x.video_created < DateTime.Parse(highDateFilter))
                            {
                                videos.Add(ApplicationUtils.CreateVideoViewModel(x));
                            }

                        }
                        //List<video> videos = db.videos.Include(v => v.user).Include(v => v.comments).Where(x => x.video_created > Convert.ToDateTime(lowDateFilter)).ToList();
                    }
                }
                return Json(new { Videos = videos.Distinct().ToList() }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Video/Details/5
        public ActionResult Details(byte? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            treca_aplikacija_model db = new treca_aplikacija_model();
            video video = db.videos.Find(id);
            UserViewModel user = ApplicationUtils.FindUserByUsername(User.Identity.GetApplicationUserUsername());
            if(user != null)
            {
                foreach (var x in db.video_like_dislike)
                {
                    if (x.users_id == user.users_id && x.video_id == video.video_id)
                    {
                        ViewBag.LikeDislikeEntity = x;
                    }
                }
                ViewBag.UserCommentLikeDislike = db.comment_like_dislike.Where(x => x.users_id == user.users_id && x.comment.comment_video_id == id).ToList();
            }



            if (video == null)
            {
                return HttpNotFound();
            }


            return View(ApplicationUtils.CreateVideoViewModel(video));
        }

        // GET: Video/Create
        [Authorize]
        public ActionResult Create()
        {
            treca_aplikacija_model db = new treca_aplikacija_model();
            ViewBag.video_user_id = new SelectList(db.users, "users_id", "user_username");
            return View();
        }

        //ZA HML POLJE
        [Authorize]
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(video videoModel)
        {

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            using (treca_aplikacija_model db = new treca_aplikacija_model())
            {


                int NewVideoId = db.videos.Count() > 0 ? db.videos.ToList().Last().video_id + 1 : 0;

                string Extension = Path.GetExtension(videoModel.ImageFile.FileName);
                string FileName = NewVideoId.ToString() + Extension;

                //Spliting embed tag
                string[] UrlStrings = videoModel.video_url.Split(' ');
                foreach (string s in UrlStrings)
                {
                    Debug.WriteLine("Url string " + s);
                    if (s.Contains("src"))
                    {

                        videoModel.video_url = s.Substring(5, s.Length - 6);
                    }
                }

                //Assigning video icon
                videoModel.video_icon = FileName;

                //Saving video thumbnail
                FileName = Path.Combine(Server.MapPath("~/Content/VideoImages/"), FileName);
                videoModel.ImageFile.SaveAs(FileName);

                //Asigning user
                string LoggedInUserName = User.Identity.GetApplicationUserUsername();
                foreach (var x in db.users)
                {
                    if (x.user_username.Equals(LoggedInUserName))
                    {

                        videoModel.video_user_id = x.users_id;
                    }
                }

                //Assigning 'created'
                videoModel.video_created = DateTime.Now;



                db.videos.Add(videoModel);
                db.SaveChanges();
            }

            ModelState.Clear();
            return RedirectToAction("Index", "Video");

        }

        [HttpPost]
        public ActionResult RemoveLikeDislike(int videoId)
        {
            using (treca_aplikacija_model db = new treca_aplikacija_model())
            {
                UserViewModel user = ApplicationUtils.FindUserByUsername(User.Identity.GetApplicationUserUsername());
                try
                {
                    ApplicationUtils.RemoveLikeDislike(true, videoId, user.users_id);
                }
                catch (Exception)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SubmitLikeDislike(int videoId, bool isLike)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { message = "redirect" }, JsonRequestBehavior.AllowGet);
            }


            using (treca_aplikacija_model db = new treca_aplikacija_model())
            {
                UserViewModel user = ApplicationUtils.FindUserByUsername(User.Identity.GetApplicationUserUsername());
                try
                {
                    ApplicationUtils.RemoveLikeDislike(true, videoId, user.users_id);
                }
                catch (Exception)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                video_like_dislike newVld = new video_like_dislike();
                newVld.users_id = (byte)user.users_id;
                newVld.video_id = (byte)videoId;
                newVld.is_like = isLike == true ? true : false;
                db.video_like_dislike.Add(newVld);
                db.SaveChanges();

            }
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }


        //GET: Video/Edit/5
        [Authorize]
        public ActionResult Edit(byte? id)
        {
            using (treca_aplikacija_model db = new treca_aplikacija_model())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                video video = db.videos.Find(id);
                if (video == null)
                {
                    return HttpNotFound();
                }
                ViewBag.video_user_id = new SelectList(db.users, "users_id", "user_username", video.video_user_id);
                return View(video);
            }

        }

        //POST: Video/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(video videoViewModel)
        {
            using (treca_aplikacija_model db = new treca_aplikacija_model())
            {


                video oldVideo = db.videos.Find(videoViewModel.video_id);
                if (videoViewModel.video_url != null)
                {
                    //Spliting embed tag
                    string[] UrlStrings = videoViewModel.video_url.Split(' ');
                    foreach (string s in UrlStrings)
                    {
                        Debug.WriteLine("Url string " + s);
                        if (s.Contains("src"))
                        {

                            oldVideo.video_url = s.Substring(5, s.Length - 6);
                        }
                    }
                }


                if (videoViewModel.ImageFile != null)
                {

                    string Extension = Path.GetExtension(videoViewModel.ImageFile.FileName);
                    string FileName = videoViewModel.video_id.ToString() + Extension;

                    string rootPath = Server.MapPath("~");
                    Debug.WriteLine("Root path" + rootPath);
                    System.IO.File.Delete(rootPath + "/Content/VideoImages/" + oldVideo.video_icon);


                    //Assigning video icon
                    oldVideo.video_icon = FileName;
                    //Saving video thumbnail
                    FileName = Path.Combine(Server.MapPath("~/Content/VideoImages/"), FileName);
                    videoViewModel.ImageFile.SaveAs(FileName);
                }
                if(videoViewModel.video_name != null)
                {
                    oldVideo.video_name = videoViewModel.video_name;
                }

                if (videoViewModel.video_description != null)
                {
                    oldVideo.video_description = videoViewModel.video_description;
                }

                if (videoViewModel.video_comments_allowed)
                {
                    oldVideo.video_comments_allowed = true;
                }
                else
                {
                    oldVideo.video_comments_allowed = false;
                }


                if (ModelState.IsValid)
                {
                    db.Entry(oldVideo).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.video_user_id = new SelectList(db.users, "users_id", "user_username", videoViewModel.video_user_id);
                return View(videoViewModel);
            }
        }


        //[Authorize(Roles = "Admin")]
        public ActionResult Ban(int id)
        {
            if(User.IsInRole("Regular"))
            {
                return RedirectToAction("Unauthorized", "Account");
            }

            using (treca_aplikacija_model db = new treca_aplikacija_model())
            {
                return View(ApplicationUtils.CreateVideoViewModel(db.videos.Find(id)));
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult BanConfirmed(int id)
        {
            using (treca_aplikacija_model db = new treca_aplikacija_model())
            {
                video v = db.videos.Find(id);

                v.video_banned = true;
                db.SaveChanges();
                return RedirectToAction("Index", "Video");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Unban(int id)
        {
            using (treca_aplikacija_model db = new treca_aplikacija_model())
            {
                video v = db.videos.Find(id);
                v.video_banned = false;
                db.SaveChanges();
                return Json(new { message = "success" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddView(int id)
        {
            using (treca_aplikacija_model db = new treca_aplikacija_model())
            {
                video v = db.videos.Find(id);
                v.video_views = (byte)((int)v.video_views + 1);
                db.SaveChanges();
                return Json(new { message = "success" }, JsonRequestBehavior.AllowGet);
            }
        }


        // GET: Video/Delete/5
        [Authorize]
        public ActionResult Delete(byte? id)
        {
            using (treca_aplikacija_model db = new treca_aplikacija_model())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                video video = db.videos.Find(id);
                if (video == null)
                {
                    return HttpNotFound();
                }
                return View(ApplicationUtils.CreateVideoViewModel(video));
            }

        }

        // POST: Video/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(byte id)
        {
            using (treca_aplikacija_model db = new treca_aplikacija_model())
            {
                video video = db.videos.Find(id);
                List<video_like_dislike> vld = db.video_like_dislike.Where(x => x.video_id == id).ToList();
                foreach (var x in vld)
                {
                    db.video_like_dislike.Remove(x);
                    db.SaveChanges();
                }

                List<comment> vc = db.comments.Where(x => x.comment_video_id == id).ToList();
                foreach (var x in vc)
                {
                    db.comments.Remove(x);
                    db.SaveChanges();
                }


                db.videos.Remove(video);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

        }

        protected override void Dispose(bool disposing)
        {
            using (treca_aplikacija_model db = new treca_aplikacija_model())
            {
                if (disposing)
                {
                    db.Dispose();
                }
                base.Dispose(disposing);
            }

        }
    }
}
