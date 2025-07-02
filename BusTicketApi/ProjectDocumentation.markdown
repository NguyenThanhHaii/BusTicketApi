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
- **Development Tools**: Visual Studio 2022 or Visual Studio Code or JetBrains Rider 2025.
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

4. **Run Migration**:

   - Ensure `BusTicketDbContext` is configured in `Program.cs` (already included). Verify the following line exists:
     ```csharp
     builder.Services.AddDbContext<BusTicketDbContext>(options =>
         options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
     ```
   - Create a migration to initialize the database schema:
     ```bash
     dotnet ef migrations add InitialCreate --project BusTicketApi.csproj --output-dir Data/Migrations
     ```
     - **Note**: This command generates migration files in the `Data/Migrations` folder. If the folder does not exist, it will be created.
   - Apply the migration to create the database and tables:
     ```bash
     dotnet ef database update --project BusTicketApi.csproj
     ```
     - **Note**: This command applies the migration to the database specified in the connection string. Ensure the database server is running and accessible.

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
│   ├── Migrations/
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
      "username": "admin.doe",
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
-- BusTypes (Loại xe)
INSERT INTO BusTypes (TypeId, TypeName, PriceMultiplier)
VALUES
    (1, 'Express', 1.0m),
    (2, 'Luxury', 1.2m),
    (3, 'Volvo Non-A/C', 1.4m),
    (4, 'Volvo A/C', 1.6m);

-- AgeBasedDiscounts (Chiết khấu theo độ tuổi)
INSERT INTO AgeBasedDiscounts (DiscountId, MinAge, MaxAge, DiscountPercentage, Description)
VALUES
    (1, 0, 4, 100, 'Free for children under 5'),
    (2, 5, 12, 50, '50% off for children 5-12'),
    (3, 13, 50, 0, 'Full price for adults'),
    (4, 51, 150, 30, '30% off for seniors over 50');

-- Routes (Tuyến đường)
INSERT INTO Routes (RouteId, RouteCode, StartLocation, EndLocation, Distance, BasePrice)
VALUES
    (1, 'R001', 'New York, NY', 'Boston, MA', 215.0m, 50.00m),
    (2, 'R002', 'Los Angeles, CA', 'San Francisco, CA', 380.0m, 80.00m),
    (3, 'R003', 'Chicago, IL', 'Denver, CO', 1000.0m, 120.00m),
    (4, 'R004', 'Houston, TX', 'Austin, TX', 200.0m, 40.00m),
    (5, 'R005', 'Seattle, WA', 'Portland, OR', 180.0m, 45.00m);

-- RouteStops (Trạm dừng)
INSERT INTO RouteStops (RouteStopId, RouteId, StopLocation, StopOrder)
VALUES
    (1, 1, 'Hartford, CT', 1),
    (2, 1, 'Providence, RI', 2),
    (3, 2, 'Santa Barbara, CA', 1),
    (4, 2, 'Monterey, CA', 2),
    (5, 3, 'Omaha, NE', 1),
    (6, 3, 'Cheyenne, WY', 2),
    (7, 4, 'San Antonio, TX', 1),
    (8, 4, 'Waco, TX', 2),
    (9, 5, 'Tacoma, WA', 1),
    (10, 5, 'Salem, OR', 2);

-- Buses (Xe buýt)
INSERT INTO Buses (BusId, BusCode, BusNumber, TypeId, TotalSeats)
VALUES
    (1, '10001', 'NY123', 1, 40), -- 40 ghế
    (2, '10002', 'CA456', 4, 30); -- 30 ghế

-- BusTrips (Chuyến xe)
INSERT INTO BusTrips (TripId, BusId, RouteId, DepartureTime, ArrivalTime, AvailableSeats, Status)
VALUES
    (1, 1, 1, '2025-07-03 08:00:00', '2025-07-03 12:00:00', 40, 'Scheduled'),
    (2, 1, 2, '2025-07-03 14:00:00', '2025-07-03 20:00:00', 40, 'Scheduled'),
    (3, 2, 3, '2025-07-03 09:00:00', '2025-07-03 19:00:00', 30, 'Scheduled'),
    (4, 2, 4, '2025-07-03 10:00:00', '2025-07-03 12:00:00', 30, 'Scheduled'),
    (5, 1, 5, '2025-07-03 15:00:00', '2025-07-03 17:00:00', 40, 'Scheduled');

