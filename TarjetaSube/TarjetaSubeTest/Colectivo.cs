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
        var colectivo = ColectivoEn(2026, 1, 5, 10);
        var tarjeta = new Tarjeta { TarjetaTipo = TarjetaTipo.FranquiciaCompleta };

        for (var i = 0; i < 10; i++)
            Assert.That(colectivo.PagarCon(tarjeta), Is.True);

        Assert.That(tarjeta.Saldo, Is.EqualTo(0));
    }

    [Test]
    public void PagarCon_FranquiciaCompleta_PagaAunConSaldoNegativoMaximo()
    {
        var colectivo = ColectivoEn(2026, 1, 5, 10);
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
        var colectivo = ColectivoEn(2026, 1, 5, 10);
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

    private static Colectivo ColectivoEn(int anio, int mes, int dia, int hora, int minuto = 0) =>
        new() { Tiempo = new RelojFalso(new DateTimeOffset(anio, mes, dia, hora, minuto, 0, TimeSpan.Zero)) };

    private static readonly int[] TiposConFranquicia =
    [
        TarjetaTipo.MedioBoletoEstudiantilId,
        TarjetaTipo.BoletoGratuitoEstudiantilId,
        TarjetaTipo.FranquiciaCompletaId
    ];

    private static Tarjeta TarjetaDeTipo(int tipoId) => new()
    {
        Saldo = 10000,
        TarjetaTipo = TarjetaTipo.Predefinidos.Single(t => t.Id == tipoId)
    };

    // 2026-10-05 es lunes, 2026-10-09 viernes, 2026-10-10 sábado y 2026-10-11 domingo.

    [Test]
    public void PagarCon_Franquicia_PuedePagarDeLunesAViernesEntreLas6Y22([Values(5, 6, 7, 8, 9)] int dia,
        [Values(6, 12, 21)] int hora,
        [ValueSource(nameof(TiposConFranquicia))] int tipoId)
    {
        var colectivo = ColectivoEn(2026, 10, dia, hora);

        Assert.That(colectivo.PagarCon(TarjetaDeTipo(tipoId)), Is.True);
    }

    [Test]
    public void PagarCon_Franquicia_PuedePagarALas21Y59([ValueSource(nameof(TiposConFranquicia))] int tipoId)
    {
        var colectivo = ColectivoEn(2026, 10, 5, 21, 59);

        Assert.That(colectivo.PagarCon(TarjetaDeTipo(tipoId)), Is.True);
    }

    [Test]
    public void PagarCon_Franquicia_PuedePagarALas6EnPunto([ValueSource(nameof(TiposConFranquicia))] int tipoId)
    {
        var colectivo = ColectivoEn(2026, 10, 5, 6, 0);

        Assert.That(colectivo.PagarCon(TarjetaDeTipo(tipoId)), Is.True);
    }

    [Test]
    public void PagarCon_Franquicia_NoPuedePagarALas22EnPunto([ValueSource(nameof(TiposConFranquicia))] int tipoId)
    {
        var colectivo = ColectivoEn(2026, 10, 5, 22, 0);

        Assert.That(colectivo.PagarCon(TarjetaDeTipo(tipoId)), Is.False);
    }

    [Test]
    public void PagarCon_Franquicia_NoPuedePagarALas5Y59([ValueSource(nameof(TiposConFranquicia))] int tipoId)
    {
        var colectivo = ColectivoEn(2026, 10, 5, 5, 59);

        Assert.That(colectivo.PagarCon(TarjetaDeTipo(tipoId)), Is.False);
    }

    [Test]
    public void PagarCon_Franquicia_NoPuedePagarDeNoche([Values(0, 3, 5, 22, 23)] int hora,
        [ValueSource(nameof(TiposConFranquicia))] int tipoId)
    {
        var colectivo = ColectivoEn(2026, 10, 6, hora);

        Assert.That(colectivo.PagarCon(TarjetaDeTipo(tipoId)), Is.False);
    }

    [Test]
    public void PagarCon_Franquicia_NoPuedePagarSabadoNiDomingo([Values(10, 11)] int dia,
        [Values(0, 6, 10, 15, 21, 23)] int hora,
        [ValueSource(nameof(TiposConFranquicia))] int tipoId)
    {
        var colectivo = ColectivoEn(2026, 10, dia, hora);

        Assert.That(colectivo.PagarCon(TarjetaDeTipo(tipoId)), Is.False);
    }

    [Test]
    public void EmitirBoleto_Franquicia_FueraDeHorarioNoEmiteBoletoNiModificaLaTarjeta(
        [ValueSource(nameof(TiposConFranquicia))] int tipoId)
    {
        var colectivo = ColectivoEn(2026, 10, 10, 12);
        var tarjeta = TarjetaDeTipo(tipoId);

        var boleto = colectivo.EmitirBoleto(tarjeta);

        Assert.That(boleto, Is.Null);
        Assert.That(tarjeta.Saldo, Is.EqualTo(10000));
        Assert.That(tarjeta.UltimoViaje, Is.Null);
        Assert.That(tarjeta.ViajesMedioBoletoHoy, Is.EqualTo(0));
        Assert.That(tarjeta.ViajesGratuitosHoy, Is.EqualTo(0));
    }

    [Test]
    public void PagarCon_FranquiciaCompleta_FueraDeHorarioNoPuedePagarAunConSaldo()
    {
        var colectivo = ColectivoEn(2026, 10, 11, 12);
        var tarjeta = new Tarjeta { Saldo = 50000, TarjetaTipo = TarjetaTipo.FranquiciaCompleta };

        Assert.That(colectivo.PagarCon(tarjeta), Is.False);
    }

    [Test]
    public void PagarCon_Franquicia_VuelveAPoderPagarAlEntrarEnLaFranja()
    {
        var reloj = new RelojFalso(new DateTimeOffset(2026, 10, 5, 5, 0, 0, TimeSpan.Zero));
        var colectivo = new Colectivo { Tiempo = reloj };
        var tarjeta = TarjetaDeTipo(TarjetaTipo.BoletoGratuitoEstudiantilId);

        Assert.That(colectivo.PagarCon(tarjeta), Is.False);

        reloj.Avanzar(TimeSpan.FromHours(1));

        Assert.That(colectivo.PagarCon(tarjeta), Is.True);
    }

    [TestCase(5, 6)]
    [TestCase(5, 12)]
    [TestCase(5, 21)]
    [TestCase(10, 12)]
    [TestCase(11, 3)]
    [TestCase(6, 23)]
    public void PagarCon_TarjetaNormal_PuedePagarEnCualquierDiaYHorario(int dia, int hora)
    {
        var colectivo = ColectivoEn(2026, 10, dia, hora);
        var tarjeta = new Tarjeta { Saldo = 5000 };

        Assert.That(colectivo.PagarCon(tarjeta), Is.True);
    }

    // Líneas interurbanas

    [Test]
    public void EmitirBoleto_LineaInterurbana_CobraLaTarifaInterurbana()
    {
        var colectivo = new Colectivo { Linea = "Gálvez", EsInterurbana = true };
        var tarjeta = new Tarjeta { Saldo = 10000 };

        var boleto = colectivo.EmitirBoleto(tarjeta);

        Assert.That(boleto!.TotalAbonado, Is.EqualTo(3000));
        Assert.That(tarjeta.Saldo, Is.EqualTo(7000));
    }

    [Test]
    public void EmitirBoleto_LineaUrbana_NoEsInterurbanaPorDefecto()
    {
        var colectivo = new Colectivo();

        Assert.That(colectivo.EsInterurbana, Is.False);
    }

    [Test]
    public void EmitirBoleto_LineaInterurbana_MedioBoletoPagaLaMitad()
    {
        var colectivo = new Colectivo { EsInterurbana = true, Tiempo = new RelojFalso(new DateTimeOffset(2026, 10, 5, 10, 0, 0, TimeSpan.Zero)) };
        var tarjeta = TarjetaDeTipo(TarjetaTipo.MedioBoletoEstudiantilId);

        var boleto = colectivo.EmitirBoleto(tarjeta);

        Assert.That(boleto!.TotalAbonado, Is.EqualTo(1500));
    }

    [Test]
    public void EmitirBoleto_LineaInterurbana_MedioBoletoPagaTarifaCompletaInterurbanaDesdeElTercerViaje()
    {
        var reloj = new RelojFalso(new DateTimeOffset(2026, 10, 5, 10, 0, 0, TimeSpan.Zero));
        var colectivo = new Colectivo { EsInterurbana = true, Tiempo = reloj };
        var tarjeta = TarjetaDeTipo(TarjetaTipo.MedioBoletoEstudiantilId);

        colectivo.PagarCon(tarjeta);
        reloj.Avanzar(TimeSpan.FromMinutes(5));
        colectivo.PagarCon(tarjeta);
        reloj.Avanzar(TimeSpan.FromMinutes(5));
        var tercero = colectivo.EmitirBoleto(tarjeta);

        Assert.That(tercero!.TotalAbonado, Is.EqualTo(3000));
    }

    [TestCase(TarjetaTipo.BoletoGratuitoEstudiantilId)]
    [TestCase(TarjetaTipo.FranquiciaCompletaId)]
    public void EmitirBoleto_LineaInterurbana_FranquiciasGratuitasNoPagan(int tipoId)
    {
        var colectivo = new Colectivo { EsInterurbana = true, Tiempo = new RelojFalso(new DateTimeOffset(2026, 10, 5, 10, 0, 0, TimeSpan.Zero)) };
        var tarjeta = TarjetaDeTipo(tipoId);

        var boleto = colectivo.EmitirBoleto(tarjeta);

        Assert.That(boleto!.TotalAbonado, Is.EqualTo(0));
        Assert.That(tarjeta.Saldo, Is.EqualTo(10000));
    }

    [Test]
    public void PagarCon_LineaInterurbana_LasFranquiciasSiguenLaFranjaHoraria()
    {
        var colectivo = new Colectivo { EsInterurbana = true, Tiempo = new RelojFalso(new DateTimeOffset(2026, 10, 10, 10, 0, 0, TimeSpan.Zero)) };

        Assert.That(colectivo.PagarCon(TarjetaDeTipo(TarjetaTipo.FranquiciaCompletaId)), Is.False);
    }

    [Test]
    public void EmitirBoleto_LineaInterurbana_AplicaUsoFrecuenteSobreLaTarifaInterurbana()
    {
        var (_, tarjeta, reloj) = UsoFrecuente(40);
        var colectivo = new Colectivo { EsInterurbana = true, Tiempo = reloj };

        var boleto = colectivo.EmitirBoleto(tarjeta);

        Assert.That(boleto!.TotalAbonado, Is.EqualTo(2400));
    }

    [Test]
    public void EmitirBoleto_LineaInterurbana_RechazaSiNoAlcanzaElSaldoNegativoMaximo()
    {
        var colectivo = new Colectivo { EsInterurbana = true };
        var tarjeta = new Tarjeta { Saldo = 900 };

        Assert.That(colectivo.PagarCon(tarjeta), Is.False);
        Assert.That(tarjeta.Saldo, Is.EqualTo(900));
    }

    // Trasbordos. 2026-10-05 es lunes y 2026-10-10 sábado.

    private static (Colectivo a, Colectivo b, RelojFalso reloj) DosLineas(int dia = 5, int hora = 10)
    {
        var reloj = new RelojFalso(new DateTimeOffset(2026, 10, dia, hora, 0, 0, TimeSpan.Zero));
        return (new Colectivo { Linea = "K", Tiempo = reloj }, new Colectivo { Linea = "145", Tiempo = reloj }, reloj);
    }

    [Test]
    public void EmitirBoleto_Trasbordo_EntreLineasDistintasDentroDeLaHoraEsGratisYSeIndicaEnElBoleto()
    {
        var (a, b, reloj) = DosLineas();
        var tarjeta = new Tarjeta { Saldo = 10000 };

        var primero = a.EmitirBoleto(tarjeta);
        reloj.Avanzar(TimeSpan.FromMinutes(30));
        var segundo = b.EmitirBoleto(tarjeta);

        Assert.That(primero!.EsTrasbordo, Is.False);
        Assert.That(segundo!.EsTrasbordo, Is.True);
        Assert.That(segundo.TotalAbonado, Is.EqualTo(0));
        Assert.That(segundo.Linea, Is.EqualTo("145"));
        Assert.That(tarjeta.Saldo, Is.EqualTo(10000 - 2150));
        Assert.That(segundo.Saldo, Is.EqualTo(10000 - 2150));
    }

    [Test]
    public void EmitirBoleto_Trasbordo_NoHayEnLaMismaLinea()
    {
        var (a, _, reloj) = DosLineas();
        var tarjeta = new Tarjeta { Saldo = 10000 };

        a.PagarCon(tarjeta);
        reloj.Avanzar(TimeSpan.FromMinutes(10));
        var segundo = a.EmitirBoleto(tarjeta);

        Assert.That(segundo!.EsTrasbordo, Is.False);
        Assert.That(tarjeta.Saldo, Is.EqualTo(10000 - 2150 - 2150));
    }

    [Test]
    public void EmitirBoleto_Trasbordo_PasadaUnaHoraSeCobraNormal()
    {
        var (a, b, reloj) = DosLineas();
        var tarjeta = new Tarjeta { Saldo = 10000 };

        a.PagarCon(tarjeta);
        reloj.Avanzar(TimeSpan.FromMinutes(61));
        var segundo = b.EmitirBoleto(tarjeta);

        Assert.That(segundo!.EsTrasbordo, Is.False);
        Assert.That(segundo.TotalAbonado, Is.EqualTo(2150));
    }

    [Test]
    public void EmitirBoleto_Trasbordo_EnElLimiteDeUnaHoraSigueSiendoGratis()
    {
        var (a, b, reloj) = DosLineas();
        var tarjeta = new Tarjeta { Saldo = 10000 };

        a.PagarCon(tarjeta);
        reloj.Avanzar(TimeSpan.FromHours(1));

        Assert.That(b.EmitirBoleto(tarjeta)!.EsTrasbordo, Is.True);
    }

    [Test]
    public void EmitirBoleto_Trasbordo_NoHayLimiteDeCantidadDentroDeLaHora()
    {
        var (a, b, reloj) = DosLineas();
        var tarjeta = new Tarjeta { Saldo = 10000 };

        a.PagarCon(tarjeta);
        for (var i = 0; i < 4; i++)
        {
            reloj.Avanzar(TimeSpan.FromMinutes(10));
            var destino = i % 2 == 0 ? b : a;
            Assert.That(destino.EmitirBoleto(tarjeta)!.EsTrasbordo, Is.True);
        }

        Assert.That(tarjeta.Saldo, Is.EqualTo(10000 - 2150));
    }

    [Test]
    public void EmitirBoleto_Trasbordo_LaHoraSeCuentaDesdeElUltimoBoletoPagadoNoDesdeElTrasbordo()
    {
        var (a, b, reloj) = DosLineas();
        var tarjeta = new Tarjeta { Saldo = 10000 };

        a.PagarCon(tarjeta);
        reloj.Avanzar(TimeSpan.FromMinutes(50));
        b.PagarCon(tarjeta);
        reloj.Avanzar(TimeSpan.FromMinutes(20));
        var tercero = a.EmitirBoleto(tarjeta);

        Assert.That(tercero!.EsTrasbordo, Is.False);
        Assert.That(tercero.TotalAbonado, Is.EqualTo(2150));
    }

    [Test]
    public void EmitirBoleto_Trasbordo_UnTrasbordoNoCuentaParaElUsoFrecuente()
    {
        var (a, b, reloj) = DosLineas();
        var tarjeta = new Tarjeta { Saldo = 10000 };

        a.PagarCon(tarjeta);
        reloj.Avanzar(TimeSpan.FromMinutes(10));
        b.PagarCon(tarjeta);

        Assert.That(tarjeta.ViajesDelMes, Is.EqualTo(1));
    }

    [Test]
    public void EmitirBoleto_Trasbordo_PuedeHacerseElSabado()
    {
        var (a, b, reloj) = DosLineas(dia: 10);
        var tarjeta = new Tarjeta { Saldo = 10000 };

        a.PagarCon(tarjeta);
        reloj.Avanzar(TimeSpan.FromMinutes(10));

        Assert.That(b.EmitirBoleto(tarjeta)!.EsTrasbordo, Is.True);
    }

    [Test]
    public void EmitirBoleto_Trasbordo_NoPuedeHacerseElDomingo()
    {
        var (a, b, reloj) = DosLineas(dia: 11);
        var tarjeta = new Tarjeta { Saldo = 10000 };

        a.PagarCon(tarjeta);
        reloj.Avanzar(TimeSpan.FromMinutes(10));

        Assert.That(b.EmitirBoleto(tarjeta)!.EsTrasbordo, Is.False);
        Assert.That(tarjeta.Saldo, Is.EqualTo(10000 - 2150 - 2150));
    }

    [Test]
    public void EmitirBoleto_Trasbordo_NoPuedeHacerseAntesDeLas7()
    {
        var reloj = new RelojFalso(new DateTimeOffset(2026, 10, 5, 6, 30, 0, TimeSpan.Zero));
        var a = new Colectivo { Linea = "K", Tiempo = reloj };
        var b = new Colectivo { Linea = "145", Tiempo = reloj };
        var tarjeta = new Tarjeta { Saldo = 10000 };

        a.PagarCon(tarjeta);
        reloj.Avanzar(TimeSpan.FromMinutes(10));

        Assert.That(b.EmitirBoleto(tarjeta)!.EsTrasbordo, Is.False);
    }

    [Test]
    public void EmitirBoleto_Trasbordo_PuedeHacerseDesdeLas7EnPunto()
    {
        var reloj = new RelojFalso(new DateTimeOffset(2026, 10, 5, 6, 55, 0, TimeSpan.Zero));
        var a = new Colectivo { Linea = "K", Tiempo = reloj };
        var b = new Colectivo { Linea = "145", Tiempo = reloj };
        var tarjeta = new Tarjeta { Saldo = 10000 };

        a.PagarCon(tarjeta);
        reloj.Avanzar(TimeSpan.FromMinutes(5));

        Assert.That(b.EmitirBoleto(tarjeta)!.EsTrasbordo, Is.True);
    }

    [Test]
    public void EmitirBoleto_Trasbordo_PuedeHacerseALas2159()
    {
        var reloj = new RelojFalso(new DateTimeOffset(2026, 10, 5, 21, 40, 0, TimeSpan.Zero));
        var a = new Colectivo { Linea = "K", Tiempo = reloj };
        var b = new Colectivo { Linea = "145", Tiempo = reloj };
        var tarjeta = new Tarjeta { Saldo = 10000 };

        a.PagarCon(tarjeta);
        reloj.Avanzar(TimeSpan.FromMinutes(19));

        Assert.That(b.EmitirBoleto(tarjeta)!.EsTrasbordo, Is.True);
    }

    [Test]
    public void EmitirBoleto_Trasbordo_NoPuedeHacerseALas22EnPunto()
    {
        var reloj = new RelojFalso(new DateTimeOffset(2026, 10, 5, 21, 40, 0, TimeSpan.Zero));
        var a = new Colectivo { Linea = "K", Tiempo = reloj };
        var b = new Colectivo { Linea = "145", Tiempo = reloj };
        var tarjeta = new Tarjeta { Saldo = 10000 };

        a.PagarCon(tarjeta);
        reloj.Avanzar(TimeSpan.FromMinutes(20));

        Assert.That(b.EmitirBoleto(tarjeta)!.EsTrasbordo, Is.False);
    }

    [Test]
    public void EmitirBoleto_Trasbordo_SinViajePreviosNoHayTrasbordo()
    {
        var (_, b, _) = DosLineas();
        var tarjeta = new Tarjeta { Saldo = 10000 };

        Assert.That(b.EmitirBoleto(tarjeta)!.EsTrasbordo, Is.False);
    }

    [Test]
    public void EmitirBoleto_Trasbordo_FuncionaConSaldoNegativoSinAgregarDeuda()
    {
        var (a, b, reloj) = DosLineas();
        var tarjeta = new Tarjeta { Saldo = 500 };

        a.PagarCon(tarjeta);
        reloj.Avanzar(TimeSpan.FromMinutes(5));
        var trasbordo = b.EmitirBoleto(tarjeta);

        Assert.That(trasbordo!.EsTrasbordo, Is.True);
        Assert.That(trasbordo.TotalAbonado, Is.EqualTo(0));
        Assert.That(tarjeta.Saldo, Is.EqualTo(500 - 2150));
    }

    [Test]
    public void EmitirBoleto_Trasbordo_SeAplicaAUnaFranquiciaDentroDeSuHorario()
    {
        var (a, b, reloj) = DosLineas();
        var tarjeta = TarjetaDeTipo(TarjetaTipo.MedioBoletoEstudiantilId);

        a.PagarCon(tarjeta);
        reloj.Avanzar(TimeSpan.FromMinutes(10));
        var segundo = b.EmitirBoleto(tarjeta);

        Assert.That(segundo!.EsTrasbordo, Is.True);
        Assert.That(tarjeta.Saldo, Is.EqualTo(10000 - 1075));
    }

    [Test]
    public void EmitirBoleto_Trasbordo_UnaFranquiciaSigueSinPoderViajarFueraDeLaFranja()
    {
        var (a, b, reloj) = DosLineas(dia: 10);
        var tarjeta = TarjetaDeTipo(TarjetaTipo.MedioBoletoEstudiantilId);

        a.PagarCon(tarjeta);
        reloj.Avanzar(TimeSpan.FromMinutes(10));

        Assert.That(b.EmitirBoleto(tarjeta), Is.Null);
    }

    [Test]
    public void EmitirBoleto_Trasbordo_EntreUrbanaEInterurbanaEsGratis()
    {
        var reloj = new RelojFalso(new DateTimeOffset(2026, 10, 5, 10, 0, 0, TimeSpan.Zero));
        var urbana = new Colectivo { Linea = "K", Tiempo = reloj };
        var interurbana = new Colectivo { Linea = "Baigorria", EsInterurbana = true, Tiempo = reloj };
        var tarjeta = new Tarjeta { Saldo = 10000 };

        urbana.PagarCon(tarjeta);
        reloj.Avanzar(TimeSpan.FromMinutes(15));
        var boleto = interurbana.EmitirBoleto(tarjeta);

        Assert.That(boleto!.EsTrasbordo, Is.True);
        Assert.That(tarjeta.Saldo, Is.EqualTo(10000 - 2150));
    }
}
