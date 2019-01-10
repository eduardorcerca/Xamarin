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
    [Activity(Label = "At_cliente_editar")]
    public class At_cliente_editar : Activity
    {
        Button botao_gravar_cliente;
        Button botao_cancelar_cliente;
        EditText edit_nome_cliente;
        EditText edit_telefone_cliente;

        int id_cliente = 0;
        bool editar = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.layout_clientes_editar);

            //widgets
            botao_gravar_cliente = FindViewById<Button>(Resource.Id.botao_gravar_cliente);
            botao_cancelar_cliente = FindViewById<Button>(Resource.Id.botao_cancelar_cliente);
            edit_nome_cliente = FindViewById<EditText>(Resource.Id.edit_nome_cliente);
            edit_telefone_cliente = FindViewById<EditText>(Resource.Id.edit_telefone_cliente);

            //eventos
            botao_cancelar_cliente.Click += delegate { this.Finish(); };
            botao_gravar_cliente.Click += Botao_gravar_cliente_Click;

            //verificar se existe a variavel 'id_cliente' no intent
            if (this.Intent.GetStringExtra("id_cliente") != null)
            {
                id_cliente = int.Parse(this.Intent.GetStringExtra("id_cliente"));
                ApresentaCliente();
                editar = true;
            }
        }

        private void ApresentaCliente()
        {
            //colocar as infos no edit_text
            DataTable dados = Cl_gestor.EXE_QUERY("SELECT * FROM clientes WHERE id_cliente =" + id_cliente);

            edit_nome_cliente.Text = dados.Rows[0]["nm_cliente"].ToString();
            edit_telefone_cliente.Text = dados.Rows[0]["telefone"].ToString();
        }

        private void Botao_gravar_cliente_Click(object sender, EventArgs e)
        {
            //edita ou grava dados de um cliente

            //verificar se os campos estão preenchidos
            if (edit_nome_cliente.Text == "" || edit_telefone_cliente.Text == "")
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
                parameter.Add(new SQLparametro("@id_cliente", Cl_gestor.ID_DISPONIVEL("clientes", "id_cliente")));
            else
                parameter.Add(new SQLparametro("@id_cliente", id_cliente));
            parameter.Add(new SQLparametro("@nm_cliente", edit_nome_cliente.Text));
            parameter.Add(new SQLparametro("@telefone", edit_telefone_cliente.Text));
            parameter.Add(new SQLparametro("@atualizacao", DateTime.Now));

            if (!editar)
            {
                //gravar novo cliente
                DataTable dados = Cl_gestor.EXE_QUERY("SELECT nm_cliente FROM clientes WHERE nm_cliente = @nm_cliente", parameter);
                if (dados.Rows.Count != 0)
                {
                    //verifica se foi encontrado um cliente com o mesmo nome
                    AlertDialog.Builder caixa = new AlertDialog.Builder(this);
                    caixa.SetTitle("ERRO!");
                    caixa.SetMessage("Já existe um cliente com o mesmo nome!");
                    caixa.SetPositiveButton("OK", delegate { });
                    caixa.Show();
                    return;
                }

                Cl_gestor.EXE_NON_QUERY(
                    "INSERT INTO clientes VALUES(" +
                    "@id_cliente, " +
                    "@nm_cliente, " +
                    "@telefone, " +
                    "@atualizacao)", parameter);

                //encerrar atividade
                Intent i = this.Intent;
                SetResult(Result.Ok, i);
                Finish();
            }

            else
            {
                //verifica se existe um cliente com o mesmo nome
                DataTable dados = Cl_gestor.EXE_QUERY("SELECT nm_cliente FROM clientes WHERE nm_cliente = @nm_cliente AND id_cliente <> @id_cliente", parameter);
                if (dados.Rows.Count != 0)
                {
                    //foi encontrado um cliente com o mesmo nome
                    AlertDialog.Builder caixa = new AlertDialog.Builder(this);
                    caixa.SetTitle("ERRO!");
                    caixa.SetMessage("Já existe um cliente com o mesmo nome!");
                    caixa.SetPositiveButton("OK", delegate { });
                    caixa.Show();
                    return;
                }

                Cl_gestor.EXE_NON_QUERY(
                    "UPDATE clientes SET " +
                    "nm_cliente = @nm_cliente, " +
                    "telefone = @telefone, " +
                    "atualizacao = @atualizacao " +
                    "WHERE id_cliente = @id_cliente ", parameter);

                //encerrar atividade
                Intent i = this.Intent;
                SetResult(Result.Ok, i);
                Finish();
            }

        }
    }
}