-- SeatsInBusTrip (Ghế trên chuyến) - Tạo đủ 40 ghế cho TripId 1, 2, 5 và 30 ghế cho TripId 3, 4
INSERT INTO SeatsInBusTrip (SeatInTripId, TripId, SeatNumber, IsAvailable)
-- TripId = 1 (40 ghế)
VALUES
    (1, 1, 'A1', true), (2, 1, 'A2', true), (3, 1, 'A3', true), (4, 1, 'A4', true),
    (5, 1, 'A5', true), (6, 1, 'A6', true), (7, 1, 'A7', true), (8, 1, 'A8', true),
    (9, 1, 'A9', true), (10, 1, 'A10', true), (11, 1, 'B1', true), (12, 1, 'B2', true),
    (13, 1, 'B3', true), (14, 1, 'B4', true), (15, 1, 'B5', true), (16, 1, 'B6', true),
    (17, 1, 'B7', true), (18, 1, 'B8', true), (19, 1, 'B9', true), (20, 1, 'B10', true),
    (21, 1, 'C1', true), (22, 1, 'C2', true), (23, 1, 'C3', true), (24, 1, 'C4', true),
    (25, 1, 'C5', true), (26, 1, 'C6', true), (27, 1, 'C7', true), (28, 1, 'C8', true),
    (29, 1, 'C9', true), (30, 1, 'C10', true), (31, 1, 'D1', true), (32, 1, 'D2', true),
    (33, 1, 'D3', true), (34, 1, 'D4', true), (35, 1, 'D5', true), (36, 1, 'D6', true),
    (37, 1, 'D7', true), (38, 1, 'D8', true), (39, 1, 'D9', true), (40, 1, 'D10', true);

-- TripId = 2 (40 ghế)
INSERT INTO SeatsInBusTrip (SeatInTripId, TripId, SeatNumber, IsAvailable)
VALUES
    (41, 2, 'A1', true), (42, 2, 'A2', true), (43, 2, 'A3', true), (44, 2, 'A4', true),
    (45, 2, 'A5', true), (46, 2, 'A6', true), (47, 2, 'A7', true), (48, 2, 'A8', true),
    (49, 2, 'A9', true), (50, 2, 'A10', true), (51, 2, 'B1', true), (52, 2, 'B2', true),
    (53, 2, 'B3', true), (54, 2, 'B4', true), (55, 2, 'B5', true), (56, 2, 'B6', true),
    (57, 2, 'B7', true), (58, 2, 'B8', true), (59, 2, 'B9', true), (60, 2, 'B10', true),
    (61, 2, 'C1', true), (62, 2, 'C2', true), (63, 2, 'C3', true), (64, 2, 'C4', true),
    (65, 2, 'C5', true), (66, 2, 'C6', true), (67, 2, 'C7', true), (68, 2, 'C8', true),
    (69, 2, 'C9', true), (70, 2, 'C10', true), (71, 2, 'D1', true), (72, 2, 'D2', true),
    (73, 2, 'D3', true), (74, 2, 'D4', true), (75, 2, 'D5', true), (76, 2, 'D6', true),
    (77, 2, 'D7', true), (78, 2, 'D8', true), (79, 2, 'D9', true), (80, 2, 'D10', true);

-- TripId = 3 (30 ghế)
INSERT INTO SeatsInBusTrip (SeatInTripId, TripId, SeatNumber, IsAvailable)
VALUES
    (81, 3, 'A1', true), (82, 3, 'A2', true), (83, 3, 'A3', true), (84, 3, 'A4', true),
    (85, 3, 'A5', true), (86, 3, 'A6', true), (87, 3, 'A7', true), (88, 3, 'A8', true),
    (89, 3, 'A9', true), (90, 3, 'A10', true), (91, 3, 'B1', true), (92, 3, 'B2', true),
    (93, 3, 'B3', true), (94, 3, 'B4', true), (95, 3, 'B5', true), (96, 3, 'B6', true),
    (97, 3, 'B7', true), (98, 3, 'B8', true), (99, 3, 'B9', true), (100, 3, 'B10', true),
    (101, 3, 'C1', true), (102, 3, 'C2', true), (103, 3, 'C3', true), (104, 3, 'C4', true),
    (105, 3, 'C5', true), (106, 3, 'C6', true), (107, 3, 'C7', true), (108, 3, 'C8', true),
    (109, 3, 'C9', true), (110, 3, 'C10', true);

