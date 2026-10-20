namespace TarjetaSubeTest;

public class RelojFalso(DateTimeOffset ahora) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => ahora;

    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
}
