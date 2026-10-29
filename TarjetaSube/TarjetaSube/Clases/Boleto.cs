namespace TarjetaSube.Clases;

public class Boleto
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string TipoTarjeta { get; set; } = string.Empty;
    public string Linea { get; set; } = string.Empty;
    public decimal TotalAbonado { get; set; }
    public decimal Saldo { get; set; }
    public bool EsTrasbordo { get; set; }
    public int IdTarjeta { get; set; }
    public Tarjeta? Tarjeta { get; set; }
    public Colectivo? Colectivo { get; set; }
}
