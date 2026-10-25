using TarjetaSube.Clases;

namespace TarjetaSubeTest;

public class TarjetaTipoTests
{
    [TestCase(TarjetaTipo.NormalId, 2150, 2150)]
    [TestCase(TarjetaTipo.MedioBoletoEstudiantilId, 2150, 1075)]
    [TestCase(TarjetaTipo.BoletoGratuitoEstudiantilId, 2150, 0)]
    [TestCase(TarjetaTipo.FranquiciaCompletaId, 2150, 0)]
    public void CalcularPasaje_AplicaElMultiplicadorDelTipo(int id, decimal tarifa, decimal esperado)
    {
        var tipo = TarjetaTipo.Predefinidos.Single(t => t.Id == id);

        Assert.That(tipo.CalcularPasaje(tarifa), Is.EqualTo(esperado));
    }

    [Test]
    public void Predefinidos_TieneLosCuatroTiposConIdsUnicos()
    {
        var ids = TarjetaTipo.Predefinidos.Select(t => t.Id).ToArray();

        Assert.That(ids, Is.EquivalentTo(new[] { 1, 2, 3, 4 }));
    }
}
