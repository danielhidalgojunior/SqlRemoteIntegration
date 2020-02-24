using System;
using System.Data;

namespace GlobalRemoteSQL.Core
{
    public class SqlCommandResponse
    {
        public DataSet DataSet { get; set; }
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
        public TimeSpan ExecutionTime { get; set; }

        public SqlCommandResponse()
        {
            DataSet = new DataSet();
        }
    }
}