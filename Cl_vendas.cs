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
    class Cl_vendas
    {
        public string nm_cliente { get; set; }
        public string nm_produto { get; set; }
        public int preco_produto { get; set; }
        public int quantidade { get; set; }
        public int preco_total { get; set; }
    }
}