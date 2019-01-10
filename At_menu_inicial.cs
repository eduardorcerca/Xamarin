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
    [Activity(Label = "At_menu_inicial", MainLauncher = true, Icon = "@drawable/Icon")]
    public class At_menu_inicial : Activity
    {
        Button botao_clientes;
        Button botao_produtos;
        Button botao_vendas;
        Button botao_estatisticas;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(savedInstanceState);

            //iniciar banco
            Cl_gestor.inicioAplicacao();

            SetContentView(Resource.Layout.layout_menu_inicial);

            //buscar botoes
            botao_clientes = FindViewById<Button>(Resource.Id.botao_clientes);
            botao_produtos = FindViewById<Button>(Resource.Id.botao_produtos);
            botao_vendas = FindViewById<Button>(Resource.Id.botao_vendas);
            botao_estatisticas = FindViewById<Button>(Resource.Id.botao_estatisticas);

            //eventos botoes
            botao_estatisticas.Click += Botao_estatisticas_Click;
            botao_vendas.Click += Botao_vendas_Click;
            botao_produtos.Click += Botao_produtos_Click;
            botao_clientes.Click += Botao_clientes_Click;

        }

        private void Botao_clientes_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(At_clientes));
        }

        private void Botao_produtos_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(At_produtos));
        }

        private void Botao_vendas_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(At_vendas));
        }

        private void Botao_estatisticas_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(At_estatistica));
        }
    }
}