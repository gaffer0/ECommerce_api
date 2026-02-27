using Microsoft.AspNetCore.Mvc;

namespace EC_V2.Repositories.Interfaces
{
    public interface IGenericRepository<T>where T:class
    {
        Task<T?> GetById(int id);
        Task<IEnumerable<T>> GetAll();
        Task Add(T entity);
        void Update(T entity);
        void Delete(T entity);

    }
}
