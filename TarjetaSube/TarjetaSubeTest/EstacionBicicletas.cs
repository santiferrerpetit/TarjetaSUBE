using Microsoft.EntityFrameworkCore;
using TarjetaSube.Clases;
using TarjetaSube.Servicios;

namespace TarjetaSubeTest;

public class EstacionBicicletasTests
{
    private static Tarjeta TarjetaDeTipo(int tipoId, decimal saldo) => new()
    {
        Saldo = saldo,
        TarjetaTipo = TarjetaTipo.Predefinidos.Single(t => t.Id == tipoId)
    };

    [Test]
    public void TarifaPorDia_Es2418Con75()
    {
        Assert.That(EstacionBicicletas.TarifaPorDia, Is.EqualTo(2418.75m));
    }

    [Test]
    public void RetirarBici_ConSaldoSuficienteDevuelveTrueYCobraLaTarifa()
    {
        var estacion = new EstacionBicicletas();
        var tarjeta = new Tarjeta { Saldo = 5000 };

        var resultado = estacion.RetirarBici(tarjeta);

        Assert.That(resultado, Is.True);
        Assert.That(tarjeta.Saldo, Is.EqualTo(5000 - 2418.75m));
    }

    [Test]
    public void RetirarBici_ConSaldoExactoDejaLaTarjetaEnCero()
    {
        var estacion = new EstacionBicicletas();
        var tarjeta = new Tarjeta { Saldo = 2418.75m };

        Assert.That(estacion.RetirarBici(tarjeta), Is.True);
        Assert.That(tarjeta.Saldo, Is.EqualTo(0));
    }

    [Test]
    public void RetirarBici_SinSaldoSuficienteUsaElSaldoNegativoPermitido()
    {
        var estacion = new EstacionBicicletas();
        var tarjeta = new Tarjeta { Saldo = 1000 };

        Assert.That(estacion.RetirarBici(tarjeta), Is.True);
        Assert.That(tarjeta.Saldo, Is.EqualTo(1000 - 2418.75m));
    }

    [Test]
    public void RetirarBici_LlegandoJustoAlSaldoNegativoMaximoEsAceptado()
    {
        var estacion = new EstacionBicicletas();
        var tarjeta = new Tarjeta { Saldo = 418.75m };

        Assert.That(estacion.RetirarBici(tarjeta), Is.True);
        Assert.That(tarjeta.Saldo, Is.EqualTo(-2000));
    }

    [Test]
    public void RetirarBici_SuperandoElSaldoNegativoMaximoRechazaYNoCobra()
    {
        var estacion = new EstacionBicicletas();
        var tarjeta = new Tarjeta { Saldo = 418.74m };

        var resultado = estacion.RetirarBici(tarjeta);

        Assert.That(resultado, Is.False);
        Assert.That(tarjeta.Saldo, Is.EqualTo(418.74m));
    }

    [Test]
    public void RetirarBici_ConSaldoNegativoYSinMargenRechaza()
    {
        var estacion = new EstacionBicicletas();
        var tarjeta = new Tarjeta { Saldo = -500 };

        Assert.That(estacion.RetirarBici(tarjeta), Is.False);
        Assert.That(tarjeta.Saldo, Is.EqualTo(-500));
    }

    [Test]
    public void RetirarBici_ConSaldoCeroRechaza()
    {
        var estacion = new EstacionBicicletas();
        var tarjeta = new Tarjeta { Saldo = 0 };

        Assert.That(estacion.RetirarBici(tarjeta), Is.False);
        Assert.That(tarjeta.Saldo, Is.EqualTo(0));
    }

    [TestCase(TarjetaTipo.NormalId)]
    [TestCase(TarjetaTipo.MedioBoletoEstudiantilId)]
    [TestCase(TarjetaTipo.BoletoGratuitoEstudiantilId)]
    [TestCase(TarjetaTipo.FranquiciaCompletaId)]
    public void RetirarBici_CualquierTipoDeTarjetaPagaLaTarifaCompleta(int tipoId)
    {
        var estacion = new EstacionBicicletas();
        var tarjeta = TarjetaDeTipo(tipoId, 10000);

        Assert.That(estacion.RetirarBici(tarjeta), Is.True);
        Assert.That(tarjeta.Saldo, Is.EqualTo(10000 - 2418.75m));
    }

