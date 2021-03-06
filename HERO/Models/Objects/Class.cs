﻿using HERO.Scheduler;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HERO.Models.Objects
{
    public class Class
    {
        public int Id { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public DateTime? Time { get; set; } // Used for Calendar
        [Required]
        [Display(Name = "Duration")]
        public float Duration { get; set; }
        [Required]
        [Display(Name = "Type of Class")]
        public string Type { get; set; }
        [Display(Name = "Max Attendance")]
        public int MaxAttendance { get; set; }
        public virtual IList<Athlete> Attendance { get; set; }
        public virtual IList<Performance> Performances { get; set; }
        public virtual IList<ClassReminders> AttachedReminders { get; set; }
        public virtual WOD WOD { get; set; }
        public virtual WeeklyClassSetup WeeklyClass { get; set; }
        public virtual SingleClassSetup SingleClass { get; set; }
    }
}