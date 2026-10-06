using TarjetaSube.Clases;

namespace TarjetaSubeTest;

public class ColectivoTests
{
    [Test]
    public void PagarCon_DevuelveFalse_SiElSaldoEsInsuficiente()
    {
        var colectivo = new Colectivo();
        var tarjeta = new Tarjeta();

        var resultado = colectivo.PagarCon(tarjeta);

        Assert.That(resultado, Is.False);
    }

    [Test]
    public void PagarCon_DescuentaLaTarifaYDevuelveTrue_SiHaySaldo()
    {
        var colectivo = new Colectivo();
        var tarjeta = new Tarjeta();
        tarjeta.Cargar(2000);

        var resultado = colectivo.PagarCon(tarjeta);

        Assert.That(resultado, Is.True);
        Assert.That(tarjeta.Saldo, Is.EqualTo(420));
    }

    [Test]
    public void PagarCon_PermitePagarConElSaldoJusto()
    {
        var colectivo = new Colectivo();
        var tarjeta = new Tarjeta { Saldo = 1580 };

        var resultado = colectivo.PagarCon(tarjeta);

        Assert.That(resultado, Is.True);
        Assert.That(tarjeta.Saldo, Is.EqualTo(0));
    }

    [Test]
    public void PagarCon_NoModificaElSaldo_SiElSaldoEsInsuficiente()
    {
        var colectivo = new Colectivo();
        var tarjeta = new Tarjeta { Saldo = 1000 };

        var resultado = colectivo.PagarCon(tarjeta);

        Assert.That(resultado, Is.False);
        Assert.That(tarjeta.Saldo, Is.EqualTo(1000));
    }

    [Test]
    public void PagarCon_DescuentaLaTarifaEnCadaViaje()
    {
        var colectivo = new Colectivo();
        var tarjeta = new Tarjeta { Saldo = 5000 };

        colectivo.PagarCon(tarjeta);
        colectivo.PagarCon(tarjeta);

        Assert.That(tarjeta.Saldo, Is.EqualTo(1840));
    }
}
