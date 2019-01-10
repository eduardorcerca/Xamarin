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
using Mono.Data.Sqlite;

namespace App1
{
    public class SQLparametro
    {
        public string nome { get; set; }
        public object valor { get; set; }

        public SQLparametro(string parametro, object valor)
        {
            this.nome = parametro;
            this.valor = valor;
        }
    }
}