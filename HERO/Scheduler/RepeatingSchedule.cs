﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HERO.Scheduler
{
    public abstract class RepeatingSchedule : Schedule
    {
        public Period SchedulingRange { get; set; }
        protected bool DateIsInPeriod(DateTime date)
        {
            return date >= SchedulingRange.Start && date <= SchedulingRange.End;
        }
    }
}
