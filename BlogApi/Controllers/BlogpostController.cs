using BlogApi.Models;
using BlogApi.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace BlogApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogpostController : ControllerBase
    {
        public string ConnectionString = "server=localhost;uid=root;password=;database=blog;";

        [HttpGet]
        public List<Blogpost> GetBlogposts()
        {
            List<Blogpost> blogposts = new List<Blogpost>();

            var connection = new MySqlConnection(ConnectionString);

            connection.Open();

            string sql = "SELECT * FROM blogpost;";

            var cmd = new MySqlCommand(sql, connection);

            var data = cmd.ExecuteReader();

            while (data.Read())
            {
                var blogpost = new Blogpost
                {
                    Id = data.GetInt32("Id"),
                    Title = data.GetString("Title"),
                    Content = data.GetString("Content"),
                    PostTime = data.GetDateTime("postTime"),
                    UpdateTime = data.GetDateTime("updateTime"),
                    BlogId = data.GetInt32("blogId")
                };

                blogposts.Add(blogpost);
            }

            connection.Close();

            return blogposts;
        }

        [HttpPost]
        public object AddNewBlogpost([FromBody] AddNewBlogpostDto addNewBlogpostDto)
        {
            var connection = new MySqlConnection(ConnectionString);

            connection.Open();

            string sql = @"INSERT INTO `blogpost`
                           (`Title`, `Content`, `postTime`, `updateTime`, `blogId`)
                           VALUES
                           (@title, @content, @postTime, @updateTime, @blogId)";

            var cmd = new MySqlCommand(sql, connection);

            cmd.Parameters.AddWithValue("@title", addNewBlogpostDto.Title);
            cmd.Parameters.AddWithValue("@content", addNewBlogpostDto.Content);
            cmd.Parameters.AddWithValue("@postTime", DateTime.Now);
            cmd.Parameters.AddWithValue("@updateTime", DateTime.Now);
            cmd.Parameters.AddWithValue("@blogId", addNewBlogpostDto.BlogId);

            cmd.ExecuteNonQuery();

            connection.Close();

            return new
            {
                message = "Sikeres felvétel.",
                result = addNewBlogpostDto
            };
        }

        [HttpDelete]
        public object DeleteBlogpost(int id)
        {
            var connection = new MySqlConnection(ConnectionString);

            connection.Open();

            string sql = @"DELETE FROM `blogpost` WHERE `Id` = @id";

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
        public object UpdateBlogpost([FromQuery] int id, UpdateBlogpostDto updateBlogpostDto)
        {
            var connection = new MySqlConnection(ConnectionString);

            connection.Open();

            string sql = @"UPDATE `blogpost` SET `Title` = @title,`Content` = @content,`updateTime` = @updateTime,
                               `blogId` = @blogId
                           WHERE `Id` = @id;";

            var cmd = new MySqlCommand(sql, connection);

            cmd.Parameters.AddWithValue("@title", updateBlogpostDto.Title);
            cmd.Parameters.AddWithValue("@content", updateBlogpostDto.Content);
            cmd.Parameters.AddWithValue("@updateTime", DateTime.Now);
            cmd.Parameters.AddWithValue("@blogId", updateBlogpostDto.BlogId);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();

            connection.Close();

            return new
            {
                message = "Sikeres frissítés.",
                result = updateBlogpostDto
            };
        }

        [HttpGet("byId")]
        public object GetBlogpostById(int id)
        {
            var connection = new MySqlConnection(ConnectionString);

            connection.Open();

            string sql = @"SELECT * FROM `blogpost` WHERE `Id` = @id";

            var cmd = new MySqlCommand(sql, connection);

            cmd.Parameters.AddWithValue("@id", id);

            var datareader = cmd.ExecuteReader();

            object? data = null;

            if (datareader.Read() == true)
            {
                var blogpost = new Blogpost
                {
                    Id = datareader.GetInt32(0),
                    Title = datareader.GetString(1),
                    Content = datareader.GetString(2),
                    PostTime = datareader.GetDateTime(3),
                    UpdateTime = datareader.GetDateTime(4),
                    BlogId = datareader.GetInt32(5)
                };

                data = new
                {
                    message = "sikeres lekérdezés.",
                    result = blogpost
                };
            }
            else
            {
                data = new
                {
                    message = "nincs ilyen blogpost.",
                    result = ""
                };
            }

            connection.Close();

            return data;
        }

        // 4 adott blogger neve és email
        [HttpGet("bloggerInfo")]
        public object GetBloggerInfo(string name)
        {
            var connection = new MySqlConnection(ConnectionString);

            connection.Open();

            string sql = @"SELECT `Name`, `Email`
                           FROM `blogger`
                           WHERE `Name` = @name";

            var cmd = new MySqlCommand(sql, connection);

            cmd.Parameters.AddWithValue("@name", name);

            var datareader = cmd.ExecuteReader();

            object? data = null;

            if (datareader.Read() == true)
            {
                data = new
                {
                    message = "Sikeres lekérdezés.",
                    result = new
                    {
                        Name = datareader.GetString("Name"),
                        Email = datareader.IsDBNull(datareader.GetOrdinal("Email"))
                            ? null
                            : datareader.GetString("Email")
                    }
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

        // 5 adott blogger osszes postja inner joinnal
        [HttpGet("bloggerPosts")]
        public List<object> GetBloggerPosts(string name)
        {
            List<object> posts = new List<object>();

            var connection = new MySqlConnection(ConnectionString);

            connection.Open();

            string sql = @"SELECT blogger.Name,
                                  blogpost.Title,
                                  blogpost.Content
                           FROM blogger
                           INNER JOIN blogpost
                           ON blogger.Id = blogpost.blogId
                           WHERE blogger.Name = @name;";

            var cmd = new MySqlCommand(sql, connection);

            cmd.Parameters.AddWithValue("@name", name);

            var datareader = cmd.ExecuteReader();

            while (datareader.Read())
            {
                posts.Add(new
                {
                    Name = datareader.GetString("Name"),
                    Title = datareader.GetString("Title"),
                    Content = datareader.GetString("Content")
                });
            }

            connection.Close();

            return posts;
        }

        //6 osszes blogpost szama
        [HttpGet("postCount")]
        public object GetPostCount()
        {
            var connection = new MySqlConnection(ConnectionString);

            connection.Open();

            string sql = @"SELECT COUNT(*) FROM `blogpost`";

            var cmd = new MySqlCommand(sql, connection);

            int count = Convert.ToInt32(cmd.ExecuteScalar());

            connection.Close();

            return new
            {
                message = "Sikeres lekérdezés.",
                result = count
            };
        }

        // 7. Egy adott blogger postjainak száma
        [HttpGet("bloggerPostCount")]
        public object GetBloggerPostCount(int id)
        {
            var connection = new MySqlConnection(ConnectionString);

            connection.Open();

            string sql = @"SELECT COUNT(*)
                           FROM `blogpost`
                           WHERE `blogId` = @id";

            var cmd = new MySqlCommand(sql, connection);

            cmd.Parameters.AddWithValue("@id", id);

            int count = Convert.ToInt32(cmd.ExecuteScalar());

            connection.Close();

            return new
            {
                message = "Sikeres lekérdezés.",
                result = count
            };
        }
    }
}