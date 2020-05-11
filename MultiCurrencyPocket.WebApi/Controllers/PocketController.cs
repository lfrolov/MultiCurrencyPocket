using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MultiCurrencyPocket.Services.DataService;
using MultiCurrencyPocket.Services.DataService.DTO;
using MultiCurrencyPocket.Services.Exceptions;

namespace MultiCurrencyPocket.WebApi.Controllers
{
    [Route("api/[controller]/[action]", Name = "[controller]_[action]")]
    [ApiController]
    public class PocketController : ControllerBase
    {
        protected IPocketService PocketService { get; }

        public PocketController(IPocketService pocketService)
        {
            PocketService = pocketService;
        }

        // GET: api/Pocket/GetPocketStatus/5
        [HttpGet("{accountNumber}")]
        public async Task<ActionResult> GetPocketStatus(string accountNumber, [FromBody] GetPocketStatusDTO request)
        {
            if (string.IsNullOrEmpty(accountNumber) ||
                accountNumber != request.MasterAccount ||
                !ModelState.IsValid)
            {
                return BadRequest(request);
            }
            StatusResultDTO resultDTO;
            try
            {
                resultDTO = await PocketService.GetPocketStatusAsync(request);
            }
            //ToDo: replace with custom exception 
            catch (AccountNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            return new JsonResult(resultDTO);
        }


        [HttpPost("{accountNumber}")]
        public async Task<IActionResult> DepositCurrencyAccount(string accountNumber,[FromBody] DepositCurrencyDTO request)
        {
            if (string.IsNullOrEmpty(accountNumber) ||
                accountNumber != request.CurrencyAccount ||
                !ModelState.IsValid)
            {
                return BadRequest(request);
            }
            try
            {
                var result = await PocketService.DepositCurrencyAccountAsync(request);
                return new JsonResult(result);
            }
            catch (AccountNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ConcurrencyException ex) 
            {
                throw; 
            }
        }

        [HttpPost("{accountNumber}")]
        public async Task<IActionResult> WithdrawCurrencyAccount(string accountNumber, [FromBody] WithdrawCurrencyDTO request)
        {
            if (string.IsNullOrEmpty(accountNumber) ||
                accountNumber != request.CurrencyAccount ||
                !ModelState.IsValid)
            {
                return BadRequest(request);
            }
            try
            {
                var result = await PocketService.WithdrawCurrencyAccountAsync(request);
                return new JsonResult(result);
            }
            catch (AccountNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InsufficientFundException ex) 
            {
                return BadRequest(ex.Message);
            }
            catch (ConcurrencyException ex)
            {
                throw;
            }
        }

        [HttpPost("{accountNumber}")]
        public async Task<IActionResult> DepositAccount(string accountNumber, [FromBody] DepositDTO request)
        {
            if (string.IsNullOrEmpty(accountNumber) ||
                accountNumber != request.MasterAccount)
            {
                return BadRequest(request);
            }
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await PocketService.DepositPocketAsync(request);
                return new JsonResult(result);
            }
            catch (AccountNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ConcurrencyException ex)
            {
                throw;
            }
        }


        [HttpPost("{accountNumber}")]
        public async Task<IActionResult> WithdrawAccount(string accountNumber, [FromBody] WithdrawDTO request)
        {
            if (string.IsNullOrEmpty(accountNumber) ||
                accountNumber != request.MasterAccount ||
                !ModelState.IsValid)
            {
                return BadRequest(request);
            }
            try
            {
                var result = await PocketService.WithdrawPocketAsync(request);
                return new JsonResult(result);
            }
            catch (AccountNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InsufficientFundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ConcurrencyException ex)
            {
                throw;
            }
        }



    }
}
