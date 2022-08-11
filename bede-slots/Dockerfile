#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["bede-slot-test/bede-slot-test.csproj", "bede-slot-test/"]
RUN dotnet restore "bede-slot-test/bede-slot-test.csproj"
COPY . .
WORKDIR "/src/bede-slot-test"
RUN dotnet build "bede-slot-test.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "bede-slot-test.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "bede-slot-test.dll"]