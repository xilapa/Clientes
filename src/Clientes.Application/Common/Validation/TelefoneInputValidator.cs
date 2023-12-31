﻿using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.Enums;
using FluentValidation;

namespace Clientes.Application.Common.Validation;

public sealed class TelefoneInputValidator : AbstractValidator<TelefoneInput>
{
    public TelefoneInputValidator()
    {
        RuleFor(cmmd => cmmd.DDD)
            .ValidarDDD();

        RuleFor(cmmd => cmmd.Tipo)
            .ValidarTipoTelefone();

        RuleFor(cmmd => cmmd.Numero)
            .ValidarNumeroCelular()
            .When(cmmd => TipoTelefone.Celular.Equals(cmmd.Tipo));

        RuleFor(cmmd => cmmd.Numero)
            .ValidarNumeroFixo()
            .When(cmmd => TipoTelefone.Fixo.Equals(cmmd.Tipo));
    }
}