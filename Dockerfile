# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
# For more information, please see https://aka.ms/containercompat

## Use root user
#USER root
#
### This stage is used when running from VS in fast mode (Default for Debug configuration)
### Use the ASP.NET runtime image for the final stage
### For windows based
## FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-1809 AS runtime
#
#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#
## Set the working directory
#WORKDIR /app
#
## Copy the built files from the previous stage
#COPY --from=build /app/out .
#
## Restore the project
#RUN dotnet restore --no-cache
#
## Build the project
#RUN dotnet build -c Release -o out
#
## Set a valid UID and GID
#USER 1000:1000
#
## Expose the necessary ports
#EXPOSE 8080
#EXPOSE 8081
#
## This stage is used to build the service project
#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
#
## Create a non-root user
#RUN adduser --disabled-password --gecos '' vishal
#
## Set the working directory
#WORKDIR /app
#
## Set permissions for the app directory
#RUN chown -R myuser:vishal /app
#
## Switch to the non-root user
#USER vishal
#
#COPY ["PhotocopyRevaluationApp.csproj", "."]
#RUN dotnet restore "./PhotocopyRevaluationApp.csproj"
#
### Restore dependencies
##RUN dotnet restore PhotocopyRevaluationAppMVC.csproj
#
#COPY . .
#WORKDIR "/src/."
#RUN dotnet build "./PhotocopyRevaluationApp.csproj" -c %BUILD_CONFIGURATION% -o /app/build
#
## Publish the application
## This stage is used to publish the service project to be copied to the final stage
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "./PhotocopyRevaluationApp.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false
#
##FROM build AS publish
##RUN dotnet publish "PhotocopyRevaluationApp.csproj" -c Release -o /app/publish
#
### Use the official .NET runtime image for the final image
## This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#
### Set the command to run the application
#ENTRYPOINT ["dotnet", "PhotocopyRevaluationApp.dll"]
#

# Use the ASP.NET runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Set the working directory
WORKDIR /app

# Expose the necessary ports
EXPOSE 8080
EXPOSE 8081

# Create a non-root user
RUN adduser --disabled-password --gecos '' myuser

# Set permissions for the app directory
RUN chown -R myuser:myuser /app

# Switch to the non-root user
USER myuser

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["PhotocopyRevaluationApp.csproj", "./"]
RUN dotnet restore "PhotocopyRevaluationApp.csproj"

# Copy the entire project
COPY . .

# Build the project
RUN dotnet build "PhotocopyRevaluationApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "PhotocopyRevaluationApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set the command to run the application
ENTRYPOINT ["dotnet", "PhotocopyRevaluationApp.dll"]
