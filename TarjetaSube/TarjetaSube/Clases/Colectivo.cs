using System.ComponentModel.DataAnnotations.Schema;

namespace TarjetaSube.Clases;

public class Colectivo
{
    public int Id { get; set; }
    public string Linea { get; set; } = string.Empty;

    [NotMapped]
    public TimeProvider Tiempo { get; set; } = TimeProvider.System;

    private const int Tarifa = 2150;
    private const int HoraInicioTrasbordos = 7;
    private const int HoraFinTrasbordos = 22;
    private static readonly TimeSpan PlazoTrasbordo = TimeSpan.FromHours(1);
    private const int HoraInicioFranquicias = 6;
    private const int HoraFinFranquicias = 22;
    private const int ViajesParaDescuentoMedio = 30;
    private const int ViajesParaDescuentoMaximo = 80;
    private const decimal MultiplicadorDescuentoMedio = 0.8m;
    private const decimal MultiplicadorDescuentoMaximo = 0.75m;

    public bool PagarCon(Tarjeta tarjeta) => EmitirBoleto(tarjeta) != null;

    public Boleto? EmitirBoleto(Tarjeta tarjeta)
    {
        var monto = tarjeta.CalcularPasaje(Tarifa);
        var ahora = Tiempo.GetLocalNow().DateTime;

        if (!tarjeta.EsNormal && !EnHorarioDeFranquicia(ahora))
            return null;

        if (EsTrasbordo(tarjeta, ahora))
        {
            tarjeta.LineaUltimoViaje = Linea;
            return CrearBoleto(tarjeta, ahora, 0, esTrasbordo: true);
        }

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

        if (tarjeta.EsNormal)
        {
            var mes = new DateTime(ahora.Year, ahora.Month, 1);
            if (tarjeta.MesViajesFrecuentes != mes)
            {
                tarjeta.MesViajesFrecuentes = mes;
                tarjeta.ViajesDelMes = 0;
            }

            monto *= MultiplicadorUsoFrecuente(tarjeta.ViajesDelMes + 1);
        }

        var deudaPrevia = monto > 0 ? Math.Max(0, -tarjeta.Saldo) : 0;

        if (!tarjeta.Descontar(monto))
            return null;

        tarjeta.FechaUltimoPago = ahora;
        tarjeta.LineaUltimoViaje = Linea;

        if (tarjeta.EsNormal)
            tarjeta.ViajesDelMes++;

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

        return CrearBoleto(tarjeta, ahora, monto + deudaPrevia);
    }

    private Boleto CrearBoleto(Tarjeta tarjeta, DateTime fecha, decimal totalAbonado, bool esTrasbordo = false) => new()
    {
        Fecha = fecha,
        TipoTarjeta = tarjeta.NombreTipo,
        Linea = Linea,
        TotalAbonado = totalAbonado,
        Saldo = tarjeta.Saldo,
        IdTarjeta = tarjeta.Id,
        Tarjeta = tarjeta,
        Colectivo = this,
        EsTrasbordo = esTrasbordo
    };

    private bool EsTrasbordo(Tarjeta tarjeta, DateTime ahora) =>
        tarjeta.FechaUltimoPago is { } ultimoPago
        && tarjeta.LineaUltimoViaje != Linea
        && ahora >= ultimoPago
        && ahora - ultimoPago <= PlazoTrasbordo
        && ahora.DayOfWeek is >= DayOfWeek.Monday and <= DayOfWeek.Saturday
        && ahora.Hour >= HoraInicioTrasbordos
        && ahora.Hour < HoraFinTrasbordos;

    private static decimal MultiplicadorUsoFrecuente(int viaje) => viaje switch
    {
        >= ViajesParaDescuentoMaximo => MultiplicadorDescuentoMaximo,
        >= ViajesParaDescuentoMedio => MultiplicadorDescuentoMedio,
        _ => 1m
    };

    private static bool EnHorarioDeFranquicia(DateTime fecha) =>
        fecha.DayOfWeek is >= DayOfWeek.Monday and <= DayOfWeek.Friday
        && fecha.Hour >= HoraInicioFranquicias
        && fecha.Hour < HoraFinFranquicias;
}
