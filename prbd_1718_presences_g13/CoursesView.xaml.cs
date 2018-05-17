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
        public ObservableCollection<Course> AllCourses { get; private set; }

        public ICommand ClearFilter { get; set; }
        public ICommand DisplayCoursesDetails { get; set; }

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

            Courses = new ObservableCollection<Course>(App.Model.course);

            AllCourses = new ObservableCollection<Course>(App.Model.course);

            Users = new ObservableCollection<User>(App.Model.user);


            StudentsCount();

            ClearFilter = new RelayCommand(() => { Filter = ""; prof.SelectedValue = ""; Courses = new ObservableCollection<Course>(App.Model.course); });
            
            InitializeComponent();

            prof.SelectionChanged += new SelectionChangedEventHandler(ProfChanged);
        }

        private void StudentsCount()
        {
            foreach (Course c in Courses)
                c.Student.Count();
        }

        private void ProfChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilterAction();
            Filter = "";
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
            }
            else if(string.IsNullOrEmpty(Filter) && prof.SelectedValue !=null) 
            {
                var filtered = from c in Courses
                               where
                               c.User.Equals(prof.SelectedValue) 
                               select c;

                Courses = new ObservableCollection<Course>(filtered);

            }
            else if(StartDate.SelectedDate!= null && FinishDate.SelectedDate != null)
            {
                var filtered = from c in Courses
                               where
                               c.StartDate.ToShortDateString().Equals(StartDate.SelectedDate.Value) && c.FinishDate.ToShortDateString().Equals(FinishDate.SelectedDate.Value)
                               select c;

                Courses = new ObservableCollection<Course>(filtered);
            }
            else
                Courses = new ObservableCollection<Course>(App.Model.course);


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
