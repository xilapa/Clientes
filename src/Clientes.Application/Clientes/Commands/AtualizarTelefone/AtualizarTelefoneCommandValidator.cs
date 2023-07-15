using Clientes.Application.Common.Validation;
using Clientes.Domain.Clientes.DTOs;
using FluentValidation;
using static Clientes.Application.Common.Constants.ApplicationErrors;

namespace Clientes.Application.Clientes.Commands.AtualizarTelefone;

public sealed class AtualizarTelefoneCommandValidator : AbstractValidator<AtualizarTelefoneCommand>
{
    public AtualizarTelefoneCommandValidator()
    {
        RuleFor(cmmd => cmmd.ClienteId)
            .ValidarGuid(nameof(AtualizarTelefoneCommand.ClienteId));

        RuleFor(cmmd => cmmd.TelefoneId)
            .ValidarGuid(nameof(AtualizarTelefoneCommand.TelefoneId));

        RuleFor(cmmd => cmmd.Telefone)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage(PropriedadeVazia(nameof(AtualizarTelefoneCommand.Telefone)))
            .SetValidator(new TelefoneInputValidator()!);
    }
}
