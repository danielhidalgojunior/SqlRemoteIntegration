using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace GlobalRemoteSQL.Core
{
    public class SqlManager
    {
        public string ConnectionString { get; private set; }

        public SqlManager(string connectionString)
        {
            if (connectionString == null)
            {
                throw new Exception("Connection String can't be empty");
            }

            ConnectionString = connectionString;
        }

        public SqlCommandResponse SendCommand(string command)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            SqlCommandResponse res = new SqlCommandResponse();
            try
            {
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = ConnectionString;
                    con.Open();
                    using (SqlCommand com = new SqlCommand(command))
                    {
                        com.Connection = con;
                        com.CommandType = CommandType.Text;

                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(com))
                        {
                            DataTable dt = new DataTable();
                            sqlAdapter.Fill(dt);

                            res.DataSet.Tables.Add(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao tentar executar comando");
                res.HasError = true;
                res.ErrorMessage = ex.Message;
            }
            sw.Stop();
            res.ExecutionTime = sw.Elapsed;

            return res;
        }
    }
}