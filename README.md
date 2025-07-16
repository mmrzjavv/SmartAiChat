# SmartAiChat

SmartAiChat is a multi-tenant AI-powered customer support chat system built using .NET 8, following Clean Architecture principles. The application provides a customizable chatbot solution powered by OpenAI integrated with SignalR for real-time communication, and it supports multiple organizations (tenants) with isolated data. Users can communicate with AI or support agents, while support agents have the ability to manage chat interactions.

## Features

- **Multi-Tenancy:** Support for multiple organizations with isolated data.
- **AI-Powered Chat:** Customizable chatbot powered by OpenAI.
- **Real-Time Communication:** Real-time communication using SignalR.
- **Clean Architecture:** Follows Clean Architecture principles for a modular and maintainable codebase.
- **User Management:** User management for customers, support agents, and admins.
- **Tenant Management:** Tenant management for super admins.
- **FAQ Management:** FAQ management for tenants.
- **Training File Management:** Training file management for tenants.
- **Sentiment Analysis:** Sentiment analysis of chat messages.
- **Translation:** Translation of chat messages.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [MinIO](https://min.io/download)

## Setup

1. **Clone the repository:**

   ```bash
   git clone https://github.com/your-username/smartaichat.git
   ```

2. **Configure the database:**

   - Open the `appsettings.json` file in the `SmartAiChat.API` project.
   - Modify the `DefaultConnection` connection string to point to your SQL Server instance.
   - Run the following command to apply the database migrations:

     ```bash
     dotnet ef database update --project SmartAiChat.Persistence
     ```

3. **Configure MinIO:**

   - Open the `appsettings.json` file in the `SmartAiChat.API` project.
   - Modify the `Minio` section with your MinIO server details.

4. **Configure OpenAI:**

   - Open the `appsettings.json` file in the `SmartAiChat.API` project.
   - Modify the `OpenAI` section with your OpenAI API key.

5. **Run the application:**

   - Run the following command to start the application:

     ```bash
     dotnet run --project SmartAiChat.API
     ```

## Usage

- The API will be available at `https://localhost:5001`.
- The Swagger UI will be available at `https://localhost:5001/swagger`.

## Contributing

Contributions are welcome! Please feel free to submit a pull request.

## License

This project is licensed under the MIT License.
