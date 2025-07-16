# SmartAiChat Architecture

This document provides an overview of the system architecture and design decisions for the SmartAiChat application.

## Guiding Principles

- **Clean Architecture:** The application follows Clean Architecture principles to create a modular, maintainable, and testable codebase.
- **Domain-Driven Design (DDD):** The domain model is at the heart of the application, and the architecture is designed to support the domain.
- **SOLID Principles:** The code is written to follow the SOLID principles of object-oriented design.
- **Multi-Tenancy:** The application is designed to support multiple tenants with isolated data.

## Architecture Layers

The application is divided into the following layers:

- **Domain:** This layer contains the domain entities, interfaces, and business logic. It is the core of the application and has no dependencies on other layers.
- **Application:** This layer contains the application logic, such as commands, queries, and handlers. It depends on the Domain layer but not on the other layers.
- **Infrastructure:** This layer contains the implementation of the interfaces defined in the Domain layer, such as repositories, services, and external clients. It depends on the Domain and Application layers.
- **API:** This layer contains the API endpoints, controllers, and SignalR hubs. It depends on the Application and Infrastructure layers.

## Technology Stack

- **.NET 8:** The application is built using the latest version of the .NET framework.
- **ASP.NET Core:** The API is built using ASP.NET Core.
- **Entity Framework Core:** The application uses Entity Framework Core for data access.
- **SQL Server:** The application uses SQL Server as the database.
- **SignalR:** The application uses SignalR for real-time communication.
- **MediatR:** The application uses MediatR to implement the CQRS pattern.
- **AutoMapper:** The application uses AutoMapper to map between entities and DTOs.
- **FluentValidation:** The application uses FluentValidation to validate commands and queries.
- **Carter:** The application uses Carter to create a more lightweight and flexible API.
- **Serilog:** The application uses Serilog for logging.
- **MinIO:** The application uses MinIO for file storage.
- **OpenAI:** The application uses the OpenAI API to power the chatbot.

## Design Decisions

- **CQRS:** The application uses the Command Query Responsibility Segregation (CQRS) pattern to separate read and write operations. This allows for a more scalable and maintainable application.
- **Repository Pattern:** The application uses the repository pattern to abstract the data access logic. This makes it easier to test the application and to switch to a different database if needed.
- **Unit of Work Pattern:** The application uses the unit of work pattern to manage database transactions. This ensures that all changes to the database are either committed or rolled back as a single unit.
- **Factory Pattern:** The application uses the factory pattern to create the appropriate AI service based on the provider specified in the `AiConfiguration`. This makes it easy to add new AI providers in the future.
- **Middleware:** The application uses middleware for global error handling, rate limiting, and secure headers. This helps to improve the security and robustness of the application.
- **Dependency Injection:** The application uses dependency injection to manage the dependencies between the different layers. This makes the application more modular and easier to test.
- **Swagger/OpenAPI:** The application uses Swagger/OpenAPI to document the API endpoints. This makes it easier for other developers to use the API.
- **Clean Architecture:** The application follows Clean Architecture principles to create a modular, maintainable, and testable codebase.
- **Multi-Tenancy:** The application is designed to support multiple tenants with isolated data. This is achieved by adding a `TenantId` to all tenant-specific entities and by using a `TenantContext` to get the current tenant ID.
- **SignalR:** The application uses SignalR for real-time communication between the client and the server. This is used to provide real-time updates to the chat session.
- **Serilog:** The application uses Serilog for logging. This allows for structured logging, which makes it easier to search and analyze the logs.
- **MinIO:** The application uses MinIO for file storage. This is used to store training files for the AI.
- **OpenAI:** The application uses the OpenAI API to power the chatbot. This is used to generate responses to user messages.
