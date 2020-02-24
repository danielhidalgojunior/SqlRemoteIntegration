using MongoDB.Driver;
using MongoDBConnector;
using System;
using System.ComponentModel;
using System.Threading;

namespace GlobalRemoteSQL.Core
{
    public class ClientRemoteSender
    {
        private CommandRequestModel _request;
        public EventHandler<CommandResponseReceivedEventArgs> CommandResponseReceived;
        public bool IsListening { get; set; }

        private TimeSpan _checkSpan;
        private BackgroundWorker _workerCheckResponse;

        public ClientRemoteSender(TimeSpan checkSpan)
        {
            _checkSpan = checkSpan;
            _workerCheckResponse = new BackgroundWorker();
            _workerCheckResponse.DoWork += checkForResponse;
        }

        private void checkForResponse(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (IsListening)
                {
                    var filter = Builders<CommandResponseModel>.Filter.Eq(x => x.RequestId, _request.Id);
                    var res = Mongo.GetOne(filter);

                    if (res == null)
                    {
                        continue;
                    }

                    CommandResponseReceived?.Invoke(res, new CommandResponseReceivedEventArgs());
                    //_workerCheckResponse.CancelAsync();
                    IsListening = false;
                    break;
                }

                Thread.Sleep(_checkSpan);
            }
        }

        public void Send(CommandRequestModel req)
        {
            _request = req;
            Mongo.InsertOne(_request);

            IsListening = true;
            _workerCheckResponse.RunWorkerAsync();
        }
    }
}