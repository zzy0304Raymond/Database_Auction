using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using auctionapp.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.Storage;

namespace auctionapp.Controllers
{
    [Route("api/transaction")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly AuctionDbContext _context;

        public TransactionController(AuctionDbContext context)
        {
            _context = context;
        }
        //GET : api/transaction/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> createTran(decimal userId)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("Transaction is not availble!");
            }
            try
            {
                System.Console.WriteLine(userId);
                if(!_context.Transactions.Any())
                {
                    return BadRequest("No vaild Transaction");
                }
                var tran = await _context.Transactions
                    .Where(t => t.Buyeruserid == userId)
                    .Select(t => new TransactionDto
                    {
                        userName = t.Buyeruser.Username,
                        itemName = t.Auction.Item.Itemname,
                        price = t.Amount ?? 0
                    }
                    )
                    .ToListAsync();

                return Ok(tran);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }


        public struct TransactionDto
        {
            public string userName {  get; set; }
            public string itemName { get; set; }
            public decimal price    {  get; set; }



        }

    }
}
