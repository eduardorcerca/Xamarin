using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
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
    public static class Cl_gestor
    {
        static string pasta_dados;
        static string base_dados;

        public static void inicioAplicacao()
        {
            //criando a pasta da aplicacao
            pasta_dados = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "Vendas");

            if (!Directory.Exists(pasta_dados))
                Directory.CreateDirectory(pasta_dados);

            //define o caminho pro banco
            base_dados = pasta_dados + @"/dados.db";

            //criando o banco da aplicacao
            if (!File.Exists(base_dados))
            {
                //cria a pasta
                SqliteConnection.CreateFile(base_dados);

                //cria a estrutura
                SqliteConnection ligacao = new SqliteConnection("Data Source = " + base_dados);
                ligacao.Open();

                SqliteCommand comando = new SqliteCommand();
                comando.Connection = ligacao;

                //criando a tabela clientes
                comando.CommandText =
                    "CREATE TABLE clientes(" +
                    "id_cliente            INTEGER NOT NULL PRIMARY KEY, " +
                    "nm_cliente            NVARCHAR(50), " +
                    "telefone              NVARCHAR(15), " +
                    "atualizacao           DATETIME)";
                comando.ExecuteNonQuery();

                //criando a tabela produtos
                comando.CommandText =
                    "CREATE TABLE produtos(" +
                    "id_produto            INTEGER NOT NULL PRIMARY KEY, " +
                    "nm_produto            NVARCHAR(30), " +
                    "preco_produto         INTEGER, " +
                    "atualizacao           DATETIME)";
                comando.ExecuteNonQuery();

                //criando a tabela vendas
                comando.CommandText =
                    "CREATE TABLE vendas(" +
                    "id_venda              INTEGER NOT NULL PRIMARY KEY, " +
                    "id_cliente            INTEGER, " +
                    "id_produto            INTEGER, " +
                    "quantidade            INTEGER, " +
                    "atualizacao           DATETIME, " +
                    "FOREIGN KEY(id_cliente) REFERENCES clientes(id_cliente) ON DELETE CASCADE, " +
                    "FOREIGN KEY(id_produto) REFERENCES produtos(id_produto) ON DELETE CASCADE)";
                comando.ExecuteNonQuery();

                ligacao.Close();
                ligacao.Dispose();
            }
        }

        public static void EXE_NON_QUERY(string query, List<SQLparametro> parametros)
        {
            //executa inserção, atualização ou eliminação de dados no banco
            SqliteConnection ligacao = new SqliteConnection("Data Source = " + base_dados);
            ligacao.Open();

            SqliteCommand comando = new SqliteCommand(query, ligacao);

            //adicionar os parametros ao comando
            foreach (SQLparametro parametro in parametros)
            {
                comando.Parameters.Add(new SqliteParameter(parametro.nome, parametro.valor));
            }

            //comunicacao com o banco
            comando.ExecuteNonQuery();

            comando.Dispose();
            ligacao.Close();
            ligacao.Dispose();
        }

        public static void EXE_NON_QUERY(string query)
        {
            //executa inserção, atualização ou eliminação de dados no banco
            SqliteConnection ligacao = new SqliteConnection("Data Source = " + base_dados);
            ligacao.Open();

            SqliteCommand comando = new SqliteCommand(query, ligacao);

            //comunicacao com o banco
            comando.ExecuteNonQuery();

            comando.Dispose();
            ligacao.Close();
            ligacao.Dispose();
        }

        public static DataTable EXE_QUERY(string query, List<SQLparametro> parametros)
        {
            //executa a leitura de dados do banco
            SqliteConnection ligacao = new SqliteConnection("Data Source = " + base_dados);
            ligacao.Open();

            SqliteDataAdapter adaptador = new SqliteDataAdapter(query, ligacao);

            //adicionar os parametros ao comando
            foreach (SQLparametro parametro in parametros)
            {
                adaptador.SelectCommand.Parameters.Add(new SqliteParameter(parametro.nome, parametro.valor));
            }

            DataTable dados = new DataTable();

            adaptador.Fill(dados);

            return dados;
        }

        public static DataTable EXE_QUERY(string query)
        {
            //executa a leitura de dados do banco
            SqliteConnection ligacao = new SqliteConnection("Data Source = " + base_dados);
            ligacao.Open();

            SqliteDataAdapter adaptador = new SqliteDataAdapter(query, ligacao);

            DataTable dados = new DataTable();

            adaptador.Fill(dados);

            return dados;
        }

        public static int ID_DISPONIVEL(string tabela, string coluna)
        {
            //devolver o id_xxxx disponivel
            int valor = 0;
            
            SqliteConnection ligacao = new SqliteConnection("Data Source = " + base_dados);
            ligacao.Open();

            //define query
            string query = "SELECT MAX(" + coluna + ") AS MaxID FROM " + tabela;

            DataTable dados = new DataTable();
            SqliteDataAdapter adaptador = new SqliteDataAdapter(query, ligacao);
            adaptador.Fill(dados);

            //determinar o 'valor'
            if (dados.Rows.Count != 0)
            {
                //verifica se o resultado não é nulo (NULL)
                if (!DBNull.Value.Equals(dados.Rows[0][0]))
                {
                    valor = Convert.ToInt32(dados.Rows[0][0]) + 1;
                }
            }

            ligacao.Close();
            ligacao.Dispose();
            return valor;
        }

        public static void LIMPAR_TABELA(string tabela)
        {
            EXE_NON_QUERY("DELETE FROM " + tabela);
        }
    }
}