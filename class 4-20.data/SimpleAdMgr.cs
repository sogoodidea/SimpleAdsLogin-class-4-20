using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace class_4_20.data
{
    public class SimpleAdMgr
    {
        private string _conStr = @"Data Source=.\sqlexpress;Initial Catalog=SimpleAdProject;Integrated Security=True;";

        public void AddUser(User user, string password)
        {
            string hash = BCrypt.Net.BCrypt.HashPassword(password);
            using (SqlConnection conn = new SqlConnection(_conStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Users(Email, Name, PhoneNumber, PasswordHash)" +
                                  "VALUES(@email, @name, @phonenumber, @hashpassword)";
                cmd.Parameters.AddWithValue("@email", user.Email);
                cmd.Parameters.AddWithValue("@name", user.Name);
                cmd.Parameters.AddWithValue("@phonenumber", user.PhoneNumber);
                cmd.Parameters.AddWithValue("@hashpassword", hash);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public User GetUserByEmail(string email)
        {
            using (SqlConnection conn = new SqlConnection(_conStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Users WHERE email = @email";
                cmd.Parameters.AddWithValue("@email", email);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }
                return (new User
                {
                    Id = (int)reader["Id"],
                    Email = (string)reader["Email"],
                    Name = (string)reader["Name"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    PasswordHash = (string)reader["PasswordHash"]
                });
            }
        }
        public User Login(string email, string password)
        {
            var user = GetUserByEmail(email);
            if (user == null)
            {
                return null;
            }
            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (isValid)
            {
                return user;
            }
            return null;
        }
        public List<SimpleAd> GetSimpleAds()
        {
            using (SqlConnection conn = new SqlConnection(_conStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT * FROM Ads a JOIN Users u ON a.UserId = u.Id ORDER BY DateCreated DESC ";
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                List<SimpleAd> ads = new List<SimpleAd>();
                while (reader.Read())
                {
                    ads.Add(ReadAd(reader));
                }
                return ads;
            }
        }
        public void AddPost(SimpleAd ad)
        {
            using (SqlConnection conn = new SqlConnection(_conStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO Ads(UserId, DateCreated, Text, Name)
                                    VALUES(@userId, GETDATE(), @text, @name)";
                object name = ad.Name;
                if (name == null)
                {
                    name = DBNull.Value;
                }
                cmd.Parameters.AddWithValue("@userId", ad.UserId);
                cmd.Parameters.AddWithValue("@text", ad.Text);
                cmd.Parameters.AddWithValue("@name", ad.Name);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public List<SimpleAd> GetAdsForUserId(int userId)
        {
            using (SqlConnection conn = new SqlConnection(_conStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT * FROM Ads a JOIN Users u ON a.UserId = u.Id WHERE a.UserId = @id ORDER BY DateCreated DESC ";
                cmd.Parameters.AddWithValue("@id", userId);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                List<SimpleAd> ads = new List<SimpleAd>();
                while (reader.Read())
                {
                    ads.Add(ReadAd(reader));
                }
                return ads;
            }
        }
        public void DeleteAd(int adId)
        {
            using (SqlConnection conn = new SqlConnection(_conStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM Ads WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", adId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        private SimpleAd ReadAd(SqlDataReader reader)
        {
            return new SimpleAd
            {
                Id = (int)reader["Id"],
                DateCreated = (DateTime)reader["DateCreated"],
                Text = (string)reader["Text"],
                Name = reader.GetOrNull<string>("Name"),
                PhoneNumber = (string)reader["PhoneNumber"],
                UserId = (int)reader["UserId"]
            };
        }

    }



}
