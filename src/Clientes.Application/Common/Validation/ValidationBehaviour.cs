﻿using Clientes.Application.Common.Resultados;
using Clientes.Domain.Common.Erros;
using FluentValidation;
using Mediator;

namespace Clientes.Application.Common.Validation;

public sealed class ValidationBehaviour<TMessage, TResponse> : IPipelineBehavior<TMessage,TResponse>
    where TMessage : IValidable
    where TResponse : IResultado, new()
{
    private readonly IValidator<TMessage> _validator;

    public ValidationBehaviour(IValidator<TMessage> validator)
    {
        _validator = validator;
    }

    public async ValueTask<TResponse> Handle(TMessage message, CancellationToken cancellationToken,
        MessageHandlerDelegate<TMessage, TResponse> next)
    {
        var result = _validator.Validate(message);
        if (result.IsValid)
            return await next(message, cancellationToken);

        var erros = result.Errors.Select(e => e.ErrorMessage).ToArray();
        var errorResult = new TResponse();
        errorResult.DefinirErro(Erro.Validacao(erros));
        return errorResult;
    }
}