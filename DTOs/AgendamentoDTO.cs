namespace AgendamentosApi.DTOs
{
    public class AgendamentoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Servico { get; set; } = string.Empty;
        public DateOnly Data { get; set; }
        public TimeOnly Hora { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}