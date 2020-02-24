using GlobalRemoteSQL.Core;
using MongoDB.Driver;
using MongoDBConnector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GlobalRemoteSQL.Server.UI
{
    public class MainViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public ClientRemoteSender _sender;
        public string Command { get; set; }
        public AsyncObservableCollection<ClientModel> ConnectedClients { get; set; }
        public ClientModel SelectedClient { get; set; }
        public bool IsListenerActive { get; set; }
        public bool IsAbleToSendCommand { get; set; }
        public DataGrid DgResults { get; set; }

        public SendCommandCommand SendCommandCommand { get; set; }
        public RefreshClientsCommand RefreshClientsCommand { get; set; }

        public MainViewModel()
        {
            _sender = new ClientRemoteSender(TimeSpan.FromSeconds(5));
            _sender.CommandResponseReceived += CommandMessageReceived;

            SendCommandCommand = new SendCommandCommand(this);
            RefreshClientsCommand = new RefreshClientsCommand(this);

            ConnectedClients = new AsyncObservableCollection<ClientModel>();
            Task.Run(PopulateClients);

            IsAbleToSendCommand = true;
        }

        private void CommandMessageReceived(object sender, CommandResponseReceivedEventArgs args)
        {
            var res = sender as CommandResponseModel;

            var table = JsonConvert.DeserializeObject<DataSet>(res.Result).Tables[0];
            DgResults.Dispatcher.Invoke(() =>
            {
                DgResults.ItemsSource = null;
                DgResults.ItemsSource = table.DefaultView;
            });
            IsAbleToSendCommand = true;
        }

        public void SendCommand()
        {
            IsAbleToSendCommand = false;

            var req = new CommandRequestModel
            {
                ClientId = SelectedClient.Id,
                Command = Command,
                SenderComputerName = Environment.MachineName,
                Completed = false
            };

            _sender.Send(req);
        }

        public void PopulateClients()
        {
            ConnectedClients.Clear();

            var filter = Builders<ClientModel>.Filter.Gte(x => x.LastSeen, DateTime.Now.AddMinutes(-5));
            var onlineClients = Mongo.Get(filter).ToList();

            foreach (var c in onlineClients)
            {
                ConnectedClients.Add(c);
            }
        }
    }
}
