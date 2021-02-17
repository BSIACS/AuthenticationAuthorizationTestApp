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
            string adminRoleName = "administrator";
            string userRoleName = "user";            

            Role adminRole = new Role { Name = adminRoleName };
            Role userRole = new Role { Name = userRoleName };

            _db_context.Roles.AddRange(new List<Role> {
               adminRole,
               userRole
            });

            _db_context.Users.AddRange(new List<User> {
                new User { Email = "user1@mail.ru", Password = "11111", Role = userRole },
                new User { Email = "user2@mail.ru", Password = "22222", Role = userRole },
                new User { Email = "user3@mail.ru", Password = "33333", Role = userRole },
                new User { Email = "user4@mail.ru", Password = "44444", Role = userRole },
                new User { Email = "user5@mail.ru", Password = "55555", Role = userRole },
                new User { Email = "admin@mail.ru", Password = "12345", Role = adminRole },
            });

            _db_context.SaveChanges();
        }

        
    }
}
