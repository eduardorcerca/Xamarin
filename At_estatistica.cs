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
using System.IO;

namespace App1
{
    [Activity(Label = "At_estatistica")]
    public class At_estatistica : Activity
    {
        ListView lista_vendas;
        List<Cl_clientes> CLIENTES;
        List<Cl_produtos> PRODUTOS;
        List<Cl_vendas> VENDAS;
        Spinner combo_clientes;
        Spinner combo_produtos;
        Button cmd_cliente;
        Button cmd_produto;
        Button cmd_total_vendas;
        Button cmd_total;
        Button cmd_relatorio;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.layout_estatistica);

            lista_vendas = FindViewById<ListView>(Resource.Id.lista_vendas);
            combo_clientes = FindViewById<Spinner>(Resource.Id.combo_clientes);
            combo_produtos = FindViewById<Spinner>(Resource.Id.combo_produtos);
            cmd_cliente = FindViewById<Button>(Resource.Id.cmd_cliente);
            cmd_produto = FindViewById<Button>(Resource.Id.cmd_produto);
            cmd_total_vendas = FindViewById<Button>(Resource.Id.cmd_total_vendas);
            cmd_total = FindViewById<Button>(Resource.Id.cmd_total);
            cmd_relatorio = FindViewById<Button>(Resource.Id.cmd_relatorio);


            ConstroiListaClienteProduto();
            ConstroiListaVendas("SELECT * FROM vendas");
            ApresentaListaVendas();

