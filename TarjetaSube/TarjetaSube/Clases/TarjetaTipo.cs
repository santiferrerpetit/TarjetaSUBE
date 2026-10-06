namespace TarjetaSube.Clases;

public class TarjetaTipo
{
    public const int NormalId = 1;
    public const int MedioBoletoEstudiantilId = 2;
    public const int BoletoGratuitoEstudiantilId = 3;
    public const int FranquiciaCompletaId = 4;

    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal MultiplicadorTarifa { get; set; } = 1m;

    public static readonly TarjetaTipo Normal = new()
        { Id = NormalId, Nombre = "Normal", MultiplicadorTarifa = 1m };

    public static readonly TarjetaTipo MedioBoletoEstudiantil = new()
        { Id = MedioBoletoEstudiantilId, Nombre = "Medio boleto estudiantil", MultiplicadorTarifa = 0.5m };

    public static readonly TarjetaTipo BoletoGratuitoEstudiantil = new()
        { Id = BoletoGratuitoEstudiantilId, Nombre = "Boleto gratuito estudiantil", MultiplicadorTarifa = 0m };

    public static readonly TarjetaTipo FranquiciaCompleta = new()
        { Id = FranquiciaCompletaId, Nombre = "Franquicia completa", MultiplicadorTarifa = 0m };

    public static TarjetaTipo[] Predefinidos =>
        [Normal, MedioBoletoEstudiantil, BoletoGratuitoEstudiantil, FranquiciaCompleta];

    public decimal CalcularPasaje(decimal tarifa) => tarifa * MultiplicadorTarifa;
}
