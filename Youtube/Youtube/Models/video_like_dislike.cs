//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Youtube.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class video_like_dislike
    {
        public byte video_id { get; set; }
        public byte users_id { get; set; }
        public bool is_like { get; set; }
    
        public virtual user user { get; set; }
        public virtual video video { get; set; }
    }
}