            //eventos dos botoes
            cmd_cliente.Click += Cmd_cliente_Click;
            cmd_produto.Click += Cmd_produto_Click;
            cmd_total.Click += Cmd_total_Click;
            cmd_total_vendas.Click += Cmd_total_vendas_Click;
            cmd_relatorio.Click += Cmd_relatorio_Click;
        }

        private void Cmd_relatorio_Click(object sender, EventArgs e)
        {
            Cl_vendas novo = new Cl_vendas();
            VENDAS = new List<Cl_vendas>();

            DataTable dados_vendas = Cl_gestor.EXE_QUERY("SELECT * FROM vendas");

            //localizacao da pasta onde esta o arquivo
            string pasta_dados = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "Vendas");

            //definindo o caminho para o arquivo
            string local_arquivo = Path.Combine(pasta_dados + @"/relatorio.txt");

            //caso o arquivo exista
            if (File.Exists(local_arquivo))
            {
                StreamWriter arquivo = new StreamWriter(local_arquivo, false);

                foreach (DataRow linha in dados_vendas.Rows)
                {
                    //dados da 'linha' (banco de dados)
                    int id_cliente = Convert.ToInt16(linha["id_cliente"]);
                    int id_produto = Convert.ToInt16(linha["id_produto"]);
                    int quantidade = Convert.ToInt16(linha["quantidade"]);

                    //buscar nome_cliente
                    string nome_cliente = CLIENTES.Where(i => i.id_cliente == id_cliente).FirstOrDefault().nm_cliente;

                    //buscar nome_produto e preco
                    var produto = PRODUTOS.Where(i => i.id_produto == id_produto).FirstOrDefault();
                    string nome_produto = produto.nm_produto;
                    int preco_unidade = produto.preco_produto;

                    novo.nm_cliente = nome_cliente;
                    novo.nm_produto = nome_produto;
                    novo.quantidade = quantidade;
                    novo.preco_produto = preco_unidade;
                    novo.preco_total = quantidade * preco_unidade;

                    arquivo.WriteLine("Cliente: " + novo.nm_cliente + " | Produto: " + novo.nm_produto + " | Quantidade: " + novo.quantidade + " | Preco: " + novo.preco_produto + " | Total: " + novo.preco_total);
                }

                arquivo.Dispose();
            }

            //criando o arquivo caso nao exista
            if (!File.Exists(local_arquivo))
            {
                StreamWriter arquivo = new StreamWriter(local_arquivo, false);

                foreach (DataRow linha in dados_vendas.Rows)
                {
                    //dados da 'linha' (banco de dados)
                    int id_cliente = Convert.ToInt16(linha["id_cliente"]);
                    int id_produto = Convert.ToInt16(linha["id_produto"]);
                    int quantidade = Convert.ToInt16(linha["quantidade"]);

                    //buscar nome_cliente
                    string nome_cliente = CLIENTES.Where(i => i.id_cliente == id_cliente).FirstOrDefault().nm_cliente;

                    //buscar nome_produto e preco
                    var produto = PRODUTOS.Where(i => i.id_produto == id_produto).FirstOrDefault();
                    string nome_produto = produto.nm_produto;
                    int preco_unidade = produto.preco_produto;

                    novo.nm_cliente = nome_cliente;
                    novo.nm_produto = nome_produto;
                    novo.quantidade = quantidade;
                    novo.preco_produto = preco_unidade;
                    novo.preco_total = quantidade * preco_unidade;

                    arquivo.WriteLine("Cliente: " + novo.nm_cliente + " | Produto: " + novo.nm_produto + " | Quantidade: " + novo.quantidade + " | Preco: " + novo.preco_produto + " | Total: " + novo.preco_total);
                }

                arquivo.Dispose();
            }

            AlertDialog.Builder relatorio = new AlertDialog.Builder(this);
            relatorio.SetMessage("Relatório salvo com sucesso!");
            relatorio.Show();
        }

        private void Cmd_total_vendas_Click(object sender, EventArgs e)
        {
            //apresenta a lista com todas as vendas feitas
            ConstroiListaVendas("SELECT * FROM vendas");
            ApresentaListaVendas();
        }

        private void Cmd_total_Click(object sender, EventArgs e)
        {
            //apresenta o valor total de todas as vendas
            //constroi a lista completa de vendas
            ConstroiListaVendas("SELECT * FROM vendas");

            int total = 0;

            foreach (Cl_vendas venda in VENDAS)
            {
                total += venda.preco_total;
            }

            //apresenta alertDialog
            AlertDialog.Builder caixa = new AlertDialog.Builder(this);
            caixa.SetTitle("Total das vendas");
            caixa.SetMessage("Total das vendas: " + total);
            caixa.SetCancelable(false);
            caixa.SetPositiveButton("OK", delegate { });
            caixa.Show();
        }

        private void Cmd_produto_Click(object sender, EventArgs e)
        {
            //apresenta a venda de um determinado produto
            int index = combo_produtos.SelectedItemPosition;
            int id_produto = PRODUTOS[index].id_produto;

            ConstroiListaVendas("SELECT * FROM vendas WHERE id_produto = " + id_produto);
            ApresentaListaVendas();
        }

        private void Cmd_cliente_Click(object sender, EventArgs e)
        {
            //apresenta a venda de um determinado cliente
            int index = combo_clientes.SelectedItemPosition;
            int id_cliente = CLIENTES[index].id_cliente;

            ConstroiListaVendas("SELECT * FROM vendas WHERE id_cliente = " + id_cliente);
            ApresentaListaVendas();
        }

        private void ConstroiListaVendas(string query)
        {
            //construir list<cl_vendas>
            //carregar os dados da tabela vendas
            VENDAS = new List<Cl_vendas>();

            DataTable dados_vendas = Cl_gestor.EXE_QUERY(query);

            foreach (DataRow linha in dados_vendas.Rows)
            {
                Cl_vendas novo = new Cl_vendas();

                //dados da 'linha' (banco de dados)
                int id_cliente = Convert.ToInt16(linha["id_cliente"]);
                int id_produto = Convert.ToInt16(linha["id_produto"]);
                int quantidade = Convert.ToInt16(linha["quantidade"]);

                //buscar nome_cliente
                string nome_cliente = CLIENTES.Where(i => i.id_cliente == id_cliente).FirstOrDefault().nm_cliente;

                //buscar nome_produto e preco
                var produto = PRODUTOS.Where(i => i.id_produto == id_produto).FirstOrDefault();
                string nome_produto = produto.nm_produto;
                int preco_unidade = produto.preco_produto;

                novo.nm_cliente = nome_cliente;
                novo.nm_produto = nome_produto;
                novo.quantidade = quantidade;
                novo.preco_produto = preco_unidade;
                novo.preco_total = quantidade * preco_unidade;

                VENDAS.Add(novo);
            }
        }

        private void ConstroiListaClienteProduto()
        {
            //constroi lista de clientes
            DataTable dados_clientes = Cl_gestor.EXE_QUERY("SELECT * FROM clientes");
            CLIENTES = new List<Cl_clientes>();

            foreach (DataRow linha in dados_clientes.Rows)
            {
                Cl_clientes novo = new Cl_clientes();
                novo.id_cliente = Convert.ToInt16(linha["id_cliente"]);
                novo.nm_cliente = linha["nm_cliente"].ToString();
                novo.telefone = linha["telefone"].ToString();
                CLIENTES.Add(novo);
            }

            //constroi lista de produtos
            DataTable dados_produtos = Cl_gestor.EXE_QUERY("SELECT * FROM produtos");
            PRODUTOS = new List<Cl_produtos>();

            foreach (DataRow linha in dados_produtos.Rows)
            {
                Cl_produtos novo = new Cl_produtos();
                novo.id_produto = Convert.ToInt16(linha["id_produto"]);
                novo.nm_produto = linha["nm_produto"].ToString();
                novo.preco_produto = Convert.ToUInt16(linha["preco_produto"]);
                PRODUTOS.Add(novo);
            }

            //constroi combo cliente
            List<string> nome_cliente = new List<string>();
            foreach (Cl_clientes cliente in CLIENTES)
            {
                nome_cliente.Add(cliente.nm_cliente);
            }

            ArrayAdapter<string> adaptador_clientes = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, nome_cliente);
            combo_clientes.Adapter = adaptador_clientes;

            //constroi combo produto
            List<string> nome_produto = new List<string>();
            foreach (Cl_produtos produtos in PRODUTOS)
            {
                nome_produto.Add(produtos.nm_produto);
            }

            ArrayAdapter<string> adaptador_produtos = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, nome_produto);
            combo_produtos.Adapter = adaptador_produtos;

        }

        private void ApresentaListaVendas()
        {
            //apresenta atualizacao da listview que apresenta as vendas
            Adaptador_vendas adaptador = new Adaptador_vendas(this, VENDAS);
            lista_vendas.Adapter = adaptador;
        }
    }
}