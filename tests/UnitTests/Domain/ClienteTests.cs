using Clientes.Domain.Clientes;
using Clientes.Domain.Clientes.DTOs;
using Clientes.Domain.Clientes.Enums;
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
        var telefones = new CadastrarTelefoneInput
        {
            Numero = "988888888", DDD = "88", Tipo = TipoTelefone.Celular
        };
        cliente.CadastrarTelefones(new []{telefones}, DateTime.Now);
        
        var novaData = DateTime.Now.AddDays(15);
        var idTelAtualizar = cliente.Telefones.First().Id;
        var telUpdate = new AtualizarTelefoneInput
        {
            TelefoneId = idTelAtualizar.Value,
            Numero = "33332222", DDD = "99", Tipo = TipoTelefone.Fixo
        };
        
        // Act
        cliente.AtualizarTelefone(telUpdate, novaData);
        
        // Assert
        var telefoneAtualizado = cliente.Telefones.First(t => t.Id == idTelAtualizar);
        telefoneAtualizado.Tipo.Should().Be(telUpdate.Tipo);
        telefoneAtualizado.Numero.Should().Be(telUpdate.NumeroCompleto);
        telefoneAtualizado.UltimaAtualizacao.Should().Be(novaData);
        
        cliente.UltimaAtualizacao.Should().Be(novaData);
    }
}