using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace APIRh.Models
{
    public class Vaga
    {
        /// <summary>
        /// Identificador da vaga
        /// </summary>
        public int Id { get; set; }
        public string Nome { get; set; }

        public Vaga() { }

        public Vaga(int id, string nome)
        {
            Id = id;
            Nome = nome;
        }

        private readonly static string _conn = WebConfigurationManager.ConnectionStrings["_conn"].ConnectionString;
        public static List<Vaga> GetVagas()
        {
            var listaVagas = new List<Vaga>();
            var sql = "SELECT * FROM Vaga";

            try
            {
                using (var con = new SqlConnection(_conn))
                {
                    con.Open();

                    using (var comando = new SqlCommand(sql, con))
                    {
                        using (var resultado = comando.ExecuteReader())
                        {
                            if (resultado.HasRows)
                            {
                                while (resultado.Read())
                                {
                                    listaVagas.Add(new Vaga(
                                        Convert.ToInt32(resultado["id"]),
                                        resultado["nome"].ToString()
                                        ));
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro: " + ex.Message);
            }

            return listaVagas;
        }

        public string Salvar(Vaga vaga)
        {
            var sql = "";

            if (vaga.Id == 0)
            {
                sql = "INSERT INTO Vaga (nome) VALUES (@nome)";
            }
            else
            {
                sql = "UPDATE Vaga SET nome = @nome WHERE id=" + vaga.Id;
            }

            try
            {
                using (var con = new SqlConnection(_conn))
                {
                    con.Open();
                    using (var comando = new SqlCommand(sql, con))
                    {
                        comando.Parameters.AddWithValue("@nome", vaga.Nome);

                        comando.ExecuteNonQuery();
                    }
                }
                return "ok";
            }
            catch (Exception ex)
            {
                return "Erro: " + ex.Message;
            }
        }

        public string Excluir(int id)
        {
            var sql = "DELETE FROM Vaga WHERE id = " + id;

            try
            {
                using (var con = new SqlConnection(_conn))
                {
                    con.Open();
                    using (var comando = new SqlCommand(sql, con))
                    {
                        comando.ExecuteNonQuery();
                    }
                }
                return "ok";
            }
            catch (Exception ex)
            {
                return "Erro: " + ex.Message;
            }
        }

        public void GetVaga(int id)
        {
            var sql = "SELECT * FROM Vaga WHERE id = " + id;

            try
            {
                using (var con = new SqlConnection(_conn))
                {
                    con.Open();
                    using (var comando = new SqlCommand(sql, con))
                    {
                        using (var resultado = comando.ExecuteReader())
                        {
                            if (resultado.HasRows)
                            {
                                if (resultado.Read())
                                {
                                    Id = id;
                                    Nome = resultado["nome"].ToString();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro: " + ex.Message);
            }
        }
    }
}