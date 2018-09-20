using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Youtube.Models.View_Models
{
    public class CommentViewModel
    {
        public byte comment_id { get; set; }
        public string comment_content { get; set; }
        public DateTime comment_created { get; set; }
        public int comment_user_id { get; set; }
        public UserViewModel comment_user { get; set; }
        public int comment_video_id { get; set; }
        public int comment_likes { get; set; }
        public int comment_dislikes { get; set; }
        //public VideoViewModel comment_video { get; set; }
    }
    
    public partial class ApplicationUtils
    {
        public static CommentViewModel CreateCommentViewModelDTO(comment comment)
        {
            CommentViewModel cvm = new CommentViewModel();
            cvm.comment_id = comment.comment_id;
            cvm.comment_content = comment.comment_content;
            cvm.comment_created = comment.comment_created;
            cvm.comment_user_id = (int)comment.comment_user_id;
            cvm.comment_user = CreateUserViewModelDTO(comment.user);
            cvm.comment_video_id = (int)comment.comment_video_id;
            cvm.comment_likes = 0;
            cvm.comment_dislikes = 0;
            foreach(var x in comment.comment_like_dislike)
            {
                if(x.is_like)
                {
                    cvm.comment_likes++;
                } else
                {
                    cvm.comment_dislikes++;
                }
            }
            return cvm;
        }
    }
}