﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTrelloLearn.Web.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("UserProfileExtraData")]
    public class UserProfileExtraData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        public virtual string Key { get; set; }
        public virtual string Value { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual string Provider { get; set; }
    }
}