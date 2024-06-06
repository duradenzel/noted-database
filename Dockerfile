FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source
COPY . .
RUN dotnet restore "noted-database.csproj" --disable-parallel
RUN dotnet publish "noted-database.csproj" -c release -o /app --no-restore


FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app ./
EXPOSE 5170
ENV ASPNETCORE_URLS=http://+:5170

ENTRYPOINT ["dotnet", "noted-database.dll"]


