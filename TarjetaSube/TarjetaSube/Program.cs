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

Console.WriteLine();
Console.WriteLine("Medio boleto : ");

var reloj = new RelojManual(new DateTimeOffset(2026, 1, 5, 10, 0, 0, TimeSpan.Zero));
var colectivoMedio = new Colectivo { Linea = "115A", Tiempo = reloj };
var medio = new Tarjeta { TarjetaTipo = TarjetaTipo.MedioBoletoEstudiantil };
medio.Cargar(10000);

PagarConHora(colectivoMedio, medio, reloj, 0); //acepta
PagarConHora(colectivoMedio, medio, reloj, 2); // no 
PagarConHora(colectivoMedio, medio, reloj, 3); // si porque 3+2 = 5
PagarConHora(colectivoMedio, medio, reloj, 5); // si
PagarConHora(colectivoMedio, medio, reloj, 5); // si
reloj.Avanzar(TimeSpan.FromDays(1));
Console.WriteLine("(paso un dia)"); // un dia mas
PagarConHora(colectivoMedio, medio, reloj, 0);  // dia nuevo ahora vuelve a ser de790

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
    if (c.PagarCon(t))
        Console.WriteLine($"Pago aceptado. Saldo restante: {t.Saldo:C}");
    else
        Console.WriteLine($"Pago rechazado. Saldo insuficiente: {t.Saldo:C}");
}

void PagarConHora(Colectivo c, Tarjeta t, RelojManual r, int minutosAntes)
{
    r.Avanzar(TimeSpan.FromMinutes(minutosAntes));
    var antes = t.Saldo;
    var hora = r.GetLocalNow().ToString("dd/MM HH:mm");

    if (c.PagarCon(t))
        Console.WriteLine($"[{hora}] Pago aceptado. Abonado: {antes - t.Saldo:C}. Saldo: {t.Saldo:C}");
    else
        Console.WriteLine($"[{hora}] Pago rechazado. Saldo: {t.Saldo:C}");
}

class RelojManual(DateTimeOffset ahora) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => ahora;
    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
    public void Avanzar(TimeSpan tiempo) => ahora += tiempo;
}



