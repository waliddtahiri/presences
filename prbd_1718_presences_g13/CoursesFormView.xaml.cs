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

        private DataView location;
        public DataView Location
        {
            get { return location; }
            set
            {
                location = value;
                RaisePropertyChanged(nameof(Location));
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

            

            Course = course;

            Courses = new ObservableCollection<Course>(App.Model.course);
            Users = new ObservableCollection<User>(App.Model.user);
            AllStudents = new ObservableCollection<Student>(App.Model.student);
            Students = new ObservableCollection<Student>(Course.Student);
            CoursesOccurrence = new ObservableCollection<CourseOccurrence>(Course.CourseOccurrence);
            Presences = new ObservableCollection<Presence>(App.Model.presence);
            NonStudents = new ObservableCollection<Student>(AllStudents.Except(Students));

            InitializeComponent();

            var table = new DataTable();
            var columns = new Dictionary<int, int>();
            table = new DataTable();
            table.Columns.Add("Etudiant");
            int i = 1;
            foreach (var p in CoursesOccurrence)
            {
                table.Columns.Add(p.Date.ToShortDateString());
                columns[i] = p.Id;
                ++i;
            }
            foreach (var s in Students)
            {
                var row = table.NewRow();
                row[0] = s.LastName + ", " + s.FirstName;
                for (int j = 1; j < table.Columns.Count; ++j)
                {
                    int idL = columns[j];
                    var ids = from pr in s.Presence select pr.CourseOccurence;
                    string txt = ids.Contains(idL) ? "P" : "A";
                    row[j] = txt;
                }
                table.Rows.Add(row);
            }
            Location = table.DefaultView;

            

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
                RaisePropertyChanged(nameof(Title));
            }
        }

        public int DayOfWeek
        {
            get { return Course.DayOfWeek; }
            set
            {
                Course.DayOfWeek = value;
                RaisePropertyChanged(nameof(Title));
            }
        }

        public User Teacher
        {
            get { return Course.User; }
            set
            {
                Course.User = value;
                RaisePropertyChanged(nameof(Teacher));
            }
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
