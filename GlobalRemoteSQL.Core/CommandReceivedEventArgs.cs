using System;

namespace GlobalRemoteSQL.Core
{
    public class CommandReceivedEventArgs : EventArgs
    {
        private string _command;

        public CommandReceivedEventArgs(string command)
        {
            _command = command;
        }
    }
}