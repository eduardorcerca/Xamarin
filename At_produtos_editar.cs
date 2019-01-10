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
    [Activity(Label = "At_produtos_editar")]
    public class At_produtos_editar : Activity
    {
        Button botao_gravar_produto;
        Button botao_cancelar_produto;
        EditText edit_nome_produto;
        EditText edit_preco_produto;

        int id_produto = 0;
        bool editar = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.layout_produtos_editar);

            //widgets
            botao_gravar_produto = FindViewById<Button>(Resource.Id.botao_gravar_produto);
            botao_cancelar_produto = FindViewById<Button>(Resource.Id.botao_cancelar_produto);
            edit_nome_produto = FindViewById<EditText>(Resource.Id.edit_nome_produto);
            edit_preco_produto = FindViewById<EditText>(Resource.Id.edit_preco_produto);

            //eventos
            botao_cancelar_produto.Click += delegate { this.Finish(); };
            botao_gravar_produto.Click += Botao_gravar_produto_Click;

            //verificar se existe a variavel 'id_produto' no intent
            if (this.Intent.GetStringExtra("id_produto") != null)
            {
                id_produto = int.Parse(this.Intent.GetStringExtra("id_produto"));
                ApresentaProduto();
                editar = true;
            }
        }

        private void ApresentaProduto()
        {
            //colocar as infos no edit_text
            DataTable dados = Cl_gestor.EXE_QUERY("SELECT * FROM produtos WHERE id_produto =" + id_produto);

            edit_nome_produto.Text = dados.Rows[0]["nm_produto"].ToString();
            edit_preco_produto.Text = dados.Rows[0]["preco_produto"].ToString();
        }

        private void Botao_gravar_produto_Click(object sender, EventArgs e)
        {
            //edita ou grava dados de um produto

            //verificar se os campos estão preenchidos
            if (edit_nome_produto.Text == "" || edit_preco_produto.Text == "")
            {
                AlertDialog.Builder caixa = new AlertDialog.Builder(this);
                caixa.SetTitle("ERRO!");
                caixa.SetMessage("Preencha todos os campos!");
                caixa.SetPositiveButton("OK", delegate { });
                caixa.Show();
                return;
            }

            //parametros
            List<SQLparametro> parameter = new List<SQLparametro>();
            if (!editar)
                parameter.Add(new SQLparametro("@id_produto", Cl_gestor.ID_DISPONIVEL("produtos", "id_produto")));
            else
                parameter.Add(new SQLparametro("@id_produto", id_produto));
            parameter.Add(new SQLparametro("@nm_produto", edit_nome_produto.Text));
            parameter.Add(new SQLparametro("@preco_produto", edit_preco_produto.Text));
            parameter.Add(new SQLparametro("@atualizacao", DateTime.Now));

            if (!editar)
            {
                //gravar novo produto
                DataTable dados = Cl_gestor.EXE_QUERY("SELECT nm_produto FROM produtos WHERE nm_produto = @nm_produto", parameter);
                if (dados.Rows.Count != 0)
                {
                    //verifica se foi encontrado um produto com o mesmo nome
                    AlertDialog.Builder caixa = new AlertDialog.Builder(this);
                    caixa.SetTitle("ERRO!");
                    caixa.SetMessage("Já existe um produto com o mesmo nome!");
                    caixa.SetPositiveButton("OK", delegate { });
                    caixa.Show();
                    return;
                }

                Cl_gestor.EXE_NON_QUERY(
                    "INSERT INTO produtos VALUES(" +
                    "@id_produto, " +
                    "@nm_produto, " +
                    "@preco_produto, " +
                    "@atualizacao)", parameter);

                //encerrar atividade
                Intent i = this.Intent;
                SetResult(Result.Ok, i);
                Finish();
            }

            else
            {
                //verifica se existe um produto com o mesmo nome
                DataTable dados = Cl_gestor.EXE_QUERY("SELECT nm_produto FROM produtos WHERE nm_produto = @nm_produto AND id_produto <> @id_produto", parameter);
                if (dados.Rows.Count != 0)
                {
                    //foi encontrado um produto com o mesmo nome
                    AlertDialog.Builder caixa = new AlertDialog.Builder(this);
                    caixa.SetTitle("ERRO!");
                    caixa.SetMessage("Já existe um produto com o mesmo nome!");
                    caixa.SetPositiveButton("OK", delegate { });
                    caixa.Show();
                    return;
                }

                Cl_gestor.EXE_NON_QUERY(
                    "UPDATE produtos SET " +
                    "nm_produto = @nm_produto, " +
                    "preco_produto = @preco_produto, " +
                    "atualizacao = @atualizacao " +
                    "WHERE id_produto = @id_produto ", parameter);

                //encerrar atividade
                Intent i = this.Intent;
                SetResult(Result.Ok, i);
                Finish();
            }
        }
    }
}