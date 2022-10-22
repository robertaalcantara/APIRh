using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace APIRh.Models
{
    public class CandidatoTecnologia
    {
        /// <summary>
        /// Identificador da relação Candidato x Tecnologia
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Objeto do tipo Candidato
        /// </summary>
        public Candidato Candidato { get; set; }
        /// <summary>
        /// Objeto do tipo Tecnologia
        /// </summary>
        public Tecnologia Tecnologia { get; set; } 

        public CandidatoTecnologia() { }

        public CandidatoTecnologia(int id, Candidato candidato, Tecnologia tecnologia)
        {
            Id = id;
            Candidato = candidato;
            Tecnologia = tecnologia;
        }

        private readonly static string _conn = WebConfigurationManager.ConnectionStrings["_conn"].ConnectionString;
        public static List<CandidatoTecnologia> GetCandidatosTecnologias(int v)
        {
            var listaCandidatosTecnologias = new List<CandidatoTecnologia>();
            var sql = "SELECT CandidatoTecnologia.id, Candidato.nome AS candidato, Candidato.Id AS candidato_id," +
                " Tecnologia.Nome AS tecnologia, Tecnologia.Id AS tecnologia_id, Vaga.Id AS vaga_id, Vaga.nome AS vaga" +
                " FROM CandidatoTecnologia, Candidato, Tecnologia, Vaga" +
                " WHERE CandidatoTecnologia.id_candidato = Candidato.id" +
                " AND CandidatoTecnologia.id_tecnologia = Tecnologia.id AND Candidato.id_vaga = Vaga.Id";

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
                                    Vaga vaga = new Vaga(Convert.ToInt32(resultado["id_vaga"]),
                                        resultado["vaga"].ToString());
                                    Tecnologia tecnologia = new Tecnologia(Convert.ToInt32(resultado["id_tecnologia"]),
                                        resultado["tecnologia"].ToString());

                                    List<Tecnologia> listaTecnologias = GetCandidatoTecnologias(Convert.ToInt32(resultado["id_candidato"]));

                                    Candidato candidato = new Candidato(Convert.ToInt32(resultado["id_candidato"]),
                                        resultado["candidato"].ToString(), vaga, listaTecnologias);

                                    listaCandidatosTecnologias.Add(new CandidatoTecnologia(
                                        Convert.ToInt32(resultado["id"]), candidato, tecnologia
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

            Console.WriteLine(listaCandidatosTecnologias);
            return listaCandidatosTecnologias;
        }

        public string Salvar(CandidatoTecnologia candidatoTecnologia)
        {
            var sql = "";

            if (candidatoTecnologia.Id == 0)
            {
                sql = "IF NOT EXISTS(SELECT id FROM CandidatoTecnologia" +
                    " WHERE CandidatoTecnologia.id_candidato = @id_candidato" +
                    " AND CandidatoTecnologia.id_tecnologia = @id_tecnologia)" +
                    " BEGIN" +
                    " INSERT INTO CandidatoTecnologia (id_candidato, id_tecnologia)" +
                    " VALUES (@id_candidato, @id_tecnologia)" +
                    " END";
            }
            else
            {
                sql = "UPDATE CandidatoTecnologia SET id_candidato = @id_candidato, id_tecnologia = @id_tecnologia" +
                    " WHERE id=" + candidatoTecnologia.Id;
            }

            try
            {
                using (var con = new SqlConnection(_conn))
                {
                    con.Open();
                    using (var comando = new SqlCommand(sql, con))
                    {
                        comando.Parameters.AddWithValue("@id_candidato", candidatoTecnologia.Candidato.Id);
                        comando.Parameters.AddWithValue("@id_tecnologia", candidatoTecnologia.Tecnologia.Id);

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

        public static string ExcluirPorCandidato(int id)
        {
            var sql = "DELETE FROM CandidatoTecnologia WHERE id_candidato = " + id;

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

        public string ExcluirPorTecnologia(int id)
        {
            var sql = "DELETE FROM CandidatoTecnologia WHERE id_tecnologia = " + id;

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

        public static string ExcluirAoAtualizar(int id_candidato, int id_tecnologia)
        {
            var sql = "DELETE FROM CandidatoTecnologia WHERE id_candidato = " + id_candidato +
                " AND id_tecnologia = " + id_tecnologia;

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

        public static List<Tecnologia> GetCandidatoTecnologias(int id)
        {
            var listaTecnologias = new List<Tecnologia>();
            var sql = "SELECT Tecnologia.nome AS tecnologia, Tecnologia.id AS id_tecnologia" +
                " FROM Tecnologia, CandidatoTecnologia, Candidato" +
                " WHERE CandidatoTecnologia.id_candidato = Candidato.id AND CandidatoTecnologia.id_tecnologia = Tecnologia.id" +
                " AND Candidato.id = " + id;

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
                                    Tecnologia tecnologia = new Tecnologia(Convert.ToInt32(resultado["id_tecnologia"]),
                                        resultado["tecnologia"].ToString());

                                    listaTecnologias.Add(tecnologia);
                                }
                            }
                        }
                    }
                }
                return listaTecnologias;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro: " + ex.Message);
                return listaTecnologias;
            }
        }
    }
}