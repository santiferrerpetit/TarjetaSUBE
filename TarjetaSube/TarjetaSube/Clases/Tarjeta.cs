namespace TarjetaSube;

public class Tarjeta
{
    private static readonly decimal[] CargasAceptadas =
        [2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000];

    public int Id { get; set; }
    public decimal Saldo { get; set; }

    public void Cargar(decimal monto)
    {
        if (!CargasAceptadas.Contains(monto))
            throw new ArgumentException("El monto de carga no esta entre las cargas aceptadas.");

        Saldo += monto;
    }
}
