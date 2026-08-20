using MauiAppMinhasCompras.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace MauiAppMinhasCompras
{
    public partial class App : Application
    {
        static SQLiteDataBaseHelper _db; // Variável estática para armazenar a instância do banco de dados SQLite, e está privada. O _ é para sinalizar um campo

        public static SQLiteDataBaseHelper Db // O Db aqui vem sem _ para sinalizar que é uma propriedade
        {
            get
            {
                if (_db == null) // verifico se tem um objeto dentro do campo db
                {
                    string path = Path.Combine(                              // caminho ate o arquivo db3
                        Environment.GetFolderPath(
                        Environment.SpecialFolder.LocalApplicationData),
                         "banco_sqlite_compras_db3" );                           //Environment = Prove informações do ambiente que eu estou (Windows, Linux, MacOS, Android, iOS)
                                                                                //GetFolderPath(Environment.SpecialFolder.LocalApplicationData), // pega o caminho da pasta local do aplicativo

                    _db = new SQLiteDataBaseHelper(path);                // se não tiver, cria um novo objeto do tipo SQLiteDataBaseHelper e passa o nome do banco de dados como parâmetro
                } 
                return _db;                                            // e retorna o campo db, que agora tem um objeto do tipo SQLiteDataBaseHelper
            }
        } 
        public App()
        {
            InitializeComponent();

            //Maingpage = new AppShell();
            MainPage = new NavigationPage(new Views.ListaProduto());
        }
    }
}