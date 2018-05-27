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
using System.Windows.Shapes;

namespace prbd_1718_presences_g13
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainView : WindowBase
    {

        public ICommand Display { get; set; }
        public ICommand SaveChanges { get; set; }
        public ICommand CancelChanges { get; set; }

        public bool Teacher
        {
            get
            {
                return App.CurrentUser.Role == "teacher";
            }
        }

        public MainView()
        {

            DataContext = this;

            SaveChanges = new RelayCommand<Course>(c => { App.Messenger.NotifyColleagues(App.MSG_SAVE, c); });
            CancelChanges = new RelayCommand<Course>(c => {App.Messenger.NotifyColleagues(App.MSG_CANCEL, c); });

            InitializeComponent();

            App.Messenger.Register<TabItem>(App.MSG_CLOSE_TAB, tab =>
            {
                tabControl.Items.Remove(tab);
            });

            App.Messenger.Register<int>(App.MSG_CODE_CHANGED, (s) =>
            {
                (tabControl.SelectedItem as TabItem).Header = s;
            });

            App.Messenger.Register<Course>(App.MSG_DISPLAY_COURSE, course =>
            {
                if (course != null)
                {
                    var tab = (from TabItem t in tabControl.Items where (string)t.Header == "Course " + course.Code select t).FirstOrDefault();
                    if (tab == null)
                        newTabForCourse(course, false);
                    else
                        //Dispatcher.InvokeAsync(() => tab.Focus());
                        tab.Focus();
                }

            });

            App.Messenger.Register<CourseOccurrence>(App.MSG_DISPLAY_ENCODAGE, courseoccurrence =>
            {
                if (courseoccurrence != null)
                {
                    var tab = (from TabItem t in tabControl.Items where (string)t.Header == "Présences" select t).FirstOrDefault();
                    if (tab == null)
                        newTabForCourseOccurrence(courseoccurrence);
                    else
                        //Dispatcher.InvokeAsync(() => tab.Focus());
                        tab.Focus();
                }

            });

            App.Messenger.Register(App.MSG_NEW_COURSE,
                () =>
                {
                    // crée une nouvelle instance pour un nouveau client
                    var course = App.Model.course.Create();
                    newTabForCourse(course, true);
                });

            void newTabForCourse(Course course, bool isNew)
            {
                var tab = new TabItem()
                {
                    Header = isNew ? "New Course" : "Course " + course.Code,
                    Content = new CoursesFormView(course, isNew)
                };
                tab.MouseDown += (o, e) =>
                {
                    if (e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Pressed)
                        tabControl.Items.Remove(o);
                };
                tab.KeyDown += (o, e) =>
                {
                    if (e.Key == Key.W && Keyboard.IsKeyDown(Key.LeftCtrl))
                        tabControl.Items.Remove(o);
                };
                tabControl.Items.Add(tab);
                Dispatcher.InvokeAsync(() => tab.Focus());

            }

            void newTabForCourseOccurrence(CourseOccurrence courseoccurrence)
            {
                var tab = new TabItem()
                {
                    Header = "Présences -"+courseoccurrence.Course.Code+"- "+courseoccurrence.Date.ToShortDateString(),
                    Content = new EncodageView(courseoccurrence)
                };
                tab.MouseDown += (o, e) =>
                {
                    if (e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Pressed)
                        tabControl.Items.Remove(o);
                };
                tab.KeyDown += (o, e) =>
                {
                    if (e.Key == Key.W && Keyboard.IsKeyDown(Key.LeftCtrl))
                        tabControl.Items.Remove(o);
                };
                tabControl.Items.Add(tab);
                Dispatcher.InvokeAsync(() => tab.Focus());

                

            }

        }
    }
}
