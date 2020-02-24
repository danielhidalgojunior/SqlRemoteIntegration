using MongoDB.Bson;
using MongoDBConnector;
using System;

namespace GlobalRemoteSQL.Core
{
    public class CommandResponseModel : Entity
    {
        public ObjectId RequestId { get; set; }
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        public string Result { get; set; }
    }
}