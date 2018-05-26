using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prbd_1718_presences_g13
{
    partial class Course
    {
        public List<String> Days = new List<String>();
        public List<String> Day
        {
            get
            {
                Days.Add("Lundi");
                Days.Add("Mardi");
                Days.Add("Mercredi");
                Days.Add("Jeudi");
                Days.Add("Vendredi");
                return Days;
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
