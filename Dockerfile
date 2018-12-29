FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Moneteer.Backend/Moneteer.Backend.csproj", "Moneteer.Backend/"]
COPY ["Moneteer.Domain/Moneteer.Domain.csproj", "Moneteer.Domain/"]
COPY ["Moneteer.Models/Moneteer.Models.csproj", "Moneteer.Models/"]
RUN dotnet restore "Moneteer.Backend/Moneteer.Backend.csproj"
COPY . .
WORKDIR "/src/Moneteer.Backend"
RUN dotnet build "Moneteer.Backend.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Moneteer.Backend.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Moneteer.Backend.dll"]