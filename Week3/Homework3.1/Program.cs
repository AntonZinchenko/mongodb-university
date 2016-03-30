using System;
using System.Linq;
using System.Threading.Tasks;
using homework3._1.Model;
using MongoDB.Bson;
using MongoDB.Driver;

namespace homework3._1
{
    internal class Program
    {
        private const string ConnectionString = @"mongodb://localhost:27017";

        private static void Main()
        {
            MainAsync().Wait();
        }

        public static async Task MainAsync()
        {
            var collection = new MongoClient(ConnectionString)
                .GetDatabase("school")
                .GetCollection<Student>("students");

            var searchResult = await collection
                .Find(new BsonDocument())
                .ToListAsync();

            searchResult.ForEach(async student =>
            {
                var deletedObj = student.Scores.Where(score => score.Type == "homework")
                    .OrderBy(score => score.Value)
                    .FirstOrDefault();

                var updatedScores = student.Scores.ToList();

                if (deletedObj != null)
                    updatedScores.Remove(deletedObj);

                await collection.UpdateOneAsync(s => s.Id == student.Id,
                    Builders<Student>.Update.Set(s => s.Scores, updatedScores));

                Console.WriteLine("student:{0}, scores:{1}",
                    student.Id,
                    string.Join(",",
                        updatedScores.Select(score => $"type:{score.Type}, score:{score.Value}")));
            });
        }
    }
}
