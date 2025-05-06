namespace ordination_test;

using shared.Model;

[TestClass]
public class DagligSkævTest
{
    [TestMethod]
    public void SamletDosis_Correct()
    {
        var lm = new Laegemiddel("Test", 1, 1, 1, "ml");
        var daglig = new DagligSkæv(new DateTime(2025, 5, 1), new DateTime(2025, 5, 3), lm);
        daglig.opretDosis(new DateTime(2025, 5, 1, 8, 0, 0), 2);
        daglig.opretDosis(new DateTime(2025, 5, 1, 12, 0, 0), 2);

        var expected = 3 * 4; // 3 dage * 4 enheder/dag
        Assert.AreEqual(expected, daglig.samletDosis(), "Wrong total");
    }

    [TestMethod]
    public void DoegnDosis_SumsDoses()
    {
        var lm = new Laegemiddel("Test", 1, 1, 1, "ml");
        var daglig = new DagligSkæv(new DateTime(2025, 5, 1), new DateTime(2025, 5, 3), lm);
        daglig.opretDosis(new DateTime(2025, 5, 1, 8, 0, 0), 1);
        daglig.opretDosis(new DateTime(2025, 5, 1, 12, 0, 0), 2);
        Assert.AreEqual(3, daglig.doegnDosis(), "Wrong daily sum");
    }

    [TestMethod]
    public void OpretDosis_AddsDose()
    {
        var lm = new Laegemiddel("Test", 1, 1, 1, "ml");
        var daglig = new DagligSkæv(new DateTime(2025, 5, 1), new DateTime(2025, 5, 3), lm);
        daglig.opretDosis(new DateTime(2025, 5, 1, 8, 0, 0), 1);
        Assert.AreEqual(1, daglig.doser.Count, "Should have 1 dose");
    }

    [TestMethod]
    public void GetType_ReturnsDagligSkaev()
    {
        var daglig = new DagligSkæv();
        Assert.AreEqual("DagligSkæv", daglig.getType(), "Wrong type");
    }
}