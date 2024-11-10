using Microsoft.AspNetCore.Mvc;
using System.Web;
using VFX.Application.Common.Models;
using VFX.Application.Interface;

namespace VFX.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ForeignExchangeRateController : ControllerBase
{
    private readonly IForeignExchangeRateService _foreignExchangeRateService;

    public ForeignExchangeRateController(IForeignExchangeRateService foreignExchangeRateService)
    {
        _foreignExchangeRateService = foreignExchangeRateService;
    }

    /// <summary>
    /// Retrieves the exchange rate by its unique identifier.
    /// </summary>
    /// <param name="id">The ID of the exchange rate to retrieve.</param>
    /// <returns>An IActionResult containing the exchange rate data.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var exchangeRateDTO = await _foreignExchangeRateService.Get(id);
        return Ok(exchangeRateDTO);
    }

    /// <summary>
    /// Retrieves a paginated list of foreign exchange rates.
    /// </summary>
    /// <param name="pageIndex">The page index for pagination (default is 0).</param>
    /// <param name="pageSize">The number of records per page (default is 10).</param>
    /// <returns>A list of exchange rates in the response body.</returns>
    [HttpGet]
    public async Task<IActionResult> Get(int pageIndex = 0, int pageSize = 10)
    {
        var exchangeRatesDTO = await _foreignExchangeRateService.Get(pageIndex, pageSize);
        return Ok(exchangeRatesDTO);
    }

    /// <summary>
    /// Retrieves the exchange rate for a specific currency pair.
    /// </summary>
    /// <param name="currencyPair">A URL-encoded string representing the currency pair (e.g., "USD-EUR").</param>
    /// <param name="token">The cancellation token for handling task cancellation.</param>
    /// <returns>The exchange rate for the specified currency pair.</returns>
    [HttpGet("rate/{currencyPair}")]
    public async Task<IActionResult> GetExchangeRate(string currencyPair, CancellationToken token)
    {
        try
        {
            // Decode the URL-encoded currency pair
            var decodedCurrencyPair = HttpUtility.UrlDecode(currencyPair);

            var exchangeRateDTO = await _foreignExchangeRateService.GetExchangeRate(decodedCurrencyPair, token);
            return Ok(exchangeRateDTO);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Adds a new foreign exchange rate record.
    /// </summary>
    /// <param name="request">The DTO containing data for the new exchange rate.</param>
    /// <param name="token">The cancellation token for handling task cancellation.</param>
    /// <returns>A NoContent response if the operation is successful.</returns>
    [HttpPost]
    public async Task<IActionResult> Add(ForeignExchangeRateCreateDTO request, CancellationToken token)
    {
        await _foreignExchangeRateService.Add(request, token);
        return NoContent();
    }


    /// <summary>
    /// Updates an existing foreign exchange rate record.
    /// </summary>
    /// <param name="request">The DTO containing updated data for the exchange rate.</param>
    /// <param name="token">The cancellation token for handling task cancellation.</param>
    /// <returns>A NoContent response if the update is successful.</returns>
    [HttpPut]  // Maps the HTTP PUT request to this method for updating records.
    [HttpPut]
    public async Task<IActionResult> Update(ForeignExchangeRateUpdateDTO request, CancellationToken token)
    {
        await _foreignExchangeRateService.Update(request, token);
        return NoContent();
    }


    /// <summary>
    /// Deletes a foreign exchange rate record by its ID.
    /// </summary>
    /// <param name="id">The ID of the exchange rate to delete.</param>
    /// <param name="token">The cancellation token for handling task cancellation.</param>
    /// <returns>A NoContent response if the deletion is successful.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken token)
    {
        await _foreignExchangeRateService.Delete(id, token);
        return NoContent();
    }
}
