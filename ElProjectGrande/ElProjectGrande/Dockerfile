FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY ["ElProjectGrande/ElProjectGrande.csproj", "ElProjectGrande/"]
RUN dotnet restore "ElProjectGrande/ElProjectGrande.csproj"

COPY . .
WORKDIR "/app/ElProjectGrande"
RUN dotnet build "ElProjectGrande.csproj" -c Debug -o /app/build

ENTRYPOINT ["dotnet", "watch", "run", "--no-launch-profile", "--urls", "http://0.0.0.0:8080"]
