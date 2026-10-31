namespace TarjetaSube.Clases;

public class Tarjeta
{
    private static readonly decimal[] CargasAceptadas =
        [2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000];

    private const decimal SaldoMaximo = 56000;
    private const decimal SaldoNegativoMaximo = -2000;

    public int Id { get; set; }
    public decimal Saldo { get; set; }
    public decimal SaldoPendiente { get; set; }
    public int TarjetaTipoId { get; set; } = TarjetaTipo.NormalId;
    public TarjetaTipo? TarjetaTipo { get; set; }

    public decimal CalcularPasaje(decimal tarifa) =>
        (TarjetaTipo ?? TarjetaSube.Clases.TarjetaTipo.Normal).CalcularPasaje(tarifa);

    public string NombreTipo => (TarjetaTipo ?? TarjetaSube.Clases.TarjetaTipo.Normal).Nombre;
    
    public DateTime? UltimoViaje { get; set; }
    public DateTime? BiciRetiradaEn { get; set; }
    public decimal MultasBiciPendientes { get; set; }
    public DateTime? FechaUltimoPago { get; set; }
    public string? LineaUltimoViaje { get; set; }
    public int ViajesMedioBoletoHoy { get; set; }
    public int ViajesGratuitosHoy { get; set; }
    public bool EsMedioBoleto => (TarjetaTipo ?? TarjetaSube.Clases.TarjetaTipo.Normal).Id == TarjetaSube.Clases.TarjetaTipo.MedioBoletoEstudiantilId;
    public DateTime? MesViajesFrecuentes { get; set; }
    public int ViajesDelMes { get; set; }
    public bool EsNormal => (TarjetaTipo ?? TarjetaSube.Clases.TarjetaTipo.Normal).Id == TarjetaSube.Clases.TarjetaTipo.NormalId;
    public bool EsBoletoGratuito => (TarjetaTipo ?? TarjetaSube.Clases.TarjetaTipo.Normal).Id == TarjetaSube.Clases.TarjetaTipo.BoletoGratuitoEstudiantilId;
    public bool Cargar(decimal monto)
    {
        if (!CargasAceptadas.Contains(monto))
            return false;

        var acreditable = Math.Min(monto, SaldoMaximo - Saldo);
        Saldo += acreditable;
        SaldoPendiente += monto - acreditable;
        return true;
    }

    public void AcreditarCarga()
    {
        var acreditable = Math.Min(SaldoPendiente, SaldoMaximo - Saldo);
        if (acreditable <= 0)
            return;

        Saldo += acreditable;
        SaldoPendiente -= acreditable;
    }

    public bool Descontar(decimal monto)
    {
        if (Saldo - monto < SaldoNegativoMaximo)
            return false;

        Saldo -= monto;
        AcreditarCarga();
        return true;
    }
} 