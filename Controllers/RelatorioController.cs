using APIRh.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace APIRh.Controllers
{
    public class RelatorioController : ApiController
    {
        /// <summary>
        /// Recebe o id da vaga como parâmetro e retorna uma lista de candidatos com as respectivas notas finais para a vaga,
        /// ordenados de forma decrescente. 
        /// </summary>
        public List<Models.Candidato> Get(int id)
        {
            var listaCandidatos = VagaTecnologia.GetVagaCandidatos(id);

            return listaCandidatos;
        }

        /// <summary>
        /// Recebe um objeto Vaga (campo obrigatório é id) e uma lista de objetos Tecnologia (campo obrigatório é id e peso).
        /// Necessariamente deve ser informado ao menos uma Tecnologia.
        /// </summary>
        /// <param name="vagaTecnologia"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/relatorio")]
        public JObject Post([FromBody] VagaTecnologia vagaTecnologia)
        {
            var resultado = "";

            resultado = validarDados(vagaTecnologia);

            if (String.IsNullOrEmpty(resultado))
            {
                resultado = vagaTecnologia.Salvar(vagaTecnologia);
                resultado = "{resultado: \"" + resultado + "\"}";
            }

            return JObject.Parse(resultado);
        }

        //Valida dados do Body para cadastro
        public string validarDados(VagaTecnologia vagaTecnologia)
        {
            var resultado = "";

            if (vagaTecnologia == null)
            {
                resultado = "{resultado: \"Os parâmetros precisam ser informados no Body!\"}";
            }
            else if (vagaTecnologia.Id != 0)
            {
                resultado = "{resultado: \"O campo ID é gerado automaticamente e, portanto, não deve ser informado!\"}";
            }
            else if (vagaTecnologia.Tecnologias.Count == 0)
            {
                resultado = "{resultado: \"Ao menos uma tecnologia deve estar vinculada a esta vaga!\"}";
            }
            else if (vagaTecnologia.Vaga.Id == 0 || vagaTecnologia.Tecnologias.Exists(x => x.Id == 0) ||
                vagaTecnologia.Tecnologias.Exists(x => x.Peso == 0))
            {
                resultado = "{resultado: \"Os campos VAGA_ID, TECNOLOGIA_ID e TECNOLOGIA_PESO devem ser informados!\"}";
            }

            return resultado;
        }
    }
}