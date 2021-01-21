using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestDataLibrary.DAL.Context;
using TestDataLibrary.Domain.Entities;

namespace AuthenticationAuthorizationTestApp.Data
{
    public class DatabaseInitializer
    {
        private readonly TestDataLibraryContext _db_context;

        public DatabaseInitializer(TestDataLibraryContext db_context)
        {
            _db_context = db_context;
        }

        public void Initialize() {
            var db = _db_context.Database;

            db.EnsureDeleted();
            db.EnsureCreated();

            AddUsers();
        }

        private void AddUsers() {
            _db_context.Users.AddRange(new List<User> { 
                new User { Email = "user1@mail.ru", Password = "11111"},
                new User { Email = "user2@mail.ru", Password = "22222"},
                new User { Email = "user3@mail.ru", Password = "33333"},
                new User { Email = "user4@mail.ru", Password = "44444"},
                new User { Email = "user5@mail.ru", Password = "55555"},
            } );

            _db_context.SaveChanges();
        }
    }
}
