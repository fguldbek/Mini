
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Service;
using Data;
using shared.Model;
using static shared.Util; 


namespace ordination_test
{
    [TestClass]
    public class DataServiceTest
    {
        private DataService service;

        [TestInitialize]
        public void SetupBeforeEachTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<OrdinationContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
            var context = new OrdinationContext(optionsBuilder.Options);
            service = new DataService(context);
            service.SeedData();
        }

        [TestMethod]
        public void Test_GetPatienter_ReturnsAll()
        {
            var patienter = service.GetPatienter();
            Assert.AreEqual(6, patienter.Count);
        }

        [TestMethod]
        public void Test_GetLaegemidler_ReturnsAll()
        {
            var laegemidler = service.GetLaegemidler();
            Assert.AreEqual(5, laegemidler.Count);
        }

        [TestMethod]
        public void Test_GetPNs_ReturnsAllPN()
        {
            var pns = service.GetPNs();
            Assert.AreEqual(4, pns.Count); // Fra seed
        }

        [TestMethod]
        public void Test_GetDagligFaste_ReturnsAll()
        {
            var faste = service.GetDagligFaste();
            Assert.AreEqual(1, faste.Count);
        }

        [TestMethod]
        public void Test_GetDagligSkaeve_ReturnsAll()
        {
            var skaeve = service.GetDagligSkæve();
            Assert.AreEqual(1, skaeve.Count);
        }

        [TestMethod]
        public void Test_OpretPN_ValidData_CreatesPN()
        {
            var pn = service.OpretPN(1, 1, 5, DateTime.Today, DateTime.Today.AddDays(1));
            Assert.IsNotNull(pn);
            Assert.AreEqual(5, pn.antalEnheder);
        }

        [TestMethod]
        public void Test_OpretPN_InvalidPatient_ReturnsNull()
        {
            var pn = service.OpretPN(-1, 1, 5, DateTime.Today, DateTime.Today.AddDays(1));
            Assert.IsNull(pn);
        }

        [TestMethod]
        public void Test_OpretDagligFast_CreatesEntry()
        {
            var fast = service.OpretDagligFast(1, 1, 1, 1, 1, 1, DateTime.Today, DateTime.Today.AddDays(1));
            Assert.IsNotNull(fast);
            Assert.AreEqual(1, fast.MorgenDosis.antal);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_OpretDagligFast_InvalidPatient_Throws()
        {
            service.OpretDagligFast(-1, 1, 1, 1, 1, 1, DateTime.Today, DateTime.Today.AddDays(1));
        }

        [TestMethod]
        public void Test_OpretDagligSkaev_CreatesEntry()
        {
            var doser = new Dosis[]
            {
                new Dosis(CreateTimeOnly(10, 0, 0), 1.0),
                new Dosis(CreateTimeOnly(15, 0, 0), 2.0)
            };

            var skæv = service.OpretDagligSkaev(1, 1, doser, DateTime.Today, DateTime.Today.AddDays(1));
            Assert.IsNotNull(skæv);
            Assert.AreEqual(2, skæv.doser.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_OpretDagligSkaev_InvalidPatient_Throws()
        {
            var doser = new Dosis[] {
                new Dosis(CreateTimeOnly(10, 0, 0), 1.0)
            };
            service.OpretDagligSkaev(-1, 1, doser, DateTime.Today, DateTime.Today.AddDays(1));
        }

        [TestMethod]
        public void Test_AnvendOrdination_ValidDate_ReturnsSuccess()
        {
            var pn = service.GetPNs().First();
            var dato = new Dato { dato = pn.slutDen.AddDays(-1) };

            var result = service.AnvendOrdination(pn.OrdinationId, dato);
            Assert.AreEqual("Dosis givet", result);
        }

        [TestMethod]
        public void Test_AnvendOrdination_InvalidDate_ReturnsError()
        {
            var pn = service.GetPNs().First();
            var dato = new Dato { dato = pn.slutDen.AddDays(5) };

            var result = service.AnvendOrdination(pn.OrdinationId, dato);
            Assert.AreEqual("Dato uden for gyldig periode", result);
        }

        [TestMethod]
        public void Test_AnvendOrdination_NotFound_ReturnsError()
        {
            var result = service.AnvendOrdination(999, new Dato { dato = DateTime.Today });
            Assert.AreEqual("Ordination ikke fundet", result);
        }

        [TestMethod]
        public void Test_GetAnbefaletDosisPerDøgn_VægtUnder25()
        {
            // Arrange: Hent patient med vægt under 25
            var patient = service.GetPatienter().First(p => p.vaegt < 25); // f.eks. "Tiny Person"
            var laegemiddel = service.GetLaegemidler().First();

            // Act: Beregn den anbefalede dosis per døgn
            var dosis = service.GetAnbefaletDosisPerDøgn(patient.PatientId, laegemiddel.LaegemiddelId);

            // Forventet dosis baseret på patientens vægt og lægemidlets faktor for let vægt
            var forventet = patient.vaegt * laegemiddel.enhedPrKgPrDoegnLet;

            // Assert: Bekræft, at den beregnede dosis er korrekt
            Assert.AreEqual(forventet, dosis, 0.001);
        }


        [TestMethod]
        public void Test_GetAnbefaletDosisPerDøgn_VægtNormal()
        {
            var patient = service.GetPatienter().First(p => p.vaegt > 25 && p.vaegt <= 120);
            var laegemiddel = service.GetLaegemidler().First();

            var dosis = service.GetAnbefaletDosisPerDøgn(patient.PatientId, laegemiddel.LaegemiddelId);
            var forventet = patient.vaegt * laegemiddel.enhedPrKgPrDoegnNormal;

            Assert.AreEqual(forventet, dosis);
        }

        [TestMethod]
        public void Test_GetAnbefaletDosisPerDøgn_VægtOver120()
        {
            // Arrange
            var context = new OrdinationContext(
                new DbContextOptionsBuilder<OrdinationContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

            // Opret et Laegemiddel og tilføj det til konteksten
            var laegemiddel = new Laegemiddel("Test Med", 0.1, 0.15, 0.2, "Styk");
            context.Laegemiddler.Add(laegemiddel);
            context.SaveChanges();

            // Opret en patient med vægt over 120 kg og tilføj til konteksten
            var nyPatient = new Patient("999999-9999", "Stor Person", 130);
            context.Patienter.Add(nyPatient);
            context.SaveChanges();

            // Initialiser DataService med den nye in-memory database
            var testService = new DataService(context);

            // Act: Beregn den anbefalede dosis per døgn
            var dosis = testService.GetAnbefaletDosisPerDøgn(nyPatient.PatientId, laegemiddel.LaegemiddelId);

            // Forventet dosis baseret på patientens vægt og lægemidlets faktor for tung vægt
            var forventet = nyPatient.vaegt * laegemiddel.enhedPrKgPrDoegnTung;

            // Assert: Bekræft, at den beregnede dosis er korrekt
            Assert.AreEqual(forventet, dosis, 0.001);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_GetAnbefaletDosisPerDøgn_InvalidPatient_Throws()
        {
            var laegemiddel = service.GetLaegemidler().First();
            service.GetAnbefaletDosisPerDøgn(-1, laegemiddel.LaegemiddelId);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_GetAnbefaletDosisPerDøgn_InvalidMedication_Throws()
        {
            var patient = service.GetPatienter().First();
            service.GetAnbefaletDosisPerDøgn(patient.PatientId, -1);
        }
    }
}
