using System.Collections.Generic;
using System.Linq;
using FlightPlanner.Core.Models;

namespace FlightPlanner.Core.Services
{
    public interface IDbService
    {
        IQueryable<T> Query<T>() where T : Entity;
        IEnumerable<T> Get<T>() where T : Entity;
        T GetById<T>(int id) where T: Entity;
        void Create<T>(T entity) where T : Entity;
        void Update<T>(T entity) where T : Entity;
        void Delete<T>(T entity) where T : Entity;
    }
}