using Clientes.Application.Common.Validation;
using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.Enums;
using FluentValidation;
using static Clientes.Application.Common.Constants.ApplicationErrors;


namespace Clientes.Application.Clientes.Commands.AtualizarTelefone;

public sealed class AtualizarTelefoneCommandValidator : AbstractValidator<AtualizarTelefoneCommand>
{
    public AtualizarTelefoneCommandValidator()
    {
        RuleFor(cmmd => cmmd.ClienteId)
            .NotEqual(Guid.Empty)
            .WithMessage(PropriedadeVazia(nameof(AtualizarTelefoneCommand.ClienteId)));
        
        RuleFor(cmmd => cmmd.Telefone)
            .NotEmpty()
            .WithMessage(PropriedadeVazia(nameof(AtualizarTelefoneCommand.Telefone)));

        RuleFor(cmmd => cmmd.Telefone)
            .SetValidator(new AtualizarTelefoneInputValidator()!)
            .When(cmmd => cmmd.Telefone != null);
    }
}

public sealed class AtualizarTelefoneInputValidator : AbstractValidator<AtualizarTelefoneInput>
{
    public AtualizarTelefoneInputValidator()
    {
        RuleFor(cmmd => cmmd.TelefoneId)
            .NotEmpty()
            .WithMessage(PropriedadeVazia(nameof(AtualizarTelefoneInput.TelefoneId)));
        
        RuleFor(cmmd => cmmd.DDD)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(PropriedadeVazia(nameof(AtualizarTelefoneInput.DDD)))
            .DDD()
            .WithMessage(PropriedadeComValorInvalido(nameof(AtualizarTelefoneInput.DDD)));
        
        RuleFor(cmmd => cmmd.Tipo)
            .Cascade(CascadeMode.Stop)
            .NotEqual(default(TipoTelefone))
            .WithMessage(PropriedadeComValorInvalido(nameof(AtualizarTelefoneInput.Tipo)))
            .Must(t => Enum.IsDefined(typeof(TipoTelefone), (int) t))
            .WithMessage(PropriedadeComValorInvalido(nameof(AtualizarTelefoneInput.Tipo)));

        RuleFor(cmmd => cmmd.Numero)
            .NotEmpty()
            .WithMessage(PropriedadeVazia(nameof(AtualizarTelefoneInput.Numero)))
            .TelefoneCelular()
            .WithMessage(cmmd =>
                PropriedadeComValorInvalido(nameof(AtualizarTelefoneInput.Numero), cmmd.Numero))
            .When(cmmd => TipoTelefone.Celular.Equals(cmmd.Tipo));
        
        RuleFor(cmmd => cmmd.Numero)
            .NotEmpty()
            .WithMessage(PropriedadeVazia(nameof(AtualizarTelefoneInput.Numero)))
            .TelefoneFixo()
            .WithMessage(cmmd =>
                PropriedadeComValorInvalido(nameof(AtualizarTelefoneInput.Numero), cmmd.Numero))
            .When(cmmd => TipoTelefone.Fixo.Equals(cmmd.Tipo));
    }
}

