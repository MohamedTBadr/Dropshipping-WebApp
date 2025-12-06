# Dropshipping API

## Description

The Dropshipping API is a robust service designed to facilitate the management of a dropshipping business. This API allows for the creation, update, deletion, and retrieval of various entities including Products, Categories, Brands, Orders, and Dropshippers. It supports file uploads for product images and offers authentication features for secure access.

## Features

- **Product Management**: Create, update, delete, and retrieve product information including associated images.
- **Category Management**: Manage product categories including creation, update, and deletion.
- **Brand Management**: Handle brand information and associate them with products.
- **Order Management**: Create, update, delete, and retrieve orders made by dropshippers.
- **Dropshipper Management**: Create and manage dropshippers, linking them with users and wallets for transaction tracking.
- **File Upload Support**: Upload images associated with products using controlled file types and size limits.
- **Authentication and Authorization**: Secure endpoints to prevent unauthorized access.

## Installation and Setup

To set up the project locally, follow these steps:

1. Clone the repository:
   ```bash
   git clone https://github.com/<username>/dropshipping-api.git
   cd dropshipping-api
   ```

2. Ensure you have .NET SDK installed. If not, download it from the [official .NET website](https://dotnet.microsoft.com/download).

3. Restore the project dependencies:
   ```bash
   dotnet restore
   ```

4. Update the `appsettings.json` file with your database connection string and JWT settings.

5. Run the database migrations:
   ```bash
   dotnet ef database update
   ```

6. Start the application:
   ```bash
   dotnet run
   ```

## Basic Usage

### Authentication

You can interact with the API by sending requests to the endpoints listed below. For all operations requiring authorization (like creating products or categories), a valid JWT must be provided.

### Example Endpoints

- **Create a Product**
  ```http
  POST /api/products
  Content-Type: application/json
  
  {
      "name": "Sample Product",
      "description": "Sample Description",
      "price": 99.99,
      "categoryId": "some-category-id",
      "brandId": "some-brand-id",
      "productImages": [...]
  }
  ```

- **Get All Products**
  ```http
  GET /api/products
  ```

- **Delete a Product**
  ```http
  DELETE /api/products/{id}
  ```

## Configuration

### Environment Variables

You must configure the following environment variables in your application for optimal operation:

- **JwtSettings:Key**: The secret key used to generate JWT tokens.
- **JwtSettings:Issuer**: The issuer claim for the tokens.
- **JwtSettings:Audience**: The audience claim for the tokens.

## Contributing

Contributions are welcome! Please follow these steps to contribute:

1. Fork the repository.
2. Create a new branch (`git checkout -b feature-YourFeature`).
3. Make your changes and commit them (`git commit -m 'Add some feature'`).
4. Push your changes (`git push origin feature-YourFeature`).
5. Create a new Pull Request.


