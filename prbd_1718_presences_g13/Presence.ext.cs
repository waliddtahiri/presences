using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prbd_1718_presences_g13
{
    partial class Presence
    {
        
        public Presence()
        {

        }

        public Presence(int idS, int idC, short p)
        {
            Student = idS;
            CourseOccurence = idC;
            Present = p;
        }

        public Presence(int idS, int idC)
        {
            Student = idS;
            CourseOccurence = idC;
            Present = 2;
        }

        public bool IsPresent
        {
            get { return Present == 1; }
            set { Present = (byte)(value ? 1 : 0); }
        }

        public bool IsAbsent
        {
            get { return Present == 0; }
            set { Present = (byte)(value ? 0 : 1); }
        }

        public Boolean Equals(Presence P)
        {
            return this.Student==P.Student && this.CourseOccurence == P.CourseOccurence;  
            
        }
    }
}
