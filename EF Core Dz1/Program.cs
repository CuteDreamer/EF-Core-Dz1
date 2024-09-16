using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

namespace EF_Core_Dz1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("Link.json");
            var config = builder.Build();
            string getconnection = config.GetConnectionString("DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            var options = optionsBuilder.UseSqlServer(getconnection).Options;

            using (ApplicationContext db = new ApplicationContext(options))
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                List<Train> trains = new List<Train>()
                {
                    new Train { Name = "Odessa-Kiev", Sections = 12, Places = 360,  Owner = "UkrZalizn", MaxSpeed = 120 },
                    new Train { Name = "Odessa-Lviv", Sections = 15, Places = 450,  Owner = "UkrZalizn", MaxSpeed = 120 },
                    new Train { Name = "Odessa-Harkiv", Sections = 14, Places = 420,  Owner = "UkrZalizn", MaxSpeed = 120 },
                    new Train { Name = "Odessa-Chernigiv", Sections = 10, Places = 300,  Owner = "UkrZalizn", MaxSpeed = 120 },
                    new Train { Name = "Odessa-Ternopil", Sections = 7, Places = 210,  Owner = "UkrZalizn", MaxSpeed = 120 },
                    new Train { Name = "Odessa-Poltava", Sections = 8, Places = 240,  Owner = "UkrZalizn", MaxSpeed = 120 },
                    new Train { Name = "Odessa-Sumy", Sections = 6, Places = 180,  Owner = "UkrZalizn", MaxSpeed = 120 },
                    new Train { Name = "Odessa-Mikolaiv", Sections = 7, Places = 210,  Owner = "UkrZalizn", MaxSpeed = 120 }
                };

                db.Trains.AddRange(trains);
                db.SaveChanges();

                var newTrain = new Train
                {
                    Name = "New Train",
                    Sections = 12,
                    Places = 360,
                    Owner = "UkrZalizn",
                    MaxSpeed = 120
                };

                var addNewTrain = new TrainsMethods();  // добавили поезд
                addNewTrain.AddTrain(db, newTrain);
                db.SaveChanges();

                var getAllTrains = new TrainsMethods(); // Получили список поездов
                var trainlist = getAllTrains.GetTrainsList(db);

                var redactTrainLogic = new TrainsMethods();
                redactTrainLogic.RedactTrain(db, 1, "Updated Train Name", 14, 420, "New Owner", 130); // отредактировали данные

                var deleteTrainLogic = new TrainsMethods();
                deleteTrainLogic.DeleteTrain(db, 1); // удалили поезд
            }



        }
    }

    class Train
    {
        public string Name { get; set; } = null!;
        public int Sections { get; set; }
        public int Places { get; set; }
        public int Id { get; set; }
        public string Owner { get; set; } = null!;
        public int MaxSpeed { get; set; }

    }

    class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Train> Trains { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(@"Server = localhost; Database = DzTrains; Trusted_Connection = True; TrustServerCertificate = True;"); // строка подключения
        //}
    }

    class TrainsMethods
    {
        public void AddTrain(ApplicationContext db, Train train)
        {
            if (train != null)
            {
                db.Trains.Add(train);
                db.SaveChanges();
                Console.WriteLine("Train added");
            }
            else
            {
                Console.WriteLine("Error");
            }
        }

        public List<Train> GetTrainsList(ApplicationContext db)
        {
            return db.Trains.ToList();
        }

        public void RedactTrain(ApplicationContext db, int trainId, string newName, int newSections, int newPlaces, string newOwner, int newMaxSpeed)
        {
            var train = db.Trains.FirstOrDefault(e=>e.Id == trainId);

            if (train != null)
            {
                train.Name = newName;
                train.Sections = newSections;
                train.Places = newPlaces;
                train.Owner = newOwner;
                train.MaxSpeed = newMaxSpeed;

                db.SaveChanges();
                Console.WriteLine($"Поезд с ID {trainId} успешно обновлен.");
            }
            else
            {
                Console.WriteLine("Error. Train not found");
            }
        }

        public void DeleteTrain(ApplicationContext db, int trainId)
        {
            var train = db.Trains.Find(trainId);

            if (train != null)
            { 
                db.Trains.Remove(train);
                db.SaveChanges();
                Console.WriteLine($"Train with ID {trainId} has been deleted.");
            }
            else
            {
                Console.WriteLine($"Error: Train with ID {trainId} not found.");
            }
        }
    }


}