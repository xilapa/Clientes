using Clientes.Application.Common.Validation;
using FluentValidation;
using static Clientes.Application.Common.Constants.ApplicationErrors;

namespace Clientes.Application.Clientes.Commands.AtualizarTelefone;

public sealed class AtualizarTelefoneCommandValidator : AbstractValidator<AtualizarTelefoneCommand>
{
    public AtualizarTelefoneCommandValidator()
    {
        RuleFor(cmmd => cmmd.ClienteId)
            .ValidarIdGuid(nameof(AtualizarTelefoneCommand.ClienteId));

        RuleFor(cmmd => cmmd.TelefoneId)
            .ValidarIdGuid(nameof(AtualizarTelefoneCommand.TelefoneId));

        RuleFor(cmmd => cmmd.Telefone)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage(PropriedadeVazia(nameof(AtualizarTelefoneCommand.Telefone)))
            .SetValidator(new TelefoneInputValidator()!);
    }
}
