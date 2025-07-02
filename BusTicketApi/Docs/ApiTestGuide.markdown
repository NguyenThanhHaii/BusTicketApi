# BusTicketApi Test Guide

This document provides a comprehensive guide to test all APIs in the BusTicketApi project using Postman. Ensure the API is running at `http://localhost:7112` and that the database is seeded with sample data before testing.

## 1. Prerequisites

- **API Running**: Start the project with `dotnet run` from the project directory (`D:\Project\BusTicketApi`).
- **Authentication Token**: Obtain a JWT token by testing the `/api/Auth/login` endpoint (see Section 2).
- **Tool**: Use Postman for all tests.
- **Sample Data**: Verify the database contains seeded data (e.g., Buses, Routes, Employees, etc.) as per the `SeedData.sql` file.

## 2. Authentication API (/api/Auth)

### 2.1. Test `POST /api/Auth/login`
- **Description**: Authenticate to obtain a JWT token.
- **Request**:
  - Method: `POST`
  - URL: `http://localhost:7112/api/Auth/login`
  - Body: `raw` (JSON)
    ```json
    {
        "username": "admin",
        "password": "password123"
    }
    ```
- **Expected Result**: `200 OK`
  ```json
  {
      "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...[token dài]...",
      "role": "Admin"
  }
  ```
- **Error Test**:
  - Use incorrect credentials (e.g., `"username": "wrong", "password": "123"`): `401 Unauthorized`.

### 2.2. Test `POST /api/Auth/register`
- **Description**: Register a new employee (Admin only).
- **Request**:
  - Method: `POST`
  - URL: `http://localhost:7112/api/Auth/register`
  - Header: `Authorization: Bearer <token>` (from login)
  - Body: `raw` (JSON)
    ```json
    {
        "username": "new.employee",
        "password": "newpass123",
        "name": "New Employee",
        "email": "new.employee@example.com",
        "phoneNumber": "312-555-0003",
        "qualifications": "Ticketing",
        "role": "Employee",
        "age": 30,
        "location": "Chicago, IL"
    }
    ```
- **Expected Result**: `200 OK` with `"Employee registered successfully"`.
- **Error Test**:
  - Use existing username (e.g., `"username": "admin"`): `400 Bad Request`.
  - No token or invalid token: `401 Unauthorized`.

## 3. Bus Management API (/api/Buses)

### 3.1. Test `GET /api/Buses`
- **Description**: Retrieve list of buses (Admin only).
- **Request**:
  - Method: `GET`
  - URL: `http://localhost:7112/api/Buses`
  - Header: `Authorization: Bearer <token>`
- **Expected Result**: `200 OK`
  ```json
  [
      {"busId": 1, "busCode": "10001", "busNumber": "NY123", "typeId": 1, "totalSeats": 40},
      {"busId": 2, "busCode": "10002", "busNumber": "CA456", "typeId": 4, "totalSeats": 30}
  ]
  ```
- **Error Test**: No token or invalid token: `401 Unauthorized`.

### 3.2. Test `POST /api/Buses`
- **Description**: Create a new bus (Admin only).
- **Request**:
  - Method: `POST`
  - URL: `http://localhost:7112/api/Buses`
  - Header: `Authorization: Bearer <token>`
  - Body: `raw` (JSON)
    ```json
    {
        "busCode": "10003",
        "busNumber": "TX789",
        "typeId": 2,
        "totalSeats": 35
    }
    ```
- **Expected Result**: `201 Created` with new bus details.
- **Error Test**: Invalid data (e.g., negative `totalSeats`): `400 Bad Request`.

## 4. Route Management API (/api/Routes)

### 4.1. Test `GET /api/Routes`
- **Description**: Retrieve list of routes (Admin only).
- **Request**:
  - Method: `GET`
  - URL: `http://localhost:7112/api/Routes`
  - Header: `Authorization: Bearer <token>`
- **Expected Result**: `200 OK`
  ```json
  [
      {"routeId": 1, "routeCode": "R001", "startLocation": "New York, NY", ...},
      {"routeId": 2, "routeCode": "R002", "startLocation": "Los Angeles, CA", ...}
  ]
  ```
