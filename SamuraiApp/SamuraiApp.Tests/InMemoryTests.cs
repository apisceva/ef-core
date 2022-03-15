using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System.Diagnostics;

namespace SamuraiApp.Tests
{
    [TestClass]
    public class InMemoryTests
    {
        [TestMethod]
        public void CanInsertSamuraiIntoDatabase()
        {
            //var contextOptions = new DbContextOptionsBuilder<SamuraiContext>()
            //    .(@"Server=(localdb)\mssqllocaldb;Database=Test")
            //    .Options;
            var builder = new DbContextOptionsBuilder();
            builder.UseInMemoryDatabase("CanInsertSamurai");
            using (var context = new SamuraiContext(builder.Options))// contextOptions))
            {
                var samurai = new Samurai();
                context.Samurais.Add(samurai);
                Assert.AreEqual(EntityState.Added,
                    context.Entry(samurai).State);

            }
        }
    }
}
