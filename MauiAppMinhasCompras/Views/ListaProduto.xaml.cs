using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;

namespace MauiAppMinhasCompras.Views;


public partial class ListaProduto : ContentPage
{
	ObservableCollection<Produto> lista = new ObservableCollection<Produto>(); // Quando eu adicionar algo na ObservableCollection, ele vai atualizar a tela automaticamente na minha list view, diferente de uma List normal
	public ListaProduto()
	{
		InitializeComponent();

		lst_produtos.ItemsSource = lista; // Aqui estou dizendo que a minha lista de produtos vai ser a minha ObservableCollection
	}

    protected override async void OnAppearing() // Esse método é chamado quando a tela é exibida, ou seja, quando o usuário entra na tela de lista de produtos
    {
        lista.Clear(); // limpa antes de recarregar, evitando duplicar
        List<Produto> tmp = await App.Db.GetAll();
        tmp.ForEach(i => lista.Add(i));
    }

	private void ToolbarItem_Clicked(object sender, EventArgs e)
	{
		try
		{
			Navigation.PushAsync(new Views.NovoProduto()); 
			//Quando realiza o evento "clicked" em ListaProduto.Xaml ele muda de tela para adicionar novos produtos
		}
		catch (Exception ex)
		{
			DisplayAlert("Ops", ex.Message, "Ok");
		}
	}

    private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
		string q = e.NewTextValue;

		lista.Clear(); // Limpa a lista de produtos para que não fique duplicado quando o usuário digitar algo na caixa de pesquisa
		try
		{
			List<Produto> tmp = await App.Db.Search(q);
            tmp.ForEach(i => lista.Add(i));
        }
		catch (Exception ex)
		{
			DisplayAlert("Ops", ex.Message, "Ok");
		}
	}

	private void ToolbarItem_Clicked_1(object sender, EventArgs e)
	{
		double soma = lista.Sum(i => i.Total);

        string msg = $"O total é {soma:C}";

		DisplayAlert("Total dos produtos", msg, "OK");
	}

	private async void MenuItem_Clicked(object sender, EventArgs e)
    {
		// "sender é o próprio MenuItem que foi clicado
		var menuItem = (MenuItem)sender;

		// Aqui estou pegando o produto que foi selecionado na lista de produtos
		var produtoSelecionado = (Produto)menuItem.CommandParameter;

        // Aqui estou chamando o método Delete() que está no meu banco de dados, que vai deletar o produto selecionado)
        await App.Db.Delete(produtoSelecionado);

        // Aqui estou removendo o produto selecionado da minha ObservableCollection, para que a minha list view seja atualizada automaticamente
        lista.Remove(produtoSelecionado); 
    }
}