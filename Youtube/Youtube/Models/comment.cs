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
    
    public partial class comment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public comment()
        {
            this.comment_like_dislike = new HashSet<comment_like_dislike>();
        }
    
        public byte comment_id { get; set; }
        public string comment_content { get; set; }
        public System.DateTime comment_created { get; set; }
        public Nullable<byte> comment_user_id { get; set; }
        public Nullable<byte> comment_video_id { get; set; }


        public int comment_rating { get; set; }

        public virtual user user { get; set; }
        public virtual video video { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<comment_like_dislike> comment_like_dislike { get; set; }
    }
}
