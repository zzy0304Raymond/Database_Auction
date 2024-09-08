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
            if (!ModelState.IsValid)
            {
                return BadRequest("Transaction is not availble!");
            }
            try
            {
                System.Console.WriteLine(userId);
                if (!_context.Transactions.Any())
                {
                    return BadRequest("No vaild Transaction");
                }
                var tran = await _context.Transactions
                    .Where(t => t.Buyeruserid == userId)
                    .Select(t => new TransactionDto
                    {
                        transactionID = t.Transactionid,
                        userName = t.Buyeruser.Username,
                        itemID = (decimal)t.Auction.Item.Itemid,
                        itemName = t.Auction.Item.Itemname,
                        price = t.Amount ?? 0,
                        status = t.Status
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

        // POST: /api/transaction/afterpay/{transactionid}
        [HttpPost("afterpay/{transactionid}")]
        public async Task<ActionResult> AfterPay(decimal transactionid, [FromBody] PaymentDto paymentDto)
        {
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Transactionid == transactionid);

            if (transaction == null)
            {
                return NotFound(new { message = "Transaction not found." });
            }

            try
            {
                transaction.Status = "1"; // 更新交易状态为 "1" 表示已支付
                await _context.SaveChangesAsync();

                var payment = new Payment
                {
                    Paymentid = await _context.Payments.MaxAsync(p => (decimal?)p.Paymentid) ?? 0 + 1,
                    Transactionid = transaction.Transactionid,
                    Amount = transaction.Amount,
                    Paymenttime = DateTime.UtcNow,
                    Paymentmethod = paymentDto.PaymentMethod
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Payment completed and recorded." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the payment.", error = ex.Message });
            }
        }

        // GET: /api/transaction/cancel/{transactionid}
        [HttpGet("cancel/{transactionid}")]
        public async Task<ActionResult> CancelTransaction(decimal transactionid)
        {
            // 查找指定的 Transaction
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Transactionid == transactionid);

            if (transaction == null)
            {
                return NotFound(new { message = "Transaction not found." });
            }

            try
            {
                // 更新交易状态为 "2" 表示已取消
                transaction.Status = "2";
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Transaction has been cancelled." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while cancelling the transaction.", error = ex.Message });
            }
        }


        public struct TransactionDto
        {
            public decimal transactionID { get; set; }
            public string userName { get; set; }
            public decimal itemID { get; set; }
            public string itemName { get; set; }
            public decimal price { get; set; }
            public string status { get; set; }

        }

        // 用于接收支付方式
        public class PaymentDto
        {
            public string PaymentMethod { get; set; } = "Online"; // 默认支付方式
        }
    }
}