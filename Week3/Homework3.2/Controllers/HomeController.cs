using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using MongoDB.Driver;
using M101DotNet.WebApp.Models;
using M101DotNet.WebApp.Models.Home;
using MongoDB.Bson;

namespace M101DotNet.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly BlogContext _blogContext = new BlogContext();

        public async Task<ActionResult> Index()
        {
            return View(new IndexModel
            {
                RecentPosts = await _blogContext.Posts.Find(new BsonDocument())
                    .Sort(Builders<Post>.Sort.Descending(p => p.CreatedAtUtc))
                    .Limit(10)
                    .ToListAsync()
            });
        }

        [HttpGet]
        public ActionResult NewPost()
        {
            return View(new NewPostModel());
        }

        [HttpPost]
        public async Task<ActionResult> NewPost(NewPostModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var post = new Post()
            {
                Title = model.Title,
                Content = model.Content,
                Tags = model.Tags.Split(new[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries),
                Author = User.Identity.Name,
                CreatedAtUtc = DateTime.UtcNow,
                Comments = new List<Comment>()
            };
            await _blogContext.Posts.InsertOneAsync(post);

            return RedirectToAction("Post", new {id = post.Id});
        }

        [HttpGet]
        public async Task<ActionResult> Post(string id)
        {
            var post = await _blogContext.Posts
                .Find(Builders<Post>.Filter.Eq(p => p.Id, id))
                .FirstOrDefaultAsync();

            if (post == null)
            {
                return RedirectToAction("Index");
            }

            return View(new PostModel
            {
                Post = post
            });
        }

        [HttpGet]
        public async Task<ActionResult> Posts(string tag = null)
        {
            var filter = tag == null
                ? new BsonDocument()
                : Builders<Post>.Filter.All(p => p.Tags, new[] {tag});

            return View(await _blogContext.Posts.Find(filter).ToListAsync());
        }

        [HttpPost]
        public async Task<ActionResult> NewComment(NewCommentModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Post", new {id = model.PostId});
            }

            await _blogContext.Posts.UpdateOneAsync(p => p.Id == model.PostId,
                Builders<Post>.Update.AddToSet(x => x.Comments, new Comment()
                {
                    Author = User.Identity.Name,
                    Content = model.Content,
                    CreatedAtUtc = DateTime.Now.ToUniversalTime()
                }));

            return RedirectToAction("Post", new {id = model.PostId});
        }
    }
}