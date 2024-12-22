# Layer 1: Base image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Layer 2: Solution file
COPY ["JobScraper.sln", "./"]

# Layer 3: Project files
COPY ["*/*.csproj", "./"]
# Create a dir for each .csproj file and move the corresponding .csproj file into that dir
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done

# Layer 4: NuGet restore
RUN dotnet restore

# Layer 5: Source code
COPY . .

# Layer 6: Build
RUN dotnet publish "JobScraper.Api/JobScraper.Api.csproj" -c Release -o /app/publish

# New stage - hver FROM starter en ny stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "JobScraper.Api.dll"]
