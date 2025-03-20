# MagicVilla API

## Description
MagicVilla API is a RESTful web service built using ASP.NET Core and Entity Framework Core. It provides CRUD operations for managing villa records, including details such as name, description, price, occupancy, and square footage. The API follows best practices in API development, including proper routing, validation, authentication, and authorization.

## Features
- CRUD operations for Villas and Villa Numbers (Create, Read, Update, Delete)
- Entity Framework Core for database interactions
- DTOs for better data management
- Fluent Validation for input validation
- Authentication and Authorization with JWT
- Swagger for API documentation
- Logging and Exception Handling

## Technologies Used
- ASP.NET Core 8
- Entity Framework Core
- SQL Server
- AutoMapper
- FluentValidation
- Serilog
- Swagger (Swashbuckle)
- JSON Web Tokens (JWT)

## Installation

### Prerequisites
- .NET 8 SDK or later
- SQL Server
- Visual Studio

### Steps
1. Clone the repository:
   ```sh
   git clone https://github.com/marwan779/MagicVilla_API.git
   ```
2. Navigate to the project directory:
   ```sh
   cd MagicVilla_API
   ```
3. Restore dependencies:
   ```sh
   dotnet restore
   ```
4. Apply database migrations:
   ```sh
   dotnet ef database update
   ```
5. Run the application:
   ```sh
   dotnet run
   ```


## API Endpoints

### Villa Endpoints
| Method | Endpoint | Description |
|--------|---------|-------------|
| GET    | `/api/villa` | Get all villas |
| GET    | `/api/villa/{id}` | Get villa by ID |
| POST   | `/api/villa` | Create a new villa |
| PUT    | `/api/villa/{id}` | Update an existing villa |
| DELETE | `/api/villa/{id}` | Delete a villa |

### Villa Number Endpoints
| Method | Endpoint | Description |
|--------|---------|-------------|
| GET    | `/api/villanumber` | Get all villa numbers |
| GET    | `/api/villanumber/{id}` | Get villa number by ID |
| POST   | `/api/villanumber` | Create a new villa number |
| PUT    | `/api/villanumber/{id}` | Update an existing villa number |
| DELETE | `/api/villanumber/{id}` | Delete a villa number |

### Authentication Endpoints
| Method | Endpoint | Description |
|--------|---------|-------------|
| POST   | `/api/auth/register` | Register a new user |
| POST   | `/api/auth/login` | Authenticate user and get token |
| POST   | `/api/auth/refresh` | Refresh access token |
| POST   | `/api/auth/revoke` | Revoke refresh token |

## Swagger Screenshot
![localhost_7001_swagger_index html](https://github.com/user-attachments/assets/2f74d13e-651c-4f1b-88b0-3eb2fa184351)

## Contact
- **GitHub**: [marwan779](https://github.com/marwan779)
- **LinkedIn**: [https://www.linkedin.com/in/marwan-mohamed-125a4b252/]

