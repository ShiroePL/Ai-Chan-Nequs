# Use the .NET Core SDK image as the base image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

# Create a directory for the app
WORKDIR /app

# Copy the necessary files for the app to the container
COPY . ./

# Restore dependencies
RUN dotnet restore

# Add the required packages
RUN dotnet add package Discord.Net
RUN dotnet add package Microsoft.Extensions.DependencyInjection
RUN dotnet add package Microsoft.AspNetCore.SignalR.Client
RUN dotnet add package Newtonsoft.Json
RUN dotnet add package LiteDB

# Copy the token file to the container
COPY mytokenfile.txt /app/tokenfile.txt

# Build the app
RUN dotnet publish -c Release -o out
RUN dotnet build -c Release

# Use a runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0

# Set the working directory
WORKDIR /app

# Create a directory for the database file
RUN mkdir /app/data

# Copy the built files from the build-env to the runtime image
COPY --from=build-env /app/out .

# Copy the token file from the build stage to the runtime image
COPY --from=build-env /app/config.json .

# Set the entrypoint for the app
ENTRYPOINT ["dotnet", "Ai-Chan.dll"]