FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 5001

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["incrementally-backend.csproj", "./"]
RUN dotnet restore "./incrementally-backend.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "incrementally-backend.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "incrementally-backend.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "incrementally-backend.dll"]
