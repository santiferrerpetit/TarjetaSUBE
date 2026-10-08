using TarjetaSube.Clases;

namespace TarjetaSubeTest;

public class ColectivoTests
{
    [Test]
    public void PagarCon_DevuelveFalse_SiSeSuperaElSaldoNegativoMaximo()
    {
        var colectivo = new Colectivo();
        var tarjeta = new Tarjeta { Saldo = -1000 };

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
        var tarjeta = new Tarjeta { Saldo = -1000 };

        var resultado = colectivo.PagarCon(tarjeta);

        Assert.That(resultado, Is.False);
        Assert.That(tarjeta.Saldo, Is.EqualTo(-1000));
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

    [Test]
    public void PagarCon_PermiteViajesPlusYDescuentaElSaldoCorrectamente()
    {
        var colectivo = new Colectivo();
        var tarjeta = new Tarjeta { Saldo = 2000 };

        var primero = colectivo.PagarCon(tarjeta);
        var plus = colectivo.PagarCon(tarjeta);
        var rechazado = colectivo.PagarCon(tarjeta);

        Assert.That(primero, Is.True);
        Assert.That(plus, Is.True);
        Assert.That(rechazado, Is.False);
        Assert.That(tarjeta.Saldo, Is.EqualTo(-1160));
    }

    [Test]
    public void PagarCon_FranquiciaCompleta_SiemprePuedePagar_AunSinSaldo()
    {
        var colectivo = new Colectivo();
        var tarjeta = new Tarjeta { TarjetaTipo = TarjetaTipo.FranquiciaCompleta };

        for (var i = 0; i < 10; i++)
            Assert.That(colectivo.PagarCon(tarjeta), Is.True);

        Assert.That(tarjeta.Saldo, Is.EqualTo(0));
    }

    [Test]
    public void PagarCon_FranquiciaCompleta_PagaAunConSaldoNegativoMaximo()
    {
        var colectivo = new Colectivo();
        var tarjeta = new Tarjeta { Saldo = -2000, TarjetaTipo = TarjetaTipo.FranquiciaCompleta };

        Assert.That(colectivo.PagarCon(tarjeta), Is.True);
        Assert.That(tarjeta.Saldo, Is.EqualTo(-2000));
    }

    [Test]
    public void PagarCon_MedioBoleto_PagaSiempreLaMitadDelNormal()
    {
        var colectivo = new Colectivo();
        var normal = new Tarjeta { Saldo = 10000 };
        var medio = new Tarjeta { Saldo = 10000, TarjetaTipo = TarjetaTipo.MedioBoletoEstudiantil };

        for (var i = 0; i < 3; i++)
        {
            var saldoNormal = normal.Saldo;
            var saldoMedio = medio.Saldo;

            colectivo.PagarCon(normal);
            colectivo.PagarCon(medio);

            Assert.That(saldoMedio - medio.Saldo, Is.EqualTo((saldoNormal - normal.Saldo) / 2));
        }

        Assert.That(medio.Saldo, Is.EqualTo(10000 - 3 * 790));
    }

    [Test]
    public void PagarCon_BoletoGratuitoEstudiantil_NoDescuentaSaldo()
    {
        var colectivo = new Colectivo();
        var tarjeta = new Tarjeta { Saldo = 500, TarjetaTipo = TarjetaTipo.BoletoGratuitoEstudiantil };

        Assert.That(colectivo.PagarCon(tarjeta), Is.True);
        Assert.That(tarjeta.Saldo, Is.EqualTo(500));
    }
}
