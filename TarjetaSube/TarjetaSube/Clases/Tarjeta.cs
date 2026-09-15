namespace TarjetaSube.Clases;

public class Tarjeta
{
    private static readonly decimal[] CargasAceptadas =
        [2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000];

    private const decimal SaldoMaximo = 40000;

    public int Id { get; set; }
    public decimal Saldo { get; set; }

    public bool Cargar(decimal monto)
    {
        if (!CargasAceptadas.Contains(monto))
            return false;

        if (Saldo + monto > SaldoMaximo)
            return false;

        Saldo += monto;
        return true;
    }
} 