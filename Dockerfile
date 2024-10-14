# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
# For more information, please see https://aka.ms/containercompat

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-1809 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0-nanoserver-1809 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PhotocopyRevaluationApp.csproj", "."]
RUN dotnet restore "./PhotocopyRevaluationApp.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./PhotocopyRevaluationApp.csproj" -c %BUILD_CONFIGURATION% -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PhotocopyRevaluationApp.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PhotocopyRevaluationApp.dll"]

## Use the official .NET SDK image as the base image
#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#
## Set the working directory in the container
#WORKDIR /app
#
## Copy the specific .csproj file to the container
#COPY PhotocopyRevaluationAppMVC/PhotocopyRevaluationAppMVC.csproj ./
#
## Restore dependencies
#RUN dotnet restore PhotocopyRevaluationAppMVC.csproj
#
## Copy the remaining files and publish the application
#COPY . ./
#RUN dotnet publish PhotocopyRevaluationAppMVC.csproj -c Release -o out
#
## Use the official .NET runtime image for the final image
#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
#WORKDIR /app
#COPY --from=build /app/out .
#
## Set the command to run the application
#ENTRYPOINT ["dotnet", "PhotocopyRevaluationAppMVC.dll"]
#
