namespace TarjetaSube.Clases;

public class EstacionBicicletas
{
    public const decimal TarifaPorDia = 2418.75m;

    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;

    public bool RetirarBici(Tarjeta tarjeta) => tarjeta.Descontar(TarifaPorDia);
}
