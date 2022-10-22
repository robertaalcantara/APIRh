using APIRh.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace APIRh.Controllers
{
    public class TecnologiaController : ApiController
    {
        /// <summary>
        /// Lista as tecnologias cadastradas no banco
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/tecnologia")]
        public List<Tecnologia> Get()
        {
            var listaTecnologias = Tecnologia.GetTecnologias();

            return listaTecnologias;
        }

        /// <summary>
        /// Busca uma tecnologia, de acordo com seu id que deve ser informado na URL
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Tecnologia Get(int id)
        {
            Tecnologia tecnologia = new Tecnologia();
            tecnologia.GetTecnologia(id);

            return tecnologia;
        }

        /// <summary>
        /// Insere uma nova tecnologia no banco (é obrigatório o nome da tecnologia)
        /// </summary>
        /// <param name="tecnologia"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/tecnologia")]
        public JObject Post([FromBody] Tecnologia tecnologia)
        {
            var resultado = "";

            resultado = validarDados(tecnologia);

            if (String.IsNullOrEmpty(resultado))
            {
                resultado = tecnologia.Salvar(tecnologia);
                resultado = "{resultado: \"" + resultado + "\"}";
            }

            return JObject.Parse(resultado);
        }

        /// <summary>
        /// Edita os dados de uma tecnologia, de acordo com seu id que deve ser informado na URL e dos dados no Body
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tecnologia"></param>
        /// <returns></returns>
        public JObject Put(int id, [FromBody] Tecnologia tecnologia)
        {
            var resultado = "";
            tecnologia.Id = id;
            resultado = tecnologia.Salvar(tecnologia);
            resultado = "{resultado: \"" + resultado + "\"}";

            return JObject.Parse(resultado);
        }

        /// <summary>
        /// Exclui uma tecnologia, de acordo com seu id que deve ser informado na URL
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JObject Delete(int id)
        {
            // exclui as chaves estrangeiras que a tecnologia esta vinculada
            var candidatoTecnologia = new CandidatoTecnologia();
            var resultado = candidatoTecnologia.ExcluirPorTecnologia(id);

            if (resultado == "ok")
            {
                var vagaTecnologia = new VagaTecnologia();
                resultado = vagaTecnologia.ExcluirPorTecnologia(id);
            }

            if (resultado == "ok")
            {
                var tecnologia = new Tecnologia();
                resultado = tecnologia.Excluir(id);
            }

            resultado = "{resultado: \"" + resultado + "\"}";

            return JObject.Parse(resultado);
        }

        //Valida dados de cadastro
        public string validarDados(Tecnologia tecnologia)
        {
            var resultado = "";

            if (tecnologia == null)
            {
                resultado = "{resultado: \"Os parâmetros precisam ser informados no Body!\"}";
            }
            else if (tecnologia.Id != 0)
            {
                resultado = "{resultado: \"O campo ID é gerado automaticamente e, portanto, não deve ser informado!\"}";
            }
            else if (String.IsNullOrEmpty(tecnologia.Nome))
            {
                resultado = "{resultado: \"O campo NOME deve ser informado!\"}";
            }

            return resultado;
        }
    }
}