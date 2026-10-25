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

    private static (Colectivo colectivo, Tarjeta tarjeta, RelojFalso reloj) UsoFrecuente(
        int viajesPrevios, TarjetaTipo? tipo = null)
    {
        var reloj = new RelojFalso(new DateTimeOffset(2026, 10, 5, 10, 0, 0, TimeSpan.Zero));
        var colectivo = new Colectivo { Tiempo = reloj };
        var tarjeta = new Tarjeta
        {
            Saldo = 56000,
            TarjetaTipo = tipo,
            MesViajesFrecuentes = new DateTime(2026, 10, 1),
            ViajesDelMes = viajesPrevios
        };
        return (colectivo, tarjeta, reloj);
    }

    [TestCase(0, 2150)]
    [TestCase(27, 2150)]
    [TestCase(28, 2150)]
    [TestCase(29, 1720)]
    [TestCase(30, 1720)]
    [TestCase(77, 1720)]
    [TestCase(78, 1720)]
    [TestCase(79, 1612.5)]
    [TestCase(80, 1612.5)]
    [TestCase(200, 1612.5)]
    public void EmitirBoleto_UsoFrecuente_AplicaElDescuentoSegunLaCantidadDeViajesDelMes(int viajesPrevios, double esperado)
    {
        var (colectivo, tarjeta, _) = UsoFrecuente(viajesPrevios);

        var boleto = colectivo.EmitirBoleto(tarjeta);

        Assert.That(boleto!.TotalAbonado, Is.EqualTo((decimal)esperado));
        Assert.That(tarjeta.Saldo, Is.EqualTo(56000 - (decimal)esperado));
    }

    [Test]
    public void PagarCon_CuentaLosViajesDelMesDeLaTarjetaNormal()
    {
        var (colectivo, tarjeta, _) = UsoFrecuente(0);

        for (var i = 0; i < 3; i++)
            colectivo.PagarCon(tarjeta);

        Assert.That(tarjeta.ViajesDelMes, Is.EqualTo(3));
    }

    [Test]
    public void PagarCon_ElViaje30DelMesTieneDescuentoDel20PorCiento()
    {
        var (colectivo, tarjeta, _) = UsoFrecuente(0);
        tarjeta.Saldo = 100000;

        for (var i = 0; i < 29; i++)
            colectivo.PagarCon(tarjeta);
        var saldoAntes = tarjeta.Saldo;
        colectivo.PagarCon(tarjeta);

        Assert.That(saldoAntes - tarjeta.Saldo, Is.EqualTo(1720));
        Assert.That(tarjeta.ViajesDelMes, Is.EqualTo(30));
    }

    [Test]
    public void PagarCon_ElViaje80DelMesTieneDescuentoDel25PorCiento()
    {
        var (colectivo, tarjeta, _) = UsoFrecuente(78);

        colectivo.PagarCon(tarjeta);
        var saldoAntes = tarjeta.Saldo;
        colectivo.PagarCon(tarjeta);

        Assert.That(saldoAntes - tarjeta.Saldo, Is.EqualTo(1612.5m));
        Assert.That(tarjeta.ViajesDelMes, Is.EqualTo(80));
    }

    [Test]
    public void EmitirBoleto_UsoFrecuente_ReiniciaLaCuentaAlCambiarDeMes()
    {
        var (colectivo, tarjeta, reloj) = UsoFrecuente(100);
        reloj.Avanzar(TimeSpan.FromDays(31));

        var boleto = colectivo.EmitirBoleto(tarjeta);

        Assert.That(boleto!.TotalAbonado, Is.EqualTo(2150));
        Assert.That(tarjeta.ViajesDelMes, Is.EqualTo(1));
        Assert.That(tarjeta.MesViajesFrecuentes, Is.EqualTo(new DateTime(2026, 11, 1)));
    }

    [Test]
    public void EmitirBoleto_UsoFrecuente_NoReiniciaLaCuentaDentroDelMismoMes()
    {
        var (colectivo, tarjeta, reloj) = UsoFrecuente(30);
        reloj.Avanzar(TimeSpan.FromDays(20));

        var boleto = colectivo.EmitirBoleto(tarjeta);

        Assert.That(boleto!.TotalAbonado, Is.EqualTo(1720));
        Assert.That(tarjeta.ViajesDelMes, Is.EqualTo(31));
    }

    [Test]
    public void EmitirBoleto_UsoFrecuente_ReiniciaLaCuentaCuandoCambiaElAnioConElMismoMes()
    {
        var (colectivo, tarjeta, reloj) = UsoFrecuente(100);
        reloj.Avanzar(TimeSpan.FromDays(365));

        var boleto = colectivo.EmitirBoleto(tarjeta);

        Assert.That(boleto!.TotalAbonado, Is.EqualTo(2150));
        Assert.That(tarjeta.ViajesDelMes, Is.EqualTo(1));
    }

    [Test]
    public void EmitirBoleto_UsoFrecuente_ElPrimerViajeInicializaElMes()
    {
        var reloj = new RelojFalso(new DateTimeOffset(2026, 10, 5, 10, 0, 0, TimeSpan.Zero));
        var colectivo = new Colectivo { Tiempo = reloj };
        var tarjeta = new Tarjeta { Saldo = 5000 };

        colectivo.EmitirBoleto(tarjeta);

        Assert.That(tarjeta.MesViajesFrecuentes, Is.EqualTo(new DateTime(2026, 10, 1)));
        Assert.That(tarjeta.ViajesDelMes, Is.EqualTo(1));
    }

    [Test]
    public void EmitirBoleto_UsoFrecuente_NoContaNiDescuentaSiElPagoEsRechazado()
    {
        var (colectivo, tarjeta, _) = UsoFrecuente(30);
        tarjeta.Saldo = -2000;

        var boleto = colectivo.EmitirBoleto(tarjeta);

        Assert.That(boleto, Is.Null);
        Assert.That(tarjeta.ViajesDelMes, Is.EqualTo(30));
    }

    [TestCase(TarjetaTipo.MedioBoletoEstudiantilId, 1075)]
    [TestCase(TarjetaTipo.BoletoGratuitoEstudiantilId, 0)]
    [TestCase(TarjetaTipo.FranquiciaCompletaId, 0)]
    public void EmitirBoleto_UsoFrecuente_NoAplicaAOtrosTiposDeTarjeta(int tipoId, int esperado)
    {
        var tipo = TarjetaTipo.Predefinidos.Single(t => t.Id == tipoId);
        var (colectivo, tarjeta, _) = UsoFrecuente(100, tipo);

        var boleto = colectivo.EmitirBoleto(tarjeta);

        Assert.That(boleto!.TotalAbonado, Is.EqualTo(esperado));
        Assert.That(tarjeta.ViajesDelMes, Is.EqualTo(100));
    }

    [Test]
    public void EmitirBoleto_UsoFrecuente_ElBoletoInformaElMontoConDescuento()
    {
        var (colectivo, tarjeta, _) = UsoFrecuente(40);

        var boleto = colectivo.EmitirBoleto(tarjeta);

        Assert.That(boleto!.TotalAbonado, Is.EqualTo(1720));
        Assert.That(boleto.Saldo, Is.EqualTo(56000 - 1720));
    }
}
