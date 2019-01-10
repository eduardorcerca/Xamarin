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
    [Activity(Label = "At_clientes")]
    public class At_clientes : Activity
    {
        Button botao_adicionar_cliente;
        ListView lista_clientes;
        TextView label_numero_clientes;
        List<Cl_clientes> CLIENTES;
        List<string> NOMES;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.layout_clientes);

            botao_adicionar_cliente = FindViewById<Button>(Resource.Id.botao_adicionar_cliente);
            lista_clientes = FindViewById<ListView>(Resource.Id.lista_clientes);
            label_numero_clientes = FindViewById<TextView>(Resource.Id.label_numero_clientes);

            //construir a lista de clientes
            ConstroiListaClientes();

            //eventos
            botao_adicionar_cliente.Click += Botao_adicionar_cliente_Click;
            lista_clientes.ItemLongClick += Lista_clientes_ItemLongClick;
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            //quando voltar apos editar ou adicionar cliente
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                ConstroiListaClientes();
            }
        }

        private void Lista_clientes_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            //seleciona cliente para excluir ou alterar dados
            Cl_clientes cliente_selecionado = CLIENTES[e.Position];

            //caixa de mensagem com as opções
            AlertDialog.Builder caixa_editar_eliminar = new AlertDialog.Builder(this);
            caixa_editar_eliminar.SetTitle("EDITAR | ELIMINAR");
            caixa_editar_eliminar.SetMessage(cliente_selecionado.nm_cliente);

            //editar cliente
            caixa_editar_eliminar.SetPositiveButton("EDITAR", delegate { EditarCliente(cliente_selecionado.id_cliente); });

            //eliminar cliente
            caixa_editar_eliminar.SetNegativeButton("ELIMINAR", delegate { EliminarCliente(cliente_selecionado.id_cliente); });

            caixa_editar_eliminar.Show();
        }

        private void EliminarCliente(int id_cliente)
        {
            Cl_gestor.EXE_NON_QUERY("DELETE FROM clientes WHERE id_cliente = " + id_cliente);

            //atualiza a lista de cliente do banco
            ConstroiListaClientes();
        }

        private void EditarCliente(int id_cliente)
        {
            //abrir atividade para editar os dados dos clientes
            Intent i = new Intent(this, typeof(At_cliente_editar));
            i.PutExtra("id_cliente", id_cliente.ToString());
            StartActivityForResult(i, 0);
        }

        private void Botao_adicionar_cliente_Click(object sender, EventArgs e)
        {
            //abre a tela adicionar cliente
            Intent i = new Intent(this, typeof(At_cliente_editar));
            StartActivityForResult(i, 0);
        }

        private void ConstroiListaClientes()
        {
            //construir a lista de clientes
            CLIENTES = new List<Cl_clientes>();

            DataTable dados = Cl_gestor.EXE_QUERY("SELECT * FROM clientes ORDER BY nm_cliente ASC");

            foreach (DataRow linha in dados.Rows)
            {
                CLIENTES.Add(new Cl_clientes()
                {
                    id_cliente = Convert.ToInt32(linha["id_cliente"]),
                    nm_cliente = linha["nm_cliente"].ToString(),
                    telefone = linha["telefone"].ToString()
                });
            }

            NOMES = new List<string>();

            foreach (Cl_clientes cliente in CLIENTES)
            {
                NOMES.Add(cliente.nm_cliente);
            }

            ArrayAdapter<string> adaptador = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, NOMES);
            lista_clientes.Adapter = adaptador;

            //atualizar o numero de clientes total
            ApresentaTotalCliente(CLIENTES.Count);
        }

        private void ApresentaTotalCliente(int total_clientes)
        {
            //apresenta o total de clientes registrados
            label_numero_clientes.Text = "Clientes registrados: " + total_clientes;
        }
    }
}