- **Error Test**: No token: `401 Unauthorized`.

### 4.2. Test `POST /api/Routes`
- **Description**: Create a new route (Admin only).
- **Request**:
  - Method: `POST`
  - URL: `http://localhost:7112/api/Routes`
  - Header: `Authorization: Bearer <token>`
  - Body: `raw` (JSON)
    ```json
    {
        "routeCode": "R006",
        "startLocation": "Denver, CO",
        "endLocation": "Phoenix, AZ",
        "distance": 850.0,
        "basePrice": 100.00
    }
    ```
- **Expected Result**: `201 Created` with new route details.
- **Error Test**: Invalid data (e.g., negative `basePrice`): `400 Bad Request`.

## 5. Employee Management API (/api/Employees)

### 5.1. Test `GET /api/Employees`
- **Description**: Retrieve list of employees (Admin only).
- **Request**:
  - Method: `GET`
  - URL: `http://localhost:7112/api/Employees`
  - Header: `Authorization: Bearer <token>`
- **Expected Result**: `200 OK`
  ```json
  [
      {"employeeId": 100000, "username": "admin", "role": "Admin", ...}
  ]
  ```
- **Error Test**: No token: `401 Unauthorized`.

### 5.2. Test `POST /api/Employees`
- **Description**: Create a new employee (Admin only).
- **Request**:
  - Method: `POST`
  - URL: `http://localhost:7112/api/Employees`
  - Header: `Authorization: Bearer <token>`
  - Body: `raw` (JSON)
    ```json
    {
        "username": "new.staff",
        "password": "staffpass123",
        "name": "New Staff",
        "email": "new.staff@example.com",
        "phoneNumber": "303-555-1111",
        "qualifications": "Customer Service",
        "role": "Employee",
        "age": 25,
        "location": "Denver, CO"
    }
    ```
- **Expected Result**: `201 Created` with new employee details.
- **Error Test**: Duplicate username: `400 Bad Request`.

## 6. Seat Management API (/api/SeatsInBusTrip)

### 6.1. Test `GET /api/SeatsInBusTrip`
- **Description**: Retrieve list of seats (Admin only).
- **Request**:
  - Method: `GET`
  - URL: `http://localhost:7112/api/SeatsInBusTrip`
  - Header: `Authorization: Bearer <token>`
- **Expected Result**: `200 OK`
  ```json
  [
      {"seatInTripId": 1, "tripId": 1, "seatNumber": "A1", "isAvailable": true}
  ]
  ```
- **Error Test**: No token: `401 Unauthorized`.

### 6.2. Test `POST /api/SeatsInBusTrip`
- **Description**: Create a new seat (Admin only).
- **Request**:
  - Method: `POST`
  - URL: `http://localhost:7112/api/SeatsInBusTrip`
  - Header: `Authorization: Bearer <token>`
  - Body: `raw` (JSON)
    ```json
    {
        "tripId": 1,
        "seatNumber": "E1",
        "isAvailable": true
    }
    ```
- **Expected Result**: `201 Created` with new seat details.
- **Error Test**: Duplicate seat number: `400 Bad Request`.

### 6.3. Test `POST /api/SeatsInBusTrip/bulkByTrip`
- **Description**: Create multiple seats for a trip (Admin only).
- **Request**:
  - Method: `POST`
  - URL: `http://localhost:7112/api/SeatsInBusTrip/bulkByTrip`
  - Header: `Authorization: Bearer <token>`
  - Body: `raw` (JSON)
    ```json
    {
        "tripId": 1,
        "seatNumbers": ["E2", "E3", "E4"]
    }
    ```
- **Expected Result**: `201 Created` with new seat list.
- **Error Test**: Duplicate seat numbers: `400 Bad Request`.

## 7. Booking Management API (/api/Bookings)

