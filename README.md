# run the following commands: 
open the terminal <br />
cd RateFetcherPrinter <br />
dotnet restore <br />
dotnet build <br />
dotnet run <br />
on your browser open "http://localhost:5067/swagger/index.html" <br />
please note that the external api has rate limitation and therefore it doesn't always work best <br />
and also I increased the time the data is refreshed to 10 minutes instead of 10 seconds to prevent overloading on the external API