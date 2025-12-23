using BookMyDoctor.Server.Data;
using BookMyDoctor.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Linq.Expressions;
using System.Data;

namespace BookMyDoctor.Server.Repositories.Implementations
{
    public class BmdRepository<T> : IBmdRepository<T> where T : class
    {
        protected readonly BmdContext _context;
        protected readonly DbSet<T> _dbSet;
        protected readonly string _tableName;
        protected readonly string _idColumn;

        public BmdRepository(BmdContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
            _tableName = typeof(T).Name + "s"; // Assumes plural table names
            _idColumn = typeof(T).Name + "Id"; // Assumes Id column naming convention
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            var result = await _context.Set<T>().FromSqlRaw(
                "EXEC sp_Generic_GetById @TableName, @IdColumn, @Id",
                new SqlParameter("@TableName", _tableName),
                new SqlParameter("@IdColumn", _idColumn),
                new SqlParameter("@Id", id)
            ).ToListAsync();
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().FromSqlRaw(
                "EXEC sp_Generic_GetAll @TableName",
                new SqlParameter("@TableName", _tableName)
            ).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(string includeProperties)
        {
            if (string.IsNullOrWhiteSpace(includeProperties))
            {
                return await GetAllAsync();
            }
            
            return await _context.Set<T>().FromSqlRaw(
                "EXEC sp_Generic_GetAllWithIncludes @TableName, @IncludeProperties",
                new SqlParameter("@TableName", _tableName),
                new SqlParameter("@IncludeProperties", includeProperties)
            ).ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            // For complex expressions, fall back to EF Core
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            // For complex expressions, fall back to EF Core
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            return entities;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task DeleteByIdAsync(int id)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_Generic_Delete @TableName, @IdColumn, @Id",
                new SqlParameter("@TableName", _tableName),
                new SqlParameter("@IdColumn", _idColumn),
                new SqlParameter("@Id", id)
            );
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null)
            {
                var result = await _context.Database.SqlQueryRaw<int>(
                    "EXEC sp_Generic_Count @TableName",
                    new SqlParameter("@TableName", _tableName)
                ).FirstAsync();
                return result;
            }
            return await _dbSet.CountAsync(predicate);
        }
        
        public async Task<IEnumerable<T>> FindWithConditionAsync(string whereClause)
        {
            return await _context.Set<T>().FromSqlRaw(
                "EXEC sp_Generic_Find @TableName, @WhereClause",
                new SqlParameter("@TableName", _tableName),
                new SqlParameter("@WhereClause", whereClause)
            ).ToListAsync();
        }
    }
}