    [TestCase(TarjetaTipo.MedioBoletoEstudiantilId)]
    [TestCase(TarjetaTipo.BoletoGratuitoEstudiantilId)]
    [TestCase(TarjetaTipo.FranquiciaCompletaId)]
    public void RetirarBici_LasFranquiciasTambienSonRechazadasSinSaldoSuficiente(int tipoId)
    {
        var estacion = new EstacionBicicletas();
        var tarjeta = TarjetaDeTipo(tipoId, 100);

        Assert.That(estacion.RetirarBici(tarjeta), Is.False);
        Assert.That(tarjeta.Saldo, Is.EqualTo(100));
    }

    [Test]
    public void RetirarBici_NoTieneRestriccionDeDiaNiHorarioParaLasFranquicias()
    {
        // Franquicias en colectivo solo de lunes a viernes de 6 a 22; la estación no aplica esa regla.
        var estacion = new EstacionBicicletas();
        var tarjeta = TarjetaDeTipo(TarjetaTipo.FranquiciaCompletaId, 10000);

        Assert.That(estacion.RetirarBici(tarjeta), Is.True);
    }

    [Test]
    public void RetirarBici_PuedeRetirarseMasDeUnaVezMientrasHayaSaldo()
    {
        var estacion = new EstacionBicicletas();
        var tarjeta = new Tarjeta { Saldo = 10000 };

        Assert.That(estacion.RetirarBici(tarjeta), Is.True);
        Assert.That(estacion.RetirarBici(tarjeta), Is.True);
        Assert.That(tarjeta.Saldo, Is.EqualTo(10000 - 2 * 2418.75m));
    }

    [Test]
    public void RetirarBici_ElSegundoRetiroEsRechazadoSiYaNoQuedaMargen()
    {
        var estacion = new EstacionBicicletas();
        var tarjeta = new Tarjeta { Saldo = 2500 };

        Assert.That(estacion.RetirarBici(tarjeta), Is.True);
        Assert.That(estacion.RetirarBici(tarjeta), Is.False);
        Assert.That(tarjeta.Saldo, Is.EqualTo(2500 - 2418.75m));
    }

    [Test]
    public void RetirarBici_LuegoDelPagoAcreditaElSaldoPendiente()
    {
        var estacion = new EstacionBicicletas();
        var tarjeta = new Tarjeta { Saldo = 56000, SaldoPendiente = 3000 };

        estacion.RetirarBici(tarjeta);

        Assert.That(tarjeta.Saldo, Is.EqualTo(56000));
        Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(3000 - 2418.75m));
    }

    [Test]
    public void RetirarBici_NoAfectaLosContadoresDeColectivo()
    {
        var estacion = new EstacionBicicletas();
        var tarjeta = new Tarjeta { Saldo = 10000 };

        estacion.RetirarBici(tarjeta);

        Assert.That(tarjeta.ViajesDelMes, Is.EqualTo(0));
        Assert.That(tarjeta.FechaUltimoPago, Is.Null);
        Assert.That(tarjeta.LineaUltimoViaje, Is.Null);
        Assert.That(tarjeta.UltimoViaje, Is.Null);
    }

    [Test]
    public void EstacionBicicletas_SePersisteConSuNombre()
    {
        var opciones = new DbContextOptionsBuilder<TarjetaSubeDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var db = new TarjetaSubeDbContext(opciones))
        {
            db.EstacionesBicicletas.Add(new EstacionBicicletas { Nombre = "Plaza Pringles" });
            db.SaveChanges();
        }

        using (var db = new TarjetaSubeDbContext(opciones))
        {
            Assert.That(db.EstacionesBicicletas.Single().Nombre, Is.EqualTo("Plaza Pringles"));
        }
    }
}
