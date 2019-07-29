#Build Stage
FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /app

COPY Moneteer.Backend.sln .
COPY Moneteer.Backend/Moneteer.Backend.csproj ./Moneteer.Backend/
COPY Moneteer.Backend.Tests/Moneteer.Backend.Tests.csproj ./Moneteer.Backend.Tests/
COPY Moneteer.Domain/Moneteer.Domain.csproj ./Moneteer.Domain/
COPY Moneteer.Domain.Tests/Moneteer.Domain.Tests.csproj ./Moneteer.Domain.Tests/
COPY Moneteer.Models/Moneteer.Models.csproj ./Moneteer.Models/
RUN dotnet restore

COPY . .

RUN dotnet publish Moneteer.Backend/Moneteer.Backend.csproj -c Release -o /publish --no-restore

#Runtime Image Stage
FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /public
COPY --from=build /publish .
ENV ASPNETCORE_URLS http://+:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "Moneteer.Backend.dll"]