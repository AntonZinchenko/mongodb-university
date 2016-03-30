using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace homework3._1.Model
{
    internal class Student
    {
        public int Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("scores")]
        public IList<Score> Scores { get; set; }
    }
}