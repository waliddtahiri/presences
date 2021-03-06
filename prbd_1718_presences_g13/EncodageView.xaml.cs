﻿using PRBD_Framework;
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
    /// Logique d'interaction pour EncodageView.xaml
    /// </summary>
    public partial class EncodageView : UserControlBase
    {
        public CourseOccurrence CourseOccurence { get; set; }

        public ObservableCollection<Course> Courses { get; private set; }
        public ObservableCollection<Presence> Presences { get; private set; }
        public ObservableCollection<Presence> Presence { get; private set; }
        public ObservableCollection<Presence> Pres { get; private set; }
        public ObservableCollection<CourseOccurrence> CoursesOccurrence { get; private set; }
        public ObservableCollection<User> Users { get; private set; }
        public ObservableCollection<Student> Students { get; private set; }

        public EncodageView(CourseOccurrence courseoccurence)
        {
            DataContext = this;

            CourseOccurence = courseoccurence;

            Courses = new ObservableCollection<Course>(App.CurrentUser.Course);
            Students = new ObservableCollection<Student>(CourseOccurence.Course.Student);
            Presence = new ObservableCollection<Presence>(App.Model.presence);
            Users = new ObservableCollection<User>(App.Model.user);
            CoursesOccurrence = new ObservableCollection<CourseOccurrence>(App.Model.courseoccurrence);
            Pres = new ObservableCollection<Presence>(CourseOccurence.Presence);
            Presences = new ObservableCollection<Presence>(Pres);


            App.Messenger.Register<Course>(App.MSG_SAVE, Course => { App.Model.SaveChanges(); App.Messenger.NotifyColleagues(App.MSG_COURSE_CHANGED, Course); });
            App.Messenger.Register<Course>(App.MSG_CANCEL, Course => { CancelChanges(); });


            InitializeComponent();

        }

        public void CancelChanges()
        {
            foreach (Presence p in CourseOccurence.Presence)
            {
                var change = (from c in App.Model.ChangeTracker.Entries<Presence>()
                              where c.Entity == p
                              select c).FirstOrDefault();
                if (change != null)
                {
                    change.Reload();
                    Presences.RefreshFromModel(CourseOccurence.Presence);
                }
            }
        }

    }
}
