FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["CoreWebApp/CoreWebApp.csproj", "CoreWebApp/"]
RUN dotnet restore "CoreWebApp/CoreWebApp.csproj"
COPY . .
WORKDIR "/src/CoreWebApp"
RUN dotnet build "CoreWebApp.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "CoreWebApp.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CoreWebApp.dll"]