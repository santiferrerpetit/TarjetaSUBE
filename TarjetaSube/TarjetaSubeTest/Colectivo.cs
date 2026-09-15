using TarjetaSube.Clases;

namespace TarjetaSubeTest;

public class ColectivoTests
{
    [Test]
    public void PagarCon_DevuelveNull_SiElSaldoEsInsuficiente()
    {
        var colectivo = new Colectivo();
        var tarjeta = new Tarjeta();

        var boleto = colectivo.PagarCon(tarjeta);

        Assert.That(boleto, Is.Null);
    }

    [Test]
    public void PagarCon_DescuentaLaTarifaYDevuelveElBoleto_SiHaySaldo()
    {
        var colectivo = new Colectivo();
        var tarjeta = new Tarjeta();
        tarjeta.Cargar(2000);

        var boleto = colectivo.PagarCon(tarjeta);

        Assert.That(boleto, Is.Not.Null);
        Assert.That(boleto!.Monto, Is.EqualTo(1580));
        Assert.That(tarjeta.Saldo, Is.EqualTo(420));
    }

    [Test]
    public void PagarCon_PermitePagarConElSaldoJusto()
    {
        var colectivo = new Colectivo();
        var tarjeta = new Tarjeta { Saldo = 1580 };

        var boleto = colectivo.PagarCon(tarjeta);

        Assert.That(boleto, Is.Not.Null);
        Assert.That(tarjeta.Saldo, Is.EqualTo(0));
    }
}
