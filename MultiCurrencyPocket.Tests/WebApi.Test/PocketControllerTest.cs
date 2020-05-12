using Microsoft.AspNetCore.Mvc;
using Moq;
using MultiCurrencyPocket.Data.Models;
using MultiCurrencyPocket.Services.DataService;
using MultiCurrencyPocket.Services.DataService.DTO;
using MultiCurrencyPocket.Services.Exceptions;
using MultiCurrencyPocket.WebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MultiCurrencyPocket.Tests.WebApi.Test
{
    public class PocketControllerTest
    {
        private const string ValidTestMasterAccount = "012345";
        private const string RubAccountNumber = "000001";
        private const string UsdAccountNumber = "000002";
        private const string ValidTestPinCode = "0000";
        
        private Mock<IPocketService> _mockService;
        private readonly PocketController _controller;
        private readonly PocketHolder _testData;

        public PocketControllerTest() 
        {
            _mockService = new Mock<IPocketService>();
            _controller = new PocketController(_mockService.Object);
            _testData = InitTestData();
        }

        private PocketHolder InitTestData() 
        {
            var testData = new PocketHolder()
            {
                Id = 1,
                FirstName = "Vasya",
                LastName = "Pupkin",
                DateOfBirth = new DateTime(1987, 12, 25),
                ExpirationDate = DateTime.Today.AddYears(5),
                Gender = GenderCode.Male,
                MasterAccount = ValidTestMasterAccount,
                PINCode = ValidTestPinCode,
                Accounts = new List<CurrencyAccount>
                {
                     new CurrencyAccount {Id = 1, Currency = "RUB", Debit = 1000m, PocketHolderId = 1, Number = RubAccountNumber },
                     new CurrencyAccount { Id = 2, Currency = "USD", Debit = 800m, PocketHolderId = 1, Number = UsdAccountNumber }
                }
            };
            testData.Accounts[0].Holder = _testData;
            testData.Accounts[1].Holder = _testData;

            return testData;
        }
        #region GetPocketStatus
        [Fact]
        public async Task GetPocketStatus_Valid_SuccessAsync() 
        {
            _mockService
                .Setup(x => x.GetPocketStatusAsync(It.IsAny<GetPocketStatusDTO>()))
                .ReturnsAsync((GetPocketStatusDTO request) =>
                {
                    return new StatusResultDTO()
                    {
                        Deposits = _testData.Accounts.Select(x => new DepositItemDTO { AccountNumber = x.Number, Currency = x.Currency, Sum = x.Debit })
                    };
            });
            var result = await _controller.GetPocketStatus(ValidTestMasterAccount,
                new GetPocketStatusDTO
                {
                    MasterAccount = ValidTestMasterAccount,
                    PinCode = ValidTestPinCode
                });
            var json = Assert.IsType<JsonResult>(result);
            var jsonValue = Assert.IsType<StatusResultDTO>(json.Value);
            var accounts = jsonValue.Deposits.Select(d => d.AccountNumber);
            Assert.All(_testData.Accounts, acc => accounts.Contains(acc.Number));
        }

        [Fact]
        public async Task GetPocketStatus_InvalidModelState() 
        {
            _controller.ModelState.AddModelError("MasterAccount", "MasterAccount is required");
            var result = await _controller.GetPocketStatus(ValidTestMasterAccount, new GetPocketStatusDTO());
            Assert.IsType<BadRequestObjectResult>(result);
            _mockService.Verify(x => x.GetPocketStatusAsync(It.IsAny<GetPocketStatusDTO>()), Times.Never);
        }

        [Fact]
        public async Task GetPocketStatus_ThrowException()
        {
            _mockService.Setup(x => x.GetPocketStatusAsync(It.IsAny<GetPocketStatusDTO>())).Throws<AccountNotFoundException>();
            var result = await _controller.GetPocketStatus("123123", new GetPocketStatusDTO { MasterAccount = "123123", PinCode = "1111" });
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.False(_controller.ModelState.IsValid);
            Assert.Equal(1, _controller.ModelState.ErrorCount);
        }
        #endregion

        #region Deposit
        [Fact]
        public async Task DepositAccount_Valid_Success() 
        {
            Setup_DebitAccount();
            var result = await _controller.DepositAccount(ValidTestMasterAccount, new DepositDTO { MasterAccount = ValidTestMasterAccount, Currency = "USD", Sum = 800m });
            var json = Assert.IsType<JsonResult>(result);
            var value = Assert.IsType<decimal>(json.Value);
            Assert.Equal(1600m,value);
        }
        
        [Fact]
        public async Task DepositAccount_ModelStateError() 
        {
            _controller.ModelState.AddModelError("Sum", "Sum should be in range 1 .. 1,000,000");
            var result = await _controller.DepositAccount(ValidTestMasterAccount, new DepositDTO { MasterAccount = ValidTestMasterAccount, Currency = "USD", Sum = 800m });
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            _mockService.Verify(x => x.DepositPocketAsync(It.IsAny<DepositDTO>()), Times.Never);
        }

        [Fact]
        public async Task DepositAccount_ThrowException() 
        {
            _mockService.Setup(x => x.DepositPocketAsync(It.IsAny<DepositDTO>())).Throws<AccountNotFoundException>();
            var result = await _controller.DepositAccount(ValidTestMasterAccount, new DepositDTO { MasterAccount = ValidTestMasterAccount, Currency = "USD", Sum = 800m });
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.False(_controller.ModelState.IsValid);
            Assert.Equal(1, _controller.ModelState.ErrorCount);
        }

        private void Setup_DebitAccount() 
        {
            _mockService
                .Setup(x => x.DepositPocketAsync(It.IsAny<DepositDTO>()))
                .ReturnsAsync((DepositDTO request) =>
                {
                    var acc = _testData.Accounts.FirstOrDefault(a => a.Currency == request.Currency);
                    acc.Debit += request.Sum;
                    return acc.Debit;
                });
        }
        #endregion

        #region Deposit Currency Account

        [Fact]
        public async Task DepositCurrencyAccount_Valid_Success() 
        {
            Setup_DepositCurrencyAccount();
            var result = await _controller.DepositCurrencyAccount(RubAccountNumber, new DepositCurrencyDTO { CurrencyAccount = RubAccountNumber, Sum = 500m });
            _mockService.Verify(x => x.DepositCurrencyAccountAsync(It.IsAny<DepositCurrencyDTO>()), Times.Once);
            var json = Assert.IsType<JsonResult>(result);
            var val = Assert.IsType<decimal>(json.Value);
            Assert.Equal(1500m, val);
        }

        [Fact]
        public async Task DepositCurrencyAccount_ModelStateError() 
        {
            _controller.ModelState.AddModelError("PinCode", "PinCode should be in format ####");
            var result = await _controller.DepositCurrencyAccount(RubAccountNumber, new DepositCurrencyDTO {CurrencyAccount = RubAccountNumber, PinCode = "X7Pass" });
            _mockService.Verify(x => x.DepositCurrencyAccountAsync(It.IsAny<DepositCurrencyDTO>()), Times.Never);
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
        }

        public async Task DepositCurrencyAccount_ThrowException() 
        {
            _mockService.Setup(x => x.DepositCurrencyAccountAsync(It.IsAny<DepositCurrencyDTO>())).Throws<ConcurrencyException>();
            var result = await _controller.DepositCurrencyAccount(RubAccountNumber, new DepositCurrencyDTO() { CurrencyAccount = RubAccountNumber });
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.False(_controller.ModelState.IsValid);
            Assert.Equal(1, _controller.ModelState.ErrorCount);
        }

        private void Setup_DepositCurrencyAccount()
        {
            _mockService
                .Setup(x => x.DepositCurrencyAccountAsync(It.IsAny<DepositCurrencyDTO>()))
                .ReturnsAsync((DepositCurrencyDTO request) =>
                {
                    var acc = _testData.Accounts.FirstOrDefault(a => a.Number == request.CurrencyAccount);
                    acc.Debit += request.Sum;
                    return acc.Debit;
                });
        }
        #endregion

        #region Withdraw Account

        [Fact]
        public async Task WithdrawAccount_Valid_Success()
        {
            Setup_WithdrawAccount();
            var result = await _controller.WithdrawAccount(ValidTestMasterAccount, new WithdrawDTO { MasterAccount = ValidTestMasterAccount, Currency = "USD", Sum = 800m });
            var json = Assert.IsType<JsonResult>(result);
            var value = Assert.IsType<decimal>(json.Value);
            Assert.Equal(0.0m, value);
        }

        [Fact]
        public async Task WithdrawAccount_ModelStateError()
        {
            _controller.ModelState.AddModelError("Sum", "Sum should be in range 1 .. 1,000,000");
            var result = await _controller.WithdrawAccount(ValidTestMasterAccount, new WithdrawDTO { MasterAccount = ValidTestMasterAccount, Currency = "USD", Sum = 8e6m });
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            _mockService.Verify(x => x.WithdrawPocketAsync(It.IsAny<WithdrawDTO>()), Times.Never);
        }

        [Fact]
        public async Task WithdrawAccount_ThrowException()
        {
            _mockService.Setup(x => x.WithdrawPocketAsync(It.IsAny<WithdrawDTO>())).Throws<InsufficientFundException>();
            var result = await _controller.WithdrawAccount(ValidTestMasterAccount, new WithdrawDTO { MasterAccount = ValidTestMasterAccount, Currency = "USD", Sum = 10000 });
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.False(_controller.ModelState.IsValid);
            Assert.Equal(1, _controller.ModelState.ErrorCount);
        }

        private void Setup_WithdrawAccount()
        {
            _mockService
                .Setup(x => x.WithdrawCurrencyAccountAsync(It.IsAny<WithdrawCurrencyDTO>()))
                .ReturnsAsync((DepositDTO request) =>
                {
                    var acc = _testData.Accounts.FirstOrDefault(a => a.Currency == request.Currency);
                    if (acc.Debit < request.Sum)
                        throw new InsufficientFundException();
                    acc.Debit -= request.Sum;
                    return acc.Debit;
                });
        }

        #endregion
        
        #region Withdraw Currency Account

        [Fact]
        public async Task WithdrawCurrencyAccount_Valid_Success()
        {
            Setup_WithdrawCurrencyAccount();
            var result = await _controller.DepositCurrencyAccount(RubAccountNumber, new DepositCurrencyDTO { CurrencyAccount = RubAccountNumber, Sum = 1000m });
            _mockService.Verify(x => x.DepositCurrencyAccountAsync(It.IsAny<DepositCurrencyDTO>()), Times.Once);
            var json = Assert.IsType<JsonResult>(result);
            var val = Assert.IsType<decimal>(json.Value);
            Assert.Equal(0m, val);
        }

        [Fact]
        public async Task WithdrawCurrencyAccount_ModelStateError()
        {
            _controller.ModelState.AddModelError("PinCode", "PinCode should be in format ####");
            var result = await _controller.WithdrawCurrencyAccount(RubAccountNumber, new WithdrawCurrencyDTO { CurrencyAccount = RubAccountNumber, PinCode = "X7Pass" });
            _mockService.Verify(x => x.WithdrawCurrencyAccountAsync(It.IsAny<WithdrawCurrencyDTO>()), Times.Never);
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
        }

        public async Task WithdrarCurrencyAccount_ThrowException()
        {
            _mockService.Setup(x => x.WithdrawCurrencyAccountAsync(It.IsAny<WithdrawCurrencyDTO>())).Throws<InsufficientFundException>();
            var result = await _controller.WithdrawCurrencyAccount(RubAccountNumber, new WithdrawCurrencyDTO() { CurrencyAccount = RubAccountNumber, Sum = 10000m });
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.False(_controller.ModelState.IsValid);
            Assert.Equal(1, _controller.ModelState.ErrorCount);
        }

        private void Setup_WithdrawCurrencyAccount()
        {
            _mockService
                .Setup(x => x.WithdrawCurrencyAccountAsync(It.IsAny<WithdrawCurrencyDTO>()))
                .ReturnsAsync((DepositCurrencyDTO request) =>
                {
                    var acc = _testData.Accounts.FirstOrDefault(a => a.Number == request.CurrencyAccount);
                    if (acc.Debit < request.Sum)
                    {
                        throw new InsufficientFundException();
                    }
                    acc.Debit -= request.Sum;
                    return acc.Debit;
                });
        }

        #endregion

        #region Convert Currency

        [Fact]
        public async Task ConvertCurrency_ValidSuccess() 
        {
            Setup_ConvertCurrency();
            var dto = new CurrencyConvertionDTO { MasterAccount = ValidTestMasterAccount, SourceCurrency = "USD", Sum = 800m, DestinationCurrency = "RUB" };
            var result = await _controller.ConvertCurrency(ValidTestMasterAccount, dto);
            _mockService.Verify(x => x.ConvertCurrencyAsync(It.IsAny<CurrencyConvertionDTO>()), Times.Once);
            Assert.IsType<NoContentResult>(result);
            Assert.Equal(41000m, _testData.Accounts.Single(a => a.Currency == dto.DestinationCurrency).Debit);
            Assert.Equal(0m, _testData.Accounts.Single(a => a.Currency == dto.SourceCurrency).Debit);
        }

        [Fact]
        public async Task ConvertCurrency_ModelStateError()
        {
            _controller.ModelState.AddModelError("Sum", "Sum should be in range 1..1,000,000");
            var dto = new CurrencyConvertionDTO { MasterAccount = ValidTestMasterAccount, SourceCurrency = "USD", Sum = 99e6m, DestinationCurrency = "RUB" };
            var result = await _controller.ConvertCurrency(ValidTestMasterAccount, dto);
            _mockService.Verify(x => x.ConvertCurrencyAsync(It.IsAny<CurrencyConvertionDTO>()), Times.Never);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.False(_controller.ModelState.IsValid);
            Assert.Equal(1, _controller.ModelState.ErrorCount);
        }

        [Fact]
        public async Task ConvertCurrency_ThrowsException()
        {
            Setup_ConvertCurrency();
            var dto = new CurrencyConvertionDTO { MasterAccount = ValidTestMasterAccount, SourceCurrency = "BUZ", Sum = 100m, DestinationCurrency = "RUB" };
            var result = await _controller.ConvertCurrency(ValidTestMasterAccount, dto);
            _mockService.Verify(x => x.ConvertCurrencyAsync(It.IsAny<CurrencyConvertionDTO>()), Times.Once);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.False(_controller.ModelState.IsValid);
            Assert.Equal(1, _controller.ModelState.ErrorCount);
        }

        private const decimal Usd_To_Rub = 50m;
        private const decimal Rub_To_Usd = 0.02m;

        private void Setup_ConvertCurrency()
        {
            // Conver USD to RUB with 50 rate
            _mockService.Setup(
                x => x.ConvertCurrencyAsync(It.Is<CurrencyConvertionDTO>(dto => dto.SourceCurrency == "USD" && dto.DestinationCurrency == "RUB")))
                .Callback((CurrencyConvertionDTO dto) =>
                {
                    var acc1 = _testData.Accounts.First(a => a.Currency == dto.SourceCurrency);
                    var acc2 = _testData.Accounts.First(a => a.Currency == dto.DestinationCurrency);
                    acc1.Debit -= dto.Sum;
                    acc2.Debit += Usd_To_Rub * dto.Sum;
                });

            // Conver RUB to USD with 0.02 rate
            _mockService.Setup(
                x => x.ConvertCurrencyAsync(It.Is<CurrencyConvertionDTO>(dto => dto.SourceCurrency == "RUB" && dto.DestinationCurrency == "USD")))
                .Callback((CurrencyConvertionDTO dto) =>
                {
                    var acc = _testData.Accounts.First(a => a.Currency == dto.DestinationCurrency);
                    acc.Debit += Rub_To_Usd * dto.Sum;
                });

            _mockService.Setup(
                x => x.ConvertCurrencyAsync(It.Is<CurrencyConvertionDTO>(dto => dto.SourceCurrency == "BUZ")))
                .Throws(new CurrencyRateNotFoundException());
        }      

        #endregion

    }
}
