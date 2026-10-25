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
        tarjeta.Cargar(3000);

        var resultado = colectivo.PagarCon(tarjeta);

        Assert.That(resultado, Is.True);
        Assert.That(tarjeta.Saldo, Is.EqualTo(850));
    }

    [Test]
    public void PagarCon_PermitePagarConElSaldoJusto()
    {
        var colectivo = new Colectivo();
        var tarjeta = new Tarjeta { Saldo = 2150 };

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

        Assert.That(tarjeta.Saldo, Is.EqualTo(700));
    }

    [Test]
    public void PagarCon_PermiteViajesPlusYDescuentaElSaldoCorrectamente()
    {
        var colectivo = new Colectivo();
        var tarjeta = new Tarjeta { Saldo = 3000 };

        var primero = colectivo.PagarCon(tarjeta);
        var plus = colectivo.PagarCon(tarjeta);
        var rechazado = colectivo.PagarCon(tarjeta);

        Assert.That(primero, Is.True);
        Assert.That(plus, Is.True);
        Assert.That(rechazado, Is.False);
        Assert.That(tarjeta.Saldo, Is.EqualTo(-1300));
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
    public void PagarCon_MedioBoleto_PagaLaMitadDelNormalEnLosPrimerosDosViajes()
    {
        var reloj = new RelojFalso(new DateTimeOffset(2026, 1, 5, 10, 0, 0, TimeSpan.Zero));
        var colectivo = new Colectivo { Tiempo = reloj };
        var normal = new Tarjeta { Saldo = 10000 };
        var medio = new Tarjeta { Saldo = 10000, TarjetaTipo = TarjetaTipo.MedioBoletoEstudiantil };

        for (var i = 0; i < 2; i++)
        {
            var saldoNormal = normal.Saldo;
            var saldoMedio = medio.Saldo;

            colectivo.PagarCon(normal);
            colectivo.PagarCon(medio);
            reloj.Avanzar(TimeSpan.FromMinutes(5));

            Assert.That(saldoMedio - medio.Saldo, Is.EqualTo((saldoNormal - normal.Saldo) / 2));
        }

        Assert.That(medio.Saldo, Is.EqualTo(10000 - 2 * 1075));
    }

    [Test]
    public void PagarCon_BoletoGratuitoEstudiantil_NoDescuentaSaldo()
    {
        var colectivo = new Colectivo();
        var tarjeta = new Tarjeta { Saldo = 500, TarjetaTipo = TarjetaTipo.BoletoGratuitoEstudiantil };

        Assert.That(colectivo.PagarCon(tarjeta), Is.True);
        Assert.That(tarjeta.Saldo, Is.EqualTo(500));
    }

    [Test]
    public void PagarCon_MedioBoleto_RechazaViajeAntesDeLos5Minutos()
    {
        var reloj = new RelojFalso(new DateTimeOffset(2026, 1, 5, 10, 0, 0, TimeSpan.Zero));
        var colectivo = new Colectivo { Tiempo = reloj };
        var tarjeta = new Tarjeta { Saldo = 10000, TarjetaTipo = TarjetaTipo.MedioBoletoEstudiantil };

        Assert.That(colectivo.PagarCon(tarjeta), Is.True);

        reloj.Avanzar(TimeSpan.FromMinutes(4));
        Assert.That(colectivo.PagarCon(tarjeta), Is.False);
        Assert.That(tarjeta.Saldo, Is.EqualTo(10000 - 1075));

        reloj.Avanzar(TimeSpan.FromMinutes(1));
        Assert.That(colectivo.PagarCon(tarjeta), Is.True);
    }

    [Test]
    public void PagarCon_MedioBoleto_TercerViajeDelDiaCobraTarifaCompleta()
    {
        var reloj = new RelojFalso(new DateTimeOffset(2026, 1, 5, 10, 0, 0, TimeSpan.Zero));
        var colectivo = new Colectivo { Tiempo = reloj };
        var tarjeta = new Tarjeta { Saldo = 10000, TarjetaTipo = TarjetaTipo.MedioBoletoEstudiantil };

        for (var i = 0; i < 3; i++)
        {
            Assert.That(colectivo.PagarCon(tarjeta), Is.True);
            reloj.Avanzar(TimeSpan.FromMinutes(5));
        }

        Assert.That(tarjeta.Saldo, Is.EqualTo(10000 - 1075 - 1075 - 2150));
    }

    [Test]
    public void PagarCon_BoletoGratuito_PermiteDosViajesGratisPorDia()
    {
        var reloj = new RelojFalso(new DateTimeOffset(2026, 1, 5, 10, 0, 0, TimeSpan.Zero));
        var colectivo = new Colectivo { Tiempo = reloj };
        var tarjeta = new Tarjeta { Saldo = 10000, TarjetaTipo = TarjetaTipo.BoletoGratuitoEstudiantil };

        Assert.That(colectivo.PagarCon(tarjeta), Is.True);
        Assert.That(colectivo.PagarCon(tarjeta), Is.True);

        Assert.That(tarjeta.Saldo, Is.EqualTo(10000));
    }

    [Test]
    public void PagarCon_BoletoGratuito_ViajesPosterioresAlSegundoCobranTarifaCompleta()
    {
        var reloj = new RelojFalso(new DateTimeOffset(2026, 1, 5, 10, 0, 0, TimeSpan.Zero));
        var colectivo = new Colectivo { Tiempo = reloj };
        var tarjeta = new Tarjeta { Saldo = 10000, TarjetaTipo = TarjetaTipo.BoletoGratuitoEstudiantil };

        colectivo.PagarCon(tarjeta);
        colectivo.PagarCon(tarjeta);
        colectivo.PagarCon(tarjeta);
        colectivo.PagarCon(tarjeta);

        Assert.That(tarjeta.Saldo, Is.EqualTo(10000 - 2150 - 2150));
    }

    [Test]
    public void PagarCon_BoletoGratuito_ReiniciaElContadorAlDiaSiguiente()
    {
        var reloj = new RelojFalso(new DateTimeOffset(2026, 1, 5, 10, 0, 0, TimeSpan.Zero));
        var colectivo = new Colectivo { Tiempo = reloj };
        var tarjeta = new Tarjeta { Saldo = 10000, TarjetaTipo = TarjetaTipo.BoletoGratuitoEstudiantil };

        colectivo.PagarCon(tarjeta);
        colectivo.PagarCon(tarjeta);
        reloj.Avanzar(TimeSpan.FromDays(1));
        colectivo.PagarCon(tarjeta);

        Assert.That(tarjeta.Saldo, Is.EqualTo(10000));
    }

    [Test]
    public void PagarCon_LuegoDelViajeRecargaElPendienteHastaElMaximo()
    {
        var colectivo = new Colectivo();
        var tarjeta = new Tarjeta { Saldo = 56000, SaldoPendiente = 3000 };

        colectivo.PagarCon(tarjeta);

        Assert.That(tarjeta.Saldo, Is.EqualTo(56000));
        Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(850));
    }
}
