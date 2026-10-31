using Microsoft.EntityFrameworkCore;
using TarjetaSube.Clases;
using TarjetaSube.Servicios;

namespace TarjetaSubeTest;

public class EstacionBicicletasMultasTests
{
    private RelojFalso _reloj = null!;
    private EstacionBicicletas _estacion = null!;

    [SetUp]
    public void SetUp()
    {
        _reloj = new RelojFalso(new DateTimeOffset(2026, 10, 28, 10, 0, 0, TimeSpan.Zero));
        _estacion = new EstacionBicicletas { Tiempo = _reloj };
    }

    [Test]
    public void RetirarBici_ConSaldoSuficienteCobraLaTarifaCorrectaSinMulta()
    {
        var tarjeta = new Tarjeta { Saldo = 5000 };

        Assert.That(_estacion.RetirarBici(tarjeta), Is.True);
        Assert.That(tarjeta.Saldo, Is.EqualTo(5000 - 2418.75m));
        Assert.That(tarjeta.MultasBiciPendientes, Is.EqualTo(0));
    }

    [Test]
    public void DevolverBici_DentroDeLaHoraNoGeneraMulta()
    {
        var tarjeta = new Tarjeta { Saldo = 5000 };
        _estacion.RetirarBici(tarjeta);
        _reloj.Avanzar(TimeSpan.FromMinutes(30));

        Assert.That(_estacion.DevolverBici(tarjeta), Is.EqualTo(0));
        Assert.That(tarjeta.MultasBiciPendientes, Is.EqualTo(0));
    }

    [Test]
    public void DevolverBici_ExactamenteEnUnaHoraNoGeneraMulta()
    {
        var tarjeta = new Tarjeta { Saldo = 5000 };
        _estacion.RetirarBici(tarjeta);
        _reloj.Avanzar(TimeSpan.FromHours(1));

        Assert.That(_estacion.DevolverBici(tarjeta), Is.EqualTo(0));
        Assert.That(tarjeta.MultasBiciPendientes, Is.EqualTo(0));
    }

    [Test]
    public void DevolverBici_ExcediendoLaHoraGeneraMultaDe1250()
    {
        var tarjeta = new Tarjeta { Saldo = 5000 };
        _estacion.RetirarBici(tarjeta);
        _reloj.Avanzar(TimeSpan.FromHours(1) + TimeSpan.FromSeconds(1));

        Assert.That(_estacion.DevolverBici(tarjeta), Is.EqualTo(1250));
        Assert.That(tarjeta.MultasBiciPendientes, Is.EqualTo(1250));
    }

    [Test]
    public void DevolverBici_LaMultaNoSeCobraHastaElProximoRetiro()
    {
        var tarjeta = new Tarjeta { Saldo = 5000 };
        _estacion.RetirarBici(tarjeta);
        var saldoTrasRetiro = tarjeta.Saldo;
        _reloj.Avanzar(TimeSpan.FromHours(2));

        _estacion.DevolverBici(tarjeta);

        Assert.That(tarjeta.Saldo, Is.EqualTo(saldoTrasRetiro));
    }

    [Test]
    public void DevolverBici_SinBiciRetiradaNoHaceNada()
    {
        var tarjeta = new Tarjeta { Saldo = 5000 };

        Assert.That(_estacion.DevolverBici(tarjeta), Is.EqualTo(0));
        Assert.That(tarjeta.MultasBiciPendientes, Is.EqualTo(0));
    }

    [Test]
    public void DevolverBici_DosVecesSeguidasNoDuplicaLaMulta()
    {
        var tarjeta = new Tarjeta { Saldo = 5000 };
        _estacion.RetirarBici(tarjeta);
        _reloj.Avanzar(TimeSpan.FromHours(2));

        _estacion.DevolverBici(tarjeta);
        _estacion.DevolverBici(tarjeta);

        Assert.That(tarjeta.MultasBiciPendientes, Is.EqualTo(1250));
    }

    [Test]
    public void RetirarBici_ConMultaPendienteCobraTarifaMasMultaYLaLimpia()
    {
        var tarjeta = new Tarjeta { Saldo = 10000 };
        _estacion.RetirarBici(tarjeta);
        _reloj.Avanzar(TimeSpan.FromHours(2));
        _estacion.DevolverBici(tarjeta);
        var saldoAntes = tarjeta.Saldo;

        Assert.That(_estacion.RetirarBici(tarjeta), Is.True);
        Assert.That(tarjeta.Saldo, Is.EqualTo(saldoAntes - 2418.75m - 1250));
        Assert.That(tarjeta.MultasBiciPendientes, Is.EqualTo(0));
    }

