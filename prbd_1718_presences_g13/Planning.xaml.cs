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
        public ObservableCollection<User> Users { get; private set; }
        public ObservableCollection<Student> Students { get; private set; }

        public ICommand PreviousWeek { get; set; }
        public ICommand NextWeek { get; set; }
        public ICommand DisplayEncodage { get; set; }

        public Planning()
        {
            DataContext = this;

            Courses = new ObservableCollection<Course>(App.CurrentUser.Course);
            CoursesOccurrence = new ObservableCollection<CourseOccurrence>(App.Model.courseoccurrence);

            var s = from c in CoursesOccurrence
                    where c.Course.Equals(Courses)
                    select c;
            CourseOccurrence = new ObservableCollection<CourseOccurrence>(s);

            Presences = new ObservableCollection<Presence>(App.Model.presence);
            PreviousWeek = new RelayCommand(() => { Datum.SelectedDate = Date.AddDays(-7); });
            NextWeek = new RelayCommand(() => { Datum.SelectedDate = Date.AddDays(+7); });

            DisplayEncodage = new RelayCommand<CourseOccurrence>(c => { App.Messenger.NotifyColleagues(App.MSG_DISPLAY_ENCODAGE, c); });

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

        public CourseOccurrence Selected
        {

            get
            {
                return CoursesOccurrence.Where(c => c.Date.Equals(Date)).First();

            }

            set
            {
            }
        }

    }
}
