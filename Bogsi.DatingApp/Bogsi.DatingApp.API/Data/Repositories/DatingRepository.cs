using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogsi.DatingApp.API.Helpers;
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

        public async Task<PagedList<User>> GetUsers(UserParameters parameters)
        {
            var users = this._context
                .Users
                .Include(x => x.Photos)
                .OrderByDescending(x => x.LastActive) as IQueryable<User>;

            // Filter out self
            users = users.Where(x => x.Id != parameters.UserId);

            // Filter on gender
            users = users.Where(x => x.Gender == parameters.Gender);

            // filter on age 
            if (parameters.MinAge != 18 || parameters.MaxAge != 99)
            {
                var minDob = DateTime.Today.AddYears(-parameters.MaxAge - 1);
                var maxDob = DateTime.Today.AddYears(-parameters.MinAge);

                users = users.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);
            }

            // Ordering
            if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
            {
                switch (parameters.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(x => x.Created);
                        break;
                    default:
                        users = users.OrderByDescending(x => x.LastActive);
                        break;
                }
            }

            return await PagedList<User>.CreateAsync(users, parameters.PageNumber, parameters.PageSize);
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