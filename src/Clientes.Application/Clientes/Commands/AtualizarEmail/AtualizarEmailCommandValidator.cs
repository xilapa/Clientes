using FluentValidation;
using static Clientes.Application.Common.Constants.ApplicationErrors;


namespace Clientes.Application.Clientes.Commands.AtualizarEmail;

public sealed class AtualizarEmailCommandValidator : AbstractValidator<AtualizarEmailCommand>
{
    public AtualizarEmailCommandValidator()
    {
        RuleFor(cmmd => cmmd.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(PropriedadeVazia(nameof(AtualizarEmailCommand.Email)))
            .EmailAddress()
            .WithMessage(PropriedadeComValorInvalido(nameof(AtualizarEmailCommand.Email)));

        RuleFor(cmmd => cmmd.ClienteId)
            .NotEqual(Guid.Empty)
            .WithMessage(PropriedadeVazia(nameof(AtualizarEmailCommand.ClienteId)));
    }
}