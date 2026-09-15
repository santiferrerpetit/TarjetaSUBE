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
    public void Cargar_DevuelveFalseYNoModificaElSaldo_SiSuperaElLimiteDe40000()
    {
        var tarjeta = new Tarjeta();
        tarjeta.Cargar(30000);

        var resultado = tarjeta.Cargar(15000);

        Assert.That(resultado, Is.False);
        Assert.That(tarjeta.Saldo, Is.EqualTo(30000));
    }

    [Test]
    public void Cargar_PermiteLlegarJustoAlLimiteDe40000()
    {
        var tarjeta = new Tarjeta();
        tarjeta.Cargar(30000);

        var resultado = tarjeta.Cargar(10000);

        Assert.That(resultado, Is.True);
        Assert.That(tarjeta.Saldo, Is.EqualTo(40000));
    }

    [Test]
    public void Cargar_DevuelveFalseYNoModificaElSaldo_SiYaEstaEnElLimiteDe40000()
    {
        var tarjeta = new Tarjeta();
        tarjeta.Cargar(30000);
        tarjeta.Cargar(10000);

        var resultado = tarjeta.Cargar(2000);

        Assert.That(resultado, Is.False);
        Assert.That(tarjeta.Saldo, Is.EqualTo(40000));
    }

    [Test]
    public void Descontar_RestaElMontoDelSaldoYDevuelveTrue_SiHaySaldoSuficiente()
    {
        var tarjeta = new Tarjeta();
        tarjeta.Cargar(2000);

        var resultado = tarjeta.Descontar(1580);

        Assert.That(resultado, Is.True);
        Assert.That(tarjeta.Saldo, Is.EqualTo(420));
    }

    [Test]
    public void Descontar_DevuelveFalseYNoModificaElSaldo_SiElSaldoEsInsuficiente()
    {
        var tarjeta = new Tarjeta();
        tarjeta.Cargar(2000);

        var resultado = tarjeta.Descontar(3000);

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
}
