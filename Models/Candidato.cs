using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace APIRh.Models
{
    /// <summary>
    /// Classe que identifica o candidato, a ser cadastrado na etapa da entrevista
    /// </summary>
    public class Candidato
    {
        /// <summary>
        /// Identificador do candidato (gerado automaticamente)
        /// </summary>
        public int Id { get; set; }
        public string Nome { get; set; }
        /// <summary>
        /// Vaga que o candidato está atrelado
        /// </summary>
        public Vaga Vaga { get; set; }
        /// <summary>
        /// Tecnologias que o candidato tem conhecimento
        /// </summary>
        public List<Tecnologia> Tecnologias { get; set; }
        /// <summary>
        /// Nota do candidato ao final do processo de avaliação
        /// </summary>
        public int Nota { get; set; }

        public Candidato() { }

        public Candidato(int id, string nome, Vaga vaga, List<Tecnologia> tecnologias)
        {
            Id = id;
            Nome = nome;
            Vaga = vaga;
            Tecnologias = tecnologias;
        }

        //construtor utilizado na listagem dos resultados da vaga
        public Candidato(int id, string nome, int nota)
        {
            Id = id;
            Nome = nome;
            Nota = nota;
        }

        private readonly static string _conn = WebConfigurationManager.ConnectionStrings["_conn"].ConnectionString;

        public static List<Candidato> GetCandidatos()
        {
            var listaCandidato = new List<Candidato>();
            var sql = "SELECT Candidato.id as id_candidato, Candidato.nome as candidato, id_vaga, Vaga.nome as vaga " +
                " FROM Candidato, Vaga WHERE Candidato.id_vaga = Vaga.id";

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

                                    //busca as tecnologias que o candidado tem conhecimento, para preencher os dados do objeto
                                    List<Tecnologia> tecnologias = CandidatoTecnologia.GetCandidatoTecnologias(Convert.ToInt32(resultado["id_candidato"]));

                                    listaCandidato.Add(new Candidato(
                                        Convert.ToInt32(resultado["id_candidato"]),
                                        resultado["candidato"].ToString(),
                                        vaga,
                                        tecnologias
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

            Console.WriteLine(listaCandidato);
            return listaCandidato;
        }

        public string Salvar(Candidato candidato)
        {
            var sql = "";

            if (candidato.Id == 0)
            {
                sql = "INSERT INTO Candidato (nome, id_vaga)" +
                    " VALUES (@nome, @id_vaga)";
            }
            else
            {
                sql = "UPDATE Candidato SET nome = @nome, id_vaga = @id_vaga" +
                    " WHERE id=" + candidato.Id;
            }

            try
            {
                using (var con = new SqlConnection(_conn))
                {
                    con.Open();
                    using (var comando = new SqlCommand(sql, con))
                    {
                        comando.Parameters.AddWithValue("@nome", candidato.Nome);
                        comando.Parameters.AddWithValue("@id_vaga", candidato.Vaga.Id);

                        comando.ExecuteNonQuery();
                    }
                }

                //busca o id do novo candidato se for um insert
                if (candidato.Id == 0) 
                    candidato.Id = candidato.GetId(candidato.Nome);

                //busca as antigas tecnologias cadastradas para o candidato
                List<Tecnologia> tecnologias = CandidatoTecnologia.GetCandidatoTecnologias(candidato.Id);

                //verifica se as tecnologias antigas foram retiradas do objeto atual. Caso sejam, elas são excluídas do banco.
                foreach (var tecnologia in tecnologias)
                {
                    if (!candidato.Tecnologias.Contains(tecnologia))
                    {
                        CandidatoTecnologia.ExcluirAoAtualizar(candidato.Id, tecnologia.Id);
                    }
                    else
                    {
                        candidato.Tecnologias.Remove(tecnologia);
                    }
                }

                //analisa todas as tecnologias indicadas no body do candidato e cria uma nova relação Candidato x Tecnologia
                //para cada uma das tecnologias informadas, caso a relação não exista
                foreach (var tecnologia in candidato.Tecnologias)
                {
                    CandidatoTecnologia candidatoTecnologia = new CandidatoTecnologia();
                    candidatoTecnologia.Candidato = candidato;
                    candidatoTecnologia.Tecnologia = tecnologia;
                    candidatoTecnologia.Salvar(candidatoTecnologia);
                }
                
                return "ok";
            }
            catch (Exception ex)
            {
                return "Erro: " + ex.Message;
            }
        }

        public int GetId(String nome)
        {   
            //busca o id de um candidato a partir do nome
            var sql = "SELECT id FROM Candidato WHERE nome = '" + nome + "'";

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
                                    Id = Convert.ToInt16(resultado["id"]);
                                }
                            }
                        }
                    }
                }
                return Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro: " + ex.Message);
                return Id;
            }
        }

        public string Excluir(int id)
        {
            var sql = "DELETE FROM Candidato WHERE id = " + id;

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

        public static string ExcluirPorVaga(int id)
        {
            List<Candidato> candidatos = GetCandidatosPorVaga(id);

            foreach(var candidato in candidatos)
            {
                CandidatoTecnologia.ExcluirPorCandidato(candidato.Id);
            }

            var sql = "DELETE FROM Candidato WHERE id_vaga = " + id;

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

        public static List<Candidato> GetCandidatosPorVaga(int id)
        {
            var listaCandidato = new List<Candidato>();
            var sql = "SELECT Candidato.id as id_candidato, Candidato.nome as candidato, id_vaga, Vaga.nome as vaga " +
                " FROM Candidato, Vaga WHERE Candidato.id_vaga = Vaga.id AND Vaga.id = " + id;

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
                                    Candidato candidato = new Candidato();
                                    candidato.Id = Convert.ToInt32(resultado["id_candidato"]);

                                    listaCandidato.Add(candidato);
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

            Console.WriteLine(listaCandidato);
            return listaCandidato;
        }

        public void GetCandidato(int id)
        {
            var sql = "SELECT Candidato.id as id_candidato, Candidato.nome as candidato, id_vaga, Vaga.nome as vaga" +
                " FROM Candidato, Vaga WHERE Candidato.id_vaga = Vaga.id AND Candidato.id = " + id;

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
                                    Nome = resultado["candidato"].ToString();

                                    Vaga = new Vaga();
                                    Vaga.Nome = resultado["vaga"].ToString();
                                    Vaga.Id = Convert.ToInt16(resultado["id_vaga"]);

                                    CandidatoTecnologia candidatoTecnologia = new CandidatoTecnologia();
                                    Tecnologias = CandidatoTecnologia.GetCandidatoTecnologias(id);
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