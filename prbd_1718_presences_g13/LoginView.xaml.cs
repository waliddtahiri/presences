using PRBD_Framework;
using System;
using System.Collections.Generic;
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
    /// Logique d'interaction pour LoginView.xaml
    /// </summary>
    public partial class LoginView : WindowBase
    {
        public User User { get; set; }

        public ICommand Login { get; set; }
        public ICommand Cancel { get; set; }


        private string pseudo;
        public string Pseudo { get { return pseudo; } set { pseudo = value; Validate(); } }

        private string password;
        public string Password { get { return password; } set { password = value; Validate(); } }

        public LoginView()
        {
            InitializeComponent();


            Login = new RelayCommand(LoginAction, () => { return pseudo != null && password != null && !HasErrors; });
            Cancel = new RelayCommand(() => Close());
            DataContext = this;
        }

        private new User Validate()
        {
            ClearErrors();

            var user = App.Model.user.Where(u => u.Pseudo == Pseudo).SingleOrDefault();
            if (string.IsNullOrEmpty(Pseudo))
            {
                AddError("Pseudo", Properties.Resources.Error_Required);
                Console.WriteLine("Pseudo empty");
            }
            if (Pseudo != null)
            {
                if (Pseudo.Length < 3)
                    AddError("Pseudo", Properties.Resources.Error_LengthGreaterEqual3);
                else
                {
                    if (user == null)
                        AddError("Pseudo", Properties.Resources.Error_DoesNotExist);
                }
            }

            if (string.IsNullOrEmpty(Password))
                AddError("Password", Properties.Resources.Error_Required);
            if (Password != null)
            {
                if (Password.Length < 3)
                    AddError("Password", Properties.Resources.Error_LengthGreaterEqual3);
                else if (user != null && user.Password != Password)
                    AddError("Password", Properties.Resources.Error_WrongPassword);
            }

            RaiseErrors();

            return user;
        }

        private void LoginAction()
        {
            var user = Validate();
            if (!HasErrors)
            {
                App.CurrentUser = user;
                ShowMainView();
                Close();
            }
        }

        private static void ShowMainView()
        {
            var mainView = new MainView();
            mainView.Show();
            Application.Current.MainWindow = mainView;
        }

    }
}