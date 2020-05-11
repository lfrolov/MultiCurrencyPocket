using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MultiCurrencyPocket.Services.DataService;
using MultiCurrencyPocket.Services.DataService.DTO;
using MultiCurrencyPocket.Services.Exceptions;

namespace MultiCurrencyPocket.WebApi.Controllers
{
    //Todo: Change all exception handling to ModelState.AddError
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
        public async Task<ActionResult> GetPocketStatus(string accountNumber,[FromQuery] GetPocketStatusDTO request)
        {
            if (string.IsNullOrEmpty(accountNumber) ||
                accountNumber != request.MasterAccount)
            {
                ModelState.AddModelError("", AccountNotFoundException.DefaultErrorMessage);
                return BadRequest(ModelState);
            }
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                var resultDTO = await PocketService.GetPocketStatusAsync(request);
                return new JsonResult(resultDTO);
            }
            catch (Exception ex) 
            {
                return HandleException(ex);
            }
            
        }


        [HttpPost("{accountNumber}")]
        public async Task<IActionResult> DepositCurrencyAccount(string accountNumber,[FromBody] DepositCurrencyDTO request)
        {
            if (string.IsNullOrEmpty(accountNumber) ||
                accountNumber != request.CurrencyAccount
                )
            {
                ModelState.AddModelError("", AccountNotFoundException.DefaultErrorMessage);
                return BadRequest(ModelState);
            }
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await PocketService.DepositCurrencyAccountAsync(request);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("{accountNumber}")]
        public async Task<IActionResult> WithdrawCurrencyAccount(string accountNumber, [FromBody] WithdrawCurrencyDTO request)
        {
            if (string.IsNullOrEmpty(accountNumber) ||
                accountNumber != request.CurrencyAccount)
            {
                ModelState.AddModelError("", AccountNotFoundException.DefaultErrorMessage);
                return BadRequest(ModelState);
            }
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await PocketService.WithdrawCurrencyAccountAsync(request);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("{accountNumber}")]
        public async Task<IActionResult> DepositAccount(string accountNumber, [FromBody] DepositDTO request)
        {
            if (string.IsNullOrEmpty(accountNumber) ||
                accountNumber != request.MasterAccount)
            {
                ModelState.AddModelError("", AccountNotFoundException.DefaultErrorMessage);
                return BadRequest(ModelState);
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
            catch (Exception ex) 
            {
                return HandleException(ex);
            }
        }


        [HttpPost("{accountNumber}")]
        public async Task<IActionResult> WithdrawAccount(string accountNumber, [FromBody] WithdrawDTO request)
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
                var result = await PocketService.WithdrawPocketAsync(request);
                return new JsonResult(result);
            }
            catch (Exception ex) 
            {
                return HandleException(ex);
            }
        }

        [HttpPost("{accountNumber}")]
        public async Task<IActionResult> ConvertCurrency(string accountNumber, [FromBody] CurrencyConvertionDTO request) 
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
                await PocketService.ConvertCurrencyAsync(request);
                return NoContent();
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        //ToDo: Add logging
        protected ActionResult HandleException(Exception ex) 
        {
            //handle custom exceptions
            if (ex is AccountNotFoundException || 
                ex is InsufficientFundException || 
                ex is CurrencyRateNotFoundException ||
                ex is ConcurrencyException)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }



    }
}
