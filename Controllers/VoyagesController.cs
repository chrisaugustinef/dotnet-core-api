using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoyagesApi.Models;
using Microsoft.Extensions.Logging;
using VoyagesApi.Services;
using System.Net;
using System;

namespace VoyagesApi.Controllers
{
    [Route("api/[controller]")] 
    [ApiController]
    public class VoyagesController : Controller
    {
        private readonly IVoyagesService _voyagesService;
        private readonly ILogger _logger;

        public VoyagesController(IVoyagesService voyagesService, ILogger<VoyagesController> logger)
        {
            _logger = logger;
            _voyagesService = voyagesService;
        }

        [HttpGet]
        [Route("GetVoyages")]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(IEnumerable<Voyages>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetVoyages()
        {
            try
            {
                _logger.LogInformation("Getting list of Voyages");
                return Ok(await _voyagesService.GetVoyages());
            }
            catch (Exception ex)
            {
                _logger.LogError("GetVoyages Error : " + ex);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Route("GetCurrencyExchangeRates")]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(IEnumerable<CurrencyExchangeRates>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCurrencyExchangeRates()
        {
            try
            {
                _logger.LogInformation("Getting list of Currency Exchange Rates");
                return Ok(await _voyagesService.GetCurrencyExchangeRates());
            }
            catch (Exception ex)
            {
                _logger.LogError("GetCurrencyExchangeRates Error : " + ex);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Route("GetAverage")]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(decimal), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAverage(string voyageCode, string currency)
        {
            try
            {
                return Ok(await _voyagesService.GetAverage(voyageCode, currency));
            }
            catch (Exception ex)
            {
                _logger.LogError("GetAverage Error : " + ex);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Route("UpdatePrice")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> UpdatePrice(Voyages voyagesItem)
        {
            try
            {
                _logger.LogInformation("Modifying Voyage Item");
                await _voyagesService.UpdatePrice(voyagesItem);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VoyagesItemExists(voyagesItem.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        private bool VoyagesItemExists(long id)
        {
            return _voyagesService.VoyagesItemExists(id);
        }
    }
}
