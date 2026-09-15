namespace TarjetaSube.Clases;

public class Colectivo
{
    public int Id { get; set; }
    public string Linea { get; set; } = string.Empty;

    private const int Tarifa = 1580;

    public Boleto? PagarCon(Tarjeta tarjeta)
    {
        if (!tarjeta.Descontar(Tarifa))
            return null;

        return new Boleto
        {
            Monto = Tarifa,
            Tarjeta = tarjeta,
            Colectivo = this
        };
    }
}
