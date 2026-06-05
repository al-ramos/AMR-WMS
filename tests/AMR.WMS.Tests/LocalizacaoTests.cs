using AMR.WMS.Domain.Entities;
using AMR.WMS.Domain.Enums;

namespace AMR.WMS.Tests;

public class LocalizacaoTests
{
    [Fact]
    public void Criar_ComDadosValidos_DeveCriarLocalizacaoCorretamente()
    {
        var loc = Localizacao.Criar("A", "01", "A", "01", 50, TipoLocalizacao.Picking);

        loc.Id.Should().NotBeEmpty();
        loc.Codigo.Should().Be("A-01-A-01");
        loc.Zona.Should().Be("A");
        loc.Capacidade.Should().Be(50);
        loc.Ocupacao.Should().Be(0);
        loc.TipoLocalizacao.Should().Be(TipoLocalizacao.Picking);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_CodigoNaoVazio_DeveLancarExcecao(string zona)
    {
        Action act = () => Localizacao.Criar(zona, "01", "A", "01", 50);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Criar_CapacidadeZero_DeveLancarExcecao()
    {
        Action act = () => Localizacao.Criar("A", "01", "A", "01", 0);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*Capacidade deve ser maior que zero*");
    }

    [Fact]
    public void Criar_CapacidadeNegativa_DeveLancarExcecao()
    {
        Action act = () => Localizacao.Criar("A", "01", "A", "01", -5);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AtualizarOcupacao_DentroCapacidade_DeveAtualizar()
    {
        var loc = Localizacao.Criar("B", "02", "B", "03", 100, TipoLocalizacao.Reserva);
        loc.AtualizarOcupacao(40);
        loc.Ocupacao.Should().Be(40);
    }

    [Fact]
    public void AtualizarOcupacao_AcimaCapacidade_DeveLancarExcecao()
    {
        var loc = Localizacao.Criar("B", "02", "B", "03", 50);
        Action act = () => loc.AtualizarOcupacao(51);
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*Ocupação excede a capacidade*");
    }

    [Fact]
    public void AtualizarOcupacao_Negativa_DeveLancarExcecao()
    {
        var loc = Localizacao.Criar("C", "01", "A", "01", 100);
        Action act = () => loc.AtualizarOcupacao(-5);
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*Ocupação não pode ser negativa*");
    }

    [Fact]
    public void Atualizar_NovaCapacidade_DeveAtualizar()
    {
        var loc = Localizacao.Criar("A", "01", "A", "01", 50, TipoLocalizacao.Picking);
        loc.Atualizar(200, TipoLocalizacao.Reserva);
        loc.Capacidade.Should().Be(200);
        loc.TipoLocalizacao.Should().Be(TipoLocalizacao.Reserva);
    }

    [Fact]
    public void Atualizar_CapacidadeMenorQueOcupacao_DeveLancarExcecao()
    {
        var loc = Localizacao.Criar("A", "01", "A", "01", 100);
        loc.AtualizarOcupacao(80);
        Action act = () => loc.Atualizar(50, TipoLocalizacao.Picking);
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*Capacidade não pode ser menor*");
    }

    [Fact]
    public void Criar_CodigoDeveSerGeradoEmMaiusculo()
    {
        var loc = Localizacao.Criar("a", "01", "b", "02", 30);
        loc.Codigo.Should().Be("A-01-B-02");
        loc.Zona.Should().Be("A");
    }
}
