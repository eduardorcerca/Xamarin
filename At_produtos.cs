using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Data;

namespace App1
{
    [Activity(Label = "At_produtos")]
    public class At_produtos : Activity
    {
        Button botao_adicionar_produtos;
        ListView list_produtos;
        TextView label_numero_produtos;
        List<Cl_produtos> PRODUTOS;
        List<string> NOMES;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.layout_produtos);

            botao_adicionar_produtos = FindViewById<Button>(Resource.Id.botao_adicionar_produtos);
            list_produtos = FindViewById<ListView>(Resource.Id.list_produtos);
            label_numero_produtos = FindViewById<TextView>(Resource.Id.label_numero_produtos);

            ConstroiListaProdutos();

            botao_adicionar_produtos.Click += Botao_adicionar_produtos_Click;
            list_produtos.ItemLongClick += List_produtos_ItemLongClick;
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                ConstroiListaProdutos();
            }
        }

        private void List_produtos_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            //seleciona produto para excluir ou alterar dados
            Cl_produtos produto_selecionado = PRODUTOS[e.Position];

            //caixa de mensagem com as opções
            AlertDialog.Builder caixa_editar_eliminar = new AlertDialog.Builder(this);
            caixa_editar_eliminar.SetTitle("EDITAR | ELIMINAR");
            caixa_editar_eliminar.SetMessage(produto_selecionado.nm_produto);

            //editar cliente
            caixa_editar_eliminar.SetPositiveButton("EDITAR", delegate { EditarProduto(produto_selecionado.id_produto); });

            //eliminar cliente
            caixa_editar_eliminar.SetNegativeButton("ELIMINAR", delegate { EliminarProduto(produto_selecionado.id_produto); });

            caixa_editar_eliminar.Show();
        }

        private void EliminarProduto(int id_produto)
        {
            Cl_gestor.EXE_NON_QUERY("DELETE FROM produtos WHERE id_produto = " + id_produto);

            //atualiza a lista de cliente do banco
            ConstroiListaProdutos();
        }

        private void EditarProduto(int id_produto)
        {
            //abrir atividade para editar os dados dos clientes
            Intent i = new Intent(this, typeof(At_produtos_editar));
            i.PutExtra("id_produto", id_produto.ToString());
            StartActivityForResult(i, 0);
        }


        private void Botao_adicionar_produtos_Click(object sender, EventArgs e)
        {
            //abre a tela adicionar produto
            Intent i = new Intent(this, typeof(At_produtos_editar));
            StartActivityForResult(i, 0);
        }

        private void ConstroiListaProdutos()
        {
            PRODUTOS = new List<Cl_produtos>();

            DataTable dados = Cl_gestor.EXE_QUERY("SELECT * FROM produtos ORDER BY nm_produto ASC");

            foreach (DataRow linha in dados.Rows)
            {
                PRODUTOS.Add(new Cl_produtos()
                {
                    id_produto = Convert.ToInt32(linha["id_produto"]),
                    nm_produto = linha["nm_produto"].ToString(),
                    preco_produto = Convert.ToInt32(linha["preco_produto"])
                });
            }

            NOMES = new List<string>();

            foreach (Cl_produtos produtos in PRODUTOS)
            {
                NOMES.Add(produtos.nm_produto);
            }

            ArrayAdapter<string> adaptador = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, NOMES);
            list_produtos.Adapter = adaptador;

            //atualizar o numero de produtos total
            ApresentaTotalProdutos(PRODUTOS.Count);
        }

        private void ApresentaTotalProdutos(int total_produtos)
        {
            label_numero_produtos.Text = "Produtos registrados: " + total_produtos;
        }
    }
}