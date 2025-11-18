namespace AgendamentosApi.DTOs
{
    public class RegistrarAgendamentoDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Servico { get; set; } = string.Empty;
        public DateOnly Data { get; set; }
        public TimeOnly Hora { get; set; }
    }
}