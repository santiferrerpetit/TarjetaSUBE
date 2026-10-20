namespace TarjetaSubeTest;

public class RelojFalso(DateTimeOffset ahora) : TimeProvider
{
    private DateTimeOffset _ahora = ahora;

    public override DateTimeOffset GetUtcNow() => _ahora;

    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;

    public void Avanzar(TimeSpan tiempo) => _ahora += tiempo;
}
