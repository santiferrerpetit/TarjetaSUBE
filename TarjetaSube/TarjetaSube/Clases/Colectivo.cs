using System.ComponentModel.DataAnnotations.Schema;

namespace TarjetaSube.Clases;

public class Colectivo
{
    public int Id { get; set; }
    public string Linea { get; set; } = string.Empty;

    [NotMapped]
    public TimeProvider Tiempo { get; set; } = TimeProvider.System;

    private const int Tarifa = 2150;

    public bool PagarCon(Tarjeta tarjeta) => EmitirBoleto(tarjeta) != null;

    public Boleto? EmitirBoleto(Tarjeta tarjeta)
    {
        var monto = tarjeta.CalcularPasaje(Tarifa);
        var ahora = Tiempo.GetLocalNow().DateTime;

        if (tarjeta.EsMedioBoleto)
        {
            if (tarjeta.UltimoViaje?.Date != ahora.Date)
                tarjeta.ViajesMedioBoletoHoy = 0;

            if (tarjeta.UltimoViaje is { } ultimo && ahora - ultimo < TimeSpan.FromMinutes(5))
                return null;

            if (tarjeta.ViajesMedioBoletoHoy >= 2)
                monto = Tarifa;
        }

        if (tarjeta.EsBoletoGratuito)
        {
            if (tarjeta.UltimoViaje?.Date != ahora.Date)
                tarjeta.ViajesGratuitosHoy = 0;

            if (tarjeta.ViajesGratuitosHoy >= 2)
                monto = Tarifa;
        }

        var deudaPrevia = monto > 0 ? Math.Max(0, -tarjeta.Saldo) : 0;

        if (!tarjeta.Descontar(monto))
            return null;

        if (tarjeta.EsMedioBoleto)
        {
            tarjeta.UltimoViaje = ahora;
            tarjeta.ViajesMedioBoletoHoy++;
        }

        if (tarjeta.EsBoletoGratuito)
        {
            tarjeta.UltimoViaje = ahora;
            tarjeta.ViajesGratuitosHoy++;
        }

        return new Boleto
        {
            Fecha = ahora,
            TipoTarjeta = tarjeta.NombreTipo,
            Linea = Linea,
            TotalAbonado = monto + deudaPrevia,
            Saldo = tarjeta.Saldo,
            IdTarjeta = tarjeta.Id,
            Tarjeta = tarjeta,
            Colectivo = this
        };
    }
}
