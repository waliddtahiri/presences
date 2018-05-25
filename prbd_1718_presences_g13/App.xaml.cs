using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Threading;
using System.Globalization;
using prbd_1718_presences_g13.Properties;

namespace prbd_1718_presences_g13
{
    public partial class App : Application
    {
        public const string MSG_DISPLAY_COURSE = "MSG_DISPLAY_COURSE";
        public const string MSG_DISPLAY_ENCODAGE = "MSG_DISPLAY_ENCODAGE";
        public const string MSG_NEW_COURSE = "MSG_NEW_COURSE";
        public const string MSG_PRESENCE_CHANGED = "MSG_PRESENCE_CHANGED";


        public static Entities Model { get; private set; } = new Entities();
        public static Messenger Messenger { get; } = new Messenger();
        public static User CurrentUser { get; set; }


        public App()
        {
            PrepareDatabase();

            TestingEntityFramework();

            ColdStart();

            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Settings.Default.Culture);

            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
            Thread.CurrentThread.CurrentCulture = ci;

        }

        private static void TestingEntityFramework()
        {
            // Donne une valeur à la propriété "DataProperty" qui est utilisée comme dossier de base 
            // dans App.config pour la connection string vers la DB.
            var dbPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));
            AppDomain.CurrentDomain.SetData("DataDirectory", dbPath);

            // création du modèle
            var model = new Entities();

            // activation du log dans la console
            model.Database.Log = Console.Write;

        }

        private void ColdStart()
        {
            Model.user.Find(1);
        }



        private void PrepareDatabase()
        {
            // Donne une valeur à la propriété "DataProperty" qui est utilisée comme dossier de base dans App.config pour
            // la connection string vers la DB. Cette valeur est calculée en chemin relatif à partir du dossier de 
            // l'exécutable, soit <dossier projet>/bin/Debug.
            var dbPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\database"));
            Console.WriteLine("Database path: " + dbPath);
            AppDomain.CurrentDomain.SetData("DataDirectory", dbPath);

            // Si la base de données n'existe pas, la créer en exécutant le script SQL
            if (!File.Exists(Path.Combine(dbPath, "prbd_1718_presences_gXX.mdf")))
            {
                Console.WriteLine("Creating database...");
                string script = File.ReadAllText(Path.Combine(dbPath, "prbd_1718_presences_gXX.sql"));

                // dans le script, on remplace "{DBPATH}" par le dossier où on veut créer la DB
                script = script.Replace("{DBPATH}", dbPath);

                // On splitte le contenu du script en une liste de strings, chacune contenant une commande SQL.
                // Pour faire le split, on se sert des commandes "GO" comme délimiteur.
                IEnumerable<string> commandStrings = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

                // On se connecte au driver de base de données "(localdb)\MSSQLLocalDB" qui permet de travailler avec des
                // fichiers de données SQL Server attachés sans nécessiter qu'une instance de SQL Server ne soit présente.
                string sqlConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=True";
                SqlConnection connection = new SqlConnection(sqlConnectionString);
                connection.Open();
                // On exécute les commandes SQL une par une.
                foreach (string commandString in commandStrings)
                    if (commandString.Trim() != "")
                        using (var command = new SqlCommand(commandString, connection))
                            command.ExecuteNonQuery();
                connection.Close();
            }
        }

    }
}

