using MongoDB.Driver;
using MongoDBConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace GlobalRemoteSQL.Core
{
    public class ClientRemoteListener
    {
        private ClientModel _client;
        private TimeSpan _checkSpan;
        private SqlManager _sqlManager;
        public bool IsListening { get; set; }
        private BackgroundWorker _workerCheck;
        private BackgroundWorker _workerExecuteCommands;
        private BackgroundWorker _workerUpdateClientStatus;

        public List<CommandRequestModel> CommandQueue { get; private set; }
        public EventHandler<CommandReceivedEventArgs> CommandReceived;
        public EventHandler<CommandExecutedEventArgs> CommandExecuted;

        public ClientRemoteListener(ClientModel client, TimeSpan checkSpan, SqlManager sqlManager)
        {
            _client = client;
            _checkSpan = checkSpan;
            _sqlManager = sqlManager;
            IsListening = false;

            CommandQueue = new List<CommandRequestModel>();
            _workerCheck = new BackgroundWorker();
            _workerExecuteCommands = new BackgroundWorker();
            _workerUpdateClientStatus = new BackgroundWorker();

            _workerCheck.DoWork += checkForCommands;
            _workerExecuteCommands.DoWork += executeCommands;
            _workerUpdateClientStatus.DoWork += updateClientStatus;
        }

        private void updateClientStatus(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (IsListening)
                {
                    UpdateClientStatus();
                }

                Thread.Sleep(TimeSpan.FromMinutes(5));
            }
        }

        private void executeCommands(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (IsListening)
                {
                    if (CommandQueue.Count > 0)
                    {
                        var commandToExecute = CommandQueue.First();
                        var sqlRes = _sqlManager.SendCommand(commandToExecute.Command);
                        CommandQueue.Remove(commandToExecute);
                        commandToExecute.Completed = true;

                        var filter = Builders<CommandRequestModel>.Filter.Eq(x => x.Id, commandToExecute.Id);
                        var update = Builders<CommandRequestModel>.Update.Set(x => x.Completed, commandToExecute.Completed);

                        if (!Mongo.UpdateField(filter, update))
                        {
                            commandToExecute.Completed = false;
                            Console.WriteLine("Não foi possível atualizar o valor do campo");
                        }

                        CommandExecuted?.Invoke(commandToExecute, new CommandExecutedEventArgs(commandToExecute.Command, sqlRes.ExecutionTime, sqlRes.HasError, sqlRes.ErrorMessage, sqlRes.DataSet));
                    }

                    Thread.Sleep(_checkSpan);
                }
            }
        }

        private void UpdateClientStatus()
        {
            _client.LastSeen = DateTime.Now;

            var filter = Builders<ClientModel>.Filter.Eq(x => x.Id, _client.Id);
            var update = Builders<ClientModel>.Update.Set(x => x.LastSeen, _client.LastSeen);

            if (!Mongo.UpdateField(filter, update))
            {
                Console.WriteLine("Não foi possível atualizar o valor do campo de status do cliente");
            }
        }

        private void checkForCommands(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (IsListening)
                {
                    var commandsToQueue = GetCommandsToExecute();

                    if (commandsToQueue == null || commandsToQueue.Count == 0)
                    {
                        continue;
                    }

                    foreach (var cmd in commandsToQueue.ToList())
                    {
                        if (CommandQueue.Exists(x => x.Id == cmd.Id))
                        {
                            continue;
                        }

                        CommandQueue.Add(cmd);
                        CommandReceived?.Invoke(cmd, new CommandReceivedEventArgs(cmd.Command));
                        Console.WriteLine("Command received");
                    }
                }

                Thread.Sleep(_checkSpan);
            }
        }

        public void Start()
        {
            IsListening = true;
            _workerCheck.RunWorkerAsync();
            _workerExecuteCommands.RunWorkerAsync();
            _workerUpdateClientStatus.RunWorkerAsync();
        }

        public void Stop()
        {
            IsListening = false;
            _workerCheck.CancelAsync();
            _workerExecuteCommands.CancelAsync();
            _workerUpdateClientStatus.CancelAsync();
        }

        public List<CommandRequestModel> GetCommandsToExecute()
        {
            var clientFilter = Builders<CommandRequestModel>.Filter.Eq(x => x.ClientId, _client.Id);
            var waitingResponseFilter = Builders<CommandRequestModel>.Filter.Eq(x => x.Completed, false);
            var filter = Builders<CommandRequestModel>.Filter.And(clientFilter, waitingResponseFilter);

            var results = Mongo.Get(filter).ToList();

            return results;
        }
    }
}