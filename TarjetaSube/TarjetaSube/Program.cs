using TarjetaSube.Clases;

var tarjeta = new Tarjeta();

CargarYMostrar(tarjeta, 5000);
CargarYMostrar(tarjeta, 30000);
CargarYMostrar(tarjeta, 10000);
CargarYMostrar(tarjeta, 5000);
CargarYMostrar(tarjeta, 1000);

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
