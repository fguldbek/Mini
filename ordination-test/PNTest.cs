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
    
    
    
        [TestMethod]
        public void OpretPNTest()
        {
            
            
            //her laver vi noget seeddata kan man sige til ordinationen 
            var patient = new Patient("121256-7890", "Anna Hansen", 65.0);
            var laegemiddel = new Laegemiddel("Paracetamol", 1, 1.5, 2.0, "ml");
            var startDato = new DateTime(2025, 5, 1);
            var slutDato = new DateTime(2025, 5, 10);
            double antalEnheder = 2;
            //her laver vi et PN objekt med overstånede data inden i 
            var pnOrdination = new PN(startDato, slutDato, antalEnheder, laegemiddel);
            
            //Her bliver ordinationen puttet ind i den liste som af ordinationer som hver patient har 
            patient.ordinationer.Add(pnOrdination);

            
            //Her tjekkes der om outputtet er som forventet 
            Assert.AreEqual("PN", pnOrdination.getType());
            Assert.AreEqual(startDato, pnOrdination.startDen);
            Assert.AreEqual(slutDato, pnOrdination.slutDen);
            Assert.AreEqual(antalEnheder, pnOrdination.antalEnheder);
            Assert.AreEqual(1, patient.ordinationer.Count);
            Assert.AreSame(pnOrdination, patient.ordinationer[0]);
            
        }
    }

