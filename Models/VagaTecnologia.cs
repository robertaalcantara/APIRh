using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace APIRh.Models
{
    /// <summary>
    /// Classe referente a etapa final do processo de seleção
    /// </summary>
    public class VagaTecnologia
    {
        /// <summary>
        /// Identificador da relação Vaga x Tecnologia
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Objeto do tipo Vaga
        /// </summary>
        public Vaga Vaga { get; set; }
        /// <summary>
        /// Objeto do tipo Tecnologia
        /// </summary>
        public List<Tecnologia> Tecnologias { get; set; }

        public VagaTecnologia() { }

        public VagaTecnologia(int id, Vaga vaga, List<Tecnologia> tecnologia)
        {
            Id = id;
            Vaga = vaga;
            Tecnologias = tecnologia;
        }

        private readonly static string _conn = WebConfigurationManager.ConnectionStrings["_conn"].ConnectionString;

        public string Salvar(VagaTecnologia vagaTecnologia)
        {
            foreach(var tecnologia in vagaTecnologia.Tecnologias)
            {
                var sql = "";

                if (vagaTecnologia.Id == 0)
                {
                    //salva somente se não existir o par Vaga x Tecnologia presente em vagaTecnologia
                    sql = "IF NOT EXISTS(SELECT id FROM VagaTecnologia" +
                        " WHERE VagaTecnologia.id_vaga = @id_vaga" +
                        " AND VagaTecnologia.id_tecnologia = @id_tecnologia)" +
                        " BEGIN" +
                        " INSERT INTO VagaTecnologia (id_vaga, id_tecnologia, peso)" +
                        " VALUES (@id_vaga, @id_tecnologia, @peso)" +
                        " END";
                }
                else
                {
                    sql = "UPDATE VagaTecnologia SET id_vaga = @id_vaga, id_tecnologia = @id_tecnologia, " +
                        " peso = @peso WHERE id = " + vagaTecnologia.Id;
                }

                try
                {
                    using (var con = new SqlConnection(_conn))
                    {
                        con.Open();
                        using (var comando = new SqlCommand(sql, con))
                        {
                            comando.Parameters.AddWithValue("@id_vaga", vagaTecnologia.Vaga.Id);
                            comando.Parameters.AddWithValue("@id_tecnologia", tecnologia.Id);
                            comando.Parameters.AddWithValue("@peso", tecnologia.Peso);

                            comando.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    return "Erro: " + ex.Message;
                }
            }
            return "ok";
        }

        public string ExcluirPorVaga(int id)
        {
            var sql = "DELETE FROM VagaTecnologia WHERE id_vaga = " + id;

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
            var sql = "DELETE FROM VagaTecnologia WHERE id_tecnologia = " + id;

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

        public static string ExcluirAoAtualizar(int id_vaga, int id_tecnologia)
        {
            var sql = "DELETE FROM VagaTecnologia WHERE id_vaga = " + id_vaga +
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

        public static List<Tecnologia> GetVagaTecnologias(int id)
        {
            var listaTecnologias = new List<Tecnologia>();
            //busca as tecnologias que fazem parte da vaga identificada pelo id
            var sql = "SELECT Tecnologia.id AS id_tecnologia, Tecnologia.nome AS tecnologia, VagaTecnologia.peso" +
                " FROM Tecnologia, VagaTecnologia" +
                " WHERE VagaTecnologia.id_tecnologia = Tecnologia.id" +
                " AND VagaTecnologia.id_vaga = " + id;

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

        public static List<Candidato> GetVagaCandidatos(int id)
        {
            var listaCandidatos = new List<Candidato>();
            /*lista os candidatos atrelados a vaga identificada pelo id. Se houver tecnologias em comum entre o candidato 
              e a vaga, a nota corresponde ao somatório do peso das tecnologias em comum. Caso contrário, nota = 0 */

            var sql = "SELECT Candidato.id, Candidato.nome, ISNULL(SUM(VagaTecnologia.peso), 0) AS nota" +
                " FROM Candidato" +
                " LEFT JOIN CandidatoTecnologia" +
                " ON Candidato.id = CandidatoTecnologia.id_candidato" +
                " LEFT JOIN VagaTecnologia" +
                " ON VagaTecnologia.id_tecnologia = CandidatoTecnologia.id_tecnologia" +
                " WHERE VagaTecnologia.id_vaga = " + id +
                " AND Candidato.id_vaga = VagaTecnologia.id_vaga" +
                " OR(CandidatoTecnologia.id_candidato IS NULL AND Candidato.id_vaga = "+ id +")" +
                " GROUP BY Candidato.id, Candidato.nome" +
                " ORDER BY nota DESC";

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
                                    Candidato candidato = new Candidato(
                                        Convert.ToInt32(resultado["id"]),
                                        resultado["nome"].ToString(),
                                        Convert.ToInt32(resultado["nota"])
                                        );

                                    listaCandidatos.Add(candidato);
                                }
                            }
                        }
                    }
                }
                return listaCandidatos;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro: " + ex.Message);
                return listaCandidatos;
            }
        }
    }
}