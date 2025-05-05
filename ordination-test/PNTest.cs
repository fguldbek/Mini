namespace ordination_test;

using shared.Model;

[TestClass]
public class PNTest
{

    [TestMethod]
    public void givDosis_ValidDato()
    {
        var lm = new Laegemiddel("TestMedicin", 1, 1.0, 1.0, "ml");
        var pn = new PN(new DateTime(2015, 5, 1), new DateTime(2015, 5, 5), 2, lm);
        var dato = new Dato();

        dato.dato = new DateTime(2025, 5, 5);
        
        bool result = pn.givDosis(dato);
        Assert.IsTrue(result);
        Assert.AreEqual(1, pn.dates.Count);

    }
}