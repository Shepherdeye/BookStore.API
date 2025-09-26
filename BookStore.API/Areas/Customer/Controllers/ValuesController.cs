using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookStore.API.Areas.Customer.Controllers

{
    [Area(SD.CustomerArea)]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IRepository<Book> _repositoryBook;

        public ValuesController(IRepository<Book> repositoryBook)
        {
            _repositoryBook = repositoryBook;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(int page = 1)
        {

            var books = (await _repositoryBook.GetAsync(includes: [e => e.Auther])).AsQueryable();

            var totalPages = Math.Ceiling(books.Count() / 30.00);
            //pagination
            books = books.Skip((page - 1) * 30).Take(30);

            var retuenedBooks = books.Adapt<List<BookResponse>>();

            return Ok(new
            {
                TotalPages = totalPages,
                Books = retuenedBooks,
                Page = page
            });

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {

            var book = await _repositoryBook.GetOneAsync(e => e.Id == id, includes: [e => e.Auther]);
            if (book is null) return NotFound();

            var retuenedBook = book.Adapt<BookResponse>();


            var relatedBooks = (await _repositoryBook.GetAsync(e => e.AutherId == book.AutherId && e.Id != book.Id))
                .Skip(0).Take(4);

            var mostRatedBooks = (await _repositoryBook.GetAsync(e => e.Id != book.Id))
                .OrderByDescending(e => e.Rate).Skip(0).Take(4);


            var similarBooks = (await _repositoryBook.GetAsync(e => e.Title.Contains(book.Title) && e.Id != book.Id))
                 .Skip(0).Take(4);



            return Ok(new
            {
                Book = retuenedBook,
                RelatedBooks= relatedBooks,
                MostRatedBooks= mostRatedBooks,
                SimilarBooks= similarBooks
            });



        }

    }
}
