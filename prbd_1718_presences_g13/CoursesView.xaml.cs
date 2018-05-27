using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace prbd_1718_presences_g13
{
    /// <summary>
    /// Logique d'interaction pour MainView.xaml
    /// </summary>
    public partial class CoursesView : UserControlBase
    {


        private ObservableCollection<Course> courses;

        public ObservableCollection<Course> Courses
        {
            get
            {
                return courses;
            }
            set
            {
                courses = value;
                RaisePropertyChanged(nameof(Courses));
            }
        }
        public ObservableCollection<User> Users { get; private set; }
        public ObservableCollection<Student> Students { get; private set; }

        public ICommand ClearFilter { get; set; }
        public ICommand DisplayCoursesDetails { get; set; }
        public ICommand NewCourse { get; set; }

        private string filter;
        public string Filter
        {
            get { return filter; }
            set
            {
                filter = value;
                ApplyFilterAction();
                RaisePropertyChanged(nameof(Filter));
            }
        }

        public CoursesView()
        {

            DataContext = this;

            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            DisplayCoursesDetails = new RelayCommand<Course>(c => { App.Messenger.NotifyColleagues(App.MSG_DISPLAY_COURSE, c); });

            App.Messenger.Register<Course>(App.MSG_COURSE_CHANGED, Course => { ApplyFilterAction(); });

            App.Messenger.Register<Course>(App.MSG_CANCEL, Course => { App.CancelChanges(); ApplyFilterAction(); });

            App.Messenger.Register<Course>(App.MSG_SAVE, Course => { ApplyFilterAction(); });

            Courses = new ObservableCollection<Course>(App.Model.course);

            Users = new ObservableCollection<User>(App.Model.user);

            NewCourse = new RelayCommand(() => { App.Messenger.NotifyColleagues(App.MSG_NEW_COURSE); });



            if (App.CurrentUser.Role == "admin")
            {
                ClearFilter = new RelayCommand(() =>
                {
                    Filter = ""; prof.SelectedValue = ""; StartDate.SelectedDate = null;
                    FinishDate.SelectedDate = null; Courses = new ObservableCollection<Course>(App.Model.course);
                });
            }
            else
            {
                ClearFilter = new RelayCommand(() => {
                    Filter = ""; StartDate.SelectedDate = null;
                    FinishDate.SelectedDate = null; Courses = new ObservableCollection<Course>(App.CurrentUser.Course);
                });
            }

            InitializeComponent();

            prof.SelectionChanged += new SelectionChangedEventHandler(ProfChanged);
            StartDate.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(StartDateChanged);
            FinishDate.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(FinishDateChanged);

            Teacher();
        }

        private void Teacher()
        {
            if (App.CurrentUser.Role == "teacher")
            {
                Courses = new ObservableCollection<Course>(App.CurrentUser.Course);
                prof.SelectedItem = App.CurrentUser;
            }
        }

        public bool Admin
        {
            get
            {
                return App.CurrentUser.Role == "admin";
            }
        }


        private void ProfChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilterAction();
        }

        private void StartDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilterAction();
        }

        private void FinishDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilterAction();
        }

        private void ApplyFilterAction()
        {
            if (!string.IsNullOrEmpty(Filter))
            {
                var filtered = from c in App.Model.course
                               where
                               c.Title.Contains(Filter)
                               select c;

                Courses = new ObservableCollection<Course>(filtered);

                if (prof.SelectedValue != null)
                {
                    var filterez = from c in Courses
                                   where
                                   c.User.Equals(prof.SelectedValue)
                                   select c;

                    Courses = new ObservableCollection<Course>(filterez);
                }

                if (StartDate.SelectedDate != null && FinishDate.SelectedDate != null && StartDate.SelectedDate <= FinishDate.SelectedDate)
                {
                    var filteredd = from c in Courses
                                    where
                                    c.StartDate.CompareTo(StartDate.SelectedDate) >= 0 && c.FinishDate.CompareTo(FinishDate.SelectedDate) <= 0
                                    select c;

                    Courses = new ObservableCollection<Course>(filteredd);
                }
                if (Inscrit)
                {
                    var filter = from c in Courses
                                 where c.Student.Count > 0
                                 select c;

                    Courses = new ObservableCollection<Course>(filter);

                }
            }
            else if (string.IsNullOrEmpty(Filter))
            {
                if (prof.SelectedValue != null)
                {
                    var filtered = from c in Courses
                                   where
                                   c.User.Equals(prof.SelectedValue)
                                   select c;

                    Courses = new ObservableCollection<Course>(filtered);
                }

                else if (StartDate.SelectedDate != null && FinishDate.SelectedDate != null && StartDate.SelectedDate <= FinishDate.SelectedDate)
                {
                    var filteredd = from c in Courses
                                    where
                                    c.StartDate.CompareTo(StartDate.SelectedDate) >= 0 && c.FinishDate.CompareTo(FinishDate.SelectedDate) <= 0
                                    select c;

                    Courses = new ObservableCollection<Course>(filteredd);
                }

                else if (Inscrit)
                {
                    var filter = from c in Courses
                                 where c.Student.Count > 0
                                 select c;

                    Courses = new ObservableCollection<Course>(filter);

                }

                else
                    Courses = new ObservableCollection<Course>(App.Model.course);
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

        public bool inscrit;
        public bool Inscrit
        {
            get
            {
                return inscrit;
            }

            set
            {
                inscrit = value;
                ApplyFilterAction();
            }

        }
    }
}
