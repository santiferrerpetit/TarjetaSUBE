using System.ComponentModel.DataAnnotations.Schema;

namespace TarjetaSube.Clases;

public class EstacionBicicletas
{
    public const decimal TarifaPorDia = 2418.75m;
    public const decimal MultaPorExceso = 1250m;
    public static readonly TimeSpan TiempoMaximoDeUso = TimeSpan.FromHours(1);

    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;

    [NotMapped]
    public TimeProvider Tiempo { get; set; } = TimeProvider.System;

    public bool RetirarBici(Tarjeta tarjeta)
    {
        var total = TarifaPorDia + tarjeta.MultasBiciPendientes;

        if (!tarjeta.Descontar(total))
            return false;

        tarjeta.MultasBiciPendientes = 0;
        tarjeta.BiciRetiradaEn = Tiempo.GetLocalNow().DateTime;
        return true;
    }

    public decimal DevolverBici(Tarjeta tarjeta)
    {
        if (tarjeta.BiciRetiradaEn is not { } retiro)
            return 0;

        tarjeta.BiciRetiradaEn = null;

        if (Tiempo.GetLocalNow().DateTime - retiro <= TiempoMaximoDeUso)
            return 0;

        tarjeta.MultasBiciPendientes += MultaPorExceso;
        return MultaPorExceso;
    }
}
