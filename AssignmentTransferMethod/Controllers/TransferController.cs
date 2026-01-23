using AssignmentTransferMethod.DTO;
using AssignmentTransferMethod.Servies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentTransferMethod.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        private readonly TransferServices _transferServices;

        public TransferController(TransferServices transferServices)
        {
            _transferServices = transferServices;
        }

        [HttpPost]
        public async Task<IActionResult> Transfer([FromBody] TransferRequest request, CancellationToken ct)
        {
            var result = await _transferServices.TransferAsync(
                request.FromAccountId,
                request.ToAccountId,
                request.Amount,
                request.IdempotencyKey,
                ct);

            if (result.Succeeded)
                return Ok(result);

            return result.ErrorCode switch
            {
                "AccountNotFound" => NotFound(result),
                "InsufficientFunds" => BadRequest(result),
                "Duplicate" => Conflict(result),
                "ConcurrencyConflict" => Conflict(result),
                _ => BadRequest(result)
            };
        }
    }
}
