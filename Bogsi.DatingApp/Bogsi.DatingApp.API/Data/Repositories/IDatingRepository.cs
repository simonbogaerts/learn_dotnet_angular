using System.Collections.Generic;
using System.Threading.Tasks;
using Bogsi.DatingApp.API.Helpers;
using Bogsi.DatingApp.API.Models;

namespace Bogsi.DatingApp.API.Data.Repositories
{
    public interface IDatingRepository
    {
        void Add<T>(T entity) where T: class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAll();

        Task<PagedList<User>> GetUsers(UserParameters parameters);
        Task<User> GetUser(int id);

        Task<Photo> GetPhoto(int id);
        Task<Photo> GetMainPhoto(int userId);

        // Likes
        Task<Like> GetLike(int userId, int recipientId);

        // Message
        Task<Message> GetMessage(int id);
        Task<PagedList<Message>> GetMessagesForUser();
        Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId);
    }
}