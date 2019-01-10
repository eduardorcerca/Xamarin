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

namespace App1
{
    class Adaptador_vendas : BaseAdapter<Cl_vendas>
    {
        private List<Cl_vendas> VENDAS;
        private Context contexto;

        public Adaptador_vendas(Context contexto, List<Cl_vendas> vendas)
        {
            this.VENDAS = vendas;
            this.contexto = contexto;
        }

        public override Cl_vendas this[int position]
        {
            get { return VENDAS[position]; }
        }

        public override int Count
        {
            get { return VENDAS.Count(); }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            //implementar visualizacao da linha
            View linha = convertView;

            if (linha == null)
            {
                linha = LayoutInflater.From(contexto).Inflate(Resource.Layout.lst_vendas, null, false);
            }

            TextView nome_cliente = linha.FindViewById<TextView>(Resource.Id.lista_nome_cliente);
            TextView nome_produto = linha.FindViewById<TextView>(Resource.Id.lista_nome_produto);
            TextView preco = linha.FindViewById<TextView>(Resource.Id.lista_preco);
            TextView quantidade = linha.FindViewById<TextView>(Resource.Id.lista_quantidade);
            TextView total = linha.FindViewById<TextView>(Resource.Id.lista_total);

            nome_cliente.Text = VENDAS[position].nm_cliente;
            nome_produto.Text = VENDAS[position].nm_produto;
            preco.Text = VENDAS[position].preco_produto.ToString();
            quantidade.Text = VENDAS[position].quantidade.ToString();
            total.Text = VENDAS[position].preco_total.ToString();

            return linha;
        }
    }
}