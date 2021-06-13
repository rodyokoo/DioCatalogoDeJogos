using ApiCatalogoJogos.Exceptions;
using ApiCatalogoJogos.InputModel;
using ApiCatalogoJogos.Services;
using ApiCatalogoJogos.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCatalogoJogos.Controllers.V1
{
    [Route("api/V1/[controller]")]
    [ApiController]
    public class JogosController : ControllerBase
    {
        private readonly IJogoService _jogoService;
        public JogosController(IJogoService jogoService)
        {
            _jogoService = jogoService;
        }

        /// <summary>
        /// Buscar todos os jogos de forma paginada
        /// </summary>
        /// <remarks>
        /// Não é possível retornar os jogos sem paginação
        /// </remarks>
        /// <param name="pagina">Indica qual página está sendo consultada. Mínimo 1</param>
        /// <param name="quantidade">Indica a quantidade de registros por página. Mínimo 1 e máximo 50</param>
        /// <response code="200">Retorna a lista de jogos</response>
        /// <response code="204">Caso não haja jogos</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JogoViewModel>>> Obter([FromQuery, Range(1, int.MaxValue)] int pagina = 1, [FromQuery, Range(1, 50)] int quantidade = 5)
        {
            //throw new Exception();
            var jogos = await _jogoService.Obter(pagina, quantidade);
            if (jogos.Count() == 0)
            {
                return NoContent();
            }
            return Ok(jogos);
        }

        /// <summary>
        /// Buscar um jogo pelo seu Id
        /// </summary>
        /// <param name="idJogo">Id do jogo buscado</param>
        /// <response code="200">Retornar o jogo filtrado</response>
        /// <response code="204">Caso não haja jogo com este id</response>
        [HttpGet("{idJogo:guid}")]
        public async Task<ActionResult<JogoViewModel>> Obter([FromRoute] Guid idJogo)
        {
            var jogo = await _jogoService.Obter(idJogo);
            if (jogo == null)
                return NoContent();
            return Ok(jogo);
        }

        /// <summary>
        /// Insere jogos 
        /// </summary>
        /// <param name="jogoInputModel">Utiliza o formato JSON para inserir os dados no servidor</param>
        /// <response code="200">Insere jogo ao BD</response>
        [HttpPost]
        public async Task<ActionResult<JogoViewModel>> InserirJogo([FromBody] JogoInputModel jogoInputModel)
        {
            try
            {
                var jogo = await _jogoService.Inserir(jogoInputModel);
                return Ok(jogo);
            }
            catch (JogoJaCadastradoException ex)
            {
                return UnprocessableEntity("Já existe um jogo com este nome para esta produtora");
            }
        }

        /// <summary>
        /// Modifica um jogo já inserido no banco
        /// </summary>
        /// <param name="idJogo">Id do jogo a ser modificado</param>
        /// <param name="jogoInputModel">Parâmetro no formato JSON</param>
        /// <response code="200">Jogo modificado</response>
        /// <response code="204">Caso não haja jogo com este id</response>
        [HttpPut("{idJogo:guid}")]
        public async Task<ActionResult> AtualizarJogo([FromRoute] Guid idJogo, [FromBody] JogoInputModel jogoInputModel)
        {
            try
            {
                await _jogoService.Atualizar(idJogo, jogoInputModel);
                return Ok();
            }
            catch (JogoNaoCadastradoException ex)
            {
                return NotFound("Jogo não existente");
            }
        }

        /// <summary>
        /// Modifica o preço de um jogo
        /// </summary>
        /// <param name="idJogo">Id do jogo que com o preço a ser modificado</param>
        /// <param name="preco">Novo preço do jogo</param>
        /// <response code="200">Preço do jogo modificado com sucesso</response>
        /// <response code="204">Caso não haja jogo com este id</response>
        [HttpPatch("{idJogo:guid}/preco/{preco:double}")]
        public async Task<ActionResult> AtualizarJogo([FromRoute] Guid idJogo, [FromRoute] double preco)
        {
            try
            {
                await _jogoService.Atualizar(idJogo, preco);
                return Ok();
            }
            catch (JogoNaoCadastradoException ex)
            {
                return NotFound("Esse jogo não existe");
            }
        }

        /// <summary>
        /// Deletar um jogo pelo id
        /// </summary>
        /// <param name="idJogo">Id do jogo a ser deletado</param>
        /// <response code="200">Jogo deletado</response>
        /// <response code="204">Caso não haja jogo com este id</response>
        [HttpDelete("{idJogo:guid}")]
        public async Task<ActionResult> ApagarJogo([FromRoute] Guid idJogo)
        {
            try
            {
                await _jogoService.Remover(idJogo);
                return Ok();
            }
            catch (JogoNaoCadastradoException ex)
            {
                return NotFound("Esse jogo não existe");
            }
        }
    }
}
