using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.Interfaces;

namespace ShoppingOnline.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IPostRepository _postRepository;

        public ArticleController(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }
        [Route("post/{alias}")]
        public async Task<IActionResult> Index(string alias)
        {
            var post = await _postRepository.GetByStringAsync(alias);
            return View(post);
        }
    }
}
