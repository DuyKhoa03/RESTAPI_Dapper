using Dapper;
using Microsoft.AspNetCore.Mvc;
using RestAPI_Dapper.Models;
using System.Data.SqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RestAPI_Dapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly string _connectionString;
        public ProductController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DbConnectionString");
        }
        // GET: api/<ProductController>
        [HttpGet]
        public async Task<IEnumerable<Product>> Get()
        {
            using(var conn = new SqlConnection(_connectionString))
            {
                if(conn.State == System.Data.ConnectionState.Closed) 
                    conn.Open();
                var result = await conn.QueryAsync<Product>("Get_Product_All", null,null,null,System.Data.CommandType.StoredProcedure);
                return result;
            }
            
        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public async Task<Product> Get(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                var para = new DynamicParameters();
                para.Add("@id", id);
                var result = await conn.QueryAsync<Product>("Get_Product_ById", para, null, null, System.Data.CommandType.StoredProcedure);
                return result.Single();
            }
        }

        // POST api/<ProductController>
        [HttpPost]
        public async Task<int> Post([FromBody] Product product)
        {
            int newId = 0;
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                var para = new DynamicParameters();
                para.Add("@sku", product.Sku);
                para.Add("@price", product.Price);
                para.Add("@isActive", product.IsActive);
                para.Add("@imageUrl", product.ImageUrl);
                para.Add("@id",dbType: System.Data.DbType.Int32,direction: System.Data.ParameterDirection.Output);
                var result = await conn.ExecuteAsync("Create_Product", para, null, null, System.Data.CommandType.StoredProcedure);
                newId = para.Get<int>("@id");
                
            }
            return newId;
        }

        // PUT api/<ProductController>/5
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody] Product product)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                var para = new DynamicParameters();
                para.Add("@id", id);
                para.Add("@sku", product.Sku);
                para.Add("@price", product.Price);
                para.Add("@isActive", product.IsActive);
                para.Add("@imageUrl", product.ImageUrl);
                
                await conn.ExecuteAsync("Update_Product", para, null, null, System.Data.CommandType.StoredProcedure);

            }
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                var para = new DynamicParameters();
                para.Add("@id", id);
                await conn.QueryAsync<Product>("Delete_Product_ById", para, null, null, System.Data.CommandType.StoredProcedure);
                
            }
        }
    }
}