    [Test]
    public void RetirarBici_ConVariasMultasAcumuladasLasCobraTodas()
    {
        var tarjeta = new Tarjeta { Saldo = 20000, MultasBiciPendientes = 3 * 1250 };

        Assert.That(_estacion.RetirarBici(tarjeta), Is.True);
        Assert.That(tarjeta.Saldo, Is.EqualTo(20000 - 2418.75m - 3 * 1250));
        Assert.That(tarjeta.MultasBiciPendientes, Is.EqualTo(0));
    }

    [Test]
    public void DevolverBici_ExcederseVariasVecesElMismoDiaCobraUnaMultaPorCadaExceso()
    {
        var tarjeta = new Tarjeta { Saldo = 20000 };

        for (var i = 0; i < 3; i++)
        {
            _estacion.RetirarBici(tarjeta);
            _reloj.Avanzar(TimeSpan.FromHours(1.5));
            _estacion.DevolverBici(tarjeta);
            _reloj.Avanzar(TimeSpan.FromMinutes(5));
        }

        // Cada retiro cobra la multa del exceso anterior; la tercera queda pendiente.
        Assert.That(tarjeta.Saldo, Is.EqualTo(20000 - 3 * 2418.75m - 2 * 1250));
        Assert.That(tarjeta.MultasBiciPendientes, Is.EqualTo(1250));
    }

    [Test]
    public void RetirarBici_SinSaldoParaTarifaYSinMultaEsRechazado()
    {
        var tarjeta = new Tarjeta { Saldo = -1000 };

        Assert.That(_estacion.RetirarBici(tarjeta), Is.False);
        Assert.That(tarjeta.Saldo, Is.EqualTo(-1000));
        Assert.That(tarjeta.BiciRetiradaEn, Is.Null);
    }

    [Test]
    public void RetirarBici_SinSaldoParaTarifaYConMultaPendienteEsRechazadoYConservaLaMulta()
    {
        var tarjeta = new Tarjeta { Saldo = -1000, MultasBiciPendientes = 1250 };

        Assert.That(_estacion.RetirarBici(tarjeta), Is.False);
        Assert.That(tarjeta.Saldo, Is.EqualTo(-1000));
        Assert.That(tarjeta.MultasBiciPendientes, Is.EqualTo(1250));
        Assert.That(tarjeta.BiciRetiradaEn, Is.Null);
    }

    [Test]
    public void RetirarBici_AlcanzaParaLaTarifaPeroNoParaTarifaMasMultaEsRechazado()
    {
        // 2418.75 alcanza (queda -1000), 2418.75 + 1250 no (quedaría -2250 < -2000).
        var tarjeta = new Tarjeta { Saldo = 1418.75m, MultasBiciPendientes = 1250 };

        Assert.That(_estacion.RetirarBici(tarjeta), Is.False);
        Assert.That(tarjeta.Saldo, Is.EqualTo(1418.75m));
        Assert.That(tarjeta.MultasBiciPendientes, Is.EqualTo(1250));
    }

    [Test]
    public void RetirarBici_TarifaMasMultaLlegandoJustoAlSaldoNegativoMaximoEsAceptado()
    {
        var tarjeta = new Tarjeta { Saldo = 1668.75m, MultasBiciPendientes = 1250 };

        Assert.That(_estacion.RetirarBici(tarjeta), Is.True);
        Assert.That(tarjeta.Saldo, Is.EqualTo(-2000));
        Assert.That(tarjeta.MultasBiciPendientes, Is.EqualTo(0));
    }

    [TestCase(TarjetaTipo.MedioBoletoEstudiantilId)]
    [TestCase(TarjetaTipo.BoletoGratuitoEstudiantilId)]
    [TestCase(TarjetaTipo.FranquiciaCompletaId)]
    public void RetirarBici_LasFranquiciasPagandoMultaNoTienenDescuento(int tipoId)
    {
        var tarjeta = new Tarjeta
        {
            Saldo = 10000,
            MultasBiciPendientes = 1250,
            TarjetaTipo = TarjetaTipo.Predefinidos.Single(t => t.Id == tipoId)
        };

        Assert.That(_estacion.RetirarBici(tarjeta), Is.True);
        Assert.That(tarjeta.Saldo, Is.EqualTo(10000 - 2418.75m - 1250));
    }

    [Test]
    public void MultasBici_SePersistenEnLaTarjeta()
    {
        var opciones = new DbContextOptionsBuilder<TarjetaSubeDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using (var db = new TarjetaSubeDbContext(opciones))
        {
            db.Tarjetas.Add(new Tarjeta { Saldo = 100, MultasBiciPendientes = 2500 });
            db.SaveChanges();
        }

        using (var db = new TarjetaSubeDbContext(opciones))
        {
            Assert.That(db.Tarjetas.Single().MultasBiciPendientes, Is.EqualTo(2500));
        }
    }
}
