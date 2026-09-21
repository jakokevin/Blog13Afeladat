using BlogApi.Models;
using BlogApi.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace BlogApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        public string ConnectionString = "server=localhost;uid=root;password=;database=blog;";

        [HttpGet]
        public List<Blogger> GetBloggers()
        {
            List<Blogger> bloggers = new List<Blogger>();

            var connection = new MySqlConnection(ConnectionString);

            connection.Open();

            string sql = "SELECT * FROM blogger;";

            var cmd = new MySqlCommand(sql, connection);

            var data = cmd.ExecuteReader();

            while (data.Read())
            {
                var blogger = new Blogger
                {
                    Id = data.GetInt32("id"),
                    Name = data.GetString("name"),
                    Email = data.IsDBNull(data.GetOrdinal("email")) ? null : data.GetString("email"),
                    Age = data.GetInt32("age"),
                    Password = data.IsDBNull(data.GetOrdinal("password")) ? null : data.GetString("password"),
                    RegistrationTime = data.GetDateTime("registrationTime")
                };

                bloggers.Add(blogger);
            }

            connection.Close();

            return bloggers;
        }

        [HttpPost]
        public object AddNewBlogger([FromBody] AddNewBloggerDto addNewBloggerDto)
        {
            var connection = new MySqlConnection(ConnectionString);

            connection.Open();

            string sql = @"INSERT INTO `blogger`
                           (`name`, `email`, `age`, `password`, `registrationTime`)
                           VALUES
                           (@name,@email,@age,@password,@registrationTime)";

            var cmd = new MySqlCommand(sql, connection);

            cmd.Parameters.AddWithValue("@name", addNewBloggerDto.Name);
            cmd.Parameters.AddWithValue("@email", addNewBloggerDto.Email);
            cmd.Parameters.AddWithValue("@age", addNewBloggerDto.Age);
            cmd.Parameters.AddWithValue("@password", addNewBloggerDto.Password);
            cmd.Parameters.AddWithValue("@registrationTime", DateTime.Now);

            cmd.ExecuteNonQuery();

            connection.Close();

            return new
            {
                message = "Sikeres felvétel.",
                result = addNewBloggerDto
            };
        }

        [HttpDelete]
        public object DeleteBlogger(int id)
        {
            var connection = new MySqlConnection(ConnectionString);

            connection.Open();

            string sql = @"DELETE FROM `blogger` WHERE `id` = @id";

            var cmd = new MySqlCommand(sql, connection);

            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();

            connection.Close();

            return new
            {
                message = "Sikeres törlés.",
                result = ""
            };
        }

        [HttpPut]
        public object UpdateBlogger([FromQuery] int id, UpdateBloggerDto updateBloggerDto)
        {
            var connection = new MySqlConnection(ConnectionString);

            connection.Open();

            string sql = @"UPDATE `blogger`
                           SET `name`=@name,
                               `email`=@email,
                               `age`=@age,
                               `password`=@password
                           WHERE `id` = @id;";

            var cmd = new MySqlCommand(sql, connection);

            cmd.Parameters.AddWithValue("@name", updateBloggerDto.Name);
            cmd.Parameters.AddWithValue("@email", updateBloggerDto.Email);
            cmd.Parameters.AddWithValue("@age", updateBloggerDto.Age);
            cmd.Parameters.AddWithValue("@password", updateBloggerDto.Password);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();

            connection.Close();

            return new
            {
                message = "Sikeres frissítés",
                result = updateBloggerDto
            };
        }

        [HttpGet("byId")]
        public object GetBloggerById(int id)
        {
            var connection = new MySqlConnection(ConnectionString);

            connection.Open();

            string sql = @"SELECT * FROM `blogger` WHERE `id` = @id";

            var cmd = new MySqlCommand(sql, connection);

            cmd.Parameters.AddWithValue("@id", id);

            var datareader = cmd.ExecuteReader();

            object? data = null;

            if (datareader.Read() == true)
            {
                var blogger = new Blogger
                {
                    Id = datareader.GetInt32(0),
                    Name = datareader.GetString(1),
                    Email = datareader.IsDBNull(2) ? null : datareader.GetString(2),
                    Age = datareader.GetInt32(3),
                    Password = datareader.IsDBNull(4) ? null : datareader.GetString(4),
                    RegistrationTime = datareader.GetDateTime(5)
                };

                data = new
                {
                    message = "Sikeres lekérdezés.",
                    result = blogger
                };
            }
            else
            {
                data = new
                {
                    message = "Nincs ilyen blogger.",
                    result = ""
                };
            }

            connection.Close();

            return data;
        }
    }
}