namespace ordination_test;


using shared.Model;

[TestClass]

public class DagligFastTest
{
    [TestMethod]
    public void OpretAlleDoser()
    {
        var lm = new Laegemiddel("TestMedicin", 1, 1, 1, "ml");
        var dagligFast = new DagligFast(
            new DateTime(2025, 5, 1),
            new DateTime(2025, 5, 10),
            lm,
            1, // morgen
            1, // middag
            1, // aften
            1  // nat
        );

        Assert.AreEqual(40, dagligFast.samletDosis(), "Expected total daily dose to be 4");
    }
    
    [TestMethod]
    public void DoegnDosis_CorrectSum()
    {
        var lm = new Laegemiddel("TestMedicin", 1, 1, 1, "ml");
        var dagligFast = new DagligFast(new DateTime(2025, 5, 1), new DateTime(2025, 5, 10), lm, 1, 2, 3, 4);
        Assert.AreEqual(10, dagligFast.doegnDosis(), "Daily dose should sum all times");
    }

    [TestMethod]
    public void GetDoser_ReturnsAllFour()
    {
        var lm = new Laegemiddel("TestMedicin", 1, 1, 1, "ml");
        var dagligFast = new DagligFast(new DateTime(2025, 5, 1), new DateTime(2025, 5, 10), lm, 1, 2, 3, 4);
        var doser = dagligFast.getDoser();

        Assert.AreEqual(4, doser.Length, "Should return 4 doses");
        Assert.AreEqual(1, doser[0].antal, "Morgen dose");
        Assert.AreEqual(2, doser[1].antal, "Middag dose");
        Assert.AreEqual(3, doser[2].antal, "Aften dose");
        Assert.AreEqual(4, doser[3].antal, "Nat dose");
    }
    [TestMethod]
    public void GetType_ReturnsDagligFast()
    {
        var lm = new Laegemiddel("TestMedicin", 1, 1, 1, "ml");
        var dagligFast = new DagligFast(new DateTime(2025, 5, 1), new DateTime(2025, 5, 10), lm, 1, 1, 1, 1);
        Assert.AreEqual("DagligFast", dagligFast.getType(), "Type should be 'DagligFast'");
    }
}