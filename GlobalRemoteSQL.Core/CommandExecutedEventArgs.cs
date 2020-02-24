using System;
using System.Data;

namespace GlobalRemoteSQL.Core
{
    public class CommandExecutedEventArgs : EventArgs
    {
        public string Command;
        public TimeSpan ExecutionTime;
        public bool HasError;
        public string ErrorMessage;
        public DataSet Result;

        public CommandExecutedEventArgs(string command, TimeSpan executionTime, bool hasError, string errorMessage, DataSet result)
        {
            Command = command;
            ExecutionTime = executionTime;
            HasError = hasError;
            ErrorMessage = errorMessage;
            Result = result;
        }
    }
}