﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace LendingSystem
{
    class Login
    {
        MySqlCommand cmd;
        MySqlConnection con;
        string query;

        public bool auth(string username, string password)
        {
            bool flag = false;
            try
            {
                con = Connection.con();
                con.Open();
                query = "SELECT * FROM users WHERE username=?user AND password=sha1(?pass) LIMIT 1";

                cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("?user", username);
                cmd.Parameters.AddWithValue("?pass", password);
                MySqlDataReader dr;
                dr = cmd.ExecuteReader();
                dr.Close();
                cmd.Dispose();
                con.Close();
                con.Dispose();
                return flag;
            }

            catch (Exception)
            {

                return false;
            }
        }
    }
}
