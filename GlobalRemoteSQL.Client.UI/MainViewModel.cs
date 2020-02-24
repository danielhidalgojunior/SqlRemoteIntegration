using CnxGlobalCartorios;
using GlobalRemoteSQL.Core;
using MongoDB.Driver;
using MongoDBConnector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GlobalRemoteSQL.Client.UI
{
    public class MainViewModel
    {
        public AsyncObservableCollection<CommandHistoryItemModel> History { get; set; }

        public MainViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                History = new AsyncObservableCollection<CommandHistoryItemModel>();

                History.Add(new CommandHistoryItemModel
                {
                    Action = ActionType.Received,
                    ComputerSender = "asd",
                    CreatedDate = DateTime.Now,
                    Error = false,
                    ModifiedDate = DateTime.Now,
                    ExecutionTime = TimeSpan.FromSeconds(12),
                    RequestedDateTime = DateTime.Now
                });

                History.Add(new CommandHistoryItemModel
                {
                    Action = ActionType.Executed,
                    ComputerSender = "asd",
                    CreatedDate = DateTime.Now,
                    Error = true,
                    ErrorMessage = "nao deu pra executar",
                    ModifiedDate = DateTime.Now,
                    ExecutionTime = TimeSpan.FromSeconds(12),
                    RequestedDateTime = DateTime.Now,
                });

                History.Add(new CommandHistoryItemModel
                {
                    Action = ActionType.Returned,
                    ComputerSender = "asd",
                    CreatedDate = DateTime.Now,
                    Error = false,
                    ModifiedDate = DateTime.Now,
                    ExecutionTime = TimeSpan.FromSeconds(12),
                    RequestedDateTime = DateTime.Now
                });

                return;
            }

            History = new AsyncObservableCollection<CommandHistoryItemModel>();
            var client = GetClientData();

            var filter = Builders<ClientModel>.Filter.Eq(x => x.Identifier, client.Identifier);

            var results = Mongo.Get(filter).ToList();
            if (results.Any())
            {
                client = results.FirstOrDefault();
            }
            else
            {
                Mongo.InsertOne(client);
                client = Mongo.Get(filter).FirstOrDefault();
            }

            var manager = new SqlManager(ConexaoGlobalCartorios.StringDeConexaoAoBanco);

            var listener = new ClientRemoteListener(client, TimeSpan.FromSeconds(5), manager);

            listener.CommandReceived += CommandReceived;
            listener.CommandExecuted += CommandExecuted;

            listener.Start();
        }

        private ClientModel GetClientData()
        {
            var sm = new SqlManager(ConexaoGlobalCartorios.StringDeConexaoAoBanco);
            var res = sm.SendCommand("SELECT NomeCartorio, CNPJ FROM tblGECartorio");

            var client = new ClientModel
            {
                Name = res.DataSet.Tables[0].Rows[0]["NomeCartorio"].ToString(),
                Identifier = res.DataSet.Tables[0].Rows[0]["CNPJ"].ToString()
            };

            return client;
        }

        private void CommandReceived(object sender, CommandReceivedEventArgs args)
        {
            var request = (sender as CommandRequestModel);
            var h = new CommandHistoryItemModel
            {
                Action = ActionType.Received,
                RequestId = request.Id,
                RequestedDateTime = request.CreatedDate.Value,
                ExecutionTime = TimeSpan.FromSeconds(0),
                ComputerSender = request.SenderComputerName,
                Error = false
            };

            History.Add(h);

            if (!Mongo.InsertOne(h))
            {
                Console.WriteLine("Erro ao salvar histórico");
            }
        }

        private void CommandExecuted(object sender, CommandExecutedEventArgs args)
        {
            var request = (sender as CommandRequestModel);
            var h = new CommandHistoryItemModel
            {
                Action = ActionType.Executed,
                RequestId = request.Id,
                RequestedDateTime = request.CreatedDate.Value,
                ExecutionTime = args.ExecutionTime,
                ComputerSender = request.SenderComputerName,
                Error = args.HasError,
                ErrorMessage = args.ErrorMessage,
            };

            History.Add(h);

            if (!Mongo.InsertOne(h))
            {
                Console.WriteLine("Erro ao tentar salvar comando de execução no histórico");
            }

            var res = new CommandResponseModel
            {
                ErrorMessage = args.ErrorMessage,
                HasError = args.HasError,
                ExecutionTime = args.ExecutionTime,
                RequestId = request.Id,
                Result = JsonConvert.SerializeObject(args.Result)
            };

            Mongo.InsertOne(res);

            var h2 = new CommandHistoryItemModel
            {
                Action = ActionType.Returned,
                RequestId = request.Id,
                RequestedDateTime = request.CreatedDate.Value,
                ExecutionTime = TimeSpan.FromSeconds(0),
                ComputerSender = request.SenderComputerName,
                Error = args.HasError,
                ErrorMessage = args.ErrorMessage
            };

            History.Add(h2);

            if (!Mongo.InsertOne(h2))
            {
                Console.WriteLine("Erro ao tentar salvar resultado retornado no histórico");
            }
        }
    }
}
