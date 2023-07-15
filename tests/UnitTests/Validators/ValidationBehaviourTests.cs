using Clientes.Application.Clientes.Commands.AtualizarEmail;
using Clientes.Application.Common.Behaviours;
using Clientes.Application.Common.Resultados;
using Clientes.Domain.Common.Erros;
using FluentAssertions;
using static Clientes.Application.Common.Constants.ApplicationErrors;

namespace UnitTests.Validators;

public sealed class ValidationBehaviourTests
{
    private readonly ValidationBehaviour<AtualizarEmailCommand, Resultado> _validationBehaviour;

    public ValidationBehaviourTests()
    {
        var validator = new AtualizarEmailCommandValidator();
        _validationBehaviour = new ValidationBehaviour<AtualizarEmailCommand,Resultado>(validator);
    }
    
    [Fact]
    public async Task BehaviourNaoChamaHandlerQuandoHaErroDeValidacao()
    {
        // Arrange
        var command = new AtualizarEmailCommand
        {
            ClienteId = Guid.NewGuid(),
            Email = "emailinvalido"
        };
        
        // Act
        // Se chamar o handler ira dar null reference exception
        var resultado = await _validationBehaviour.Handle(command, CancellationToken.None, null!);
        
        // Assert
        resultado.Erro.Should().BeEquivalentTo(Erro.Validacao(PropriedadeComValorInvalido("Email")));
    }
    
    [Fact]
    public async Task BehaviourChamaHandlerQuandoNaoHaErroDeValidacao()
    {
        // Arrange
        var command = new AtualizarEmailCommand
        {
            ClienteId = Guid.NewGuid(),
            Email = "email@email.com"
        };
        
        // Act
        var act = _validationBehaviour
            .Awaiting(h => h.Handle(command, CancellationToken.None, null!)) ;
        
        // Assert
        await act.Should().ThrowAsync<NullReferenceException>();
    }
}