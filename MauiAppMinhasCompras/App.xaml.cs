using Microsoft.Extensions.DependencyInjection;

namespace MauiAppMinhasCompras
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //Maingpage = new AppShell();
            MainPage = new NavigationPage(new Views.ListaProduto());
        }
    }
}