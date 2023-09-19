# E-Commerce Web API

Welcome to the E-Commerce Web API, a robust and flexible ASP.NET application designed to power your e-commerce platform. This README file provides essential information to help you understand, configure, and utilize this API effectively.

## Table of Contents

1. [Introduction](#introduction)
2. [Getting Started](#getting-started)
    - [Prerequisites](#prerequisites)
    - [Installation](#installation)
3. [Configuration](#configuration)
4. [Usage](#usage)
    - [Authentication](#authentication)
    - [Endpoints](#endpoints)
    - [Sample Requests](#sample-requests)
5. [License](#license)

## 1. Introduction

The E-Commerce Web API is a RESTful web service built using ASP.NET. It provides a set of endpoints to manage various aspects of an e-commerce platform, such as products, orders, customers, and authentication. This API serves as the backend for an e-commerce website or mobile application.

## 2. Getting Started

### Prerequisites

Before using the E-Commerce Web API, make sure you have the following prerequisites installed:

- [Visual Studio](https://visualstudio.microsoft.com/) (for development)
- [.NET Core SDK](https://dotnet.microsoft.com/download) (for running the API)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (for database storage)

### Installation

1. Clone the repository to your local machine:

    ```shell
    git clone https://github.com/dayoesq/asp-net-market-api.git
    ```

2. Open the solution in Visual Studio.

3. Configure the connection string in the `appsettings.json` file to point to your SQL Server database.

4. Run the following commands in the Package Manager Console to create the database schema and seed initial data:

    ```shell
    Update-Database
    ```

5. Build and run the application.

## 3. Configuration

- **appsettings.json**: Contains configuration settings for the application, including the database connection string, JWT authentication settings, and other application-specific options.

## 4. Usage

### Authentication

The E-Commerce Web API uses JSON Web Tokens (JWT) for authentication. To access protected endpoints, you must obtain a valid JWT token by logging in.

### Endpoints

The API provides various endpoints for managing e-commerce data. Here are some of the core endpoints:

- `/api/v1/products`: CRUD operations for products.
- `/api/v1/discounts`: CRUD opertations for discounts.
- `/api/v1/categories`: CRUD opertations for discounts.
- `/api/v1/users`: CRUD opertations for users.
- `/api/v1/auth`: User authentication and registration.

For a complete list of endpoints and their documentation, explore the codebase.

### Sample Requests

Here are some sample API requests using `curl`:

**Register a new user:**

```shell
curl -X POST -H "Content-Type: application/json" -d '{
  "email": "user@example.com",
  "password": "securepassword"
}' http://localhost:5000/api/v1/auth/register
```

**Login and obtain a JWT token:**

```shell
curl -X POST -H "Content-Type: application/json" -d '{
  "email": "user@example.com",
  "password": "securepassword"
}' http://localhost:5000/api/v1/auth/login
```

**Create a new product (requires authentication):**

```shell
curl -X POST -H "Content-Type: application/json" -H "Authorization: Bearer YOUR_JWT_TOKEN" -d '{
  "name": "Product Name",
  "price": 19.99,
  "description": "Product description"
}' http://localhost:5000/api/v1/products
```

## 5. License

This project is licensed under the [MIT License](LICENSE). Feel free to use and modify it for your e-commerce application.