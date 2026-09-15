namespace TarjetaSube.Clases;

public class Tarjeta
{
    private static readonly decimal[] CargasAceptadas =
        [2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000];

    private static readonly decimal CargasTotal = 40000;

    public int Id { get; set; }
    public decimal Saldo { get; set; }

    public void Cargar(decimal monto)
    {
        if (!CargasAceptadas.Contains(monto))
            throw new ArgumentException("El monto de carga no esta entre las cargas aceptadas.");
        
        if (Saldo + monto > CargasTotal)
            throw new ArgumentException("No es posible cargar mas saldo a su tarjeta.El maximo es 40k");
        else
            Saldo += monto;
    }
}
