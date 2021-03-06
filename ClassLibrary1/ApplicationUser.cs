﻿using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace InstaBot.Common
{
    public class ApplicationUser : IdentityUser
    {
        [Column("FirstName")]
        public string FirstName
        {
            get;
            set;
        }

        [Column("LastName")]
        public string LastName
        {
            get;
            set;
        }
    }
}
