using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prbd_1718_presences_g13
{
    partial class Course
    {
        public List<int> AllDays = new List<int>();



         public String JourSemaine
        {
            get
            {
                if (DayOfWeek == 1)
                    return "Lundi";
                if (DayOfWeek == 2)
                    return "Mardi";
                if (DayOfWeek == 3)
                    return "Mercredi";
                if (DayOfWeek == 4)
                    return "Jeudi";
                if (DayOfWeek == 5)
                    return "Vendredi";
                return "";
            }
        }

    }
}
