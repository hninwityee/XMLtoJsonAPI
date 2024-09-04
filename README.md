# XMLtoJsonApi
A simple ASP.NET Core Web API that fetches XML data from an external source, transforms it into JSON format, and returns it to the client. 
The API follows a predefined structure and is designed to handle errors smoothly.

Installation

To set up the project locally, make sure you have the .NET 8.0 SDK or later installed on your machine and follow the steps below:

1. Clone the repository:
   
   git clone https://github.com/hninwityee/XMLtoJsonAPI.git
   
2. Navigate to the project directory:
   
   cd XMLToJsonApi

3. Build the project:
   
   dotnet build
   
4. Run the application:
   
   dotnet run
  

Usage

The API provides endpoints to fetch and convert XML data to JSON. 
Below is an example of how to use the API.


Endpoint: 'GET /v1/api/xmltojson/{id}'
Example:
  
  GET https://localhost:5001/v1/api/xmltojson/1
  
Response:
  json
  {
    "id": 1,
    "name": "OpenPolytechnic",
    "description": "..is awesome"
  }

Testing

Unit tests using the MSTest framework are provided for the API.
