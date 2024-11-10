using Microsoft.AspNetCore.Mvc;
using Moq;
using VFX.Application.Common.Models;
using VFX.Application.Interface;
using VFX.WebAPI.Controllers;

namespace VFX.Unittest;

// Test class for the ForeignExchangeRateController
public class ForeignExchangeRateControllerTests
{
    private readonly Mock<IForeignExchangeRateService> _foreignExchangeRateServiceMock;
    private readonly ForeignExchangeRateController _controller;

    public ForeignExchangeRateControllerTests()
    {
        _foreignExchangeRateServiceMock = new Mock<IForeignExchangeRateService>();
        _controller = new ForeignExchangeRateController(_foreignExchangeRateServiceMock.Object);
    }


    // Test case to verify that the controller returns OK with exchange rate DTO when an exchange rate is found by Id
    [Fact]
    public async Task Get_ById_ShouldReturnOk_WhenExchangeRateExists()
    {
        // Arrange
        int id = 1;
        var expectedDto = new ForeignExchangeRateDTO { Id = id };
        _foreignExchangeRateServiceMock.Setup(s => s.Get(id))
            .ReturnsAsync(expectedDto);

        // Act
        var result = await _controller.Get(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedDto, okResult.Value);
    }

    // Test case to verify that the controller returns OK with paginated exchange rate list
    [Fact]
    public async Task Get_WithPagination_ShouldReturnOk_WithExchangeRatesList()
    {
        // Arrange
        int pageIndex = 0, pageSize = 10;
        var items = new List<ForeignExchangeRateDTO>
        {
            new ForeignExchangeRateDTO(),
            new ForeignExchangeRateDTO()
        };
        int totalCount = items.Count;

        var expectedPagination = new Pagination<ForeignExchangeRateDTO>(items, totalCount, pageIndex, pageSize);

        _foreignExchangeRateServiceMock.Setup(s => s.Get(pageIndex, pageSize))
            .ReturnsAsync(expectedPagination);

        // Act
        var result = await _controller.Get(pageIndex, pageSize);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedPagination, okResult.Value);
    }

    // Test case to verify that the controller returns OK with exchange rate when the exchange rate is found for a currency pair
    [Fact]
    public async Task GetExchangeRate_ShouldReturnOk_WhenExchangeRateIsFound()
    {
        // Arrange
        string currencyPair = "USD/EUR";
        var token = CancellationToken.None;
        var expectedDto = new ForeignExchangeRateDTO
        {
            FromCurrencyCode = "USD",
            ToCurrencyCode = "EUR"
        };

        _foreignExchangeRateServiceMock.Setup(s => s.GetExchangeRate(currencyPair, token))
            .ReturnsAsync(expectedDto);

        // Act
        var result = await _controller.GetExchangeRate(currencyPair, token);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedDto, okResult.Value);
    }

    // Test case to verify that the controller returns BadRequest when an exception is thrown for an invalid currency pair
    [Fact]
    public async Task GetExchangeRate_ShouldReturnBadRequest_WhenExceptionIsThrown()
    {
        // Arrange
        string currencyPair = "invalid";
        var token = CancellationToken.None;
        _foreignExchangeRateServiceMock.Setup(s => s.GetExchangeRate(currencyPair, token))
            .ThrowsAsync(new Exception("Invalid currency pair"));

        // Act
        var result = await _controller.GetExchangeRate(currencyPair, token);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid currency pair", badRequestResult.Value);
    }

    // Test case to verify that the controller returns NoContent when an exchange rate is added successfully
    [Fact]
    public async Task Add_ShouldReturnNoContent_WhenExchangeRateIsAddedSuccessfully()
    {
        // Arrange
        var request = new ForeignExchangeRateCreateDTO();
        var token = CancellationToken.None;
        _foreignExchangeRateServiceMock.Setup(s => s.Add(request, token))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Add(request, token);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    // Test case to verify that the controller returns NoContent when an exchange rate is updated successfully
    [Fact]
    public async Task Update_ShouldReturnNoContent_WhenExchangeRateIsUpdatedSuccessfully()
    {
        // Arrange
        var request = new ForeignExchangeRateUpdateDTO();
        var token = CancellationToken.None;
        _foreignExchangeRateServiceMock.Setup(s => s.Update(request, token))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(request, token);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    // Test case to verify that the controller returns NoContent when an exchange rate is deleted successfully
    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenExchangeRateIsDeletedSuccessfully()
    {
        // Arrange
        int id = 1;
        var token = CancellationToken.None;
        _foreignExchangeRateServiceMock.Setup(s => s.Delete(id, token))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(id, token);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}
