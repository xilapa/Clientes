﻿using Clientes.Application.Clientes.Commands.CadastrarCliente;
using Clientes.Application.Common.Resultados;
using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.Enums;
using Clientes.Domain.Clientes.Erros;
using Clientes.Domain.Common.Erros;
using FluentAssertions;
using Moq;
using UnitTests.Utils;

namespace UnitTests.Commands;

public sealed class CadastrarClienteTests : IClassFixture<BaseTestFixture>
{
    private readonly BaseTestFixture _fixture;
    private readonly CadastrarClienteCommandHandler _handler;

    public CadastrarClienteTests(BaseTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.UowMock.Invocations.Clear();
        _handler = new CadastrarClienteCommandHandler(_fixture.ClientesRepository, _fixture.TimeProvider, _fixture.UowMock.Object);
    }

    [Fact]
    public async Task ClienteNaoEhCadastradoComEmailExistente()
    {
        // Arrange
        var command = new CadastrarClienteCommand
        {
            NomeCompleto = "nome", Email = "email0@email.com",
            Telefones = new HashSet<TelefoneInput>
            {
                new () {Numero = "982345678", DDD = "29", Tipo = TipoTelefone.Celular},
            }
        };

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().BeEquivalentTo(new Resultado(ClienteErros.EmailJaCadastrado));

        _fixture.UowMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("12345678", "991234567", new []{"2912345678"})]
    [InlineData("12345678", "912345678", new []{"2912345678", "29912345678"})]
    public async Task ClienteNaoEhCadastradoComTelefoneJaExistente(string telFixo, string telCelular, string[] jaCadastrados)
    {
        // Arrange
        var command = new CadastrarClienteCommand
        {
            NomeCompleto = "nome", Email = "email@email.com",
            Telefones = new HashSet<TelefoneInput>
            {
                new () {Numero = telFixo, DDD = "29", Tipo = TipoTelefone.Fixo}, 
                new () {Numero = telCelular, DDD = "29", Tipo = TipoTelefone.Celular}, 
            }
        };

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.TipoErro.Should().Be(TipoErro.Conflito);
        var mensagemConcatenada = string.Concat(resultado.Erro.Mensagens);
        jaCadastrados.All(t => mensagemConcatenada.Contains(t)).Should().BeTrue();

        _fixture.UowMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ClienteEhCadastradoComDadosValidos()
    {
        // Arrange
        var command = new CadastrarClienteCommand
        {
            NomeCompleto = "nome", Email = "email@email.com",
            Telefones = new HashSet<TelefoneInput>
            {
                new () {DDD = "20", Numero = "912345678", Tipo = TipoTelefone.Celular}
            }
        };

        var clienteEsperado = new ClienteView
        {
            NomeCompleto = "nome", Email = "email@email.com",
            Telefones = new TelefoneView[]
            {
                new() { DDD = "20", Numero = "912345678", Tipo = TipoTelefone.Celular }
            }
        };

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Valor!.Id.Should().NotBe(Guid.Empty);
        resultado.Valor.Telefones.Select(t => t.Id).Should().NotBeNull();

        resultado.Valor.Should()
            .BeEquivalentTo(clienteEsperado, o =>
            {
                o.Excluding(c => c.Id);
                o.For(c => c.Telefones).Exclude(t => t.Id);
                return o;
            });

        _fixture.UowMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}