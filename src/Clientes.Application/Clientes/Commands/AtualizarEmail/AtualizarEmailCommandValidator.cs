using Clientes.Application.Common.Validation;
using FluentValidation;

namespace Clientes.Application.Clientes.Commands.AtualizarEmail;

public sealed class AtualizarEmailCommandValidator : AbstractValidator<AtualizarEmailCommand>
{
    public AtualizarEmailCommandValidator()
    {
        RuleFor(cmmd => cmmd.Email)
            .ValidarEmail();

        RuleFor(cmmd => cmmd.ClienteId)
            .ValidarGuid(nameof(AtualizarEmailCommand.ClienteId));
    }
}