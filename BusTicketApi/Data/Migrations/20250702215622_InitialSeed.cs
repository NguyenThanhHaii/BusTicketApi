using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusTicketApi.Data.Migrations
{
    public partial class InitialSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AgeBasedDiscounts",
                columns: table => new
                {
                    DiscountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MinAge = table.Column<int>(type: "int", nullable: false),
                    MaxAge = table.Column<int>(type: "int", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgeBasedDiscounts", x => x.DiscountId);
                    table.CheckConstraint("CK_AgeBasedDiscount_DiscountPercentage", "DiscountPercentage >= 0 AND DiscountPercentage <= 100");
                    table.CheckConstraint("CK_AgeBasedDiscount_MaxAge", "MaxAge >= MinAge");
                    table.CheckConstraint("CK_AgeBasedDiscount_MinAge", "MinAge >= 0");
                });

            migrationBuilder.CreateTable(
                name: "BusTypes",
                columns: table => new
                {
                    TypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PriceMultiplier = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusTypes", x => x.TypeId);
                    table.CheckConstraint("CK_BusType_PriceMultiplier", "PriceMultiplier >= 1.0");
                });

            migrationBuilder.CreateTable(
                name: "CancellationRules",
                columns: table => new
                {
                    RuleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DaysBeforeDeparture = table.Column<int>(type: "int", nullable: false),
                    PenaltyPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CancellationRules", x => x.RuleId);
                    table.CheckConstraint("CK_CancellationRule_DaysBeforeDeparture", "DaysBeforeDeparture >= 0");
                    table.CheckConstraint("CK_CancellationRule_PenaltyPercentage", "PenaltyPercentage >= 0 AND PenaltyPercentage <= 100");
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Qualifications = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Age = table.Column<int>(type: "int", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeId);
                    table.CheckConstraint("CK_Employee_Age", "Age >= 18");
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    RouteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RouteCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StartLocation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EndLocation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Distance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.RouteId);
                    table.CheckConstraint("CK_Route_BasePrice", "BasePrice > 0");
                    table.CheckConstraint("CK_Route_Distance", "Distance > 0");
                });

            migrationBuilder.CreateTable(
                name: "Buses",
                columns: table => new
                {
                    BusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BusNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TypeId = table.Column<int>(type: "int", nullable: false),
                    TotalSeats = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buses", x => x.BusId);
                    table.CheckConstraint("CK_Bus_BusCode", "LEN(BusCode) = 5 AND BusCode NOT LIKE '%[^0-9]%'");
                    table.CheckConstraint("CK_Bus_TotalSeats", "TotalSeats > 0");
                    table.ForeignKey(
                        name: "FK_Buses_BusTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "BusTypes",
                        principalColumn: "TypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    BookingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalTax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CancellationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RefundAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.BookingId);
                    table.CheckConstraint("CK_Booking_RefundAmount", "RefundAmount >= 0");
                    table.CheckConstraint("CK_Booking_TotalAmount", "TotalAmount >= 0");
                    table.CheckConstraint("CK_Booking_TotalTax", "TotalTax >= 0");
                    table.ForeignKey(
                        name: "FK_Bookings_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RouteStops",
                columns: table => new
                {
                    RouteStopId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RouteId = table.Column<int>(type: "int", nullable: false),
                    StopLocation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StopOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteStops", x => x.RouteStopId);
                    table.CheckConstraint("CK_RouteStop_StopOrder", "StopOrder >= 1");
                    table.ForeignKey(
                        name: "FK_RouteStops_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "RouteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusTrips",
                columns: table => new
                {
                    TripId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusId = table.Column<int>(type: "int", nullable: false),
                    RouteId = table.Column<int>(type: "int", nullable: false),
                    DepartureTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArrivalTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AvailableSeats = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusTrips", x => x.TripId);
                    table.CheckConstraint("CK_BusTrip_AvailableSeats", "AvailableSeats >= 0");
                    table.ForeignKey(
                        name: "FK_BusTrips_Buses_BusId",
                        column: x => x.BusId,
                        principalTable: "Buses",
                        principalColumn: "BusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BusTrips_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "RouteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeatsInBusTrip",
                columns: table => new
                {
                    SeatInTripId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TripId = table.Column<int>(type: "int", nullable: false),
                    SeatNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeatsInBusTrip", x => x.SeatInTripId);
                    table.ForeignKey(
                        name: "FK_SeatsInBusTrip_BusTrips_TripId",
                        column: x => x.TripId,
                        principalTable: "BusTrips",
                        principalColumn: "TripId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingDetails",
                columns: table => new
                {
                    BookingDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    SeatInTripId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    TicketPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TicketTax = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingDetails", x => x.BookingDetailId);
                    table.CheckConstraint("CK_BookingDetail_TicketPrice", "TicketPrice >= 0");
                    table.CheckConstraint("CK_BookingDetail_TicketTax", "TicketTax >= 0");
                    table.ForeignKey(
                        name: "FK_BookingDetails_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingDetails_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingDetails_SeatsInBusTrip_SeatInTripId",
                        column: x => x.SeatInTripId,
                        principalTable: "SeatsInBusTrip",
                        principalColumn: "SeatInTripId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AgeBasedDiscounts",
                columns: new[] { "DiscountId", "Description", "DiscountPercentage", "MaxAge", "MinAge" },
                values: new object[,]
                {
                    { 1, "Free for children under 5", 100m, 4, 0 },
                    { 2, "50% off for children 5-12", 50m, 12, 5 },
                    { 3, "Full price for adults", 0m, 50, 13 },
                    { 4, "30% off for seniors over 50", 30m, 150, 51 }
                });

            migrationBuilder.InsertData(
                table: "BusTypes",
                columns: new[] { "TypeId", "PriceMultiplier", "TypeName" },
                values: new object[,]
                {
                    { 1, 1.0m, "Express" },
                    { 2, 1.2m, "Luxury" },
                    { 3, 1.4m, "Volvo Non-A/C" },
                    { 4, 1.6m, "Volvo A/C" }
                });

            migrationBuilder.InsertData(
                table: "CancellationRules",
                columns: new[] { "RuleId", "DaysBeforeDeparture", "Description", "PenaltyPercentage" },
                values: new object[,]
                {
                    { 1, 2, "Full refund for cancellations 2+ days before departure", 0m },
                    { 2, 1, "15% penalty for cancellations 1 day before departure", 15m },
                    { 3, 0, "30% penalty for cancellations on departure day", 30m }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "DateOfBirth", "Email", "Name", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, new DateTime(1990, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "alice.johnson@example.com", "Alice Johnson", "617-555-9012" },
                    { 2, new DateTime(2000, 8, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "bob.williams@example.com", "Bob Williams", "213-555-3456" },
                    { 3, new DateTime(1995, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "charlie.brown@example.com", "Charlie Brown", "415-555-6789" },
                    { 4, new DateTime(1988, 11, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "dana.white@example.com", "Dana White", "206-555-1234" },
                    { 5, new DateTime(1992, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "evan.davis@example.com", "Evan Davis", "312-555-5678" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "EmployeeId", "Age", "Email", "Location", "Name", "PasswordHash", "PhoneNumber", "Qualifications", "Role", "Username" },
                values: new object[,]
                {
                    { 100000, 48, "admin@example.com", "New York, NY", "Super Admin", "$2a$12$GquGNXIqWkN9fFKR.OmDi.o3DkbahSX2MkVjf..hdeEeBPH6qSO66", "001-555-1111", "Bachelor’s Degree", "Admin", "admin" },
                    { 100001, 35, "john.doe@example.com", "New York, NY", "John Doe", "$2a$12$yv5t3yIkt3F9hYGUwiOKe.xFAapbty9TLngpxeQ0KmlbpBYj30Isu", "212-555-1234", "Bachelor’s Degree", "Admin", "john.doe" },
                    { 100002, 28, "jane.smith@example.com", "San Francisco, CA", "Jane Smith", "$2a$12$4FleyqJI7PgWsDZ/E1Gf3eocMPjVfQiuZX6ONVx9GKXiSy8JI5AJW", "415-555-5678", "Associate Degree", "Employee", "jane.smith" },
                    { 100003, 32, "mike.jones@example.com", "Los Angeles, CA", "Mike Jones", "$2a$12$8kP9m2nQ3rL6vT8J4yZ9u.eF7pQ8vX2MkYjf..hdeEeBPH6qSO66", "213-555-9012", "High School Diploma", "Employee", "mike.jones" },
                    { 100004, 40, "sarah.lee@example.com", "Seattle, WA", "Sarah Lee", "$2a$12$3jK7pL9mQ2rT4vH8yZ6u.eF7pQ8vX2MkYjf..hdeEeBPH6qSO66", "206-555-3456", "Bachelor’s Degree", "Employee", "sarah.lee" }
                });

            migrationBuilder.InsertData(
                table: "Routes",
                columns: new[] { "RouteId", "BasePrice", "Distance", "EndLocation", "RouteCode", "StartLocation" },
                values: new object[,]
                {
                    { 1, 50.00m, 215.0m, "Boston, MA", "R001", "New York, NY" },
                    { 2, 80.00m, 380.0m, "San Francisco, CA", "R002", "Los Angeles, CA" },
                    { 3, 120.00m, 1000.0m, "Denver, CO", "R003", "Chicago, IL" },
                    { 4, 40.00m, 200.0m, "Austin, TX", "R004", "Houston, TX" },
                    { 5, 45.00m, 180.0m, "Portland, OR", "R005", "Seattle, WA" }
                });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "BookingId", "BookingDate", "CancellationDate", "EmployeeId", "RefundAmount", "Status", "TotalAmount", "TotalTax" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 7, 2, 10, 0, 0, 0, DateTimeKind.Unspecified), null, 100000, null, "Confirmed", 50.00m, 5.00m },
                    { 2, new DateTime(2025, 7, 2, 11, 0, 0, 0, DateTimeKind.Unspecified), null, 100001, null, "Confirmed", 80.00m, 8.00m },
                    { 3, new DateTime(2025, 7, 2, 14, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 7, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), 100002, 98.00m, "Cancelled", 140.00m, 14.00m },
                    { 4, new DateTime(2025, 7, 3, 9, 30, 0, 0, DateTimeKind.Unspecified), null, 100003, null, "Confirmed", 210.00m, 21.00m },
                    { 5, new DateTime(2025, 7, 3, 15, 0, 0, 0, DateTimeKind.Unspecified), null, 100004, null, "Confirmed", 45.00m, 4.50m }
                });

            migrationBuilder.InsertData(
                table: "Buses",
                columns: new[] { "BusId", "BusCode", "BusNumber", "TotalSeats", "TypeId" },
                values: new object[,]
                {
                    { 1, "10001", "NY123", 40, 1 },
                    { 2, "10002", "CA456", 30, 4 }
                });

            migrationBuilder.InsertData(
                table: "RouteStops",
                columns: new[] { "RouteStopId", "RouteId", "StopLocation", "StopOrder" },
                values: new object[,]
                {
                    { 1, 1, "Hartford, CT", 1 },
                    { 2, 1, "Providence, RI", 2 },
                    { 3, 2, "Santa Barbara, CA", 1 },
                    { 4, 2, "Monterey, CA", 2 },
                    { 5, 3, "Omaha, NE", 1 },
                    { 6, 3, "Cheyenne, WY", 2 },
                    { 7, 4, "San Antonio, TX", 1 },
                    { 8, 4, "Waco, TX", 2 },
                    { 9, 5, "Tacoma, WA", 1 },
                    { 10, 5, "Salem, OR", 2 }
                });

            migrationBuilder.InsertData(
                table: "BusTrips",
                columns: new[] { "TripId", "ArrivalTime", "AvailableSeats", "BusId", "DepartureTime", "RouteId", "Status" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 7, 3, 12, 0, 0, 0, DateTimeKind.Unspecified), 40, 1, new DateTime(2025, 7, 3, 8, 0, 0, 0, DateTimeKind.Unspecified), 1, "Scheduled" },
                    { 2, new DateTime(2025, 7, 3, 20, 0, 0, 0, DateTimeKind.Unspecified), 40, 1, new DateTime(2025, 7, 3, 14, 0, 0, 0, DateTimeKind.Unspecified), 2, "Scheduled" },
                    { 3, new DateTime(2025, 7, 3, 19, 0, 0, 0, DateTimeKind.Unspecified), 30, 2, new DateTime(2025, 7, 3, 9, 0, 0, 0, DateTimeKind.Unspecified), 3, "Scheduled" },
                    { 4, new DateTime(2025, 7, 3, 12, 0, 0, 0, DateTimeKind.Unspecified), 30, 2, new DateTime(2025, 7, 3, 10, 0, 0, 0, DateTimeKind.Unspecified), 4, "Scheduled" },
                    { 5, new DateTime(2025, 7, 3, 17, 0, 0, 0, DateTimeKind.Unspecified), 40, 1, new DateTime(2025, 7, 3, 15, 0, 0, 0, DateTimeKind.Unspecified), 5, "Scheduled" }
                });

            migrationBuilder.InsertData(
                table: "SeatsInBusTrip",
                columns: new[] { "SeatInTripId", "IsAvailable", "SeatNumber", "TripId" },
                values: new object[,]
                {
                    { 1, true, "A1", 1 },
                    { 2, true, "A2", 1 },
                    { 3, true, "A3", 1 },
                    { 4, true, "A4", 1 },
                    { 5, true, "A5", 1 },
                    { 6, true, "A6", 1 },
                    { 7, true, "A7", 1 },
                    { 8, true, "A8", 1 },
                    { 9, true, "A9", 1 },
                    { 10, true, "A10", 1 },
                    { 11, true, "B1", 1 },
                    { 12, true, "B2", 1 },
                    { 13, true, "B3", 1 },
                    { 14, true, "B4", 1 },
                    { 15, true, "B5", 1 },
                    { 16, true, "B6", 1 },
                    { 17, true, "B7", 1 },
                    { 18, true, "B8", 1 },
                    { 19, true, "B9", 1 },
                    { 20, true, "B10", 1 },
                    { 21, true, "C1", 1 },
                    { 22, true, "C2", 1 },
                    { 23, true, "C3", 1 },
                    { 24, true, "C4", 1 },
                    { 25, true, "C5", 1 },
                    { 26, true, "C6", 1 },
                    { 27, true, "C7", 1 },
                    { 28, true, "C8", 1 },
                    { 29, true, "C9", 1 },
                    { 30, true, "C10", 1 },
                    { 31, true, "D1", 1 },
                    { 32, true, "D2", 1 },
                    { 33, true, "D3", 1 },
                    { 34, true, "D4", 1 },
                    { 35, true, "D5", 1 },
                    { 36, true, "D6", 1 },
                    { 37, true, "D7", 1 },
                    { 38, true, "D8", 1 },
                    { 39, true, "D9", 1 },
                    { 40, true, "D10", 1 },
                    { 41, true, "A1", 2 },
                    { 42, true, "A2", 2 }
                });

            migrationBuilder.InsertData(
                table: "SeatsInBusTrip",
                columns: new[] { "SeatInTripId", "IsAvailable", "SeatNumber", "TripId" },
                values: new object[,]
                {
                    { 43, true, "A3", 2 },
                    { 44, true, "A4", 2 },
                    { 45, true, "A5", 2 },
                    { 46, true, "A6", 2 },
                    { 47, true, "A7", 2 },
                    { 48, true, "A8", 2 },
                    { 49, true, "A9", 2 },
                    { 50, true, "A10", 2 },
                    { 51, true, "B1", 2 },
                    { 52, true, "B2", 2 },
                    { 53, true, "B3", 2 },
                    { 54, true, "B4", 2 },
                    { 55, true, "B5", 2 },
                    { 56, true, "B6", 2 },
                    { 57, true, "B7", 2 },
                    { 58, true, "B8", 2 },
                    { 59, true, "B9", 2 },
                    { 60, true, "B10", 2 },
                    { 61, true, "C1", 2 },
                    { 62, true, "C2", 2 },
                    { 63, true, "C3", 2 },
                    { 64, true, "C4", 2 },
                    { 65, true, "C5", 2 },
                    { 66, true, "C6", 2 },
                    { 67, true, "C7", 2 },
                    { 68, true, "C8", 2 },
                    { 69, true, "C9", 2 },
                    { 70, true, "C10", 2 },
                    { 71, true, "D1", 2 },
                    { 72, true, "D2", 2 },
                    { 73, true, "D3", 2 },
                    { 74, true, "D4", 2 },
                    { 75, true, "D5", 2 },
                    { 76, true, "D6", 2 },
                    { 77, true, "D7", 2 },
                    { 78, true, "D8", 2 },
                    { 79, true, "D9", 2 },
                    { 80, true, "D10", 2 },
                    { 81, true, "A1", 3 },
                    { 82, true, "A2", 3 },
                    { 83, true, "A3", 3 },
                    { 84, true, "A4", 3 }
                });

            migrationBuilder.InsertData(
                table: "SeatsInBusTrip",
                columns: new[] { "SeatInTripId", "IsAvailable", "SeatNumber", "TripId" },
                values: new object[,]
                {
                    { 85, true, "A5", 3 },
                    { 86, true, "A6", 3 },
                    { 87, true, "A7", 3 },
                    { 88, true, "A8", 3 },
                    { 89, true, "A9", 3 },
                    { 90, true, "A10", 3 },
                    { 91, true, "B1", 3 },
                    { 92, true, "B2", 3 },
                    { 93, true, "B3", 3 },
                    { 94, true, "B4", 3 },
                    { 95, true, "B5", 3 },
                    { 96, true, "B6", 3 },
                    { 97, true, "B7", 3 },
                    { 98, true, "B8", 3 },
                    { 99, true, "B9", 3 },
                    { 100, true, "B10", 3 },
                    { 101, true, "C1", 3 },
                    { 102, true, "C2", 3 },
                    { 103, true, "C3", 3 },
                    { 104, true, "C4", 3 },
                    { 105, true, "C5", 3 },
                    { 106, true, "C6", 3 },
                    { 107, true, "C7", 3 },
                    { 108, true, "C8", 3 },
                    { 109, true, "C9", 3 },
                    { 110, true, "C10", 3 },
                    { 111, true, "A1", 4 },
                    { 112, true, "A2", 4 },
                    { 113, true, "A3", 4 },
                    { 114, true, "A4", 4 },
                    { 115, true, "A5", 4 },
                    { 116, true, "A6", 4 },
                    { 117, true, "A7", 4 },
                    { 118, true, "A8", 4 },
                    { 119, true, "A9", 4 },
                    { 120, true, "A10", 4 },
                    { 121, true, "B1", 4 },
                    { 122, true, "B2", 4 },
                    { 123, true, "B3", 4 },
                    { 124, true, "B4", 4 },
                    { 125, true, "B5", 4 },
                    { 126, true, "B6", 4 }
                });

            migrationBuilder.InsertData(
                table: "SeatsInBusTrip",
                columns: new[] { "SeatInTripId", "IsAvailable", "SeatNumber", "TripId" },
                values: new object[,]
                {
                    { 127, true, "B7", 4 },
                    { 128, true, "B8", 4 },
                    { 129, true, "B9", 4 },
                    { 130, true, "B10", 4 },
                    { 131, true, "C1", 4 },
                    { 132, true, "C2", 4 },
                    { 133, true, "C3", 4 },
                    { 134, true, "C4", 4 },
                    { 135, true, "C5", 4 },
                    { 136, true, "C6", 4 },
                    { 137, true, "C7", 4 },
                    { 138, true, "C8", 4 },
                    { 139, true, "C9", 4 },
                    { 140, true, "C10", 4 },
                    { 141, true, "A1", 5 },
                    { 142, true, "A2", 5 },
                    { 143, true, "A3", 5 },
                    { 144, true, "A4", 5 },
                    { 145, true, "A5", 5 },
                    { 146, true, "A6", 5 },
                    { 147, true, "A7", 5 },
                    { 148, true, "A8", 5 },
                    { 149, true, "A9", 5 },
                    { 150, true, "A10", 5 },
                    { 151, true, "B1", 5 },
                    { 152, true, "B2", 5 },
                    { 153, true, "B3", 5 },
                    { 154, true, "B4", 5 },
                    { 155, true, "B5", 5 },
                    { 156, true, "B6", 5 },
                    { 157, true, "B7", 5 },
                    { 158, true, "B8", 5 },
                    { 159, true, "B9", 5 },
                    { 160, true, "B10", 5 },
                    { 161, true, "C1", 5 },
                    { 162, true, "C2", 5 },
                    { 163, true, "C3", 5 },
                    { 164, true, "C4", 5 },
                    { 165, true, "C5", 5 },
                    { 166, true, "C6", 5 },
                    { 167, true, "C7", 5 },
                    { 168, true, "C8", 5 }
                });

            migrationBuilder.InsertData(
                table: "SeatsInBusTrip",
                columns: new[] { "SeatInTripId", "IsAvailable", "SeatNumber", "TripId" },
                values: new object[,]
                {
                    { 169, true, "C9", 5 },
                    { 170, true, "C10", 5 },
                    { 171, true, "D1", 5 },
                    { 172, true, "D2", 5 },
                    { 173, true, "D3", 5 },
                    { 174, true, "D4", 5 },
                    { 175, true, "D5", 5 },
                    { 176, true, "D6", 5 },
                    { 177, true, "D7", 5 },
                    { 178, true, "D8", 5 },
                    { 179, true, "D9", 5 },
                    { 180, true, "D10", 5 }
                });

            migrationBuilder.InsertData(
                table: "BookingDetails",
                columns: new[] { "BookingDetailId", "BookingId", "CustomerId", "SeatInTripId", "TicketPrice", "TicketTax" },
                values: new object[,]
                {
                    { 1, 1, 1, 1, 50.00m, 5.00m },
                    { 2, 2, 2, 41, 80.00m, 8.00m },
                    { 3, 3, 3, 81, 140.00m, 14.00m },
                    { 4, 4, 4, 111, 210.00m, 21.00m },
                    { 5, 5, 5, 141, 45.00m, 4.50m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgeBasedDiscounts_MinAge_MaxAge",
                table: "AgeBasedDiscounts",
                columns: new[] { "MinAge", "MaxAge" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingDetails_BookingId",
                table: "BookingDetails",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingDetails_CustomerId",
                table: "BookingDetails",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingDetails_SeatInTripId",
                table: "BookingDetails",
                column: "SeatInTripId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_EmployeeId",
                table: "Bookings",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Buses_BusCode",
                table: "Buses",
                column: "BusCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Buses_BusNumber",
                table: "Buses",
                column: "BusNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Buses_TypeId",
                table: "Buses",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BusTrips_BusId_DepartureTime",
                table: "BusTrips",
                columns: new[] { "BusId", "DepartureTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusTrips_RouteId",
                table: "BusTrips",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_BusTypes_TypeName",
                table: "BusTypes",
                column: "TypeName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Name_DateOfBirth",
                table: "Customers",
                columns: new[] { "Name", "DateOfBirth" });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Username",
                table: "Employees",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Routes_RouteCode",
                table: "Routes",
                column: "RouteCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RouteStops_RouteId_StopLocation",
                table: "RouteStops",
                columns: new[] { "RouteId", "StopLocation" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RouteStops_RouteId_StopOrder",
                table: "RouteStops",
                columns: new[] { "RouteId", "StopOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SeatsInBusTrip_TripId_SeatNumber",
                table: "SeatsInBusTrip",
                columns: new[] { "TripId", "SeatNumber" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgeBasedDiscounts");

            migrationBuilder.DropTable(
                name: "BookingDetails");

            migrationBuilder.DropTable(
                name: "CancellationRules");

            migrationBuilder.DropTable(
                name: "RouteStops");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "SeatsInBusTrip");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "BusTrips");

            migrationBuilder.DropTable(
                name: "Buses");

            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.DropTable(
                name: "BusTypes");
        }
    }
}
