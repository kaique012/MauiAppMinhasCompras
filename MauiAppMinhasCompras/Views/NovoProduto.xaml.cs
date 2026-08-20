using MauiAppMinhasCompras.Models;

namespace MauiAppMinhasCompras.Views;

public partial class NovoProduto : ContentPage
{
	public NovoProduto()
	{
		InitializeComponent();
	}

    private async void ToolbarItem_Clicked (object sender, EventArgs e)
	{
		try
		{
			Produto p = new Produto // Declarei uma variavel do tipo p que recebe um produto
			{
				Descricao = txt_descricao.Text,
				Quantidade = Convert.ToDouble(txt_quantidade.Text),
				Preco = Convert.ToDouble(txt_preco.Text)
			};

			await App.Db.Insert(p); // Todo metodo "await" tem que ser async
			await DisplayAlertAsync("Sucesso!", "Registro Inserido", "OK");
        }
		catch (Exception ex)
		{
			await DisplayAlertAsync("Ops", ex.Message, "OK");
		}
	}
}