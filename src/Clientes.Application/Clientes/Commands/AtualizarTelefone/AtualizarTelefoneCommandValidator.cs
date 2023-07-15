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

        RuleFor(cmmd => cmmd.Telefone)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage(PropriedadeVazia(nameof(AtualizarTelefoneCommand.Telefone)))
            .SetValidator(new AtualizarTelefoneInputValidator()!);
    }
}

public sealed class AtualizarTelefoneInputValidator : AbstractValidator<AtualizarTelefoneInput>
{
    public const string TelefoneId = nameof(TelefoneId);
    public AtualizarTelefoneInputValidator()
    {
        RuleFor(cmmd => cmmd.Id)
            .ValidarGuid(TelefoneId);

        Include(new BaseTelefoneInputValidator());
    }
}

