# BusTicketApi Project Documentation - Bus Ticket Booking System

## 1. Project Overview

### 1.1. General Description

The Bus Ticket Booking System is a web application designed to automate the ticket booking process, manage bus, route, employee, and seat information for transportation companies. The project focuses on providing robust backend APIs to support key functionalities such as ticket booking, cancellation, PDF ticket generation, and revenue reporting. The system is designed to serve both employees and administrators with clear role-based access control using JWT authentication.

### 1.2. Project Objectives

- Provide a scalable and robust backend API for transportation data management.
- Ensure security through JWT authentication and role-based authorization.
- Support PDF ticket generation with detailed information (phone number, email).
- Integrate revenue reporting for financial management.

---

## 2. API Documentation

(See details in `Docs/ApiDocs.md`.)

---

## 3. Installation Guide

### 3.1. System Requirements

- **Operating System**: Windows 10/11 or Linux.
- **.NET SDK**: Version 6.0 or higher (download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download)).
- **Development Tools**: Visual Studio 2022 or Visual Studio Code.
- **Database**: SQL Server 2019 or higher.
- **Git**: For version control (download from [git-scm.com](https://git-scm.com/)).

### 3.2. Project Installation

1. **Clone Repository**:

   ```bash
   git clone https://github.com/NguyenThanhHaii/BusTicketApi.git
   cd BusTicketApi
   ```

2. **Install Dependencies**:

   ```bash
   dotnet restore
   ```

3. **Configure Connection String**:

   - Open `appsettings.json` and update the SQL Server connection string:
     ```json
     {
       "Jwt": {
         "Key": "your_secret_key_here",
         "Issuer": "your_issuer",
         "Audience": "your_audience"
       },
       "ConnectionStrings": {
         "DefaultConnection": "Server=your_server;Database=BusTicketDB;Trusted_Connection=True;"
       }
     }
     ```
   - Replace `your_server` with your SQL Server name, and update `Jwt` as needed.

4. **Run Migration** (if database is not yet created):

   - Ensure `BusTicketDbContext` is configured in `Program.cs` (already included).
   - Create and apply migrations:
     ```bash
     dotnet ef migrations add InitialCreate
     dotnet ef database update
     ```

5. **Add Sample Data**:

   - Run the SQL commands in `SeedData.sql` (if available) or use the sample data below.

6. **Run the Project**:
   ```bash
   dotnet run
   ```
   - The API will run on `http://localhost:7112` (default).

### 3.3. Troubleshooting

- If EF Core errors occur (e.g., `ThenInclude`), check the version in `BusTicketApi.csproj` and update:
  ```bash
  dotnet add package Microsoft.EntityFrameworkCore --version 6.0.0
  dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 6.0.0
  dotnet restore
  ```
- If PDF errors occur (`iText`), ensure `itext7` and `itext7.bouncy-castle-adapter` version 8.0.3 are installed:
  ```bash
  dotnet add package itext7 --version 8.0.3
  dotnet add package itext7.bouncy-castle-adapter --version 8.0.3
  dotnet restore
  ```

---

## 4. Technologies Used

### 4.1. Languages and Frameworks

- **C#**: Primary language for backend development.
- **.NET 6**: Framework for building RESTful APIs.
- **Entity Framework Core 6.0**: ORM for SQL Server interaction.

### 4.2. Libraries

- **iText 7 (8.0.3)**: PDF generation for tickets.
- **Microsoft.EntityFrameworkCore.SqlServer**: SQL Server connectivity.
- **Microsoft.AspNetCore.Authentication.JwtBearer**: JWT authentication support.
- **BCrypt.Net-Next**: Password hashing.

### 4.3. Database

- **SQL Server**: Stores data (Buses, Routes, Employees, Bookings, Seats, etc.).

### 4.4. Supporting Tools

- **Git**: Version control.
- **Postman/Swagger**: API testing.

---

## 5. Project Structure

```
BusTicketApi/
├── Controllers/
│   ├── AuthController.cs
│   ├── BookingController.cs
│   ├── BusController.cs
│   ├── RouteController.cs
│   ├── EmployeeController.cs
│   ├── ReportController.cs
│   ├── SeatInBusTripController.cs
├── Data/
│   ├── BusTicketDbContext.cs
├── Models/
│   ├── Dtos/
│   │   ├── BookingRequest.cs
│   │   ├── BookingResponse.cs
│   │   ├── CancellationResponse.cs
│   │   ├── BusRequest.cs
│   │   ├── BusResponse.cs
│   │   ├── RouteRequest.cs
│   │   ├── RouteResponse.cs
│   │   ├── EmployeeRequest.cs
│   │   ├── EmployeeResponse.cs
│   │   ├── ReportResponse.cs
│   │   ├── TicketResponse.cs
│   │   ├── SeatInBusTripRequest.cs
│   │   ├── SeatInBusTripResponse.cs
│   │   ├── SeatInBusTripBulkRequest.cs
│   │   ├── LoginRequest.cs
│   │   ├── RegisterRequest.cs
│   │   ├── LoginResponse.cs
│   ├── Entities/
│   │   ├── Booking.cs
│   │   ├── BookingDetail.cs
│   │   ├── Customer.cs
│   │   ├── Employee.cs
│   │   ├── Bus.cs
│   │   ├── BusType.cs
│   │   ├── BusTrip.cs
│   │   ├── SeatInBusTrip.cs
│   │   ├── Route.cs
│   │   ├── AgeBasedDiscount.cs
│   │   ├── CancellationRule.cs
├── Docs/
│   ├── ApiDocs.md
│   ├── ProjectDocumentation.md
├── BusTicketApi.csproj
├── Program.cs
├── appsettings.json
```

---

## 6. Usage Guide

### 6.1. Authentication

- Use JWT tokens to access APIs.
- **Obtain Token**:
  - `POST /api/Auth/login`:
    ```json
    {
      "username": "admin",
      "password": "admin@123"
    }
    ```
    - Response: `200 OK` with `{ "token": "string", "role": "string" }`.
- **Use Token**:
  - Add header `Authorization: Bearer <token>` for other APIs.

### 6.2. API Testing

- Use Postman:
  - Method: GET/POST/PUT/DELETE.
  - Header: `Authorization: Bearer <token>` (if required).
  - Body: JSON as per `ApiDocs.md` formats.
- Example test `POST /api/SeatsInBusTrip/bulkByTrip`:
  ```json
  {
    "tripId": 1,
    "seatNumbers": ["A3", "A4", "A5"]
  }
  ```

### 6.3. Project Extension

- Add new APIs (e.g., `/api/Reports/detailed`) in future days.
- Develop frontend in Phase 4.

---

## 7. Development Roadmap

- **Day 6**: Enhance security (refresh token, password strength).
- **Day 7-15**: Frontend development and deployment (if required).

---

## 8. Version History

- **Version 1.0 (Day 5, July 02, 2025)**: Completed all backend APIs, including authentication.

---

## 9. Sample Data

```sql
INSERT INTO Buses (BusId, BusCode, BusNumber, TypeId, TotalSeats)
VALUES
    (1, '10000', 'NY-12345', 1, 40);

INSERT INTO Routes (RouteId, RouteCode, StartLocation, EndLocation, Distance, BasePrice)
VALUES
    (1, 'NY-BOS', 'New York, NY', 'Boston, MA', 215.0, 50.00),
    (2, 'BOS-NY', 'Boston, MA', 'New York, NY', 215.0, 50.00);

INSERT INTO BusTypes (TypeId, TypeName, PriceMultiplier)
VALUES
    (1, 'Express', 1.0);

INSERT INTO BusTrips (TripId, BusId, RouteId, DepartureTime, AvailableSeats, Status)
VALUES
    (1, 1, 1, '2025-07-03 08:00:00', 38, 'Scheduled'),
    (2, 1, 2, '2025-07-03 16:00:00', 38, 'Scheduled');

INSERT INTO Employees (EmployeeId, Username, PasswordHash, Role, Name, Email, PhoneNumber, Qualifications, Age, Location)
VALUES
    (1, 'admin.doe', '$2a$10$Xo5Fz1u3w...hashed_password...', 'Admin', 'Admin Doe', 'admin@example.com', '212-555-0000', 'Management', 35, 'New York, NY'),
    (2, 'employee.doe', '$2a$10$Xo5Fz1u3w...hashed_password...', 'Employee', 'Employee Doe', 'employee@example.com', '212-555-0001', 'Ticketing', 30, 'Boston, MA');

INSERT INTO Bookings (BookingId, EmployeeId, BookingDate, TotalAmount, TotalTax, Status)
VALUES
    (1, 1, '2025-07-01 10:00:00', 100.00, 10.00, 'Confirmed');

INSERT INTO Customers (CustomerId, Name, DateOfBirth, Email, PhoneNumber)
VALUES
    (1, 'Alice Johnson', '1990-05-15', 'alice.johnson@example.com', '212-555-1234'),
    (2, 'Bob Williams', '1985-08-20', 'bob.williams@example.com', '212-555-5678');

INSERT INTO BookingDetails (BookingDetailId, BookingId, SeatInTripId, CustomerId, TicketPrice, TicketTax)
VALUES
    (1, 1, 1, 1, 50.00, 5.00),
    (2, 1, 3, 2, 50.00, 5.00);

INSERT INTO SeatsInBusTrip (SeatInTripId, TripId, SeatNumber, IsAvailable)
VALUES
    (1, 1, 'A1', false),
    (2, 1, 'A2', true),
    (3, 2, 'A1', false),
    (4, 2, 'A2', true);

INSERT INTO AgeBasedDiscounts (DiscountId, MinAge, MaxAge, DiscountPercentage)
VALUES
    (1, 0, 12, 20),
    (2, 13, 64, 0),
    (3, 65, 120, 15);

INSERT INTO CancellationRules (RuleId, DaysBeforeDeparture, PenaltyPercentage)
VALUES
    (1, 0, 50),
    (2, 1, 30),
    (3, 2, 0);
```

**Note**: Replace `hashed_password` with BCrypt-hashed passwords (e.g., hash of "password123").
