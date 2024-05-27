using System.Data.SqlClient;
using static System.Net.Mime.MediaTypeNames;

namespace Homework03._11.Data
{
    public class Manager
    {
        private string _connectionString;

        public Manager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool CorrectPassword(int id, string password)
        {
            SqlConnection connection = new(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Password FROM Images WHERE Id = @id";

            cmd.Parameters.AddWithValue("@id", id);

            connection.Open();

            SqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return false;
            }

            return (string)reader["Password"] == password;
        }

        public void Add(ImageClass image)
        {
            SqlConnection connection = new(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Images  (Password, ImagePath,Views) " +
                "VALUES (@password, @path, 0) SELECT SCOPE_IDENTITY()";

            cmd.Parameters.AddWithValue("@password", image.Password);
            cmd.Parameters.AddWithValue("@path", image.ImagePath);

            connection.Open();
            image.Id = (int)(decimal)cmd.ExecuteScalar();
        }

        public ImageClass GetImage(int id)
        {
            SqlConnection connection = new(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Images WHERE Id = @id";

            cmd.Parameters.AddWithValue("@id", id);

            connection.Open();

            SqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new()
            {
                Password = (string)reader["Password"],
                Id = (int)reader["Id"],
                ImagePath = (string)reader["ImagePath"],
                Views = (int)reader["Views"]
            };

        }

        public void AddView(int id)
        {
            SqlConnection connection = new(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"UPDATE Images
                                SET Views = Views + 1
                                WHERE Id = @id";

            cmd.Parameters.AddWithValue("@id", id);

            connection.Open();
            cmd.ExecuteNonQuery();

        }
    }
}
