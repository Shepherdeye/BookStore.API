using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Threading.Tasks;

namespace BookStore.API.Areas.Customer.Controllers
{
    [Authorize]
    [Area(SD.CustomerArea)]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Book> _bookRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Promotion> _promotionRepository;

        public CartsController(IRepository<Cart> cartRepository,
            IRepository<Book> bookRepository,
            UserManager<ApplicationUser> userManager,
            IRepository<Promotion> promotionRepository

            )
        {
            _cartRepository = cartRepository;
            _bookRepository = bookRepository;
            _userManager = userManager;
            _promotionRepository = promotionRepository;
        }


        [HttpPost("")]
        public async Task<IActionResult> AddToCart(CartRequestAdd cartRequestAdd)
        {

            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound(new { msg = "User not Found" });

            var exist = await _cartRepository.GetOneAsync(e => e.BookId == cartRequestAdd.BookId && e.ApplicationUserId == user.Id);

            if (exist != null)
            {

                exist.Count++;

            }
            else
            {
                var newCartItem = new Cart
                {
                    ApplicationUserId = user.Id,
                    BookId = cartRequestAdd.BookId,
                    Count = cartRequestAdd.Count,
                };
                var createdItem = await _cartRepository.CreateAsync(newCartItem);
            }

            await _cartRepository.CommitAsync();

            return Created();

        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? code = null)
        {

            var cartsItems = await _cartRepository.GetAsync(includes: [e => e.Book]);


            var total = cartsItems.Sum(e => e.Count * e.Book.Price);

            var msg = "";

            if (code is not null)
            {
                var Promotion = await _promotionRepository.GetOneAsync(e => e.Code == code);


                if (Promotion == null || Promotion.ValidTo < DateTime.UtcNow || Promotion.Status == false || Promotion.NumberOfUse == 3)
                {
                    msg = "Warning : Expired Code !";

                }
                else
                {
                    total = total - (total * ((decimal)Promotion.Discount / 100));

                    Promotion.NumberOfUse++;

                    await _promotionRepository.CommitAsync();
                    msg = "Code Applied Successfully !";
                }
            }

            return Ok(new
            {
                CartsItems = cartsItems,
                Total = total,
                msg = msg
            });
        }

        [HttpPatch("IncrementCart/{bookId}")]

        public async Task<IActionResult> IncrementCart(int bookId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound(new { msg = "User not found in cart" });
            }
            var cartItem = await _cartRepository.GetOneAsync(e => e.BookId == bookId && e.ApplicationUserId == user.Id);

            if (cartItem is null)
            {
                return NotFound(new { msg = "Product not found in cart" });
            }

            cartItem.Count++;
            await _cartRepository.CommitAsync();

            return NoContent();

        }



        [HttpPatch("{bookId}")]
        public async Task<IActionResult> DecrementCart(int bookId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound(new { msg = "User not found in cart" });
            }
            var cartItem = await _cartRepository.GetOneAsync(e => e.BookId == bookId && e.ApplicationUserId == user.Id);

            if (cartItem is null)
            {
                return NotFound(new { msg = "Product not found in cart" });
            }


            if (cartItem.Count > 1)
            {
                cartItem.Count--;
            }

            await _cartRepository.CommitAsync();
            return NoContent();

        }

        [HttpDelete("{bookid}")]
        public async Task<IActionResult> DeleteCart(int bookid)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound(new { msg = "User not found in cart" });
            }

            var cartitem = await _cartRepository.GetOneAsync(e => e.BookId == bookid);

            if (cartitem is null)
                return NotFound(new { msg = "Cartitem not found in cart" });

            _cartRepository.Delete(cartitem);

            await _cartRepository.CommitAsync();


            return NoContent();
        }


        [HttpGet("pay")]

        public async Task<IActionResult> Pay()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound(new { msg = "User not found" });
            }

            var carts = await _cartRepository.GetAsync(e => e.ApplicationUserId == user.Id, includes: [e => e.Book]);

            if (carts is null) { return NotFound(); }



            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/checkout/success",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/checkout/cancel",
            };

            foreach (var item in carts)
            {

                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Book.Title,
                            Description = item.Book.Description,

                        },
                        UnitAmount = (long)item.Book.Price * 100,
                    },
                    Quantity = item.Count,
                });
            }

            var service = new SessionService();
            var session = service.Create(options);

            return Ok(new
            {
                url = session.Url
            });

        }
      

    }
}
