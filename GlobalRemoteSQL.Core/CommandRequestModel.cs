using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDBConnector;
using System;
using System.Collections.Generic;

namespace GlobalRemoteSQL.Core
{
    public class CommandRequestModel : Entity, IModifiable, IReadable
    {
        public ObjectId ClientId { get; set; }

        [BsonIgnore]
        private ClientModel _client;

        [BsonIgnore]
        public ClientModel Client
        {
            get
            {
                if (_client == null && ClientId != null)
                {
                    _client = Mongo.GetOneById<ClientModel>(ClientId);
                }

                return _client;
            }
        }

        public string SenderComputerName { get; set; }
        public string Command { get; set; }
        public bool Completed { get; set; }

        public bool Delete(out Entity deletedEntity)
        {
            throw new NotImplementedException();
        }

        public Entity GetOneById(ObjectId id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Entity> List()
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            throw new NotImplementedException();
        }
    }
}