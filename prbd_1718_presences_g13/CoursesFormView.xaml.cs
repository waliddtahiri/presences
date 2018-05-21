using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace prbd_1718_presences_g13
{
    /// <summary>
    /// Logique d'interaction pour CoursesFormView.xaml
    /// </summary>
    public partial class CoursesFormView : UserControlBase
    {

        public Course Course { get; set; }
        public ObservableCollection<Course> Courses { get; private set; }
        public ObservableCollection<Presence> Presences { get; private set; }
        public ObservableCollection<CourseOccurrence> CoursesOccurrence { get; private set; }
        public ObservableCollection<User> Users { get; private set; }
        public ObservableCollection<Student> AllStudents { get; private set; }
        public ObservableCollection<Student> Students { get; private set; }
        public ObservableCollection<Student> NonStudents { get; set; }

        String[] Jours = { "Lundi", "Mardi", "Mercredi", "Jeudi", "Vendredi", "Samedi", "Dimanche" };

        public List<String> Days { get; private set; }

        private DataView presence;
        public DataView Presence
        {
            get { return presence; }
            set
            {
                presence = value;
                RaisePropertyChanged(nameof(presence));
            }
        }

        private bool isNew;
        public bool IsNew
        {
            get { return isNew; }
            set
            {
                isNew = value;
                RaisePropertyChanged(nameof(IsNew));
            }
        }

        public CoursesFormView(Course course, bool isNew)
        {
            DataContext = this;

            IsNew = isNew;

            Course = course;

            Courses = new ObservableCollection<Course>(App.Model.course);
            Users = new ObservableCollection<User>(App.Model.user);
            AllStudents = new ObservableCollection<Student>(App.Model.student);
            Students = new ObservableCollection<Student>(Course.Student);
            CoursesOccurrence = new ObservableCollection<CourseOccurrence>(Course.CourseOccurrence);
            Presences = new ObservableCollection<Presence>(App.Model.presence);
            NonStudents = new ObservableCollection<Student>(AllStudents.Except(Students));

            

            InitializeComponent();

            Professor();

            var table = new DataTable();
            var columns = new Dictionary<int, int>();
            table = new DataTable();
            table.Columns.Add("Etudiant");
            int i = 1;
            foreach (CourseOccurrence p in CoursesOccurrence)
            {
                table.Columns.Add(p.Date.ToShortDateString());
                columns[i] = p.Id;
                ++i;
            }
            foreach (Student s in Students)
            {
                var row = table.NewRow();
                row[0] = s.LastName + ", " + s.FirstName; 
                for (int j = 1; j < table.Columns.Count; ++j)
                {
                    int idL = columns[j];
                    var idp = from p in s.Presence where s.Id == p.Student && p.Present == 1 select p.CourseOccurrence.Id;
                    var ida = from a in s.Presence where s.Id == a.Student && a.Present == 0 select a.CourseOccurrence.Id;
                    string txt = "";
                    if (idp.Contains(idL))
                        txt = "P";
                    else if (ida.Contains(idL))
                        txt = "A";
                    else
                        txt = "?";
                    row[j] = txt;
                }
                table.Rows.Add(row);
            }
            Presence = table.DefaultView;
        }

        private void Professor()
        {
            if(App.CurrentUser.Role == "teacher")
            {
                code.IsEnabled = false;
                titre.IsEnabled = false;
                prof.IsEnabled = false;
                dayofweek.IsEnabled = false;
                Sdate.IsEnabled = false;
                Fdate.IsEnabled = false;
                Stime.IsEnabled = false;
                Etime.IsEnabled = false;
            }
        }

        public int Code
        {
            get { return Course.Code; }
            set
            {
                Course.Code = value;
                RaisePropertyChanged(nameof(Code));
            }
        }

        public String Title
        {
            get { return Course.Title; }
            set
            {
                Course.Title = value;
                Valid();
                RaisePropertyChanged(nameof(Title));
            }
        }

        public int DayOfWeek
        {
            get { return Course.DayOfWeek; }
            set
            {
                Course.DayOfWeek = value;
                RaisePropertyChanged(nameof(DayOfWeek));
            }
        }


        public User Teacher
        {
            get { return Course.User; }
            set
            {
                Course.User = value;
                Valid();
                RaisePropertyChanged(nameof(Teacher));
            }
        }

        public DateTime StartDate
        {
            get { return Course.StartDate; }
            set
            {
                Course.StartDate = value;
                RaisePropertyChanged(nameof(StartDate));
                Valid();
            }
        }

        public DateTime FinishDate
        {
            get { return Course.FinishDate; }
            set
            {
                Course.FinishDate = value;
                RaisePropertyChanged(nameof(FinishDate));
                Valid();
            }
        }

        public TimeSpan StartTime
        {
            get { return Course.StartTime; }
            set
            {
                Course.StartTime = value;
                RaisePropertyChanged(nameof(StartTime));
                Valid();
            }
        }

        public TimeSpan EndTime
        {
            get { return Course.EndTime; }
            set
            {
                Course.EndTime = value;
                RaisePropertyChanged(nameof(EndTime));
                Valid();
            }
        }

        private bool Valid()
        {
            if (isNew)
            {
                ClearErrors();

                if (string.IsNullOrEmpty(Title))
                {
                    AddError("Title", Properties.Resources.Error_RequiredTitle);
                }
                if (Teacher == null)
                {
                        AddError("Teacher", Properties.Resources.Error_Required); 
                }
                if (StartDate.CompareTo(FinishDate)>0)
                {
                        AddError("StartDate", Properties.Resources.Error_StartDate);
                        AddError("FinishDate", Properties.Resources.Error_FinishDate);
                }
                if (StartTime.CompareTo(EndTime) > 0)
                {
                    AddError("StartTime", Properties.Resources.Error_StartTime);
                    AddError("EndTime", Properties.Resources.Error_EndTime);
                }

            }
            
            RaiseErrors();

            return true;
        }

        private Course selectedCourse;
        public Course SelectedCourse
        {

            get { return selectedCourse; }
            set
            {
                selectedCourse = value;
            }
        }

    }
}
