using TarjetaSube.Clases;

namespace TarjetaSubeTest;

public class TarjetaTipoTests
{
    [TestCase(TarjetaTipo.NormalId, 1580, 1580)]
    [TestCase(TarjetaTipo.MedioBoletoEstudiantilId, 1580, 790)]
    [TestCase(TarjetaTipo.BoletoGratuitoEstudiantilId, 1580, 0)]
    [TestCase(TarjetaTipo.FranquiciaCompletaId, 1580, 0)]
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
