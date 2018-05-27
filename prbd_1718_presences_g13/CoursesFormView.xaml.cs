using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
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
        public ObservableCollection<String> Day { get; set; }


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

        public ICommand DisplayEncodage { get; set; }
        public ICommand DesToIns { get; set; }
        public ICommand InsToDes { get; set; }
        public ICommand Delete { get; set; }

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
            Day = new ObservableCollection<String>(Course.Day);

            App.Messenger.Register<Course>(App.MSG_CANCEL, Course => { CancelChanges(); });

            App.Messenger.Register<Course>(App.MSG_SAVE, Course => { SaveChanges(); });

            DisplayEncodage = new RelayCommand<CourseOccurrence>(c => { App.Messenger.NotifyColleagues(App.MSG_DISPLAY_ENCODAGE, c); });

            DesToIns = new RelayCommand(() => { if (NonStudents.Count != 0) {
                    Course.Student.Add(NonStudents.First());
                    NonStudents.Remove(NonStudents.First());
                    Students.RefreshFromModel(Course.Student);
                    App.Messenger.NotifyColleagues(App.MSG_COURSE_CHANGED, Course);
                }
            });


            InsToDes = new RelayCommand(() => { if (Students.Count != 0) {
                    Course.Student.Remove(Course.Student.First());
                    Students.Remove(Students.First()); }
                    Students.RefreshFromModel(Course.Student);
                    NonStudents.RefreshFromModel(AllStudents.Except(Students));
                    App.Messenger.NotifyColleagues(App.MSG_COURSE_CHANGED, Course);
            });

            Delete = new RelayCommand(DeleteAction, () => { return !IsNew; });

            InitializeComponent();

            Students.RefreshFromModel(Course.Student);

            HistoriquePrésences();
        }


        private void HistoriquePrésences()
        {
            var table = new DataTable();
            var columns = new Dictionary<int, CourseOccurrence>();
            table = new DataTable();
            table.Columns.Add("Etudiant");
            int i = 1;
            foreach (CourseOccurrence p in CoursesOccurrence)
            {
                table.Columns.Add(p.Date.ToShortDateString());
                columns[i] = p;
                ++i;
            }
            foreach (Student s in Students)
            {
                var row = table.NewRow();
                row[0] = s.LastName + ", " + s.FirstName;
                for (int j = 1; j < table.Columns.Count; ++j)
                {
                    CourseOccurrence idL = columns[j];
                    var idp = from p in s.Presence where s.Id == p.Student && p.Present == 1 select p.CourseOccurrence;
                    var ida = from a in s.Presence where s.Id == a.Student && a.Present == 0 select a.CourseOccurrence;
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


        public bool Admin
        {
            get
            {
                return App.CurrentUser.Role == "admin";
            }
        }

        public int Code
        {
            get {
                 return Course.Code; }
            set
            {
                Course.Code = value;
                RaisePropertyChanged(nameof(Code));
                App.Messenger.NotifyColleagues(App.MSG_CODE_CHANGED, string.IsNullOrEmpty(value.ToString()) ? "<New Course>" : "Course " + value);
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
                App.Messenger.NotifyColleagues(App.MSG_COURSE_CHANGED, Course);
            }
        }

        public int DaysOfWeek
        {
            get { return Course.DayOfWeek; }
            set
            {
                Course.DayOfWeek = value;
                RaisePropertyChanged(nameof(DaysOfWeek));
                App.Messenger.NotifyColleagues(App.MSG_COURSE_CHANGED, Course);
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
                App.Messenger.NotifyColleagues(App.MSG_COURSE_CHANGED, Course);
            }
        }

        public DateTime StartDate
        {
            get { if (IsNew)
                    return DateTime.Now;
                  return Course.StartDate; }
            set
            {
                Course.StartDate = value;
                RaisePropertyChanged(nameof(StartDate));
                Valid();
                App.Messenger.NotifyColleagues(App.MSG_COURSE_CHANGED, Course);
            }
        }

        public DateTime FinishDate
        {
            get {
                 if (IsNew)
                    return DateTime.Now;
                 return Course.FinishDate; }
            set
            {
                Course.FinishDate = value;
                RaisePropertyChanged(nameof(FinishDate));
                Valid();
                App.Messenger.NotifyColleagues(App.MSG_COURSE_CHANGED, Course);
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
                App.Messenger.NotifyColleagues(App.MSG_COURSE_CHANGED, Course);
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
                App.Messenger.NotifyColleagues(App.MSG_COURSE_CHANGED, Course);
            }
        }

        public bool Valid()
        {
                ClearErrors();

                if (IsNew && Courses.Any(c => Code == c.Code))
                {
                     AddError("Code", Properties.Resources.Error_Required);
                }
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

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private CourseOccurrence selectedCourseOccurrence;
        public CourseOccurrence SelectedCourseOccurrence
        {

            get { return selectedCourseOccurrence; }
            set
            {
                selectedCourseOccurrence = value;
            }
        }

        public Student selectedStudent;
        public Student SelectedStudent
        {
            get
            {
                return selectedStudent;
            }
            set
            {
                selectedStudent = value;
            }
        }

        private void SaveChanges()
        {
            if (IsNew && Valid() && !HasErrors)
            {
                App.Model.course.Add(Course);
                IsNew = false;
            }
            else if (Valid() && !HasErrors)
            {
                App.Model.SaveChanges();
            }
            else
            {
                App.Messenger.NotifyColleagues(App.MSG_CANCEL, Course);
            }
            App.Model.SaveChanges();
            App.Messenger.NotifyColleagues(App.MSG_COURSE_CHANGED, Course);
        }

        public void CancelChanges()
        {
            App.CancelChanges();
            RaisePropertyChanged(nameof(Title));
            RaisePropertyChanged(nameof(DaysOfWeek));
            RaisePropertyChanged(nameof(Teacher));
            RaisePropertyChanged(nameof(StartDate));
            RaisePropertyChanged(nameof(FinishDate));
            RaisePropertyChanged(nameof(StartTime));
            RaisePropertyChanged(nameof(EndTime));
            Courses = new ObservableCollection<Course>(App.Model.course);
            Users = new ObservableCollection<User>(App.Model.user);
            AllStudents = new ObservableCollection<Student>(App.Model.student);
            Students = new ObservableCollection<Student>(App.Model.student);
            CoursesOccurrence = new ObservableCollection<CourseOccurrence>(App.Model.courseoccurrence);
            Presences = new ObservableCollection<Presence>(App.Model.presence);
            NonStudents = new ObservableCollection<Student>(App.Model.student);
            Day = new ObservableCollection<String>(Course.Day);
        }

        private void DeleteAction()
        {
            var courseToDelete = App.Model.course.Where(c => c.Code == Course.Code)
            .Include("CourseOccurrence.Presence")
            .Include("Student")
            .SingleOrDefault();
            App.Model.course.Remove(courseToDelete);
            App.Model.SaveChanges();
            App.Messenger.NotifyColleagues(App.MSG_COURSE_CHANGED, Course);
            App.Messenger.NotifyColleagues(App.MSG_CLOSE_TAB, Parent);
        }
    }
}
