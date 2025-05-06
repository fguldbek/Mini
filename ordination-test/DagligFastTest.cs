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
}