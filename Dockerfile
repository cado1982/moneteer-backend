FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Moneteer.Backend/Moneteer.Backend.csproj", "Moneteer.Backend/"]
COPY ["Moneteer.Domain/Moneteer.Domain.csproj", "Moneteer.Domain/"]
COPY ["Moneteer.Models/Moneteer.Models.csproj", "Moneteer.Models/"]
COPY ["Moneteer.Backend.Tests/Moneteer.Backend.Tests.csproj", "Moneteer.Backend.Tests/"]
COPY ["Moneteer.Domain.Tests/Moneteer.Domain.Tests.csproj", "Moneteer.Domain.Tests/"]

RUN dotnet restore "Moneteer.Backend/Moneteer.Backend.csproj"
RUN dotnet restore "Moneteer.Backend.Tests/Moneteer.Backend.Tests.csproj"
RUN dotnet restore "Moneteer.Domain.Tests/Moneteer.Domain.Tests.csproj"
COPY . .
RUN dotnet build "Moneteer.Backend/Moneteer.Backend.csproj" -c Release -o /app
RUN dotnet build "Moneteer.Backend.Tests/Moneteer.Backend.Tests.csproj" -c Release -o /app
RUN dotnet build "Moneteer.Domain.Tests/Moneteer.Domain.Tests.csproj" -c Release -o /app
RUN dotnet test "Moneteer.Backend.Tests/Moneteer.Backend.Tests.csproj"
RUN dotnet test "Moneteer.Domain.Tests/Moneteer.Domain.Tests.csproj"

FROM build AS publish
RUN dotnet publish "Moneteer.Backend/Moneteer.Backend.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Moneteer.Backend.dll"]