using AgendamentosApi.Data;
using AgendamentosApi.DTOs;
using AgendamentosApi.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgendamentosApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AgendamentosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IValidator<RegistrarAgendamentoDTO> _validator;
        private readonly ILogger<AgendamentosController> _logger;

        public AgendamentosController(AppDbContext context,
            IValidator<RegistrarAgendamentoDTO> validator,
            ILogger<AgendamentosController> logger)
        {
            _context = context;
            _validator = validator;
            _logger = logger;
        }

        /// <summary>
        /// Lista todos os agendamentos
        /// </summary>
        /// <returns>Lista de agendamentos</returns>
        /// <response code="200">Retorna a lista de agendamentos</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<AgendamentoDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<AgendamentoDTO>>> GetAll()
        {
            _logger.LogInformation("Buscando todos os agendamentos");

            var agendamentos = await _context.Agendamentos
                .AsNoTracking()
                .OrderBy(a => a.Data)
                .ThenBy(a => a.Hora)
                .Select(a => new AgendamentoDTO
                {
                    Id = a.Id,
                    Nome = a.Nome,
                    Servico = a.Servico,
                    Data = a.Data,
                    Hora = a.Hora,
                    DataCriacao = a.DataCriacao
                })
                .ToListAsync();

            _logger.LogInformation("{Count} agendamentos encontrados", agendamentos.Count);

            return Ok(agendamentos);
        }

        /// <summary>
        /// Busca um agendamento por ID
        /// </summary>
        /// <param name="id">ID do agendamento</param>
        /// <returns>Detalhes do agendamento</returns>
        /// <response code="200">Retorna o agendamento</response>
        /// <response code="404">Agendamento não encontrado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AgendamentoDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AgendamentoDTO>> GetById(int id)
        {
            _logger.LogInformation("Buscando agendamento com ID: {Id}", id);

            var agendamento = await _context.Agendamentos
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new AgendamentoDTO
                {
                    Id = a.Id,
                    Nome = a.Nome,
                    Servico = a.Servico,
                    Data = a.Data,
                    Hora = a.Hora,
                    DataCriacao = a.DataCriacao
                })
                .FirstOrDefaultAsync();

            if (agendamento == null)
            {
                _logger.LogWarning("Agendamento com ID {Id} não encontrado", id);
                return NotFound(new { message = $"Agendamento com ID {id} não encontrado" });
            }

            return Ok(agendamento);
        }

        /// <summary>
        /// Registra um novo agendamento
        /// </summary>
        /// <param name="dto">Dados do agendamento</param>
        /// <returns>Agendamento criado</returns>
        /// <response code="201">Agendamento criado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="409">Conflito de horário</response>
        [HttpPost]
        [ProducesResponseType(typeof(AgendamentoDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AgendamentoDTO>> Create([FromBody] RegistrarAgendamentoDTO dto)
        {
            _logger.LogInformation("Criando agendamento para {Nome} - Serviço: {Servico}", dto.Nome, dto.Servico);

            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Falha ao validar: {Errors}", validationResult.Errors);
                return BadRequest(new 
                { 
                    message = "Dados inválidos",
                    errors = validationResult.Errors.Select(e => new 
                    { 
                        campo = e.PropertyName, 
                        mensagem = e.ErrorMessage 
                    })
                });
            }

            var conflito = await _context.Agendamentos
                .AnyAsync(a => a.Data == dto.Data && a.Hora == dto.Hora);

            if (conflito)
            {
                _logger.LogWarning("Conflito de horário: {Data} às {Hora}", dto.Data, dto.Hora);
                return Conflict(new { message = "Já existe um agendamento para esta data e horário" });
            }

            var agendamento = new Agendamento
            {
                Nome = dto.Nome,
                Servico = dto.Servico,
                Data = dto.Data,
                Hora = dto.Hora,
                DataCriacao = DateTime.UtcNow
            };

            _context.Agendamentos.Add(agendamento);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Agendamento criado com ID: {Id}", agendamento.Id);

            var resultado = new AgendamentoDTO
            {
                Id = agendamento.Id,
                Nome = agendamento.Nome,
                Servico = agendamento.Servico,
                Data = agendamento.Data,
                Hora = agendamento.Hora,
                DataCriacao = agendamento.DataCriacao
            };

            return CreatedAtAction(nameof(GetById), new { id = agendamento.Id }, resultado);
        }

        /// <summary>
        /// Exclui um agendamento
        /// </summary>
        /// <param name="id">ID do agendamento</param>
        /// <returns>Status da operação</returns>
        /// <response code="204">Agendamento excluído com sucesso</response>
        /// <response code="404">Agendamento não encontrado</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Excluindo agendamento com ID: {Id}", id);

            var agendamento = await _context.Agendamentos.FindAsync(id);

            if (agendamento == null)
            {
                _logger.LogWarning("Agendamento com ID {Id} não encontrado para exclusão", id);
                return NotFound(new { message = $"Agendamento com ID {id} não encontrado" });
            }

            _context.Agendamentos.Remove(agendamento);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Agendamento {Id} excluído com sucesso", id);

            return NoContent();
        }
    }
}