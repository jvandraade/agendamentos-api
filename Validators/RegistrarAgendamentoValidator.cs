using AgendamentosApi.DTOs;
using FluentValidation;

namespace AgendamentosApi.Validators
{
    public class RegistrarAgendamentoValidator : AbstractValidator<RegistrarAgendamentoDTO>
    {
        public RegistrarAgendamentoValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome é obrigatório")
                .Length(2, 200).WithMessage("O nome deve ter entre 2 e 200 caracteres")
                .Matches(@"^[a-zA-ZÀ-ÿ\s]+$").WithMessage("O nome deve conter apenas letras");

            RuleFor(x => x.Servico)
                .NotEmpty().WithMessage("O serviço é obrigatório")
                .Length(2, 200).WithMessage("O serviço deve ter entre 2 e 200 caracteres");

            RuleFor(x => x.Data)
                .NotEmpty().WithMessage("A data é obrigatória")
                .Must(BeAValidDate).WithMessage("A data deve ser hoje ou futura")
                .Must(BeAWeekday).WithMessage("Agendamentos apenas em dias úteis");

            RuleFor(x => x.Hora)
                .NotEmpty().WithMessage("A hora é obrigatória")
                .Must(BeInBusinessHours).WithMessage("Horário de atendimento: 08:00 às 18:00");

            // Validação combinada de Data + Hora
            RuleFor(x => x)
                .Must(x => BeAValidDateTime(x.Data, x.Hora))
                .WithMessage("O agendamento deve ser para data e hora futuras")
                .WithName("DataHora");
        }

        private bool BeAValidDate(DateOnly data)
        {
            // Aceita hoje ou datas futuras
            return data >= DateOnly.FromDateTime(DateTime.UtcNow);
        }

        private bool BeAWeekday(DateOnly data)
        {
            var diaSemana = data.DayOfWeek;
            return diaSemana != DayOfWeek.Saturday && diaSemana != DayOfWeek.Sunday;
        }

        private bool BeInBusinessHours(TimeOnly hora)
        {
            return hora >= new TimeOnly(8, 0) && hora <= new TimeOnly(18, 0);
        }

        private bool BeAValidDateTime(DateOnly data, TimeOnly hora)
        {
            // Combina data + hora para validação
            var agora = DateTime.UtcNow;
            var dataHoraAgendamento = data.ToDateTime(hora);

            // Se for hoje, a hora precisa ser futura (com margem de 30 minutos)
            if (data == DateOnly.FromDateTime(agora))
            {
                // Adiciona margem de 30 minutos para evitar agendamentos imediatos
                return dataHoraAgendamento >= agora.AddMinutes(30);
            }

            // Se for data futura, aceita qualquer hora dentro do horário comercial
            return data > DateOnly.FromDateTime(agora);
        }
    }
}