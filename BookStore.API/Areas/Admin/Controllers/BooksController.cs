using BookStore.API.Repositories;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace BookStore.API.Areas.Admin.Controllers
{
    [Area(SD.AdminArea)]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IRepository<Book> _bookRepository;

        public BooksController(IRepository<Book> bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var books = await _bookRepository.GetAsync(includes: [e => e.Auther]);

            var returnedBooks = books.Adapt<List<BookResponse>>();

            return Ok(returnedBooks);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookRepository.GetOneAsync(e => e.Id == id, includes: [e => e.Auther]);
            if (book is null)
            {
                return NotFound();
            } 

            var returnedBook = book.Adapt<BookResponse>();

            return Ok(returnedBook);

        }
        [HttpPost("")]
        public async Task<IActionResult> Create(BookRequest bookRequest)
        {
            var book = bookRequest.Adapt<Book>();

            if (bookRequest.ImageUrl is not null && bookRequest.ImageUrl.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(bookRequest.ImageUrl.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await bookRequest.ImageUrl.CopyToAsync(stream);
                }

                book.ImageUrl = fileName;

                var createdBook = await _bookRepository.CreateAsync(book);
                await _bookRepository.CommitAsync();

                return Created($"{Request.Scheme}://{Request.Host}/api/admin/books/{createdBook.Id}",
                    new { msg = "Book Created Successfully !" });

            }
            else
            {
                return BadRequest();
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, BookEditRequest bookRequestEdit)
        {

            var bookDB = await _bookRepository.GetOneAsync(e => e.Id == id, tracked: false);
            if (bookDB is null)
                return NotFound();

            //we set  the id  here  to make  it  update  not create a new one
            var book = bookRequestEdit.Adapt<Book>();
            book.Id = id;


            if (bookRequestEdit.ImageUrl is not null && bookRequestEdit.ImageUrl.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(bookRequestEdit.ImageUrl.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await bookRequestEdit.ImageUrl.CopyToAsync(stream);
                }

                book.ImageUrl = fileName;


                //remove the old image
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", bookDB.ImageUrl);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }

            }
            else
            {
                book.ImageUrl = bookDB.ImageUrl;
            }


            _bookRepository.Update(book);
            await _bookRepository.CommitAsync();


            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {

            var book = await _bookRepository.GetOneAsync(e => e.Id == id);


            if (book is null)
                return NotFound();
            _bookRepository.Delete(book);
            await _bookRepository.CommitAsync();
            return NoContent();
        }
    }
}
