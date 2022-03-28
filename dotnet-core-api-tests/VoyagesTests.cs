using System;
using VoyagesApi.Services;
using VoyagesApi.Models;
using VoyagesApi.Utils;
using Xunit;
using System.Threading.Tasks;
using Moq;
using VoyagesApi.Controllers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace dotnet_core_api_tests
{
    public class CurrencyUtilsTests
    {
        private readonly ICurrencyUtils _currencyUtils;
        private readonly CurrencyUtils _currencyUtilsInternal;
        public CurrencyUtilsTests()
        {
            _currencyUtils = new CurrencyUtils();
            //For internal unit test
            _currencyUtilsInternal = new CurrencyUtils();
        }

        [Fact]
        public void ExchangeCurrencyWithTargetAndBaseCodeInCorrectOrder()
        {
            //Arrange
            var expectedValue = 121.484M;
            var exchangeRates = JsonConvert.DeserializeObject<List<CurrencyExchangeRates>>("[{\"id\":3,\"baseCurrency\":\"GBP\",\"targetCurrency\":\"EUR\",\"exchangeRate\":1.21},{\"id\":2,\"baseCurrency\":\"USD\",\"targetCurrency\":\"EUR\",\"exchangeRate\":0.91},{\"id\":1,\"baseCurrency\":\"USD\",\"targetCurrency\":\"GBP\",\"exchangeRate\":0.76}]");
            //Act
            var actualValue = _currencyUtils.ExchangeCurrency("GBP", "EUR", 100.4M, exchangeRates);
            //Assert
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void ExchangeCurrencyWithTargetAndBaseCodeInReverseOrder()
        {
            //Arrange
            var expectedValue = 100.4M;
            var exchangeRates = JsonConvert.DeserializeObject<List<CurrencyExchangeRates>>("[{\"id\":3,\"baseCurrency\":\"GBP\",\"targetCurrency\":\"EUR\",\"exchangeRate\":1.21},{\"id\":2,\"baseCurrency\":\"USD\",\"targetCurrency\":\"EUR\",\"exchangeRate\":0.91},{\"id\":1,\"baseCurrency\":\"USD\",\"targetCurrency\":\"GBP\",\"exchangeRate\":0.76}]");
            //Act
            var actualValue = _currencyUtils.ExchangeCurrency("EUR", "GBP", 121.484M, exchangeRates);
            //Assert
            Assert.Equal(expectedValue, Math.Round(actualValue, 1));
        }

        [Fact]
        public void GetExchangeRateWithTargetAndBaseCodeInCorrectOrder()
        {
            //Arrange
            var expectedValue = 1.21M;
            var exchangeRates = JsonConvert.DeserializeObject<List<CurrencyExchangeRates>>("[{\"id\":3,\"baseCurrency\":\"GBP\",\"targetCurrency\":\"EUR\",\"exchangeRate\":1.21},{\"id\":2,\"baseCurrency\":\"USD\",\"targetCurrency\":\"EUR\",\"exchangeRate\":0.91},{\"id\":1,\"baseCurrency\":\"USD\",\"targetCurrency\":\"GBP\",\"exchangeRate\":0.76}]");
            //Act
            var actualValue = _currencyUtilsInternal.GetKnownRate("GBP", "EUR", exchangeRates);
            //Assert
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void GetExchangeRateWithTargetAndBaseCodeInReverseOrder()
        {
            var expectedValue = 0.8M;
            var exchangeRates = JsonConvert.DeserializeObject<List<CurrencyExchangeRates>>("[{\"id\":3,\"baseCurrency\":\"GBP\",\"targetCurrency\":\"EUR\",\"exchangeRate\":1.21},{\"id\":2,\"baseCurrency\":\"USD\",\"targetCurrency\":\"EUR\",\"exchangeRate\":0.91},{\"id\":1,\"baseCurrency\":\"USD\",\"targetCurrency\":\"GBP\",\"exchangeRate\":0.76}]");
            //Act
            var actualValue = _currencyUtilsInternal.GetKnownRate("EUR", "GBP", exchangeRates);
            //Assert
            Assert.Equal(expectedValue, Math.Round(actualValue, 1));
        }
    }
    public class VoyagesTests
    {
        private Mock<ILogger<VoyagesController>> _loggerMock;
        public VoyagesTests()
        {
            _loggerMock = new Mock<ILogger<VoyagesController>>();
        }

        [Fact]
        public async Task GetAverageWhenInvokedReturnsResponse()
        {
            //Arrange
            var response = It.IsAny<decimal>();
            var voyageServiceMock = new Mock<IVoyagesService>();
            voyageServiceMock.Setup(p => p.GetAverage(It.IsAny<string>(), It.IsAny<string>()))
                                     .ReturnsAsync(response)
                                     .Verifiable("GetAverage should be called in this case");
            //Act
            var controller = new VoyagesController(voyageServiceMock.Object, _loggerMock.Object);
            var result = await controller.GetAverage(It.IsAny<string>(), It.IsAny<string>()) as OkObjectResult;
            var actualResult = result;
            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, (int)((HttpStatusCode)result.GetType().GetProperty("StatusCode").GetValue(result, null)));
            voyageServiceMock.Verify();
        }

        [Fact]
        public async Task GetAverageWhenInvokedReturnsInternalServerErrorOnException()
        {
            //Arrange
            var expectedStatusCode = (int)HttpStatusCode.InternalServerError;
            var voyagesServiceMock = new Mock<IVoyagesService>();
            voyagesServiceMock.Setup(p => p.GetAverage(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());
            //Act
            var controller = new VoyagesController(voyagesServiceMock.Object, _loggerMock.Object);
            var result = await controller.GetAverage(It.IsAny<string>(), It.IsAny<string>());
            //Assert
            Assert.Equal(expectedStatusCode, (int)((HttpStatusCode)result.GetType().GetProperty("StatusCode").GetValue(result, null)));
        }

        [Fact]
        public async Task UpdatePriceWhenInvokedReturnsResponse()
        {
            //Arrange
            var response = It.IsAny<decimal>();
            var voyageServiceMock = new Mock<IVoyagesService>();
            voyageServiceMock.Setup(p => p.UpdatePrice(It.IsAny<Voyages>()))
                                     .Returns(Task.CompletedTask)
                                     .Verifiable("GetAverage should be called in this case");
            //Act
            var controller = new VoyagesController(voyageServiceMock.Object, _loggerMock.Object);
            var result = await controller.UpdatePrice(It.IsAny<Voyages>());
            //Assert
            Assert.Equal((int)HttpStatusCode.NoContent, (int)((HttpStatusCode)result.GetType().GetProperty("StatusCode").GetValue(result, null)));
            voyageServiceMock.Verify();
        }

        [Fact]
        public async Task GetVoyagesWhenInvokedReturnsResponse()
        {
            //Arrange
            int expectedResultcount = 10;
            var response = JsonConvert.DeserializeObject<List<Voyages>>("[{\"id\":1,\"voyageCode\":\"400S\",\"price\":111110,\"currency\":\"EUR\",\"timestamp\":\"2022-03-28T03:52:27.397+00:00\"},{\"id\":2,\"voyageCode\":\"401S\",\"price\":45.9,\"currency\":\"USD\",\"timestamp\":\"2022-03-28T09:22:32.7806255+05:30\"},{\"id\":3,\"voyageCode\":\"402S\",\"price\":55.9,\"currency\":\"USD\",\"timestamp\":\"2022-03-28T09:22:32.7810518+05:30\"},{\"id\":4,\"voyageCode\":\"403S\",\"price\":65.9,\"currency\":\"EUR\",\"timestamp\":\"2022-03-28T09:22:32.7810994+05:30\"},{\"id\":5,\"voyageCode\":\"404S\",\"price\":75.9,\"currency\":\"USD\",\"timestamp\":\"2022-03-28T09:22:32.7811649+05:30\"},{\"id\":6,\"voyageCode\":\"405S\",\"price\":85.9,\"currency\":\"USD\",\"timestamp\":\"2022-03-28T09:22:32.7811988+05:30\"},{\"id\":7,\"voyageCode\":\"406S\",\"price\":95.9,\"currency\":\"USD\",\"timestamp\":\"2022-03-28T09:22:32.78122+05:30\"},{\"id\":8,\"voyageCode\":\"407S\",\"price\":15.9,\"currency\":\"EUR\",\"timestamp\":\"2022-03-28T09:22:32.7812392+05:30\"},{\"id\":9,\"voyageCode\":\"408S\",\"price\":38.9,\"currency\":\"GBP\",\"timestamp\":\"2022-03-28T09:22:32.7812746+05:30\"},{\"id\":10,\"voyageCode\":\"409S\",\"price\":34.9,\"currency\":\"USD\",\"timestamp\":\"2022-03-28T09:22:32.7813025+05:30\"}]");
            var voyageServiceMock = new Mock<IVoyagesService>();
            voyageServiceMock.Setup(p => p.GetVoyages())
                                     .ReturnsAsync(response)
                                     .Verifiable("GetVoyages should be called in this case");
            //Act
            var controller = new VoyagesController(voyageServiceMock.Object, _loggerMock.Object);
            var result = await controller.GetVoyages() as OkObjectResult;
            var actualResult = result.Value;
            //Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResultcount, ((List<Voyages>)actualResult).Count);
            voyageServiceMock.Verify();
        }

        [Fact]
        public async Task GetVoyagesWhenInvokedReturnsInternalServerErrorOnException()
        {
            //Arrange
            var expectedStatusCode = (int)HttpStatusCode.InternalServerError;
            List<Voyages> response = new List<Voyages>();
            var voyagesServiceMock = new Mock<IVoyagesService>();
            voyagesServiceMock.Setup(p => p.GetVoyages()).Throws(new Exception());
            //Act
            var controller = new VoyagesController(voyagesServiceMock.Object, _loggerMock.Object);
            var result = await controller.GetVoyages();
            //Assert
            Assert.Equal(expectedStatusCode, (int)((HttpStatusCode)result.GetType().GetProperty("StatusCode").GetValue(result, null)));
        }

        [Fact]
        public async Task GetCurrencyExchangeRatesWhenInvokedReturnsResponse()
        {
            //Arrange
            int expectedResultcount = 3;
            var response = JsonConvert.DeserializeObject<List<CurrencyExchangeRates>>("[{\"id\":3,\"baseCurrency\":\"GBP\",\"targetCurrency\":\"EUR\",\"exchangeRate\":1.21},{\"id\":2,\"baseCurrency\":\"USD\",\"targetCurrency\":\"EUR\",\"exchangeRate\":0.91},{\"id\":1,\"baseCurrency\":\"USD\",\"targetCurrency\":\"GBP\",\"exchangeRate\":0.76}]");
            var voyageServiceMock = new Mock<IVoyagesService>();
            voyageServiceMock.Setup(p => p.GetCurrencyExchangeRates())
                                     .ReturnsAsync(response)
                                     .Verifiable("GetCurrencyExchangeRates should be called in this case");
            //Act
            var controller = new VoyagesController(voyageServiceMock.Object, _loggerMock.Object);
            var result = await controller.GetCurrencyExchangeRates() as OkObjectResult;
            var actualResult = result.Value;
            //Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResultcount, ((List<CurrencyExchangeRates>)actualResult).Count);
            voyageServiceMock.Verify();
        }

        [Fact]
        public async Task GetCurrencyExchangeRatesWhenInvokedReturnsInternalServerErrorOnException()
        {
            //Arrange
            var expectedStatusCode = (int)HttpStatusCode.InternalServerError;
            List<CurrencyExchangeRates> response = new List<CurrencyExchangeRates>();
            var voyagesServiceMock = new Mock<IVoyagesService>();
            voyagesServiceMock.Setup(p => p.GetCurrencyExchangeRates()).Throws(new Exception());
            //Act
            var controller = new VoyagesController(voyagesServiceMock.Object, _loggerMock.Object);
            var result = await controller.GetCurrencyExchangeRates();
            //Assert
            Assert.Equal(expectedStatusCode, (int)((HttpStatusCode)result.GetType().GetProperty("StatusCode").GetValue(result, null)));
        }
    }
}
