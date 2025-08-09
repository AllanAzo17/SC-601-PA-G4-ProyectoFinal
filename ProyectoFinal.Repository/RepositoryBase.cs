namespace ProyectoFinal.Repository
{
    using ProyectoFinal.Data;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System;

    public interface IRepositoryBase<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(int id);
        void Save();
    }

    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected readonly G4ProyectoFinalDBEntities _context;
        protected readonly DbSet<T> _set;

        public RepositoryBase()
        {
            _context = new G4ProyectoFinalDBEntities();
            _set = _context.Set<T>();
        }

        public IEnumerable<T> GetAll()
        {
            return _set.ToList();
        }

        public T GetById(int id)
        {
            return _set.Find(id);
        }

        public void Add(T entity)
        {
            _set.Add(entity);
            Save();
        }

        // Actualiza una entidad existente en la base de datos usando Entity Framework
        public void Update(T entity)
        {
            try
            {
                var existingEntity = _set.Find(GetEntityId(entity));
                
                if (existingEntity != null)
                {
                    _context.Entry(existingEntity).CurrentValues.SetValues(entity);
                    _context.Entry(existingEntity).State = EntityState.Modified;
                }
                else
                {
                    if (_context.Entry(entity).State == EntityState.Detached)
                        _set.Attach(entity);
                    
                    _context.Entry(entity).State = EntityState.Modified;
                }
                
                Save();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en Update: {ex.Message}");
                throw;
            }
        }

        // Obtiene el ID de una entidad usando reflexión para buscar propiedades comunes de ID
        private int GetEntityId(T entity)
        {
            var idProperty = typeof(T).GetProperty("TaskId") ?? 
                           typeof(T).GetProperty("Id") ?? 
                           typeof(T).GetProperty("ID");
            
            if (idProperty != null)
            {
                return (int)idProperty.GetValue(entity);
            }
            
            throw new InvalidOperationException("No se pudo encontrar la propiedad ID de la entidad");
        }

        public void Delete(int id)
        {
            T entityToDelete = _set.Find(id);
            if (entityToDelete != null)
            {
                _set.Remove(entityToDelete);
                Save();
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }

}