-- TripId = 4 (30 ghế)
INSERT INTO SeatsInBusTrip (SeatInTripId, TripId, SeatNumber, IsAvailable)
VALUES
    (111, 4, 'A1', true), (112, 4, 'A2', true), (113, 4, 'A3', true), (114, 4, 'A4', true),
    (115, 4, 'A5', true), (116, 4, 'A6', true), (117, 4, 'A7', true), (118, 4, 'A8', true),
    (119, 4, 'A9', true), (120, 4, 'A10', true), (121, 4, 'B1', true), (122, 4, 'B2', true),
    (123, 4, 'B3', true), (124, 4, 'B4', true), (125, 4, 'B5', true), (126, 4, 'B6', true),
    (127, 4, 'B7', true), (128, 4, 'B8', true), (129, 4, 'B9', true), (130, 4, 'B10', true),
    (131, 4, 'C1', true), (132, 4, 'C2', true), (133, 4, 'C3', true), (134, 4, 'C4', true),
    (135, 4, 'C5', true), (136, 4, 'C6', true), (137, 4, 'C7', true), (138, 4, 'C8', true),
    (139, 4, 'C9', true), (140, 4, 'C10', true);

-- TripId = 5 (40 ghế)
INSERT INTO SeatsInBusTrip (SeatInTripId, TripId, SeatNumber, IsAvailable)
VALUES
    (141, 5, 'A1', true), (142, 5, 'A2', true), (143, 5, 'A3', true), (144, 5, 'A4', true),
    (145, 5, 'A5', true), (146, 5, 'A6', true), (147, 5, 'A7', true), (148, 5, 'A8', true),
    (149, 5, 'A9', true), (150, 5, 'A10', true), (151, 5, 'B1', true), (152, 5, 'B2', true),
    (153, 5, 'B3', true), (154, 5, 'B4', true), (155, 5, 'B5', true), (156, 5, 'B6', true),
    (157, 5, 'B7', true), (158, 5, 'B8', true), (159, 5, 'B9', true), (160, 5, 'B10', true),
    (161, 5, 'C1', true), (162, 5, 'C2', true), (163, 5, 'C3', true), (164, 5, 'C4', true),
    (165, 5, 'C5', true), (166, 5, 'C6', true), (167, 5, 'C7', true), (168, 5, 'C8', true),
    (169, 5, 'C9', true), (170, 5, 'C10', true), (171, 5, 'D1', true), (172, 5, 'D2', true),
    (173, 5, 'D3', true), (174, 5, 'D4', true), (175, 5, 'D5', true), (176, 5, 'D6', true),
    (177, 5, 'D7', true), (178, 5, 'D8', true), (179, 5, 'D9', true), (180, 5, 'D10', true);

-- Employees (Nhân viên)
INSERT INTO Employees (EmployeeId, Username, PasswordHash, Role, Name, Email, PhoneNumber, Qualifications, Age, Location)
VALUES
    (100000, 'admin', '$2a$12$GquGNXIqWkN9fFKR.OmDi.o3DkbahSX2MkVjf..hdeEeBPH6qSO66', 'Admin', 'Super Admin', 'admin@example.com', '001-555-1111', 'Bachelor’s Degree', 48, 'New York, NY'),
    (100001, 'john.doe', '$2a$12$yv5t3yIkt3F9hYGUwiOKe.xFAapbty9TLngpxeQ0KmlbpBYj30Isu', 'Admin', 'John Doe', 'john.doe@example.com', '212-555-1234', 'Bachelor’s Degree', 35, 'New York, NY'),
    (100002, 'jane.smith', '$2a$12$4FleyqJI7PgWsDZ/E1Gf3eocMPjVfQiuZX6ONVx9GKXiSy8JI5AJW', 'Employee', 'Jane Smith', 'jane.smith@example.com', '415-555-5678', 'Associate Degree', 28, 'San Francisco, CA'),
    (100003, 'mike.jones', '$2a$12$8kP9m2nQ3rL6vT8J4yZ9u.eF7pQ8vX2MkYjf..hdeEeBPH6qSO66', 'Employee', 'Mike Jones', 'mike.jones@example.com', '213-555-9012', 'High School Diploma', 32, 'Los Angeles, CA'),
    (100004, 'sarah.lee', '$2a$12$3jK7pL9mQ2rT4vH8yZ6u.eF7pQ8vX2MkYjf..hdeEeBPH6qSO66', 'Employee', 'Sarah Lee', 'sarah.lee@example.com', '206-555-3456', 'Bachelor’s Degree', 40, 'Seattle, WA'),
    (100005, 'david.brown', '$2a$12$9kL3pM7nQ4rT6vJ8yZ8u.eF7pQ8vX2MkYjf..hdeEeBPH6qSO66', 'Employee', 'David Brown', 'david.brown@example.com', '303-555-6789', 'Associate Degree', 29, 'Denver, CO');

