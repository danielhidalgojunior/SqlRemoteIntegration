using MongoDB.Bson;
using MongoDBConnector;
using System;
using System.Collections.Generic;

namespace GlobalRemoteSQL.Core
{
    public class CommandHistoryItemModel : Entity, IReadable, IModifiable
    {
        public ActionType Action { get; set; }
        public DateTime RequestedDateTime { get; set; }
        public ObjectId RequestId { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        public string ComputerSender { get; set; }
        public bool Error { get; set; }
        public string ErrorMessage { get; set; }

        private CommandRequestModel _request;

        public CommandRequestModel Request
        {
            get
            {
                if (_request == null || RequestId != null)
                {
                    _request = Mongo.GetOneById<CommandRequestModel>(RequestId);
                }

                return _request;
            }
            set
            {
                _request = value;
            }
        }

        public IEnumerable<Entity> List()
        {
            throw new NotImplementedException();
        }

        public Entity GetOneById(ObjectId id)
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            throw new NotImplementedException();
        }

        public bool Delete(out Entity deletedEntity)
        {
            throw new NotImplementedException();
        }
    }
}