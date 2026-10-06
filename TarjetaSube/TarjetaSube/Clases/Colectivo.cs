namespace TarjetaSube.Clases;

public class Colectivo
{
    public int Id { get; set; }
    public string Linea { get; set; } = string.Empty;

    private const int Tarifa = 1580;

    public bool PagarCon(Tarjeta tarjeta)
    {
        return tarjeta.Descontar(tarjeta.CalcularPasaje(Tarifa));
    }
}
