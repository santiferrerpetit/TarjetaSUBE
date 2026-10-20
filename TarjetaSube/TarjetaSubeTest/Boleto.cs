using TarjetaSube.Clases;

namespace TarjetaSubeTest;

public class BoletoTests
{
    private static readonly DateTimeOffset Ahora = new(2026, 10, 20, 8, 30, 0, TimeSpan.Zero);

    private static Colectivo NuevoColectivo() =>
        new() { Linea = "144 Negro", Tiempo = new RelojFalso(Ahora) };

    [Test]
    public void EmitirBoleto_TarjetaNormal_CompletaTodosLosDatos()
    {
        var colectivo = NuevoColectivo();
        var tarjeta = new Tarjeta { Id = 7, Saldo = 5000 };

        var boleto = colectivo.EmitirBoleto(tarjeta);

        Assert.That(boleto, Is.Not.Null);
        Assert.That(boleto!.Fecha, Is.EqualTo(Ahora.DateTime));
        Assert.That(boleto.TipoTarjeta, Is.EqualTo("Normal"));
        Assert.That(boleto.Linea, Is.EqualTo("144 Negro"));
        Assert.That(boleto.TotalAbonado, Is.EqualTo(1580));
        Assert.That(boleto.Saldo, Is.EqualTo(3420));
        Assert.That(boleto.IdTarjeta, Is.EqualTo(7));
    }

    [Test]
    public void EmitirBoleto_MedioBoleto_InformaLaMitadYElTipo()
    {
        var tarjeta = new Tarjeta { Saldo = 5000, TarjetaTipo = TarjetaTipo.MedioBoletoEstudiantil };

        var boleto = NuevoColectivo().EmitirBoleto(tarjeta);

        Assert.That(boleto!.TipoTarjeta, Is.EqualTo("Medio boleto estudiantil"));
        Assert.That(boleto.TotalAbonado, Is.EqualTo(790));
        Assert.That(boleto.Saldo, Is.EqualTo(4210));
    }

    [Test]
    public void EmitirBoleto_BoletoGratuito_NoCobraYNoCambiaElSaldo()
    {
        var tarjeta = new Tarjeta { Saldo = 500, TarjetaTipo = TarjetaTipo.BoletoGratuitoEstudiantil };

        var boleto = NuevoColectivo().EmitirBoleto(tarjeta);

        Assert.That(boleto!.TipoTarjeta, Is.EqualTo("Boleto gratuito estudiantil"));
        Assert.That(boleto.TotalAbonado, Is.EqualTo(0));
        Assert.That(boleto.Saldo, Is.EqualTo(500));
    }

    [Test]
    public void EmitirBoleto_FranquiciaCompleta_NoCobra()
    {
        var tarjeta = new Tarjeta { TarjetaTipo = TarjetaTipo.FranquiciaCompleta };

        var boleto = NuevoColectivo().EmitirBoleto(tarjeta);

        Assert.That(boleto!.TipoTarjeta, Is.EqualTo("Franquicia completa"));
        Assert.That(boleto.TotalAbonado, Is.EqualTo(0));
    }

    [Test]
    public void EmitirBoleto_ViajePlus_InformaElSaldoNegativoRestante()
    {
        var tarjeta = new Tarjeta { Saldo = 1000 };

        var boleto = NuevoColectivo().EmitirBoleto(tarjeta);

        Assert.That(boleto!.TotalAbonado, Is.EqualTo(1580));
        Assert.That(boleto.Saldo, Is.EqualTo(-580));
    }

    [Test]
    public void EmitirBoleto_DevuelveNull_SiNoSePuedePagar()
    {
        var tarjeta = new Tarjeta { Saldo = -2000 };

        Assert.That(NuevoColectivo().EmitirBoleto(tarjeta), Is.Null);
    }

    [Test]
    public void Tarjeta_NombreTipo_SinTipoAsignadoEsNormal()
    {
        Assert.That(new Tarjeta().NombreTipo, Is.EqualTo("Normal"));
    }
}
