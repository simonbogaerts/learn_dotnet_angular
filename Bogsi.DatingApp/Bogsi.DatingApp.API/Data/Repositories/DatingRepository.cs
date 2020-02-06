using System.Collections.Generic;
using System.Threading.Tasks;
using Bogsi.DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Bogsi.DatingApp.API.Data.Repositories
{
    public class DatingRepository: IDatingRepository
    {
        private readonly DataContext _context;

        public DatingRepository(DataContext context)
        {
            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            this._context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            this._context.Remove(entity);
        }

        public async Task<bool> SaveAll()
        {
            return await this._context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await this._context
                .Users
                .Include(x => x.Photos)
                .ToListAsync();

            return users;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await this._context
                .Users
                .Include(x => x.Photos)
                .FirstOrDefaultAsync(x => x.Id == id);

            return user;
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await this._context
                .Photos
                .FirstOrDefaultAsync(x => x.Id == id);

            return photo;
        }

        public async Task<Photo> GetMainPhoto(int userId)
        {
            var photo = await this._context
                .Photos
                .FirstOrDefaultAsync(x => x.IsMain && x.UserId == userId);

            return photo;
        }
    }
}