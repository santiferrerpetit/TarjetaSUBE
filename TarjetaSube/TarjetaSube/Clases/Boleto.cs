namespace TarjetaSube;

public class Boleto
{
    public int Id { get; set; }
    public decimal Monto { get; set; }
    public Tarjeta? Tarjeta { get; set; }
    public Colectivo? Colectivo { get; set; }
}
