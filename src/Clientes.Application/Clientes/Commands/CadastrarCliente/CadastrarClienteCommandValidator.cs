using Clientes.Application.Common.Validation;
using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.Enums;
using FluentValidation;
using static Clientes.Application.Common.Constants.ApplicationErrors;

namespace Clientes.Application.Clientes.Commands.CadastrarCliente;

public sealed class CadastrarClienteCommandValidator : AbstractValidator<CadastrarClienteCommand>
{
    public CadastrarClienteCommandValidator()
    {
        RuleFor(cmmd => cmmd.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage(PropriedadeVazia(nameof(CadastrarClienteCommand.Email)))
            .EmailAddress()
                .WithMessage(PropriedadeComValorInvalido(nameof(CadastrarClienteCommand.Email)));
        
        RuleFor(cmmd => cmmd.NomeCompleto)
            .NotEmpty().WithMessage(PropriedadeVazia(nameof(CadastrarClienteCommand.NomeCompleto)));

        RuleFor(cmmd => cmmd.Telefones)
            .ForEach(t => t.NotNull())
            .ForEach(t => t.SetValidator(new CadastrarTelefoneInputValidator()));
    }
}

public sealed class CadastrarTelefoneInputValidator : AbstractValidator<CadastrarTelefoneInput>
{
    public CadastrarTelefoneInputValidator()
    {
        RuleFor(cmmd => cmmd.DDD)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage(PropriedadeVazia(nameof(CadastrarTelefoneInput.DDD)))
            .DDD()
                .WithMessage(PropriedadeComValorInvalido(nameof(CadastrarTelefoneInput.DDD)));
        
        RuleFor(cmmd => cmmd.Tipo)
            .Cascade(CascadeMode.Stop)
            .NotEqual(default(TipoTelefone))
                .WithMessage(PropriedadeComValorInvalido(nameof(CadastrarTelefoneInput.Tipo)))
            .Must(t => Enum.IsDefined(typeof(TipoTelefone), (int) t))
                .WithMessage(PropriedadeComValorInvalido(nameof(CadastrarTelefoneInput.Tipo)));

        RuleFor(cmmd => cmmd.Numero)
            .NotEmpty()
                .WithMessage(PropriedadeVazia(nameof(CadastrarTelefoneInput.Numero)))
            .TelefoneCelular()
                .WithMessage(cmmd =>
                    PropriedadeComValorInvalido(nameof(CadastrarTelefoneInput.Numero), cmmd.Numero))
            .When(cmmd => TipoTelefone.Celular.Equals(cmmd.Tipo));
        
        RuleFor(cmmd => cmmd.Numero)
            .NotEmpty()
            .WithMessage(PropriedadeVazia(nameof(CadastrarTelefoneInput.Numero)))
            .TelefoneFixo()
            .WithMessage(cmmd =>
                PropriedadeComValorInvalido(nameof(CadastrarTelefoneInput.Numero), cmmd.Numero))
            .When(cmmd => TipoTelefone.Fixo.Equals(cmmd.Tipo));
    }
    
}