using Clientes.Application.Clientes.Commands.CadastrarCliente;
using FluentValidation;
using static Clientes.Application.Common.Constants.ApplicationErrors;

namespace Clientes.Application.Clientes.Commands.ExcluirCliente;

public class ExcluirClienteCommandValidator : AbstractValidator<ExcluirClienteCommand>
{
    public ExcluirClienteCommandValidator()
    {
        RuleFor(cmmd => cmmd.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(PropriedadeVazia(nameof(CadastrarClienteCommand.Email)))
            .EmailAddress()
            .WithMessage(PropriedadeComValorInvalido(nameof(CadastrarClienteCommand.Email)));
    }
}