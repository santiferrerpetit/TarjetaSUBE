using System.ComponentModel.DataAnnotations.Schema;

namespace TarjetaSube.Clases;

public class Colectivo
{
    public int Id { get; set; }
    public string Linea { get; set; } = string.Empty;

    [NotMapped]
    public TimeProvider Tiempo { get; set; } = TimeProvider.System;

    private const int Tarifa = 1580;

    public bool PagarCon(Tarjeta tarjeta) => EmitirBoleto(tarjeta) != null;

    public Boleto? EmitirBoleto(Tarjeta tarjeta)
    {
        var monto = tarjeta.CalcularPasaje(Tarifa);
        if (!tarjeta.Descontar(monto))
            return null;

        return new Boleto
        {
            Fecha = Tiempo.GetLocalNow().DateTime,
            TipoTarjeta = tarjeta.NombreTipo,
            Linea = Linea,
            TotalAbonado = monto,
            Saldo = tarjeta.Saldo,
            IdTarjeta = tarjeta.Id,
            Tarjeta = tarjeta,
            Colectivo = this
        };
    }
}
