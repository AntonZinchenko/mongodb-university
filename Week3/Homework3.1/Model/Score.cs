using MongoDB.Bson.Serialization.Attributes;

namespace homework3._1.Model
{
    internal class Score
    {
        [BsonElement("type")]
        public string Type { get; set; }

        [BsonElement("score")]
        public double Value { get; set; }
    }
}