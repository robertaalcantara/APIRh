using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace APIRh.Models
{
    public class Tecnologia
    {
        /// <summary>
        /// Identificador da tecnologia
        /// </summary>
        public int Id { get; set; }
        public string Nome { get; set; }
        /// <summary>
        /// Peso que a tecnologia tem em uma vaga (cadastrado apenas na etapa 3)
        /// </summary>
        public int Peso { get; set; }

        public Tecnologia() { }

        public Tecnologia(int id, string nome)
        {
            Id = id;
            Nome = nome;
        }

        private readonly static string _conn = WebConfigurationManager.ConnectionStrings["_conn"].ConnectionString;

        public static List<Tecnologia> GetTecnologias()
        {
            var listaTecnologias = new List<Tecnologia>();
            var sql = "SELECT * FROM Tecnologia";

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
                                    listaTecnologias.Add(new Tecnologia(
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

            return listaTecnologias;
        }

        public string Salvar(Tecnologia tecnologia)
        {
            var sql = "";

            if (tecnologia.Id == 0)
            {
                sql = "INSERT INTO Tecnologia (nome) VALUES (@nome)";
            }
            else
            {
                sql = "UPDATE Tecnologia SET nome = @nome WHERE id=" + tecnologia.Id;
            }

            try
            {
                using (var con = new SqlConnection(_conn))
                {
                    con.Open();
                    using (var comando = new SqlCommand(sql, con))
                    {
                        comando.Parameters.AddWithValue("@nome", tecnologia.Nome);

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
            var sql = "DELETE FROM Tecnologia WHERE id = " + id;

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

        public void GetTecnologia(int id)
        {
            var sql = "SELECT * FROM Tecnologia WHERE id = " + id;

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