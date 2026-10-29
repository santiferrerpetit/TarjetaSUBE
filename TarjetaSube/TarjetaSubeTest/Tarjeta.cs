using TarjetaSube.Clases;

namespace TarjetaSubeTest;

public class TarjetaTests
{
    [TestCase(2000)]
    [TestCase(3000)]
    [TestCase(4000)]
    [TestCase(5000)]
    [TestCase(8000)]
    [TestCase(10000)]
    [TestCase(15000)]
    [TestCase(20000)]
    [TestCase(25000)]
    [TestCase(30000)]
    public void Cargar_SumaElMontoAlSaldoYDevuelveTrue(decimal monto)
    {
        var tarjeta = new Tarjeta();

        var resultado = tarjeta.Cargar(monto);

        Assert.That(resultado, Is.True);
        Assert.That(tarjeta.Saldo, Is.EqualTo(monto));
    }

    [Test]
    public void Cargar_DevuelveFalseYNoModificaElSaldo_SiElMontoNoEstaEnLaListaDeCargasAceptadas()
    {
        var tarjeta = new Tarjeta();

        var resultado = tarjeta.Cargar(1000);

        Assert.That(resultado, Is.False);
        Assert.That(tarjeta.Saldo, Is.EqualTo(0));
    }

    [Test]
    public void Cargar_AcreditaHastaElMaximoYDejaElExcedentePendiente()
    {
        var tarjeta = new Tarjeta { Saldo = 50000 };

        var resultado = tarjeta.Cargar(10000);

        Assert.That(resultado, Is.True);
        Assert.That(tarjeta.Saldo, Is.EqualTo(56000));
        Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(4000));
    }

    [Test]
    public void Cargar_PermiteLlegarJustoAlLimiteDe56000SinPendiente()
    {
        var tarjeta = new Tarjeta { Saldo = 46000 };

        var resultado = tarjeta.Cargar(10000);

        Assert.That(resultado, Is.True);
        Assert.That(tarjeta.Saldo, Is.EqualTo(56000));
        Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(0));
    }

    [Test]
    public void Cargar_SiYaEstaEnElMaximo_TodoQuedaPendiente()
    {
        var tarjeta = new Tarjeta { Saldo = 56000 };

        tarjeta.Cargar(2000);

        Assert.That(tarjeta.Saldo, Is.EqualTo(56000));
        Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(2000));
    }

    [Test]
    public void AcreditarCarga_AcreditaElPendienteHastaElMaximo()
    {
        var tarjeta = new Tarjeta { Saldo = 55000, SaldoPendiente = 4000 };

        tarjeta.AcreditarCarga();

        Assert.That(tarjeta.Saldo, Is.EqualTo(56000));
        Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(3000));
    }

    [Test]
    public void AcreditarCarga_AcreditaTodoElPendienteSiEntra()
    {
        var tarjeta = new Tarjeta { Saldo = 10000, SaldoPendiente = 2000 };

        tarjeta.AcreditarCarga();

        Assert.That(tarjeta.Saldo, Is.EqualTo(12000));
        Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(0));
    }

    [Test]
    public void AcreditarCarga_SinPendiente_NoModificaElSaldo()
    {
        var tarjeta = new Tarjeta { Saldo = 10000 };

        tarjeta.AcreditarCarga();

        Assert.That(tarjeta.Saldo, Is.EqualTo(10000));
        Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(0));
    }

    [Test]
    public void Descontar_LuegoDelViajeAcreditaElPendienteHastaElMaximo()
    {
        var tarjeta = new Tarjeta { Saldo = 56000, SaldoPendiente = 4000 };

        tarjeta.Descontar(2150);

        Assert.That(tarjeta.Saldo, Is.EqualTo(56000));
        Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(1850));
    }

    [Test]
    public void Descontar_RestaElMontoDelSaldoYDevuelveTrue_SiHaySaldoSuficiente()
    {
        var tarjeta = new Tarjeta();
        tarjeta.Cargar(3000);

        var resultado = tarjeta.Descontar(2150);

        Assert.That(resultado, Is.True);
        Assert.That(tarjeta.Saldo, Is.EqualTo(850));
    }

    [Test]
    public void Descontar_DevuelveFalseYNoModificaElSaldo_SiElSaldoEsInsuficiente()
    {
        var tarjeta = new Tarjeta();
        tarjeta.Cargar(2000);

        var resultado = tarjeta.Descontar(4500);

        Assert.That(resultado, Is.False);
        Assert.That(tarjeta.Saldo, Is.EqualTo(2000));
    }

    [Test]
    public void Descontar_PermiteDescontarElSaldoJusto()
    {
        var tarjeta = new Tarjeta();
        tarjeta.Cargar(2000);

        var resultado = tarjeta.Descontar(2000);

        Assert.That(resultado, Is.True);
        Assert.That(tarjeta.Saldo, Is.EqualTo(0));
    }

    [Test]
    public void Descontar_PermiteQuedarConSaldoNegativoHastaElLimite()
    {
        var tarjeta = new Tarjeta { Saldo = 1000 };

        var resultado = tarjeta.Descontar(3000);

        Assert.That(resultado, Is.True);
        Assert.That(tarjeta.Saldo, Is.EqualTo(-2000));
    }

    [Test]
    public void Descontar_NoPermiteQuedarPorDebajoDelSaldoNegativoMaximo()
    {
        var tarjeta = new Tarjeta { Saldo = 1000 };

        var resultado = tarjeta.Descontar(3001);

        Assert.That(resultado, Is.False);
        Assert.That(tarjeta.Saldo, Is.EqualTo(1000));
    }

    [Test]
    public void Cargar_DescuentaDelSaldoLoConsumidoEnViajesPlus()
    {
        var tarjeta = new Tarjeta { Saldo = -1160 };

        var resultado = tarjeta.Cargar(2000);

        Assert.That(resultado, Is.True);
        Assert.That(tarjeta.Saldo, Is.EqualTo(840));
    }

    [Test]
    public void TarjetaNueva_EsDeTipoNormal()
    {
        var tarjeta = new Tarjeta();

        Assert.That(tarjeta.TarjetaTipoId, Is.EqualTo(TarjetaTipo.NormalId));
    }

    [Test]
    public void CalcularPasaje_SinTipoAsignado_CobraTarifaNormal()
    {
        var tarjeta = new Tarjeta();

        Assert.That(tarjeta.CalcularPasaje(2150), Is.EqualTo(2150));
    }

    [Test]
    public void CalcularPasaje_UsaElTipoAsignado()
    {
        var tarjeta = new Tarjeta { TarjetaTipo = TarjetaTipo.MedioBoletoEstudiantil };

        Assert.That(tarjeta.CalcularPasaje(2150), Is.EqualTo(1075));
    }

    [Test]
    public void Cargar_VariasCargasConExcedenteAcumulanElPendiente()
    {
        var tarjeta = new Tarjeta { Saldo = 55000 };

        tarjeta.Cargar(5000);
        tarjeta.Cargar(3000);

        Assert.That(tarjeta.Saldo, Is.EqualTo(56000));
        Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(7000));
    }

    [Test]
    public void Cargar_MontoInvalido_NoModificaElSaldoPendiente()
    {
        var tarjeta = new Tarjeta { Saldo = 56000, SaldoPendiente = 1000 };

        Assert.That(tarjeta.Cargar(1234), Is.False);
        Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(1000));
    }

    [Test]
    public void AcreditarCarga_SiElSaldoYaEstaEnElMaximo_NoAcreditaNada()
    {
        var tarjeta = new Tarjeta { Saldo = 56000, SaldoPendiente = 2000 };

        tarjeta.AcreditarCarga();

        Assert.That(tarjeta.Saldo, Is.EqualTo(56000));
        Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(2000));
    }

    [Test]
    public void AcreditarCarga_ConSaldoNegativo_SumaElPendienteAlSaldo()
    {
        var tarjeta = new Tarjeta { Saldo = -500, SaldoPendiente = 2000 };

        tarjeta.AcreditarCarga();

        Assert.That(tarjeta.Saldo, Is.EqualTo(1500));
        Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(0));
    }

    [Test]
    public void Descontar_Rechazado_NoAcreditaElPendiente()
    {
        var tarjeta = new Tarjeta { Saldo = -1500, SaldoPendiente = 3000 };

        Assert.That(tarjeta.Descontar(1000), Is.False);
        Assert.That(tarjeta.Saldo, Is.EqualTo(-1500));
        Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(3000));
    }

    [Test]
    public void Descontar_AcreditaSoloLoQueEntraYDejaElRestoPendiente()
    {
        var tarjeta = new Tarjeta { Saldo = 56000, SaldoPendiente = 5000 };

        tarjeta.Descontar(1000);

        Assert.That(tarjeta.Saldo, Is.EqualTo(56000));
        Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(4000));
    }

    [Test]
    public void Descontar_ConPendienteMenorAlEspacio_AcreditaTodoElPendiente()
    {
        var tarjeta = new Tarjeta { Saldo = 50000, SaldoPendiente = 2000 };

        tarjeta.Descontar(1000);

        Assert.That(tarjeta.Saldo, Is.EqualTo(51000));
        Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(0));
    }

    [Test]
    public void TarjetaNueva_NoTieneViajeParaTrasbordo()
    {
        var tarjeta = new Tarjeta();

        Assert.That(tarjeta.FechaUltimoPago, Is.Null);
        Assert.That(tarjeta.LineaUltimoViaje, Is.Null);
    }

    [Test]
    public void PagarCon_RegistraLaFechaYLaLineaDelUltimoPago()
    {
        var ahora = new DateTimeOffset(2026, 10, 5, 10, 0, 0, TimeSpan.Zero);
        var colectivo = new Colectivo { Linea = "K", Tiempo = new RelojFalso(ahora) };
        var tarjeta = new Tarjeta { Saldo = 5000 };

        colectivo.PagarCon(tarjeta);

        Assert.That(tarjeta.FechaUltimoPago, Is.EqualTo(ahora.DateTime));
        Assert.That(tarjeta.LineaUltimoViaje, Is.EqualTo("K"));
    }

    [Test]
    public void PagarCon_PagoRechazadoNoRegistraElUltimoPago()
    {
        var colectivo = new Colectivo { Linea = "K" };
        var tarjeta = new Tarjeta { Saldo = -2000 };

        colectivo.PagarCon(tarjeta);

        Assert.That(tarjeta.FechaUltimoPago, Is.Null);
        Assert.That(tarjeta.LineaUltimoViaje, Is.Null);
    }

    [Test]
    public void PagarCon_UnTrasbordoActualizaLaLineaPeroNoLaFechaDelUltimoPago()
    {
        var inicio = new DateTimeOffset(2026, 10, 5, 10, 0, 0, TimeSpan.Zero);
        var reloj = new RelojFalso(inicio);
        var a = new Colectivo { Linea = "K", Tiempo = reloj };
        var b = new Colectivo { Linea = "145", Tiempo = reloj };
        var tarjeta = new Tarjeta { Saldo = 5000 };

        a.PagarCon(tarjeta);
        reloj.Avanzar(TimeSpan.FromMinutes(15));
        b.PagarCon(tarjeta);

        Assert.That(tarjeta.FechaUltimoPago, Is.EqualTo(inicio.DateTime));
        Assert.That(tarjeta.LineaUltimoViaje, Is.EqualTo("145"));
    }

    [Test]
    public void PagarCon_UnTrasbordoNoModificaSaldoNiSaldoPendiente()
    {
        var reloj = new RelojFalso(new DateTimeOffset(2026, 10, 5, 10, 0, 0, TimeSpan.Zero));
        var a = new Colectivo { Linea = "K", Tiempo = reloj };
        var b = new Colectivo { Linea = "145", Tiempo = reloj };
        var tarjeta = new Tarjeta { Saldo = 5000 };

        a.PagarCon(tarjeta);
        var saldo = tarjeta.Saldo;
        reloj.Avanzar(TimeSpan.FromMinutes(15));
        b.PagarCon(tarjeta);

        Assert.That(tarjeta.Saldo, Is.EqualTo(saldo));
        Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(0));
    }
}
