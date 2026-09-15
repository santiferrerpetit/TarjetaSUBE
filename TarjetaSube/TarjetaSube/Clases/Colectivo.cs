namespace TarjetaSube.Clases;

public class Colectivo
{
    public int Id { get; set; }
    public string Linea { get; set; } = string.Empty;

    private const int Tarifa = 1580;
    public Boleto PagarCon(Tarjeta tarjeta)
    {
        if (tarjeta.Saldo >= Tarifa)
        {
            tarjeta.Saldo -= Tarifa;

            Console.WriteLine("Pago aceptado, disfrute su viaje.");
            Console.WriteLine($"Su saldo es de {tarjeta.Saldo}");
            return new Boleto
            {
                Monto = Tarifa,
                Tarjeta = tarjeta,
                Colectivo = this
            };
        }
        else
        {
        throw new ArgumentException("Saldo insuficiente, cargue la tarjeta e intente de nuevo");    
        }

        
    }
}
