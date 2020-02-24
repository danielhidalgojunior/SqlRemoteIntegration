using MongoDB.Bson;
using MongoDB.Driver;
using MongoDBConnector;
using System;
using System.Collections.Generic;

namespace GlobalRemoteSQL.Core
{
    public class ClientModel : Entity, IReadable, IModifiable
    {
        public DateTime LastSeen { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }

        public bool Delete(out Entity deletedEntity)
        {
            deletedEntity = null;
            if (Mongo.DeleteOne(this))
            {
                deletedEntity = this;
                return true;
            }

            return false;
        }

        public Entity GetOneById(ObjectId id)
        {
            return Mongo.GetOneById<ClientModel>(id);
        }

        public IEnumerable<Entity> List()
        {
            return Mongo.Get(Builders<ClientModel>.Filter.Empty).ToEnumerable();
        }

        public bool Save()
        {
            try
            {
                if (Mongo.Exists<ClientModel>(Id))
                {
                    Mongo.UpdateOne(this);
                }
                else
                {
                    Mongo.InsertOne(this);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}