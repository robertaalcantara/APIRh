using APIRh.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace APIRh.Controllers
{
    public class VagaController : ApiController
    {
        /// <summary>
        /// Lista as vagas cadastradas no banco
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/vaga")]
        public List<Vaga> Get()
        {
            var listaVagas = Vaga.GetVagas();

            return listaVagas;
        }

        /// <summary>
        /// Busca uma vaga, de acordo com seu id que deve ser informado na URL
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Vaga Get(int id)
        {
            Vaga vaga = new Vaga();
            vaga.GetVaga(id);

            return vaga;
        }

        /// <summary>
        /// Cadastra uma nova vaga (é obrigatório o nome da vaga)
        /// </summary>
        /// <param name="vaga"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/vaga")]
        public JObject Post([FromBody] Vaga vaga)
        {
            var resultado = "";

            resultado = validarDados(vaga);

            if (String.IsNullOrEmpty(resultado))
            {
                resultado = vaga.Salvar(vaga);
                resultado = "{resultado: \"" + resultado + "\"}";
            }

            return JObject.Parse(resultado);
        }

        /// <summary>
        /// Edita os dados de uma vaga, de acordo com seu id que deve ser informado na URL e dos dados no Body
        /// </summary>
        /// <param name="id"></param>
        /// <param name="vaga"></param>
        /// <returns></returns>
        public JObject Put(int id, [FromBody] Vaga vaga)
        {
            var resultado = "";
            vaga.Id = id;
            resultado = vaga.Salvar(vaga);
            resultado = "{resultado: \"" + resultado + "\"}";

            return JObject.Parse(resultado);
        }

        /// <summary>
        /// Exclui uma vaga, de acordo com seu id que deve ser informado na URL
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JObject Delete(int id)
        {
            // exclui as chaves estrangeiras que a vaga esta vinculada
            var vagaTecnologia = new VagaTecnologia();
            var resultado = vagaTecnologia.ExcluirPorVaga(id);

            if(resultado == "ok")
            {
                resultado = Candidato.ExcluirPorVaga(id);
            }
            if(resultado == "ok")
            {
                var vaga = new Vaga();
                resultado = vaga.Excluir(id);
            }

            resultado = "{resultado: \"" + resultado + "\"}";

            return JObject.Parse(resultado);
        }

        //valida os dados no cadastro
        public string validarDados(Vaga vaga)
        {
            var resultado = "";

            if (vaga == null)
            {
                resultado = "{resultado: \"Os parâmetros precisam ser informados no Body!\"}";
            }
            else if (vaga.Id != 0)
            {
                resultado = "{resultado: \"O campo ID é gerado automaticamente e, portanto, não deve ser informado!\"}";
            }
            else if (String.IsNullOrEmpty(vaga.Nome))
            {
                resultado = "{resultado: \"O campo NOME deve ser informado!\"}";
            }

            return resultado;
        }
    }
}