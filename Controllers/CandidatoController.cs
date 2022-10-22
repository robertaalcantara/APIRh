using APIRh.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace APIRh.Controllers
{
    /// <summary>
    /// Classe referente a etapa de entrevista
    /// </summary>
    public class CandidatoController : ApiController
    {
        /// <summary>
        /// Lista os candidatos cadastrados no banco
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/candidato")]
        public List<Candidato> Get()
        {
            var listaCandidatos = Candidato.GetCandidatos();

            return listaCandidatos;
        }

        /// <summary>
        /// Busca um candidato, de acordo com seu id que deve ser informado na URL
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Candidato Get(int id)
        {
            Candidato candidato = new Candidato();
            candidato.GetCandidato(id);

            return candidato;
        }

        /// <summary>
        /// Cadastro de um candidato no banco de dados, cujos dados devem constar no Body
        /// (nome do candidato e id da vaga são campos obrigatórios)
        /// </summary>
        /// <param name="candidato"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/candidato")]
        public JObject Post([FromBody] Candidato candidato)
        {
            var resultado = "";

            resultado = validarDados(candidato);

            if (String.IsNullOrEmpty(resultado))
            {
                resultado = candidato.Salvar(candidato);
                resultado = "{resultado: \"" + resultado + "\"}";
            }

            return JObject.Parse(resultado);
        }

        /// <summary>
        /// Edita os dados de um candidato, de acordo com seu id que deve ser informado na URL e dos dados no Body.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="candidato"></param>
        /// <returns></returns>
        public JObject Put(int id, [FromBody] Candidato candidato)
        {
            var resultado = "";
            candidato.Id = id;
            resultado = candidato.Salvar(candidato);

            resultado = "{resultado: \"" + resultado + "\"}";

            return JObject.Parse(resultado);
        }

        /// <summary>
        /// Exclui um candidato, de acordo com seu id que deve ser informado na URL.
        /// Caso a inclusão seja feita com sucesso, o retorno é um "ok".
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JObject Delete(int id)
        {
            var candidato = new Candidato();
            //exclui as chaves estrangeiras que o candidato esta vinculado
            var resultado = CandidatoTecnologia.ExcluirPorCandidato(id);
                
            if(resultado == "ok")
            {
                 resultado = candidato.Excluir(id);
            }

            resultado = "{resultado: \"" + resultado + "\"}";

            return JObject.Parse(resultado);
        }

        //valida os dados do Body para cadastro
        public string validarDados(Candidato candidato)
        {
            var resultado = "";

            if (candidato == null)
            {
                resultado = "{resultado: \"Os parâmetros precisam ser informados no Body!\"}";
            }
            else if (candidato.Id != 0)
            {
                resultado = "{resultado: \"O campo ID é gerado automaticamente e, portanto, não deve ser informado!\"}";
            }
            else if (String.IsNullOrEmpty(candidato.Nome) || candidato.Vaga.Id == 0)
            {
                resultado = "{resultado: \"O campo NOME e VAGA_ID deve ser informado!\"}";
            }

            return resultado;
        }
    }
}