﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;


namespace LendingSystem
{
    class Connection
    {

        public static MySqlConnection con()
        {
            return new MySqlConnection("server=localhost; database=lendingsystem;user=root;password=''");
        }
    }
}
