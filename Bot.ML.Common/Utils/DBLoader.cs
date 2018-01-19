using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Bot.ML.Common.Utils
{
    public class DatabaseLoader
    {
        private SqlConnection sqlConnection;
        public DatabaseLoader(string sqlConn)
        {
            sqlConnection = new SqlConnection(sqlConn);
            if (sqlConnection == null)
            {
                throw new Exception("SQL Initialize failed");
            }
            sqlConnection.Open();
        }

        public List<Dictionary<string, string>> Load(string sql)
        {
            List<Dictionary<string, string>> results = new List<Dictionary<string, string>>();

            SqlCommand comm = new SqlCommand(sql, sqlConnection);
            SqlDataReader reader = comm.ExecuteReader();
            while (reader.Read())
            {
                Dictionary<string, string> row = new Dictionary<string, string>();
                for (int i = 0; i < reader.FieldCount; ++i)
                {
                    string sColumnName = reader.GetName(i).Trim();
                    string sValue = reader[sColumnName].ToString();

                    row[sColumnName] = sValue;
                }

                results.Add(row);
            }
            reader.Close();
            return results;
        }

        public Int64 Insert(string sql)
        {
            SqlCommand command = new SqlCommand(sql, sqlConnection);
            try
            {
                Int64 id = Convert.ToInt64(command.ExecuteScalar());
                return id;
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }
        }

        public int ExecuteCommand(string sql)
        {
            SqlCommand comm = new SqlCommand(sql, sqlConnection);
            int i = int.MaxValue;
            try
            {
                i = comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return i;
        }
    }
}
