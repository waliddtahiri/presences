using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prbd_1718_presences_g13
{
    partial class Course
    {
        public List<String> Day
        {
            get
            {
                List<String> Days = new List<String>();
                Days.Add("Lundi");
                Days.Add("Mardi");
                Days.Add("Mercredi");
                Days.Add("Jeudi");
                Days.Add("Vendredi");
                return Days;
            }
        }

        public List<String> Days
        {
            get
            {
                return Day;
            }
        }

        public double Percentage
        {
             get { return CourseOccurrence.
                    Where(co => co.Presence.Where(p=>p.Present==1 || p.Present==0).Count() == Student.Count()).Count() / (double)CourseOccurrence.Count() * 100; }
             set { }
        }

        public string Percent
        {
            get
            {
                return Percentage.ToString("0.00") + " %";
            }
        }

        public String JourSemaine
        {
            get
            {
                if (DayOfWeek == 0)
                    return "Lundi";
                if (DayOfWeek == 1)
                    return "Mardi";
                if (DayOfWeek == 2)
                    return "Mercredi";
                if (DayOfWeek == 3)
                    return "Jeudi";
                if (DayOfWeek == 4)
                    return "Vendredi";
                return "";
            }
        }

    }
}
