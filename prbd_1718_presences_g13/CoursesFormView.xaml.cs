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
        public ObservableCollection<Student> AllStudents { get; set; }
        public ObservableCollection<Student> Students { get;  set; }
        public ObservableCollection<Student> Stud { get;  set; }
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
        public ICommand Inscription { get; set; }
        public ICommand Desinscription { get; set; }
        public ICommand Delete { get; set; }

        public CoursesFormView(Course course, bool isNew)
        {
            DataContext = this;

            IsNew = isNew;

            Course = course;

            Courses = new ObservableCollection<Course>(App.Model.course);
            Users = new ObservableCollection<User>(App.Model.user);
            AllStudents = new ObservableCollection<Student>(App.Model.student);
            Stud = new ObservableCollection<Student>(Course.Student);
            Students = new ObservableCollection<Student>(Stud);
            NonStudents = new ObservableCollection<Student>(AllStudents.Except(Students));
            CoursesOccurrence = new ObservableCollection<CourseOccurrence>(Course.CourseOccurrence);
            Presences = new ObservableCollection<Presence>(App.Model.presence);
            Day = new ObservableCollection<String>(Course.Days);

            Valid();

            App.Messenger.Register<Course>(App.MSG_CANCEL, Course => { CancelChanges(); });

            App.Messenger.Register<Course>(App.MSG_SAVE, Course => { SaveChanges(); });

            DisplayEncodage = new RelayCommand<CourseOccurrence>(c => {

                foreach (Student st in c.Course.Student)
                {
                    Presence p = new Presence(st.Id, c.Id);

                    if (c.Presence.Count < c.Course.Student.Count && !Presences.Any(pp => pp.Student == p.Student && pp.CourseOccurence == p.CourseOccurence))
                    {
                        App.Model.presence.Add(p);
                    }

                }
                App.Messenger.NotifyColleagues(App.MSG_DISPLAY_ENCODAGE, c);
            });

            DesToIns = new RelayCommand(() => { while (NonStudents.Count != 0) {
                    Stud.Add(NonStudents.First());
                    NonStudents.Remove(NonStudents.First());
                }
                Students.RefreshFromModel(Stud);
                App.Messenger.NotifyColleagues(App.MSG_COURSE_CHANGED, Course);
            });

            Inscription = new RelayCommand(() => {
                if (NonStudents.Count != 0)
                {
                    Stud.Add(SelectedDesinscrit);
                    NonStudents.Remove(SelectedDesinscrit);
                    Students.RefreshFromModel(Stud);
                    App.Messenger.NotifyColleagues(App.MSG_COURSE_CHANGED, Course);
                }
            });

            Desinscription = new RelayCommand(() => {
                if (Students.Count != 0)
                {
                    Stud.Remove(SelectedInscrit);
                    Students.Remove(SelectedInscrit);
                    Students.RefreshFromModel(Stud);
                    NonStudents.RefreshFromModel(AllStudents.Except(Students));
                    App.Messenger.NotifyColleagues(App.MSG_COURSE_CHANGED, Course);
                }
            });

            InsToDes = new RelayCommand(() => { if (Students.Count != 0)
                {
                    Stud.Clear();
                    Students.RefreshFromModel(Stud);
                    NonStudents.RefreshFromModel(AllStudents.Except(Students));
                    App.Messenger.NotifyColleagues(App.MSG_COURSE_CHANGED, Course);
                }
            });

            Delete = new RelayCommand(DeleteAction, () => { return !IsNew; });

            HistoriquePrésences();

            InitializeComponent();  
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

        public bool NpSelected
        {
            get
            {
                return App.CurrentUser.Role == "admin" && SelectedDesinscrit != null;
            }
        }
        public bool IpSelected
        {
            get
            {
                return App.CurrentUser.Role == "admin" && SelectedInscrit != null;
            }
        }

        public int Code
        {
            get {
                 return Course.Code; }
            set
            {

                Course.Code = value;
                Valid();
                RaisePropertyChanged(nameof(Code));
                App.Messenger.NotifyColleagues(App.MSG_CODE_CHANGED, string.IsNullOrEmpty(value.ToString()) ? "<New Course>" : "Course " + value.ToString());
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
            get { 
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
                 return Course.FinishDate; }
            set
            {
                Course.FinishDate = value;
                RaisePropertyChanged(nameof(FinishDate));
                Valid();
                App.Messenger.NotifyColleagues(App.MSG_COURSE_CHANGED, Course);
            }
        }

        public void Occurences()
        {
            if (!IsNew && Course.CourseOccurrence.First().Date.CompareTo(StartDate) != 0 || Course.CourseOccurrence.Last().Date.CompareTo(FinishDate) != 0)
            { 
                var occ = App.Model.courseoccurrence.Where(p => p.Course.Code==Course.Code).Include("Presence").Include("Course.CourseOccurrence");
                App.Model.courseoccurrence.RemoveRange(occ);

                var presence = App.Model.presence.Where(p => p.CourseOccurrence.Course.Code == Course.Code).Include("CourseOccurrence.Presence").Include("Students");
                App.Model.presence.RemoveRange(presence);

                DateTime Occurence = StartDate;

                while ((int)Occurence.DayOfWeek != DaysOfWeek + 1)
                {
                    Occurence = Occurence.AddDays(+1);
                }
                do
                    {
                        CourseOccurrence c = new CourseOccurrence(Occurence, Course);
                        App.Model.courseoccurrence.Add(c);
                        Occurence = Occurence.AddDays(+7);
                    } while (Occurence.CompareTo(FinishDate) <= 0);
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

                if (IsNew && Courses.Any(c => Code == c.Code) || IsNew && Code == 0)
                {
                     AddError("Code", Properties.Resources.Error_Exist);
                }
                if (string.IsNullOrEmpty(Title))
                {
                    AddError("Title", Properties.Resources.Error_RequiredTitle);
                }
                if (Teacher == null)
                {
                        AddError("Teacher", Properties.Resources.Error_Required); 
                }
                if (StartDate.CompareTo(FinishDate) > 0 || FinishDate.CompareTo(StartDate) < 0)
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

        public Student selectedInscrit;
        public Student SelectedInscrit
        {
            get
            {
                return selectedInscrit;
            }
            set
            {
                selectedInscrit = value;
                RaisePropertyChanged(nameof(SelectedInscrit));
            }
        }

        public Student selectedDesinscrit;
        public Student SelectedDesinscrit
        {
            get
            {
                return selectedDesinscrit;
            }
            set
            {
                selectedDesinscrit = value;
                RaisePropertyChanged(nameof(SelectedDesinscrit));
                
            }
        }

        private void SaveChanges()
        {
            if (IsNew && Valid() && !HasErrors)
            {
                App.Model.course.Add(Course);
                DateTime Occurence = StartDate;

                while ((int)Occurence.DayOfWeek != DaysOfWeek + 1)
                {
                    Occurence = Occurence.AddDays(+1);
                }
                do
                {
                    CourseOccurrence c = new CourseOccurrence(Occurence, Course);
                    App.Model.courseoccurrence.Add(c);
                    Occurence = Occurence.AddDays(+7);
                } while (Occurence.CompareTo(FinishDate) <= 0);
                IsNew = false;
            }
            else if (!IsNew && Valid() && !HasErrors)
            {
                Occurences();
            }
            if (Stud.Count() < Course.Student.Count())
            {
                foreach(var m in Course.Student)
                {
                    var presences = App.Model.presence.Where(p=>p.Student==m.Id && p.CourseOccurrence.Course.Code==Course.Code).Include("CourseOccurrence.Presence").Include("Students");
                    App.Model.presence.RemoveRange(presences);
                }
                Course.Student.Clear();
            }
            while (Stud.Count != 0)
            {
                Course.Student.Add(Stud.First());
                Stud.Remove(Stud.First());
            }
            App.Model.SaveChanges();
            App.Messenger.NotifyColleagues(App.MSG_COURSE_CHANGED, Course);
            CoursesOccurrence.RefreshFromModel(App.Model.courseoccurrence.Where(c=>c.Course.Code==Course.Code));
            HistoriquePrésences();
            Stud = new ObservableCollection<Student>(Course.Student);
        }


        public void CancelChanges()
        {
            if (IsNew)
            {
                Code = 0;
                Title = null;
                Teacher = null;
                DaysOfWeek = 0;
                StartDate = new DateTime(0001, 01, 01);
                FinishDate = new DateTime(0001, 01, 01);
                RaisePropertyChanged(nameof(Course));
            }
            else
            {
                var change = (from c in App.Model.ChangeTracker.Entries<Course>()
                              where c.Entity == Course
                              select c).FirstOrDefault();
                if (change != null)
                {
                    change.Reload();
                    RaisePropertyChanged(nameof(Title));
                    RaisePropertyChanged(nameof(Teacher));
                    RaisePropertyChanged(nameof(DaysOfWeek));
                    RaisePropertyChanged(nameof(StartDate));
                    RaisePropertyChanged(nameof(FinishDate));
                    RaisePropertyChanged(nameof(StartTime));
                    RaisePropertyChanged(nameof(EndTime));
                    Stud.RefreshFromModel(Course.Student);
                    Students.RefreshFromModel(Course.Student);
                    NonStudents.RefreshFromModel(AllStudents.Except(Students));
                }
            }
            
        }

        private void DeleteAction()
        {
            if (MessageBox.Show("Vous êtes sur le point de supprimer un cours et toutes les informations s'y rapportant. Voulez-vous" +
                " continuer ?", "Delete Course " + Course.Code, MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
            {
                var courseToDelete = App.Model.course.Where(c => c.Code == Course.Code)
                .Include("CourseOccurrence.Presence")
                .Include("CourseOccurrence.Course")
                .Include("Student")
                .SingleOrDefault();
                App.Model.course.Remove(courseToDelete);

                var occ = App.Model.courseoccurrence.Where(p => p.Course.Code == Course.Code);
                App.Model.courseoccurrence.RemoveRange(occ);

                App.Model.SaveChanges();
                App.Messenger.NotifyColleagues(App.MSG_COURSE_CHANGED, Course);
                App.Messenger.NotifyColleagues(App.MSG_CLOSE_TAB, Parent);
            }
        }
    }
}
