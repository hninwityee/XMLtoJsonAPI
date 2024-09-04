using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using XMLtoJsonApi.Models;

namespace XMLtoJsonApi.Controllers
{
    [ApiController]
    [Route("v1/api/[controller]")]
    public class XmlToJsonController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public XmlToJsonController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetJsonFromXml(int id)
        {
            string xmlUrl = $"https://raw.githubusercontent.com/openpolytechnic/dotnet-developer-evaluation/main/xml-api/{id}.xml";
            try
            {
                // Fetch the XML data
                var response = await _httpClient.GetAsync(xmlUrl);

                // Check if the response indicates success
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        // Return a 404 Not Found error response
                        var notFoundResponse = new ErrorResponse
                        {
                            error = "Not Found",
                            error_description = $"Company with ID {id} not found."
                        };
                        return NotFound(notFoundResponse);
                    }
                    else
                    {
                        // Handle other unsuccessful responses
                        var errorResponse = new ErrorResponse
                        {
                            error = "Error",
                            error_description = $"HTTP error {response.StatusCode}: {response.ReasonPhrase}."
                        };
                        return StatusCode((int)response.StatusCode, errorResponse);
                    }
                }

                // Parse the XML data
                var xmlData = await response.Content.ReadAsStringAsync();
                var xmlDocument = XDocument.Parse(xmlData, LoadOptions.None);
                var dataElement = xmlDocument.Root;

                if (dataElement == null)
                {
                    var notFoundResponse = new ErrorResponse
                    {
                        error = "Not Found",
                        error_description = $"Company with ID {id} not found."
                    };
                    return NotFound(notFoundResponse);
                }

                // Extract data elements
                var idElement = dataElement.Element("id");
                var nameElement = dataElement.Element("name");
                var descriptionElement = dataElement.Element("description");

                if (idElement == null || nameElement == null || descriptionElement == null)
                {
                    var badRequestResponse = new ErrorResponse
                    {
                        error = "Bad Request",
                        error_description = "Missing required elements in XML."
                    };
                    return BadRequest(badRequestResponse);
                }

                // Construct the Company object
                var company = new Company
                {
                    id = (int)idElement,
                    name = (string)nameElement,
                    description = (string)descriptionElement
                };

                return Ok(company);
            }
            catch (HttpRequestException ex)
            {
                var internalErrorResponse = new ErrorResponse
                {
                    error = "Internal Server Error",
                    error_description = ex.Message
                };
                return StatusCode(500, internalErrorResponse);
            }
            catch (System.Exception ex)
            {
                var internalErrorResponse = new ErrorResponse
                {
                    error = "Internal Server Error",
                    error_description = ex.Message
                };
                return StatusCode(500, internalErrorResponse);
            }
        }
    }
}