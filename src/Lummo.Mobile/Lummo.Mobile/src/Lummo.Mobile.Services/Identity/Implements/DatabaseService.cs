
using SQLite;
using System.Linq.Expressions;

namespace Lummo.Mobile.Services.Identity.Implements;

public class DatabaseService : IAsyncDisposable
{
    private SQLiteAsyncConnection _connection;
    private const string DatabaseFilename = "Lumo.db";

    // Full path to the database file in the local application data directory.
    private string DbPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        DatabaseFilename
    );

    // Lazy initialization of the SQLite connection with appropriate flags.
    private SQLiteAsyncConnection Database =>
        _connection ??= new SQLiteAsyncConnection(
            DbPath,
            SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache
        );

    /// <summary>
    /// Ensures that a table for the specified type exists in the database.
    /// </summary>
    private async Task EnsureTableExistsAsync<T>() where T : class, new()
    {
        await Database.CreateTableAsync<T>();
    }

    /// <summary>
    /// Retrieves all records of the specified type from the database.
    /// </summary>
    public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class, new()
    {
        await EnsureTableExistsAsync<T>();
        return await Database.Table<T>().ToListAsync();
    }

    /// <summary>
    /// Retrieves records of the specified type that match a given filter condition.
    /// </summary>
    public async Task<IEnumerable<T>> GetFilteredAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new()
    {
        await EnsureTableExistsAsync<T>();
        return await Database.Table<T>().Where(predicate).ToListAsync();
    }

    /// <summary>
    /// Retrieves a single record by its primary key.
    /// </summary>
    public async Task<T> GetByIdAsync<T>(object primaryKey) where T : class, new()
    {
        await EnsureTableExistsAsync<T>();
        return await Database.GetAsync<T>(primaryKey);
    }

    /// <summary>
    /// Inserts a new record into the database.
    /// </summary>
    public async Task<bool> AddAsync<T>(T item) where T : class, new()
    {
        await EnsureTableExistsAsync<T>();
        return await Database.InsertAsync(item) > 0;
    }

    /// <summary>
    /// Updates an existing record in the database.
    /// </summary>
    public async Task<bool> UpdateAsync<T>(T item) where T : class, new()
    {
        await EnsureTableExistsAsync<T>();
        return await Database.UpdateAsync(item) > 0;
    }

    /// <summary>
    /// Deletes a record from the database.
    /// </summary>
    public async Task<bool> DeleteAsync<T>(T item) where T : class, new()
    {
        await EnsureTableExistsAsync<T>();
        return await Database.DeleteAsync(item) > 0;
    }

    /// <summary>
    /// Deletes a record by its primary key from the database.
    /// </summary
    public async Task<bool> DeleteByIdAsync<T>(object primaryKey) where T : class, new()
    {
        await EnsureTableExistsAsync<T>();
        return await Database.DeleteAsync<T>(primaryKey) > 0;
    }

    /// <summary>
    /// Asynchronously disposes of the SQLite connection if it was opened.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_connection != null)
            await _connection.CloseAsync();
    }
}
