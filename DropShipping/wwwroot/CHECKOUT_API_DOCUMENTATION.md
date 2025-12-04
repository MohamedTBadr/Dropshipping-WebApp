# Checkout Page - JSON Structure Documentation

## Overview
This document describes the JSON structure that the checkout page sends to the backend OrderController.

## API Endpoint
```
POST /api/Order
```

## Request Body Structure

The checkout page sends data according to the `OrderCreateDTO` structure:

```json
{
  "dropshipperId": "string (GUID format)",
  "items": [
    {
      "productId": "string (GUID format)",
      "quantity": number
    }
  ],
  "customer": {
    "name": "string",
    "address": "string",
    "email": "string",
    "phoneNumber": "string"
  }
}
```

## Example Request

```json
{
  "dropshipperId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "items": [
    {
      "productId": "a1b2c3d4-5678-90ab-cdef-1234567890ab",
      "quantity": 2
    },
    {
      "productId": "b2c3d4e5-6789-01bc-def1-234567890abc",
      "quantity": 1
    }
  ],
  "customer": {
    "name": "John Doe",
    "address": "123 Main Street, Apt 4B, New York, NY 10001",
    "email": "john.doe@example.com",
    "phoneNumber": "+1-555-123-4567"
  }
}
```

## Field Descriptions

### dropshipperId
- **Type**: String (GUID)
- **Required**: Yes
- **Description**: The unique identifier of the dropshipper placing the order
- **Source**: Retrieved from localStorage (key: 'userId') or authentication system

### items
- **Type**: Array of OrderItemCreateDTO
- **Required**: Yes
- **Description**: List of products being ordered

#### items[].productId
- **Type**: String (GUID)
- **Required**: Yes
- **Description**: The unique identifier of the product
- **Source**: From cart items in localStorage

#### items[].quantity
- **Type**: Integer
- **Required**: Yes
- **Description**: Quantity of the product being ordered
- **Validation**: Must be greater than 0

### customer
- **Type**: CustomerDetailsDTO object
- **Required**: Yes
- **Description**: Customer shipping and contact information

#### customer.name
- **Type**: String
- **Required**: Yes
- **Description**: Full name of the customer
- **Source**: Form input field

#### customer.address
- **Type**: String
- **Required**: Yes
- **Description**: Complete shipping address
- **Source**: Form textarea field

#### customer.email
- **Type**: String
- **Required**: Yes
- **Description**: Customer's email address
- **Validation**: Must be valid email format
- **Source**: Form input field

#### customer.phoneNumber
- **Type**: String
- **Required**: Yes
- **Description**: Customer's contact phone number
- **Source**: Form input field

## Response Structure

On successful order creation, the API returns:

```json
{
  "id": "string (GUID)",
  "dropshipperId": "string (GUID)",
  "orderDate": "datetime",
  "status": "string",
  "totalAmount": number,
  "items": [...],
  "customer": {...}
}
```

## Error Handling

The checkout page handles the following error scenarios:

1. **Empty Cart**: Shows message and prevents checkout
2. **Form Validation**: Validates all required fields before submission
3. **API Errors**: Displays error message to user
4. **Network Errors**: Shows appropriate error notification

## Cart Data Structure (localStorage)

The cart items are stored in localStorage with the following structure:

```json
[
  {
    "id": "product-guid",
    "name": "Product Name",
    "price": 99.99,
    "quantity": 2,
    "image": "path/to/image.jpg"
  }
]
```

## Important Notes

1. **Authentication**: The `dropshipperId` should come from a proper authentication system. Currently, it falls back to localStorage or a default GUID for testing.

2. **Product IDs**: Ensure that product IDs in the cart are valid GUIDs that exist in your database.

3. **Validation**: All form fields are required and validated on the client side before submission.

4. **Cart Clearing**: The cart is cleared from localStorage only after successful order creation.

5. **Loading State**: A modal is shown during API request to prevent duplicate submissions.
