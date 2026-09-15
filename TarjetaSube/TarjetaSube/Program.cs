using TarjetaSube.Clases;

var tarjeta = new Tarjeta();

CargarYMostrar(tarjeta, 5000);
CargarYMostrar(tarjeta, 30000);
CargarYMostrar(tarjeta, 10000);
CargarYMostrar(tarjeta, 5000);
CargarYMostrar(tarjeta, 1000);

var colectivo = new Colectivo { Linea = "115A" };

PagarYMostrar(colectivo, tarjeta);
PagarYMostrar(colectivo, tarjeta);
PagarYMostrar(colectivo, tarjeta);

void CargarYMostrar(Tarjeta t, decimal monto)
{
    if (t.Cargar(monto))
    {
        Console.WriteLine($"Carga de {monto:C} ok. Saldo: {t.Saldo:C}");
    }
    else
    {
        Console.WriteLine($"Carga de {monto:C} rechazada. Saldo actual: {t.Saldo:C}");
    }
}

void PagarYMostrar(Colectivo c, Tarjeta t)
{
    var boleto = c.PagarCon(t);

    if (boleto is not null)
        Console.WriteLine($"Boleto emitido: {boleto.Monto:C}. Saldo restante: {t.Saldo:C}");
    else
        Console.WriteLine($"Pago rechazado. Saldo insuficiente: {t.Saldo:C}");
}
