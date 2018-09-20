using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Youtube.Models.View_Models
{
    public class UserViewModel
    {
        public int users_id { get; set; }
        [Display(Name = "Email")]
        public string user_email { get; set; }
        [Display(Name = "Username")]
        public string user_username { get; set; }
        [Display(Name = "Password")]
        public string user_password { get; set; }
        [Display(Name = "Icon")]
        public string user_icon { get; set; }

        public enum roles {Admin, Regular}
        [Display(Name = "Role")]
        public roles user_role { get; set; }

        public DateTime user_created { get; set; }
        public bool user_banned { get; set; }

    }

    public class RoleViewModel
    {

    }

    public partial class ApplicationUtils
    {
        public static UserViewModel CreateUserViewModelDTO(user user)
        {
            UserViewModel uvm = new UserViewModel();
            uvm.users_id = user.users_id;
            uvm.user_icon = user.user_icon;
            uvm.user_created = user.user_created;
            uvm.user_email = user.user_email;
            uvm.user_password = user.user_password;
            uvm.user_username = user.user_username;
            return uvm;
        }

        public static UserViewModel FindUserByUsername(string username)
        {
            using (treca_aplikacija_model db = new treca_aplikacija_model())
            {
                foreach(var user in db.users)
                {
                    if(user.user_username.Equals(username))
                    {
                        return CreateUserViewModelDTO(user);
                    }
                }
            }
            return null;
        }

        public static void RemoveLikeDislike(bool isVideo, int entityId, int userId)
        {

            using (treca_aplikacija_model db = new treca_aplikacija_model())
            {
                if (isVideo)
                {
                    foreach(var vid in db.video_like_dislike.ToList())
                    {
                        if(vid.video_id == entityId && vid.users_id == userId)
                        {
                            db.video_like_dislike.Remove(vid);
                            db.SaveChanges();
                        } 
                    }
                } else
                {
                    foreach(var com in db.comment_like_dislike.ToList())
                    {
                        if(com.comment_id == entityId && com.users_id == userId)
                        {
                            db.comment_like_dislike.Remove(com);
                            db.SaveChanges();
                        }
                    }
                }
            }

               
        }

    }
}