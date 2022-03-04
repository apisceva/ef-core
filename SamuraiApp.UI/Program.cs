using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SamuraiApp.UI
{
    class Program
    {
        private static SamuraiContext _context = new SamuraiContext();
        private static SamuraiContext _contextNT = new SamuraiContextNoTracking();
        private static void Main(string[] args)
        {
            //AddSamuraisByName("Shimada", "Okamoto", "Kikuchio", "Hayashida");
            //GetSamurais();
            ////AddVariousTypes();
            //QueryFilters();
            //QueryAggregates();
            //RetrieveAndUpdateSamurai();
            //RetrieveAndUpdateMultipleSamurais();
            //MultipleDatabaseOperations();
            //RetrieveAndDeleteSamurai();
            //QueryAndUpdateBattles_Disconnected();
            //InsertNewSamuraiWithAQuote();
            //InsertNewSamuraiWithManyQuotes();
            //AddQuoteToExistingSamuraiWhileTracked();
            //AddQuoteToExistingSamuraiNotTracked(1);
            //Simpler_AddQuoteToExistingSamuraiNotTracked(2);
            //EagerLoadSamuraiWithQuotes();
            //ProjectSomeProperties();
            //ProjectSamuraisWithQuotes();
            //ExplicitLoadQuotes();
            //LazyLoadQuotes();
            FiteringWithRelatedData();
        }
        private static void AddVariousTypes()
        {
            _context.AddRange(new Samurai { Name = "Shimada" },
                    new Samurai { Name = "Okamoto" },
                    new Battle { Name = "Battle of Anegawa" },
                    new Battle { Name = "Battle of Nagashino" });
            _context.SaveChanges();
        }

        private static void AddSamuraisByName(params string[] names)
        {
            foreach (string name in names)
            {
                _context.Samurais.Add(new Samurai { Name = name });

            }
            _context.SaveChanges();
        }
        private static void GetSamurais()
        {
            var samurais = _contextNT.Samurais
               .TagWith("ConsoleApp.Program.GetSamurais method")
               .ToList();
            Console.WriteLine($"Samurai count is {samurais.Count}");
            foreach (var samurai in samurais)
            {
                Console.WriteLine(samurai.Name);
            }
        }
        private static void QueryFilters()
        {
            //    var name = "Sampson";
            //    var samurais = _context.Samurais.Where(s => s.Name == "Sampson").ToList();
            var filter = "J%";
            var samurais = _contextNT.Samurais
                .Where(s => EF.Functions.Like(s.Name, "J%")).ToList();
        }
        private static void QueryAggregates()
        {
            //var name = "Sampson";
            //var samurai = _context.Samurais.FirstOrDefault(s => s.Name == name);
            var samurai = _contextNT.Samurais.Find(2);
        }
        private static void RetrieveAndUpdateSamurai()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Name += "San";
            _context.SaveChanges();
        }
        private static void RetrieveAndUpdateMultipleSamurais()
        {
            var samurais = _context.Samurais.Skip(1).Take(4).ToList();
            samurais.ForEach(s => s.Name += "San");
            _context.SaveChanges();
        }
        private static void MultipleDatabaseOperations()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Name += "San";
            _context.Samurais.Add(new Samurai { Name = "Shino" });
            _context.SaveChanges();
        }
        private static void RetrieveAndDeleteSamurai()
        {
            var samurai = _context.Samurais.Find(18);
            _context.Samurais.Remove(samurai);
            _context.SaveChanges();
        }
        private static void QueryAndUpdateBattles_Disconnected()
        {
            List<Battle> disconnectedBattles;
            using (var context1 = new SamuraiContext())
            {
                disconnectedBattles = _context.Battles.ToList();
            } //context1 is disposed
            disconnectedBattles.ForEach(b =>
            {
                b.StartDate = new DateTime(1570, 01, 01);
                b.EndDate = new DateTime(1570, 12, 1);
            });
            using (var context2 = new SamuraiContext())
            {
                context2.UpdateRange(disconnectedBattles);
                context2.SaveChanges();
            }
        }
    
        private static void InsertNewSamuraiWithAQuote()
        {
            var samurai = new Samurai
            {
                Name = "Kambei Shimada",
                Quotes = new List<Quote>
                {
                    new Quote {Text = "I've come to save you"}
                }
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();

        }
        private static void InsertNewSamuraiWithManyQuotes()
        {
            var samurai = new Samurai
            {
                Name = "Kyuzo",
                Quotes = new List<Quote>
                {
                    new Quote {Text = "Watch out for my sharp sword!"},
                    new Quote {Text = "I told you to watch out for the sharp sword! Oh well!"}
                }
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();

        }
        private static void AddQuoteToExistingSamuraiWhileTracked()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Quotes.Add(new Quote
            {
                Text = "I bet you're happy that I've saved you!"
            });
            _context.SaveChanges();
        }
        private static void AddQuoteToExistingSamuraiNotTracked(int samuraiId)
        {
            var samurai = _context.Samurais.Find(samuraiId);
            samurai.Quotes.Add(new Quote
            {
                Text = "Now that I saved you, will you feed me dinner?"
            });
            using (var newContext = new SamuraiContext())
            {
                newContext.Samurais.Attach(samurai);
                newContext.SaveChanges();
            }
        }
        private static void Simpler_AddQuoteToExistingSamuraiNotTracked(int samuraiId)
        {
            var quote = new Quote { Text = "Thanks for dinner!", SamuraiId = samuraiId };
            using var newContext = new SamuraiContext();
            newContext.Quotes.Add(quote);
            newContext.SaveChanges();
        }
        private static void EagerLoadSamuraiWithQuotes()
        {
            //var samuraiWithQuotes = _context.Samurais.Include(s => s.Quotes).ToList()
            //;
            //var splitQuery = _context.Samurais.AsSplitQuery().Include(s => s.Quotes).ToList();
            //var filteredInclude = _context.Samurais
            //    .Include(s => s.Quotes.Where(q => q.Text.Contains("Thanks"))).ToList();
            var filterPrimaryEntityWithInclude =
                _context.Samurais.Where(s => s.Name.Contains("Sampson"))
                .Include(s => s.Quotes).FirstOrDefault();
        }
        private static void ProjectSomeProperties()
        {
            var someProperties = _context.Samurais.Select(s => new { s.Id, s.Name }).ToList();
            var idAndNames = _context.Samurais.Select(s => new IdAndName(s.Id, s.Name)).ToList();
        }
        public struct IdAndName
        {
            public IdAndName(int id, string name)
            {
                Id = id;
                Name = name;
            }
            public int Id;
            public string Name;
        }
        private static void ProjectSamuraisWithQuotes()
        {
            //var somePropWithQuotes = _context.Samurais
            //    .Select(s => new { s.Id, s.Name, NumberOfQuotes=s.Quotes.Count })
            //    .ToList();
            //var somePropWithQuotes = _context.Samurais
            //    .Select(s => new {s.Id,s.Name,
            //                      HappyQuotes = s.Quotes.Where(q => q.Text.Contains("happy"))})
            //    .ToList();
            var samuraisAndQuotes = _context.Samurais
                .Select(s => new
                {
                    Samurai = s,
                    HappyQuotes = s.Quotes.Where(q => q.Text.Contains("happy"))
                })
                .ToList();
            var firstsamurai = samuraisAndQuotes[0].Samurai.Name += "The Happiest";
        }
        private static void ExplicitLoadQuotes()
        { //make sure there's a horse in the DB, then clear the context's change tracker
            _context.Set<Horse>().Add(new Horse { SamuraiId = 1, Name = "Mr. Ed"});
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            //-----------------------------
            var samurai = _context.Samurais.Find(1);
            _context.Entry(samurai).Collection(s => s.Quotes).Load();
            _context.Entry(samurai).Reference(s => s.Horse).Load();
        }
        private static void LazyLoadQuotes()
        {
            var samurai = _context.Samurais.Find(2);
            var quoteCount = samurai.Quotes.Count(); //won't run without LL setup
        }
        private static void FiteringWithRelatedData()
        {
            var samurais = _context.Samurais
                                .Where(s => s.Quotes.Any(Queryable => Queryable.Text.Contains("happy")))
                                .ToList();
        }
    }

}