### 7.1. Test `POST /api/Bookings`
- **Description**: Create a new booking (Employee only).
- **Request**:
  - Method: `POST`
  - URL: `http://localhost:7112/api/Bookings`
  - Header: `Authorization: Bearer <token>` (use Employee token, e.g., `jane.smith`)
  - Body: `raw` (JSON)
    ```json
    {
        "employeeId": 100002,
        "bookingDate": "2025-07-03T12:00:00",
        "customerDetails": {
            "name": "Frank Miller",
            "dateOfBirth": "1987-09-20",
            "email": "frank.miller@example.com",
            "phoneNumber": "312-555-7890"
        },
        "seatInTripId": 2
    }
    ```
- **Expected Result**: `201 Created` with new booking details (e.g., `{"bookingId": 6, ...}`).
- **Error Test**:
  - Non-existent `seatInTripId` (e.g., 999): `400 Bad Request`.
  - Invalid `employeeId`: `400 Bad Request`.

### 7.2. Test `PUT /api/Bookings/{id}/cancel`
- **Description**: Cancel an existing booking (Employee only).
- **Request**:
  - Method: `PUT`
  - URL: `http://localhost:7112/api/Bookings/1/cancel`
  - Header: `Authorization: Bearer <token>` (Employee token)
- **Expected Result**: `200 OK` with cancellation details (e.g., `{"message": "Booking cancelled successfully", "refundAmount": 45.00}`).
- **Error Test**:
  - Non-existent `bookingId` (e.g., 999): `404 Not Found`.
  - Already cancelled booking: `400 Bad Request`.

### 7.3. Test `GET /api/Bookings/{id}/ticket`
- **Description**: Generate a PDF ticket (Employee only).
- **Request**:
  - Method: `GET`
  - URL: `http://localhost:7112/api/Bookings/1/ticket`
  - Header: `Authorization: Bearer <token>` (Employee token)
- **Expected Result**: `200 OK` with PDF file (e.g., `Ticket_1.pdf`).
- **Error Test**:
  - Non-existent `bookingId` (e.g., 999): `404 Not Found`.
  - No token: `401 Unauthorized`.

## 8. Report Management API (/api/Report)

### 8.1. Test `GET /api/Report/bookings`
- **Description**: Retrieve booking report by month (Admin only).
- **Request**:
  - Method: `GET`
  - URL: `http://localhost:7112/api/Report/bookings`
  - Header: `Authorization: Bearer <token>` (Admin token)
- **Expected Result**: `200 OK`
  ```json
  [
      {"period": "2025-07", "totalTickets": 5, "totalRevenue": 525.00, ...}
  ]
  ```
- **Error Test**: No token: `401 Unauthorized`.

### 8.2. Test `GET /api/Report/bookings/{year}/{month}`
- **Description**: Retrieve booking report for a specific month (Admin only).
- **Request**:
  - Method: `GET`
  - URL: `http://localhost:7112/api/Report/bookings/2025/7`
  - Header: `Authorization: Bearer <token>` (Admin token)
- **Expected Result**: `200 OK`
  ```json
  {"period": "2025-07", "totalTickets": 5, "totalRevenue": 525.00, ...}
  ```
- **Error Test**: Invalid month (e.g., 13): `400 Bad Request`.

### 8.3. Test `GET /api/Report/bookings/{year}/{month}/{day}`
- **Description**: Retrieve booking report for a specific day (Admin only).
- **Request**:
  - Method: `GET`
  - URL: `http://localhost:7112/api/Report/bookings/2025/7/3`
  - Header: `Authorization: Bearer <token>` (Admin token)
- **Expected Result**: `200 OK`
  ```json
  {"period": "2025-07-03", "totalTickets": 2, "totalRevenue": 255.00, ...}
  ```
- **Error Test**: Invalid date (e.g., 2025/7/32): `400 Bad Request`.

## 9. Post-Test Actions
- **Verify Database**: Check updated records (e.g., `SELECT * FROM Bookings WHERE BookingId = 6`).
- **Commit Changes**: If modifications are made, push to GitHub:
  ```bash
  cd D:\Project\BusTicketApi
  git add .
  git commit -m "Tested all APIs successfully"
  git push
  ```

## 10. Troubleshooting
- **401 Unauthorized**: Ensure token is valid and role matches policy.
- **404 Not Found**: Verify IDs exist in the database.
- **500 Internal Server Error**: Check logs or database connection.