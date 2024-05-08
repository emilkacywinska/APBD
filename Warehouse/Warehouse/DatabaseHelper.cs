using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace YourNamespace
{
    public interface IDatabaseHelper
    {
        Task<bool> CheckIfProductExists(int productId);
        Task<bool> CheckIfWarehouseExists(int warehouseId);
        Task<bool> CheckIfOrderExists(int productId, int amount, DateTime createdAt);
        Task<bool> CheckIfOrderFulfilled();
        Task UpdateOrderFulfilledDate();
        Task<int> InsertIntoProductWarehouse(int productId, int warehouseId, int amount, DateTime createdAt);
    }

    public class DatabaseHelper : IDatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        
        public async Task<bool> CheckIfProductExists(int productId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT COUNT(*) FROM Products WHERE Id = @ProductId", connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);
                    var result = (int)await command.ExecuteScalarAsync();
                    return result > 0;
                }
            }
        }

        public async Task<bool> CheckIfWarehouseExists(int warehouseId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT COUNT(*) FROM Warehouses WHERE Id = @WarehouseId", connection))
                {
                    command.Parameters.AddWithValue("@WarehouseId", warehouseId);
                    var result = (int)await command.ExecuteScalarAsync();
                    return result > 0;
                }
            }
        }

        public async Task<bool> CheckIfOrderExists(int productId, int amount, DateTime createdAt)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT COUNT(*) FROM Orders WHERE ProductId = @ProductId AND Amount = @Amount AND CreatedAt < @CreatedAt", connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);
                    command.Parameters.AddWithValue("@Amount", amount);
                    command.Parameters.AddWithValue("@CreatedAt", createdAt);
                    var result = (int)await command.ExecuteScalarAsync();
                    return result > 0;
                }
            }
        }

        public async Task<bool> CheckIfOrderFulfilled()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT COUNT(*) FROM Product_Warehouse", connection))
                {
                    var result = (int)await command.ExecuteScalarAsync();
                    return result > 0;
                }
            }
        }

        public async Task UpdateOrderFulfilledDate()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("UPDATE Orders SET FullfilledAt = GETDATE()", connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> InsertIntoProductWarehouse(int productId, int warehouseId, int amount, DateTime createdAt)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("INSERT INTO Product_Warehouse (ProductId, WarehouseId, Amount, Price, CreatedAt) VALUES (@ProductId, @WarehouseId, @Amount, (SELECT Price FROM Products WHERE Id = @ProductId) * @Amount, @CreatedAt); SELECT SCOPE_IDENTITY();", connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);
                    command.Parameters.AddWithValue("@WarehouseId", warehouseId);
                    command.Parameters.AddWithValue("@Amount", amount);
                    command.Parameters.AddWithValue("@CreatedAt", createdAt);
                    var insertedId = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(insertedId);
                }
            }
        }
    }
}
