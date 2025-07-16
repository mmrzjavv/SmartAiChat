# SmartAiChat Deployment Guide

This guide provides instructions for deploying the SmartAiChat application to a production environment.

## Prerequisites

- A server with a supported operating system (e.g., Windows Server, Linux).
- [.NET 8 Hosting Bundle](https://dotnet.microsoft.com/download/dotnet/8.0) installed on the server.
- A production-ready database (e.g., SQL Server, PostgreSQL).
- A production-ready MinIO server.
- A reverse proxy (e.g., Nginx, IIS) to forward requests to the application.

## Deployment Steps

1. **Publish the application:**

   - Run the following command to publish the application:

     ```bash
     dotnet publish SmartAiChat.API -c Release -o ./publish
     ```

2. **Copy the published files:**

   - Copy the contents of the `./publish` directory to a directory on your server.

3. **Configure the application:**

   - Create an `appsettings.Production.json` file in the application directory.
   - Configure the database connection string, MinIO server details, OpenAI API key, and other settings in the `appsettings.Production.json` file.

4. **Set up the reverse proxy:**

   - Configure your reverse proxy to forward requests to the application.
   - For example, if you are using Nginx, you can add a new server block to your Nginx configuration file:

     ```nginx
     server {
         listen 80;
         server_name smartaichat.example.com;

         location / {
             proxy_pass http://localhost:5000;
             proxy_http_version 1.1;
             proxy_set_header Upgrade $http_upgrade;
             proxy_set_header Connection keep-alive;
             proxy_set_header Host $host;
             proxy_cache_bypass $http_upgrade;
         }
     }
     ```

5. **Set up a process manager:**

   - Use a process manager (e.g., systemd, PM2) to run the application as a service.
   - This will ensure that the application is automatically restarted if it crashes.

6. **Start the application:**

   - Start the application using your process manager.

## Securing the Application

- **Enable HTTPS:** Configure your reverse proxy to use HTTPS to encrypt traffic between the client and the server.
- **Configure a firewall:** Configure a firewall to restrict access to the application to only the necessary ports.
- **Regularly update the application:** Keep the application and its dependencies up to date to protect against security vulnerabilities.

## Monitoring the Application

- **Set up logging:** Configure Serilog to write logs to a file or a log management system.
- **Set up health checks:** Use the health check endpoint to monitor the health of the application.
- **Set up performance monitoring:** Use a performance monitoring tool to monitor the performance of the application.
