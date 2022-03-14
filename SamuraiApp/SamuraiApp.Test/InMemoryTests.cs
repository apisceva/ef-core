using SamuraiApp.Data;
using SamuraiApp.Domain;
using System.Diagnostics;
using Xunit;


namespace SamuraiApp.Test
{
    public class DatabaseTests
    {
        [Fact]
        public void CanInsertSamuraiIntoDatabase()
        {
            using (var context = new SamuraiContext())
            {
                //context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                var samurai = new Samurai();
                context.Samurais.Add(samurai);
                //Debug.WriteLine($"Before save: {samurai.Id}");
                context.SaveChanges();
                //Debug.WriteLine($"After save: {samurai.Id}");

                Assert.NotEqual(0, samurai.Id);

            }
        }
    }
}
