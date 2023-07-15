using Clientes.Application.Common.Validation;
using FluentValidation;

namespace Clientes.Application.Clientes.Commands.ExcluirCliente;

public sealed class ExcluirClienteCommandValidator : AbstractValidator<ExcluirClienteCommand>
{
    public ExcluirClienteCommandValidator()
    {
        RuleFor(cmmd => cmmd.Email)
            .ValidarEmail();
    }
}