-- Customers (Khách hàng)
INSERT INTO Customers (CustomerId, Name, DateOfBirth, Email, PhoneNumber)
VALUES
    (1, 'Alice Johnson', '1990-05-15', 'alice.johnson@example.com', '617-555-9012'),
    (2, 'Bob Williams', '2000-08-22', 'bob.williams@example.com', '213-555-3456'),
    (3, 'Charlie Brown', '1995-03-10', 'charlie.brown@example.com', '415-555-6789'),
    (4, 'Dana White', '1988-11-25', 'dana.white@example.com', '206-555-1234'),
    (5, 'Evan Davis', '1992-07-12', 'evan.davis@example.com', '312-555-5678'),
    (6, 'Fiona Green', '1985-12-30', 'fiona.green@example.com', '214-555-9012');

-- Bookings (Đặt vé)
INSERT INTO Bookings (BookingId, EmployeeId, BookingDate, TotalAmount, TotalTax, Status, CancellationDate, RefundAmount)
VALUES
    (1, 100000, '2025-07-02 10:00:00', 50.00m, 5.00m, 'Confirmed', null, null),
    (2, 100001, '2025-07-02 11:00:00', 80.00m, 8.00m, 'Confirmed', null, null),
    (3, 100002, '2025-07-02 14:00:00', 140.00m, 14.00m, 'Cancelled', '2025-07-01 12:00:00', 98.00m),
    (4, 100003, '2025-07-03 09:30:00', 210.00m, 21.00m, 'Confirmed', null, null),
    (5, 100004, '2025-07-03 15:00:00', 45.00m, 4.50m, 'Confirmed', null, null),
    (6, 100005, '2025-07-03 16:00:00', 120.00m, 12.00m, 'Confirmed', null, null);

-- BookingDetails (Chi tiết đặt vé)
INSERT INTO BookingDetails (BookingDetailId, BookingId, SeatInTripId, CustomerId, TicketPrice, TicketTax)
VALUES
    (1, 1, 1, 1, 50.00m, 5.00m),    -- Alice, Trip 1, Seat A1
    (2, 2, 41, 2, 80.00m, 8.00m),   -- Bob, Trip 2, Seat A1
    (3, 3, 81, 3, 140.00m, 14.00m), -- Charlie, Trip 3, Seat A1 (hủy)
    (4, 4, 111, 4, 210.00m, 21.00m), -- Dana, Trip 4, Seat A1
    (5, 5, 141, 5, 45.00m, 4.50m),  -- Evan, Trip 5, Seat A1
    (6, 6, 2, 6, 120.00m, 12.00m);  -- Fiona, Trip 1, Seat A2

-- CancellationRules (Quy tắc hủy vé)
INSERT INTO CancellationRules (RuleId, DaysBeforeDeparture, PenaltyPercentage, Description)
VALUES
    (1, 2, 0, 'Full refund for cancellations 2+ days before departure'),
    (2, 1, 15, '15% penalty for cancellations 1 day before departure'),
    (3, 0, 30, '30% penalty for cancellations on departure day');
```

**Note**: Replace `hashed_password` with BCrypt-hashed passwords (e.g., hash of "password123").
