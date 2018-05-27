using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Logique d'interaction pour Planning.xaml
    /// </summary>
    public partial class Planning : UserControlBase
    {


        public ObservableCollection<Course> Courses { get; private set; }
        public ObservableCollection<Presence> Presences { get; private set; }
        public ObservableCollection<CourseOccurrence> CoursesOccurrence { get; private set; }
        public ObservableCollection<CourseOccurrence> CourseOccurrence { get; private set; }

        public ICommand PreviousWeek { get; set; }
        public ICommand NextWeek { get; set; }
        public ICommand DisplayEncodage { get; set; }

        public Planning()
        {
            DataContext = this;

            Courses = new ObservableCollection<Course>(App.CurrentUser.Course);
            CoursesOccurrence = new ObservableCollection<CourseOccurrence>(App.Model.courseoccurrence);
            Presences = new ObservableCollection<Presence>(App.Model.presence);

            var s = from c in CoursesOccurrence
                    where c.Course.User.Equals(App.CurrentUser) && c.Date.CompareTo(Date) >= 0 && c.Date.CompareTo(Date.AddDays(+7)) < 0
                    select c;
            CourseOccurrence = new ObservableCollection<CourseOccurrence>(s);

            App.Messenger.Register<Course>(App.MSG_CANCEL, Course => { CancelChanges(); });


            PreviousWeek = new RelayCommand(() => { Datum.SelectedDate = Date.AddDays(-7); CourseOccurrence.RefreshFromModel(s); });
            NextWeek = new RelayCommand(() => { Datum.SelectedDate = Date.AddDays(+7); CourseOccurrence.RefreshFromModel(s); });
            
            DisplayEncodage = 
            new RelayCommand<CourseOccurrence> (c => {

                foreach (Student st in c.Course.Student)
                {
                    Presence p = new Presence(st.Id, c.Id);

                    if (c.Presence.Count < c.Course.Student.Count && !Presences.Contains(p))
                    {
                        App.Model.presence.Add(p);
                    }
   
                }
                App.Messenger.NotifyColleagues(App.MSG_DISPLAY_ENCODAGE, c);
            });

            InitializeComponent();

            
        }

        DateTime DateJour = DateTime.Today;
        public DateTime Date
        {
            get
            {
                
                int today = DateJour.Day;

                while (DateJour.DayOfWeek != DayOfWeek.Monday)
                {
                    DateJour=DateJour.AddDays(-1);
                }
                return DateJour;
            }
            set
            {
                DateJour = value;
                RaisePropertyChanged(nameof(DateJour));
            }
            
        }

        public CourseOccurrence selected;
        public CourseOccurrence Selected
        {
            get
            {
                return selected;

            }

            set
            {
                selected = value;
            }
        }

        public void CancelChanges()
        {
            App.CancelChanges();
            Courses = new ObservableCollection<Course>(App.CurrentUser.Course);
            CoursesOccurrence = new ObservableCollection<CourseOccurrence>(App.Model.courseoccurrence);
            Presences = new ObservableCollection<Presence>(App.Model.presence);

            var s = from c in CoursesOccurrence
                    where c.Course.User.Equals(App.CurrentUser) && c.Date.CompareTo(Date) >= 0 && c.Date.CompareTo(Date.AddDays(+7)) < 0
                    select c;
            CourseOccurrence = new ObservableCollection<CourseOccurrence>(s);
        }

    }
}
