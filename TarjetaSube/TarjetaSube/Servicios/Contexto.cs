namespace TarjetaSube.Servicios;

public static class Contexto
{
    public static TarjetaSubeDbContext Db { get; set; } = new TarjetaSubeDbContext();
}
