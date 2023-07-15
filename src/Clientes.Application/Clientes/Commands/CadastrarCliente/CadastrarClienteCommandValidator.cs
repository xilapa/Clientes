using Clientes.Application.Common.Validation;
using FluentValidation;
using static Clientes.Application.Common.Constants.ApplicationErrors;

namespace Clientes.Application.Clientes.Commands.CadastrarCliente;

public sealed class CadastrarClienteCommandValidator : AbstractValidator<CadastrarClienteCommand>
{
    public CadastrarClienteCommandValidator()
    {
        RuleFor(cmmd => cmmd.Email)
            .ValidarEmail();

        RuleFor(cmmd => cmmd.NomeCompleto)
            .NotEmpty()
            .WithMessage(PropriedadeVazia(nameof(CadastrarClienteCommand.NomeCompleto)));

        RuleFor(cmmd => cmmd.Telefones)
            .Cascade(CascadeMode.Stop)
            .ForEach(t => t.NotNull())
            .ForEach(t => t.SetValidator(new BaseTelefoneInputValidator()));
    }
}