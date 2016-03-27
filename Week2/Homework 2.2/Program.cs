using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace homework2._2
{
    internal class Program
    {
        private static void Main()
        {
            MainAsync().Wait();
        }

        public static async Task MainAsync()
        {
            var collection = new MongoClient("mongodb://localhost:27017").GetDatabase("students")
                    .GetCollection<BsonDocument>("grades");

            var searchResult = await collection
                    .Find(Builders<BsonDocument>.Filter.Eq("type", "homework"))
                    .Sort(Builders<BsonDocument>.Sort.Ascending("student_id").Ascending("score"))
                    .ToListAsync();

            foreach (var doc in searchResult
                .GroupBy(doc => doc["student_id"].ToString())
                .Select(@group => @group.First()))
            {
                await collection.DeleteManyAsync(Builders<BsonDocument>.Filter.Eq("_id", doc["_id"]));
                Console.WriteLine("Delete doc:{0}", doc["_id"]);
            }
        }
    }
}
