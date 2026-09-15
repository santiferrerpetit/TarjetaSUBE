namespace TarjetaSube;

public class Colectivo
{
    public int Id { get; set; }
    public string Linea { get; set; } = string.Empty;

    public Boleto PagarCon(Tarjeta tarjeta)
    {
        throw new NotImplementedException();
    }
}
