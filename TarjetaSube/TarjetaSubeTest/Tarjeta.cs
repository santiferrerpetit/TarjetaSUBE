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
    public void Cargar_SumaElMontoAlSaldo(decimal monto)
    {
        var tarjeta = new TarjetaSube.Tarjeta();

        tarjeta.Cargar(monto);

        Assert.That(tarjeta.Saldo, Is.EqualTo(monto));
    }

    [Test]
    public void Cargar_RechazaUnMontoQueNoEstaEnLaListaDeCargasAceptadas()
    {
        var tarjeta = new TarjetaSube.Tarjeta();

        Assert.That(
            () => tarjeta.Cargar(1000),
            Throws.TypeOf<ArgumentException>());
    }
}
