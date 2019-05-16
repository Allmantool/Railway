#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-nanoserver-1809 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-nanoserver-1809 AS build
WORKDIR /src
COPY ["Railway.UI/Railway.UI.csproj", "Railway.UI/"]
RUN dotnet restore "Railway.UI/Railway.UI.csproj"
COPY . .
WORKDIR "/src/Railway.UI"
RUN dotnet build "Railway.UI.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Railway.UI.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Railway.UI.dll"]