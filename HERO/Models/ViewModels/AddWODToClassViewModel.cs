﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HERO.Models
{
    public class AddWODToClassViewModel
    {
        public int ClassId { get; set; }
        public string ClassType { get; set; }
        public string WODName { get; set; }
    }
}