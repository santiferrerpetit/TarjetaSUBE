using TarjetaSube.Clases;

namespace TarjetaSubeTest;

public class ColectivoTests
{
    [Test]
    public void PagarCon_ConSaldoSuficiente_DescuentaLaTarifaYCompletaElBoleto()
    {
        var tarjeta = new Tarjeta { Saldo = 3000m };
        var colectivo = new Colectivo { Linea = "115" };

        var boleto = colectivo.PagarCon(tarjeta);

        Assert.That(tarjeta.Saldo, Is.EqualTo(1420m));
        Assert.That(boleto.Monto, Is.EqualTo(1580m));
        Assert.That(boleto.Tarjeta, Is.SameAs(tarjeta));
        Assert.That(boleto.Colectivo, Is.SameAs(colectivo));
    }

    [Test]
    public void PagarCon_ConSaldoExacto_DejaElSaldoEnCero()
    {
        var tarjeta = new Tarjeta { Saldo = 1580m };
        var colectivo = new Colectivo();

        var boleto = colectivo.PagarCon(tarjeta);

        Assert.That(tarjeta.Saldo, Is.Zero);
        Assert.That(boleto.Monto, Is.EqualTo(1580m));
    }

    [Test]
    public void PagarCon_ImprimeElSaldoActualizado()
    {
        var tarjeta = new Tarjeta { Saldo = 2000m };
        var colectivo = new Colectivo();
        var salidaOriginal = Console.Out;
        using var salida = new StringWriter();

        try
        {
            Console.SetOut(salida);

            colectivo.PagarCon(tarjeta);

            Assert.That(salida.ToString(), Does.Contain("Su saldo es de 420"));
        }
        finally
        {
            Console.SetOut(salidaOriginal);
        }
    }

    [TestCase(0)]
    [TestCase(1579)]
    [TestCase(-1)]
    public void PagarCon_ConSaldoInsuficiente_LanzaExcepcionYNoModificaElSaldo(decimal saldo)
    {
        var tarjeta = new Tarjeta { Saldo = saldo };
        var colectivo = new Colectivo();

        var excepcion = Assert.Throws<ArgumentException>(() => colectivo.PagarCon(tarjeta));

        Assert.That(excepcion!.Message, Does.Contain("Saldo insuficiente"));
        Assert.That(tarjeta.Saldo, Is.EqualTo(saldo));
    }
}
