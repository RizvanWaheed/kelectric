using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using System.Web;
using CRM.Entities;


namespace Repositories.Repository
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        internal SocialCRMEntities context;
        internal DbSet<TEntity> dbSet;

        public GenericRepository(SocialCRMEntities context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public virtual IEnumerable<TEntity> Get()
        {
            IQueryable<TEntity> query = dbSet;
            return query.ToList();
        }

        public virtual TEntity GetByID(object id)
        {
            return dbSet.Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }
        public virtual TEntity InsertWithEntity(TEntity entity)
        {
            return dbSet.Add(entity);
        }
        public virtual void Insert(IEnumerable<TEntity> entity)
        {
            dbSet.AddRange(entity);
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);

        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }
        public virtual void Delete(IEnumerable<TEntity> entitiesToDelete)
        {
            foreach (var entityToDelete in entitiesToDelete)
            {
                if (context.Entry(entityToDelete).State == EntityState.Detached)
                {
                    dbSet.Attach(entityToDelete);
                }
                dbSet.Remove(entityToDelete);
            }
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public virtual IQueryable<TEntity> GetList()
        {
            return dbSet.AsQueryable();
        }
        public bool ExecStoreProcedure(string query, params object[] parameters)
        {
            return context.Database.SqlQuery<bool>(query, parameters).FirstOrDefault();
        }
    }
}
