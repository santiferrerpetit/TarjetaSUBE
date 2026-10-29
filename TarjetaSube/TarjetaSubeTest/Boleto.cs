using Microsoft.EntityFrameworkCore;
using TarjetaSube.Clases;
using TarjetaSube.Servicios;

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
        Assert.That(boleto.TotalAbonado, Is.EqualTo(2150));
        Assert.That(boleto.Saldo, Is.EqualTo(2850));
        Assert.That(boleto.IdTarjeta, Is.EqualTo(7));
    }

    [Test]
    public void EmitirBoleto_MedioBoleto_InformaLaMitadYElTipo()
    {
        var tarjeta = new Tarjeta { Saldo = 5000, TarjetaTipo = TarjetaTipo.MedioBoletoEstudiantil };

        var boleto = NuevoColectivo().EmitirBoleto(tarjeta);

        Assert.That(boleto!.TipoTarjeta, Is.EqualTo("Medio boleto estudiantil"));
        Assert.That(boleto.TotalAbonado, Is.EqualTo(1075));
        Assert.That(boleto.Saldo, Is.EqualTo(3925));
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

        Assert.That(boleto!.TotalAbonado, Is.EqualTo(2150));
        Assert.That(boleto.Saldo, Is.EqualTo(-1150));
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

    [TestCase(TarjetaTipo.NormalId, "Normal")]
    [TestCase(TarjetaTipo.MedioBoletoEstudiantilId, "Medio boleto estudiantil")]
    [TestCase(TarjetaTipo.BoletoGratuitoEstudiantilId, "Boleto gratuito estudiantil")]
    [TestCase(TarjetaTipo.FranquiciaCompletaId, "Franquicia completa")]
    public void Tarjeta_NombreTipo_DevuelveElNombreDelTipoAsignado(int tipoId, string esperado)
    {
        var tipo = TarjetaTipo.Predefinidos.Single(t => t.Id == tipoId);
        var tarjeta = new Tarjeta { TarjetaTipo = tipo };

        Assert.That(tarjeta.NombreTipo, Is.EqualTo(esperado));
    }

    [Test]
    public void Boleto_NuevoTieneValoresPorDefecto()
    {
        var boleto = new Boleto();

        Assert.That(boleto.TipoTarjeta, Is.Empty);
        Assert.That(boleto.Linea, Is.Empty);
        Assert.That(boleto.TotalAbonado, Is.EqualTo(0));
        Assert.That(boleto.Saldo, Is.EqualTo(0));
        Assert.That(boleto.IdTarjeta, Is.EqualTo(0));
        Assert.That(boleto.Tarjeta, Is.Null);
        Assert.That(boleto.Colectivo, Is.Null);
    }

    [Test]
    public void Colectivo_NuevoUsaElRelojDelSistemaPorDefecto()
    {
        Assert.That(new Colectivo().Tiempo, Is.SameAs(TimeProvider.System));
    }

    [Test]
    public void EmitirBoleto_RelacionaElBoletoConLaTarjetaYElColectivo()
    {
        var colectivo = NuevoColectivo();
        var tarjeta = new Tarjeta { Saldo = 5000 };

        var boleto = colectivo.EmitirBoleto(tarjeta);

        Assert.That(boleto!.Tarjeta, Is.SameAs(tarjeta));
        Assert.That(boleto.Colectivo, Is.SameAs(colectivo));
    }

    [Test]
    public void EmitirBoleto_UsaLaFechaDelReloj_YCambiaConElTiempo()
    {
        var reloj = new RelojFalso(Ahora);
        var colectivo = new Colectivo { Linea = "K", Tiempo = reloj };
        var tarjeta = new Tarjeta { Saldo = 10000 };

        var primero = colectivo.EmitirBoleto(tarjeta);
        reloj.Avanzar(TimeSpan.FromMinutes(10));
        var segundo = colectivo.EmitirBoleto(tarjeta);

        Assert.That(primero!.Fecha, Is.EqualTo(Ahora.DateTime));
        Assert.That(segundo!.Fecha, Is.EqualTo(Ahora.DateTime.AddMinutes(10)));
    }

    [Test]
    public void EmitirBoleto_ElSaldoDelBoletoCoincideConElDeLaTarjeta()
    {
        var colectivo = NuevoColectivo();
        var tarjeta = new Tarjeta { Saldo = 5000 };

        var primero = colectivo.EmitirBoleto(tarjeta);
        var segundo = colectivo.EmitirBoleto(tarjeta);

        Assert.That(primero!.Saldo, Is.EqualTo(2850));
        Assert.That(segundo!.Saldo, Is.EqualTo(700));
        Assert.That(tarjeta.Saldo, Is.EqualTo(segundo.Saldo));
    }

    [Test]
    public void EmitirBoleto_ConSaldoNegativoPrevio_InformaElSaldoRestanteNegativo()
    {
        var tarjeta = new Tarjeta { Saldo = -100, TarjetaTipo = TarjetaTipo.MedioBoletoEstudiantil };

        var boleto = NuevoColectivo().EmitirBoleto(tarjeta);

        Assert.That(boleto!.Saldo, Is.EqualTo(-1175));
    }

    [Test]
    public void EmitirBoleto_ConSaldoNegativoPrevio_InformaTarifaMasLoAdeudado()
    {
        var tarjeta = new Tarjeta { Saldo = -100, TarjetaTipo = TarjetaTipo.MedioBoletoEstudiantil };

        var boleto = NuevoColectivo().EmitirBoleto(tarjeta);

        Assert.That(boleto!.TotalAbonado, Is.EqualTo(1175));
        Assert.That(boleto.TotalAbonado, Is.GreaterThan(1075));
    }

    [Test]
    public void EmitirBoleto_SinSaldoNegativoPrevio_NoSumaDeuda()
    {
        var tarjeta = new Tarjeta { Saldo = 1000 };

        var boleto = NuevoColectivo().EmitirBoleto(tarjeta);

        Assert.That(boleto!.TotalAbonado, Is.EqualTo(2150));
    }

    [Test]
    public void EmitirBoleto_MedioBoletoConSaldoNegativoPrevio_SumaLaDeudaALaMitad()
    {
        var tarjeta = new Tarjeta { Saldo = -500, TarjetaTipo = TarjetaTipo.MedioBoletoEstudiantil };

        var boleto = NuevoColectivo().EmitirBoleto(tarjeta);

        Assert.That(boleto!.TotalAbonado, Is.EqualTo(1075 + 500));
    }

    [Test]
    public void EmitirBoleto_FranquiciaGratuitaConSaldoNegativo_NoSumaDeuda()
    {
        var tarjeta = new Tarjeta { Saldo = -500, TarjetaTipo = TarjetaTipo.FranquiciaCompleta };

        var boleto = NuevoColectivo().EmitirBoleto(tarjeta);

        Assert.That(boleto!.TotalAbonado, Is.EqualTo(0));
    }

    [Test]
    public void EmitirBoleto_SiNoSePuedePagar_NoModificaElSaldo()
    {
        var tarjeta = new Tarjeta { Saldo = -1000 };

        NuevoColectivo().EmitirBoleto(tarjeta);

        Assert.That(tarjeta.Saldo, Is.EqualTo(-1000));
    }

    [Test]
    public void PagarCon_DevuelveTrueSiSeEmiteBoleto_YFalseSiNo()
    {
        var colectivo = NuevoColectivo();

        Assert.That(colectivo.PagarCon(new Tarjeta { Saldo = 2000 }), Is.True);
        Assert.That(colectivo.PagarCon(new Tarjeta { Saldo = -2000 }), Is.False);
    }

    [Test]
    public void Boleto_SePersisteConTodosSusDatos()
    {
        var opciones = new DbContextOptionsBuilder<TarjetaSubeDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var boleto = new Boleto
        {
            Fecha = Ahora.DateTime,
            TipoTarjeta = "Normal",
            Linea = "144 Negro",
            TotalAbonado = 2150,
            Saldo = 2850,
            IdTarjeta = 7
        };

        using (var db = new TarjetaSubeDbContext(opciones))
        {
            db.Boletos.Add(boleto);
            db.SaveChanges();
        }

        using (var db = new TarjetaSubeDbContext(opciones))
        {
            var guardado = db.Boletos.Single();
            Assert.That(guardado.Fecha, Is.EqualTo(Ahora.DateTime));
            Assert.That(guardado.TipoTarjeta, Is.EqualTo("Normal"));
            Assert.That(guardado.Linea, Is.EqualTo("144 Negro"));
            Assert.That(guardado.TotalAbonado, Is.EqualTo(2150));
            Assert.That(guardado.Saldo, Is.EqualTo(2850));
            Assert.That(guardado.IdTarjeta, Is.EqualTo(7));
        }
    }

    [Test]
    public void EmitirBoleto_LineaInterurbana_CompletaTodosLosDatosConLaTarifaInterurbana()
    {
        var colectivo = new Colectivo { Linea = "Gálvez", EsInterurbana = true, Tiempo = new RelojFalso(Ahora) };
        var tarjeta = new Tarjeta { Id = 7, Saldo = 5000 };

        var boleto = colectivo.EmitirBoleto(tarjeta);

        Assert.That(boleto, Is.Not.Null);
        Assert.That(boleto!.Fecha, Is.EqualTo(Ahora.DateTime));
        Assert.That(boleto.TipoTarjeta, Is.EqualTo("Normal"));
        Assert.That(boleto.Linea, Is.EqualTo("Gálvez"));
        Assert.That(boleto.TotalAbonado, Is.EqualTo(3000));
        Assert.That(boleto.Saldo, Is.EqualTo(2000));
        Assert.That(boleto.IdTarjeta, Is.EqualTo(7));
    }

    [Test]
    public void EmitirBoleto_LineaInterurbana_RechazadoPorSaldoNoEmiteBoleto()
    {
        var colectivo = new Colectivo { Linea = "Baigorria", EsInterurbana = true, Tiempo = new RelojFalso(Ahora) };

        Assert.That(colectivo.EmitirBoleto(new Tarjeta { Saldo = 900 }), Is.Null);
    }

    [Test]
    public void Colectivo_EsUrbanoPorDefecto()
    {
        Assert.That(new Colectivo().EsInterurbana, Is.False);
    }

    [Test]
    public void Colectivo_SePersisteSiEsInterurbano()
    {
        var opciones = new DbContextOptionsBuilder<TarjetaSubeDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var db = new TarjetaSubeDbContext(opciones))
        {
            db.Colectivos.Add(new Colectivo { Linea = "Gálvez", EsInterurbana = true });
            db.Colectivos.Add(new Colectivo { Linea = "K" });
            db.SaveChanges();
        }

        using (var db = new TarjetaSubeDbContext(opciones))
        {
            Assert.That(db.Colectivos.Single(c => c.Linea == "Gálvez").EsInterurbana, Is.True);
            Assert.That(db.Colectivos.Single(c => c.Linea == "K").EsInterurbana, Is.False);
        }
    }

    [Test]
    public void EmitirBoleto_ViajeComun_NoEsTrasbordo()
    {
        var boleto = NuevoColectivo().EmitirBoleto(new Tarjeta { Saldo = 5000 });

        Assert.That(boleto!.EsTrasbordo, Is.False);
    }

    [Test]
    public void EmitirBoleto_Trasbordo_CompletaTodosLosDatosSinCobrar()
    {
        var reloj = new RelojFalso(Ahora);
        var primera = new Colectivo { Linea = "144 Negro", Tiempo = reloj };
        var segunda = new Colectivo { Linea = "K", Tiempo = reloj };
        var tarjeta = new Tarjeta { Id = 7, Saldo = 5000 };

        primera.EmitirBoleto(tarjeta);
        reloj.Avanzar(TimeSpan.FromMinutes(20));
        var boleto = segunda.EmitirBoleto(tarjeta);

        Assert.That(boleto!.EsTrasbordo, Is.True);
        Assert.That(boleto.Fecha, Is.EqualTo(Ahora.DateTime.AddMinutes(20)));
        Assert.That(boleto.TipoTarjeta, Is.EqualTo("Normal"));
        Assert.That(boleto.Linea, Is.EqualTo("K"));
        Assert.That(boleto.TotalAbonado, Is.EqualTo(0));
        Assert.That(boleto.Saldo, Is.EqualTo(2850));
        Assert.That(boleto.IdTarjeta, Is.EqualTo(7));
    }

    [Test]
    public void Boleto_SePersisteIndicandoSiEsTrasbordo()
    {
        var opciones = new DbContextOptionsBuilder<TarjetaSubeDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var db = new TarjetaSubeDbContext(opciones))
        {
            db.Boletos.Add(new Boleto { Linea = "K", EsTrasbordo = true });
            db.Boletos.Add(new Boleto { Linea = "144 Negro" });
            db.SaveChanges();
        }

        using (var db = new TarjetaSubeDbContext(opciones))
        {
            Assert.That(db.Boletos.Single(b => b.Linea == "K").EsTrasbordo, Is.True);
            Assert.That(db.Boletos.Single(b => b.Linea == "144 Negro").EsTrasbordo, Is.False);
        }
    }
}
