using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace App1
{
    [Activity(Label = "at_vendas")]
    public class At_vendas : Activity
    {
        Spinner combo_clientes;
        Spinner combo_produtos;
        EditText text_quantidade;
        Button cmd_venda_gravar;
        Button cmd_venda_cancelar;
        TextView text_preco_total;

        List<Cl_clientes> CLIENTES;
        List<Cl_produtos> PRODUTOS;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.layout_vendas);

            //ligando os componentes visuais
            combo_clientes = FindViewById<Spinner>(Resource.Id.combo_clientes);
            combo_produtos = FindViewById<Spinner>(Resource.Id.combo_produtos);
            text_quantidade = FindViewById<EditText>(Resource.Id.text_quantidade);
            cmd_venda_gravar = FindViewById<Button>(Resource.Id.cmd_venda_gravar);
            cmd_venda_cancelar = FindViewById<Button>(Resource.Id.cmd_venda_cancelar);
            text_preco_total = FindViewById<TextView>(Resource.Id.text_preco_total);

            //carregar os combos
            CarregarCombos();

            //eventos
            cmd_venda_cancelar.Click += delegate { this.Finish(); };
            cmd_venda_gravar.Click += Cmd_venda_gravar_Click;
            text_quantidade.TextChanged += Text_quantidade_TextChanged;
            combo_produtos.ItemSelected += Combo_produtos_ItemSelected;
        }

        private void Combo_produtos_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            AtualizaPrecoTotal();
        }

        private void Text_quantidade_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            AtualizaPrecoTotal();
        }

        private void Cmd_venda_gravar_Click(object sender, EventArgs e)
        {
            //gravar a venda no banco
            if (combo_clientes.SelectedItemPosition == -1 || combo_produtos.SelectedItemPosition == -1 || text_quantidade.Text == "")
            {
                AlertDialog.Builder caixa = new AlertDialog.Builder(this);
                caixa.SetTitle("ERRO!");
                caixa.SetMessage("Preencha todos os dados da venda!");
                caixa.Show();
                return;
            }

            int id_cliente = CLIENTES[combo_clientes.SelectedItemPosition].id_cliente;
            int id_produto = PRODUTOS[combo_produtos.SelectedItemPosition].id_produto;
            int quantidade = int.Parse(text_quantidade.Text);
            
            int id_venda = Cl_gestor.ID_DISPONIVEL("vendas", "id_venda");

            List<SQLparametro> parameter = new List<SQLparametro>()
            {
                new SQLparametro("@id_venda", id_venda),
                new SQLparametro("@id_cliente", id_cliente),
                new SQLparametro("@id_produto", id_produto),
                new SQLparametro("@quantidade", quantidade),
                new SQLparametro("@atualizacao", DateTime.Now)
            };

            Cl_gestor.EXE_NON_QUERY("INSERT INTO vendas VALUES(" +
                "@id_venda, " +
                "@id_cliente, " +
                "@id_produto, " +
                "@quantidade, " +
                "@atualizacao)", parameter);

            text_quantidade.Text = "";

            AlertDialog.Builder caixa2 = new AlertDialog.Builder(this);
            caixa2.SetMessage("Venda registrada com sucesso!");
            caixa2.Show();

        }

        private void CarregarCombos()
        {
            //carregar os combos a partir do banco
            DataTable dados_clientes = Cl_gestor.EXE_QUERY("SELECT * FROM clientes ORDER BY nm_cliente ASC");
            DataTable dados_produtos = Cl_gestor.EXE_QUERY("SELECT * FROM produtos ORDER BY nm_produto ASC");

            //clientes
            CLIENTES = new List<Cl_clientes>();
            foreach (DataRow linha in dados_clientes.Rows)
            {
                CLIENTES.Add(new Cl_clientes
                {
                    id_cliente = Convert.ToInt32(linha["id_cliente"]),
                    nm_cliente = linha["nm_cliente"].ToString(),
                    telefone = linha["telefone"].ToString()
                }
                );
            }

            //produtos
            PRODUTOS = new List<Cl_produtos>();
            foreach (DataRow linha in dados_produtos.Rows)
            {
                PRODUTOS.Add(new Cl_produtos
                {
                    id_produto = Convert.ToInt32(linha["id_produto"]),
                    nm_produto = linha["nm_produto"].ToString(),
                    preco_produto = Convert.ToInt32(linha["preco_produto"])
                }
                );
            }

            List<string> lista_nome_clientes = new List<string>();
            List<string> lista_nome_produtos = new List<string>();

            foreach (Cl_clientes cliente in CLIENTES)
            {
                lista_nome_clientes.Add(cliente.nm_cliente);
            }

            foreach (Cl_produtos produtos in PRODUTOS)
            {
                lista_nome_produtos.Add(produtos.nm_produto);
            }

            combo_clientes.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, lista_nome_clientes);
            combo_produtos.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, lista_nome_produtos);
        }

        private void AtualizaPrecoTotal()
        {
            //atualizar o preco total da venda
            if (PRODUTOS.Count == 0)
            {
                return;
            }

            int quantidade = -1;
            int preco_produto = -1;

            if (text_quantidade.Text != "")
            {
                if (int.Parse(text_quantidade.Text) > 0)
                {
                    quantidade = int.Parse(text_quantidade.Text);
                    preco_produto = PRODUTOS[combo_produtos.SelectedItemPosition].preco_produto;
                }
            }

            if (quantidade == -1)
            {
                text_preco_total.Text = "";
            }

            else
            {
                text_preco_total.Text = "Total: " + (quantidade * preco_produto).ToString() + " reais.";
            }
        }
    }
}