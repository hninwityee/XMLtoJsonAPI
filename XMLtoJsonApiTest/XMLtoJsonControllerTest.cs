using Microsoft.AspNetCore.Mvc;
using XMLtoJsonApi.Controllers;
using XMLtoJsonApi.Models;
using System.Net;
using System.Text;
using Moq;
using Moq.Protected;

namespace XMLtoJsonApiTest
{
    [TestClass]
    public class XmlToJsonControllerTests
    {
        [TestMethod]
        public async Task GetJsonFromXml_ReturnsCompany_WhenIdIsValid()
        {
            // Arrange
            var mockHttpClient = new Mock<HttpClient>();
            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        @"<?xml version=""1.0"" encoding=""UTF-8""?><Data><id>1</id><name>OpenPolytechnic</name><description>..is awesome</description></Data>",
                        Encoding.UTF8,
                        "application/xml")
                });

            var httpClient = new HttpClient(handlerMock.Object);
            var controller = new XmlToJsonController(httpClient);
            var result = await controller.GetJsonFromXml(1);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);  
            Assert.IsInstanceOfType(okResult.Value, typeof(Company));
            var company = okResult.Value as Company;

            Assert.IsNotNull(company);   
            Assert.AreEqual(1, company.id);
            Assert.AreEqual("OpenPolytechnic", company.name);
            Assert.AreEqual("..is awesome", company.description);
        }

        [TestMethod]
        public async Task GetJsonFromXml_ReturnsNotFound_WhenIdIsInvalid()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                });

            var httpClient = new HttpClient(handlerMock.Object);
            var controller = new XmlToJsonController(httpClient);
            var result = await controller.GetJsonFromXml(999);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;

            Assert.IsNotNull(notFoundResult);
            Assert.IsInstanceOfType(notFoundResult.Value, typeof(ErrorResponse));
            var errorResponse = notFoundResult.Value as ErrorResponse;

            Assert.IsNotNull(errorResponse);  
            Assert.AreEqual("Not Found", errorResponse.error);
            Assert.AreEqual("Company with ID 999 not found.", errorResponse.error_description);
        }
    }
}