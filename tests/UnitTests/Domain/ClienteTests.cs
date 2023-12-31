﻿using Clientes.Domain.Clientes;
using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.Enums;
using Clientes.Domain.Clientes.Events;
using FluentAssertions;

namespace UnitTests.Domain;

public sealed class ClienteTests
{
    [Fact]
    public void ClienteEhAtualizadoAoAutualizarEmail()
    {
        // Arrange
        var cliente = new Cliente("eu", "email", DateTime.Now);
        var novaData = DateTime.Now.AddDays(15);

        // Act
        cliente.AtualizarEmail("novo email", novaData);

        // Assert
        cliente.Email.Should().Be("novo email");
        cliente.UltimaAtualizacao.Should().Be(novaData);
    }

    [Fact]
    public void TelefoneDoClienteEhAtualizadoCorretamente()
    {
        // Arrange
        var cliente = new Cliente("eu", "email", DateTime.Now);
        var telefones = new HashSet<TelefoneInput>
        {
            new() { Numero = "988888888", DDD = "88", Tipo = TipoTelefone.Celular }
        };
        cliente.CadastrarTelefones(telefones, DateTime.Now);

        var novaData = DateTime.Now.AddDays(15);
        var idTelAtualizar = cliente.Telefones.First().Id;
        var telUpdate = new TelefoneInput
        {
            Numero = "33332222", DDD = "99", Tipo = TipoTelefone.Fixo
        };

        // Act
        cliente.AtualizarTelefone(idTelAtualizar, telUpdate, novaData);

        // Assert
        var telefoneAtualizado = cliente.Telefones.First(t => t.Id == idTelAtualizar);
        telefoneAtualizado.Should().BeEquivalentTo(new
        {
            Tipo = telUpdate.Tipo,
            DDD = telUpdate.DDD,
            Numero = telUpdate.Numero,
            UltimaAtualizacao = novaData
        });

        cliente.UltimaAtualizacao.Should().Be(novaData);
    }

    [Fact]
    public void NaoEhPossivelCadastrarTelefonesDuplicados()
    {
        // Arrange
        var cliente = new Cliente("eu", "email", DateTime.Now);
        var telefones = new HashSet<TelefoneInput>
        {
            new() { Numero = "988888888", DDD = "88", Tipo = TipoTelefone.Celular },
            new() { Numero = "988888888", DDD = "88", Tipo = TipoTelefone.Fixo },
            new() { Numero = "988888888", DDD = "77", Tipo = TipoTelefone.Celular },
            new() { Numero = "988888888", DDD = "77", Tipo = TipoTelefone.Fixo },
        };

        // Act
        cliente.CadastrarTelefones(telefones, DateTime.Now);

        // Assert
        cliente.Telefones.Should().HaveCount(2);
    }

    [Fact]
    public void ClienteAlteradoEventEhCriadoAoAtualizarTelefone()
    {
        // Arrange
        var cliente = new Cliente("eu", "email", DateTime.Now);
        var telefones = new HashSet<TelefoneInput>
        {
            new() { Numero = "988888888", DDD = "88", Tipo = TipoTelefone.Celular },
            new() { Numero = "977777777", DDD = "77", Tipo = TipoTelefone.Celular },
        };
        cliente.CadastrarTelefones(telefones, DateTime.Now);

        var telefonesEsperadosNoEvento = new TelefoneDoCliente[]
        {
            new() { Numero = "988888888", DDD = "88"},
            new() { Numero = "977777777", DDD = "77" }
        };

        var idTelAtualizar = cliente.Telefones.First().Id;
        var telUpdate = new TelefoneInput
        {
            Numero = "33332222", DDD = "99", Tipo = TipoTelefone.Fixo
        };

        // Act
        cliente.AtualizarTelefone(idTelAtualizar, telUpdate, DateTime.Now);

        // Assert
        cliente.Telefones.Should().HaveCount(2);
        cliente.Telefones.Should().Match(tel => tel.Any(t => t.Numero == "33332222"));

        var evento = cliente.DomainEvents.First(e => e is ClienteAlteradoEvent) as ClienteAlteradoEvent;
        evento!.Telefones.Should().BeEquivalentTo(telefonesEsperadosNoEvento,
            o => o.WithoutStrictOrdering());
    }

    [Fact]
    public void CadastrarClienteCriaDomainEvent()
    {
        // Arrange && Act
        var cliente = new Cliente("eu", "email", DateTime.Now);

        // Assert
        cliente.DomainEvents
            .Where(e => e is ClienteCadastradoEvent).Should().HaveCount(1);
    }

    [Fact]
    public void ClienteAlteradoEventEhCriadoAoAtualizarEmail()
    {
        // Arrange
        var cliente = new Cliente("eu", "email", DateTime.Now);
        var telefones = new HashSet<TelefoneInput>
        {
            new() { Numero = "988888888", DDD = "88", Tipo = TipoTelefone.Celular },
            new() { Numero = "977777777", DDD = "77", Tipo = TipoTelefone.Celular },
        };
        cliente.CadastrarTelefones(telefones, DateTime.Now);

        var telefonesEsperadosNoEvento = new TelefoneDoCliente[]
        {
            new() { Numero = "988888888", DDD = "88"},
            new() { Numero = "977777777", DDD = "77" }
        };

        // Act
        cliente.AtualizarEmail("novo email", DateTime.Now);

        // Assert
        cliente.Email.Should().Be("novo email");
        var evento = cliente.DomainEvents.First(e => e is ClienteAlteradoEvent) as ClienteAlteradoEvent;
        evento!.Telefones.Should().BeEquivalentTo(telefonesEsperadosNoEvento,
            o => o.WithoutStrictOrdering());
    }
}