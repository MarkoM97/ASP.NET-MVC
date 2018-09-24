using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Youtube.Models.View_Models
{
    public class VideoViewModel
    {
        public byte video_id { get; set; }
        [Display(Name = "Name")]
        public string video_name { get; set; }
        [Display(Name = "Description")]
        public string video_description { get; set; }
        [Display(Name = "Icon")]
        public string video_icon { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
        [Display(Name = "Url")]
        public string video_url { get; set; }
        public int video_user_id { get; set; }
        public UserViewModel video_user { get; set; }
        public DateTime video_created { get; set; }
        [Display(Name = "Created")]
        public string video_created_string { get; set; }
        public byte video_views { get; set; }
        public int video_likes { get; set; }
        public int video_dislikes { get; set; }
        [Display(Name = "Comments allowed?")]
        public bool video_comments_allowed { get; set; }
        public List<CommentViewModel> video_comments { get; set; }
        [Display(Name = "Banned")]
        public bool video_banned;

    }

    public partial class ApplicationUtils
    {

        public static VideoViewModel CreateVideoViewModel(video video)
        {
            VideoViewModel vvm = new VideoViewModel();
            vvm.video_id = video.video_id;
            vvm.video_name = video.video_name;
            vvm.video_description = video.video_description;
            vvm.video_icon = video.video_icon;
            vvm.video_url = video.video_url;
            vvm.video_user = CreateUserViewModelDTO(video.user);
            vvm.video_user_id = (int)video.video_user_id;
            vvm.video_created = video.video_created;
            vvm.video_created_string = String.Format("{0:ddd MMM dd yyyy}", vvm.video_created);
            vvm.video_views = video.video_views;
            vvm.video_likes = 0;
            vvm.video_dislikes = 0;
            vvm.video_banned = (bool)video.video_banned;
            foreach(var ld in video.video_like_dislike)
            {
                if(ld.is_like)
                {
                    vvm.video_likes++;
                } else
                {
                    vvm.video_dislikes++;
                }
            }
            vvm.video_comments_allowed = video.video_comments_allowed;

            vvm.video_comments = new List<CommentViewModel>();
            foreach(var c in video.comments)
            {
                vvm.video_comments.Add(CreateCommentViewModelDTO(c));
            }

            return vvm;
            
        }

        public static List<VideoViewModel> CreateVideoViewModels(List<video> videos)
        {
            List<VideoViewModel> videoViewModels = new List<VideoViewModel>();
            foreach(var x in videos)
            {
                videoViewModels.Add(CreateVideoViewModel(x));
            }
            return videoViewModels;
        }

        
        
    }
    
}