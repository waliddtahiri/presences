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
    /// Logique d'interaction pour Planning.xaml
    /// </summary>
    public partial class Planning : UserControlBase
    {


        public ObservableCollection<Course> Courses { get; private set; }
        public ObservableCollection<Presence> Presences { get; private set; }
        public ObservableCollection<CourseOccurrence> CoursesOccurrence { get; private set; }
        public ObservableCollection<User> Users { get; private set; }
        public ObservableCollection<Student> Students { get; private set; }

        public Planning()
        {
            DataContext = this;

            Courses = new ObservableCollection<Course>(App.CurrentUser.Course);
            Users = new ObservableCollection<User>(App.Model.user);
            CoursesOccurrence = new ObservableCollection<CourseOccurrence>(App.Model.courseoccurrence);
            Presences = new ObservableCollection<Presence>(App.Model.presence);
            InitializeComponent();
        }
    }
}