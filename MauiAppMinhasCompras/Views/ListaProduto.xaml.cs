using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace MauiAppMinhasCompras.Views;


public partial class ListaProduto : ContentPage
{
	ObservableCollection<Produto> lista = new ObservableCollection<Produto>(); // Quando eu adicionar algo na ObservableCollection, ele vai atualizar a tela automaticamente na minha list view, diferente de uma List normal
	public ListaProduto()
	{
		InitializeComponent();

		lst_produtos.ItemsSource = lista; // Aqui estou dizendo que a minha lista de produtos vai ser a minha ObservableCollection
	}

    protected override void OnAppearing()
    {
        CarregarProdutos();
    }

    private async void CarregarProdutos()
    {
        try
        {
            List<Produto> tmp = await App.Db.GetAll();

            string categoriaSelecionada = picker_filtro_categoria.SelectedItem as string;

            // Só filtra se o usuário realmente escolheu uma categoria no Picker
            if (!string.IsNullOrEmpty(categoriaSelecionada))
            {
                tmp = tmp.Where(p => p.Categoria == categoriaSelecionada).ToList();
            }

            lista.Clear();
            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao carregar produtos: {ex.Message}");
        }
    }

    private void picker_filtro_categoria_SelectedIndexChanged(object sender, EventArgs e)
    {
        CarregarProdutos();
    }
    private void btn_limpar_filtro_Clicked(object sender, EventArgs e)
    {
        // Limpa a seleção do Picker e recarrega a lista completa, sem filtro
        picker_filtro_categoria.SelectedItem = null;
        CarregarProdutos();
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

		lst_produtos.IsRefreshing = true;	

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
		finally
		{
			lst_produtos.IsRefreshing = false; // Aqui estou dizendo que a minha list view não está mais sendo atualizada, para que o usuário possa voltar a interagir com a lista de produtos
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
		try
		{
			// "sender é o próprio MenuItem que foi clicado
			var menuItem = (MenuItem)sender;

			// Aqui estou pegando o produto que foi selecionado na lista de produtos
			var produtoSelecionado = (Produto)menuItem.CommandParameter;

			bool confirm = await DisplayAlert("Tem Certeza?", "Remover Produto?", "SIM", "NÃO");

			// Aqui estou chamando o método Delete() que está no meu banco de dados, que vai deletar o produto selecionado)
			if (confirm)
			{
				await App.Db.Delete(produtoSelecionado);
				lista.Remove(produtoSelecionado); // Aqui estou removendo o produto selecionado da minha ObservableCollection, para que a minha list view seja atualizada automaticamente
			}
		}
		catch (Exception ex)
		{
			DisplayAlert("Ops", ex.Message, "OK");
		}
	}

	private void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
	{
		try
		{
			Produto p = e.SelectedItem as Produto;

			Navigation.PushAsync(new Views.EditarProduto
			{
				BindingContext = p,
			});
		}
		catch (Exception ex)
		{
			DisplayAlert("Ops", ex.Message, "OK");
		}

	}

	private async void lst_produtos_Refreshing(object sender, EventArgs e)
	{
		try
		{
			lista.Clear();
			List<Produto> tmp = await App.Db.GetAll();
			tmp.ForEach(i => lista.Add(i));
		}
		catch (Exception ex)
		{
			await DisplayAlert("Ops", ex.Message, "OK");
		}
		finally
		{
			lst_produtos.IsRefreshing = false; // Aqui estou dizendo que a minha list view não está mais sendo atualizada, para que o usuário possa voltar a interagir com a lista de produtos
		}
	}
    private async void ToolbarItem_Relatorio_Clicked(object sender, EventArgs e)
    {
        // Busca TODOS os produtos do banco, ignorando qualquer filtro ativo na tela,
        // porque o relatório deve mostrar o total geral por categoria
        List<Produto> todos = await App.Db.GetAll();

        var porCategoria = todos
            .GroupBy(p => p.Categoria)
            .Select(g => new { Categoria = g.Key, Total = g.Sum(p => p.Total) })
            .OrderByDescending(g => g.Total);

        string msg = "";
        foreach (var item in porCategoria)
        {
            msg += $"{item.Categoria}: {item.Total:C}\n";
        }

        if (string.IsNullOrEmpty(msg))
            msg = "Nenhum produto cadastrado ainda.";

        await DisplayAlert("Relatório por Categoria", msg, "OK");
    }
}