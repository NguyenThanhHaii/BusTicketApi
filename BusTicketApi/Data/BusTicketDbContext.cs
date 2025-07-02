using BusTicketApi.Models;
using BusTicketApi.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Route = BusTicketApi.Models.Entities.Route;

namespace BusTicketApi.Data;

public class BusTicketDbContext : DbContext
{
    public DbSet<BusType> BusTypes { get; set; }
    public DbSet<AgeBasedDiscount> AgeBasedDiscounts { get; set; }
    public DbSet<Route> Routes { get; set; }
    public DbSet<RouteStop> RouteStops { get; set; }
    public DbSet<Bus> Buses { get; set; }
    public DbSet<BusTrip> BusTrips { get; set; }
    public DbSet<SeatInBusTrip> SeatsInBusTrip { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookingDetail> BookingDetails { get; set; }
    public DbSet<CancellationRule> CancellationRules { get; set; }

    public BusTicketDbContext(DbContextOptions<BusTicketDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // BusTypes
        modelBuilder.Entity<BusType>().HasKey(bt => bt.TypeId);
        modelBuilder.Entity<BusType>()
            .HasIndex(b => b.TypeName)
            .IsUnique();
        modelBuilder.Entity<BusType>()
            .HasCheckConstraint("CK_BusType_PriceMultiplier", "PriceMultiplier >= 1.0");

        // AgeBasedDiscounts
        modelBuilder.Entity<AgeBasedDiscount>()
            .HasKey(a => a.DiscountId);
        modelBuilder.Entity<AgeBasedDiscount>()
            .HasIndex(a => new { a.MinAge, a.MaxAge })
            .IsUnique();
        modelBuilder.Entity<AgeBasedDiscount>()
            .HasCheckConstraint("CK_AgeBasedDiscount_MinAge", "MinAge >= 0");
        modelBuilder.Entity<AgeBasedDiscount>()
            .HasCheckConstraint("CK_AgeBasedDiscount_MaxAge", "MaxAge >= MinAge");
        modelBuilder.Entity<AgeBasedDiscount>()
            .HasCheckConstraint("CK_AgeBasedDiscount_DiscountPercentage", "DiscountPercentage >= 0 AND DiscountPercentage <= 100");

        // Routes
        modelBuilder.Entity<Route>().HasKey(r => r.RouteId);
        modelBuilder.Entity<Route>()
            .HasIndex(r => r.RouteCode)
            .IsUnique();
        modelBuilder.Entity<Route>()
            .HasCheckConstraint("CK_Route_Distance", "Distance > 0");
        modelBuilder.Entity<Route>()
            .HasCheckConstraint("CK_Route_BasePrice", "BasePrice > 0");

        // RouteStops
        modelBuilder.Entity<RouteStop>().HasKey(rs => rs.RouteStopId);
        modelBuilder.Entity<RouteStop>()
            .HasIndex(rs => new { rs.RouteId, rs.StopOrder })
            .IsUnique();
        modelBuilder.Entity<RouteStop>()
            .HasIndex(rs => new { rs.RouteId, rs.StopLocation })
            .IsUnique();
        modelBuilder.Entity<RouteStop>()
            .HasCheckConstraint("CK_RouteStop_StopOrder", "StopOrder >= 1");

        // Buses
        modelBuilder.Entity<Bus>().HasKey(b => b.BusId);
        modelBuilder.Entity<Bus>()
            .HasIndex(b => b.BusCode)
            .IsUnique();
        modelBuilder.Entity<Bus>()
            .HasIndex(b => b.BusNumber)
            .IsUnique();
        modelBuilder.Entity<Bus>()
            .HasCheckConstraint("CK_Bus_BusCode", "LEN(BusCode) = 5 AND BusCode NOT LIKE '%[^0-9]%'");
        modelBuilder.Entity<Bus>()
            .HasCheckConstraint("CK_Bus_TotalSeats", "TotalSeats > 0");

        // BusTrips
        modelBuilder.Entity<BusTrip>().HasKey(bt => bt.TripId);
        modelBuilder.Entity<BusTrip>()
            .HasIndex(bt => new { bt.BusId, bt.DepartureTime })
            .IsUnique();
        modelBuilder.Entity<BusTrip>()
            .HasCheckConstraint("CK_BusTrip_AvailableSeats", "AvailableSeats >= 0");

        // SeatsInBusTrip
        modelBuilder.Entity<SeatInBusTrip>().HasKey(s => s.SeatInTripId);
        modelBuilder.Entity<SeatInBusTrip>()
            .HasIndex(s => new { s.TripId, s.SeatNumber })
            .IsUnique();

        // Employees
        modelBuilder.Entity<Employee>().HasKey(e => e.EmployeeId);
        modelBuilder.Entity<Employee>()
            .HasIndex(e => e.Username)
            .IsUnique();
        modelBuilder.Entity<Employee>()
            .HasIndex(e => e.Email)
            .IsUnique();
        modelBuilder.Entity<Employee>()
            .HasCheckConstraint("CK_Employee_Age", "Age >= 18");

        // Customers
        modelBuilder.Entity<Customer>().HasKey(c => c.CustomerId);
        modelBuilder.Entity<Customer>()
            .HasIndex(c => new { c.Name, c.DateOfBirth });

        // Bookings
        modelBuilder.Entity<Booking>().HasKey(b => b.BookingId);
        modelBuilder.Entity<Booking>()
            .HasCheckConstraint("CK_Booking_TotalAmount", "TotalAmount >= 0");
        modelBuilder.Entity<Booking>()
            .HasCheckConstraint("CK_Booking_TotalTax", "TotalTax >= 0");
        modelBuilder.Entity<Booking>()
            .HasCheckConstraint("CK_Booking_RefundAmount", "RefundAmount >= 0");

        // BookingDetails
        modelBuilder.Entity<BookingDetail>().HasKey(bd => bd.BookingDetailId);
        modelBuilder.Entity<BookingDetail>()
            .HasIndex(bd => bd.SeatInTripId)
            .IsUnique();
        modelBuilder.Entity<BookingDetail>()
            .HasCheckConstraint("CK_BookingDetail_TicketPrice", "TicketPrice >= 0");
        modelBuilder.Entity<BookingDetail>()
            .HasCheckConstraint("CK_BookingDetail_TicketTax", "TicketTax >= 0");

        // CancellationRules
        modelBuilder.Entity<CancellationRule>().HasKey(cr => cr.RuleId);
        modelBuilder.Entity<CancellationRule>()
            .HasCheckConstraint("CK_CancellationRule_DaysBeforeDeparture", "DaysBeforeDeparture >= 0");
        modelBuilder.Entity<CancellationRule>()
            .HasCheckConstraint("CK_CancellationRule_PenaltyPercentage", "PenaltyPercentage >= 0 AND PenaltyPercentage <= 100");

        // Seed data
// BusTypes
    modelBuilder.Entity<BusType>().HasData(
        new BusType { TypeId = 1, TypeName = "Express", PriceMultiplier = 1.0m },
        new BusType { TypeId = 2, TypeName = "Luxury", PriceMultiplier = 1.2m },
        new BusType { TypeId = 3, TypeName = "Volvo Non-A/C", PriceMultiplier = 1.4m },
        new BusType { TypeId = 4, TypeName = "Volvo A/C", PriceMultiplier = 1.6m }
    );

    // AgeBasedDiscounts
    modelBuilder.Entity<AgeBasedDiscount>().HasData(
        new AgeBasedDiscount { DiscountId = 1, MinAge = 0, MaxAge = 4, DiscountPercentage = 100, Description = "Free for children under 5" },
        new AgeBasedDiscount { DiscountId = 2, MinAge = 5, MaxAge = 12, DiscountPercentage = 50, Description = "50% off for children 5-12" },
        new AgeBasedDiscount { DiscountId = 3, MinAge = 13, MaxAge = 50, DiscountPercentage = 0, Description = "Full price for adults" },
        new AgeBasedDiscount { DiscountId = 4, MinAge = 51, MaxAge = 150, DiscountPercentage = 30, Description = "30% off for seniors over 50" }
    );

    // Routes
    modelBuilder.Entity<Route>().HasData(
        new Route { RouteId = 1, RouteCode = "R001", StartLocation = "New York, NY", EndLocation = "Boston, MA", Distance = 215.0m, BasePrice = 50.00m },
        new Route { RouteId = 2, RouteCode = "R002", StartLocation = "Los Angeles, CA", EndLocation = "San Francisco, CA", Distance = 380.0m, BasePrice = 80.00m },
        new Route { RouteId = 3, RouteCode = "R003", StartLocation = "Chicago, IL", EndLocation = "Denver, CO", Distance = 1000.0m, BasePrice = 120.00m },
        new Route { RouteId = 4, RouteCode = "R004", StartLocation = "Houston, TX", EndLocation = "Austin, TX", Distance = 200.0m, BasePrice = 40.00m },
        new Route { RouteId = 5, RouteCode = "R005", StartLocation = "Seattle, WA", EndLocation = "Portland, OR", Distance = 180.0m, BasePrice = 45.00m }
    );

    // RouteStops
    modelBuilder.Entity<RouteStop>().HasData(
        new RouteStop { RouteStopId = 1, RouteId = 1, StopLocation = "Hartford, CT", StopOrder = 1 },
        new RouteStop { RouteStopId = 2, RouteId = 1, StopLocation = "Providence, RI", StopOrder = 2 },
        new RouteStop { RouteStopId = 3, RouteId = 2, StopLocation = "Santa Barbara, CA", StopOrder = 1 },
        new RouteStop { RouteStopId = 4, RouteId = 2, StopLocation = "Monterey, CA", StopOrder = 2 },
        new RouteStop { RouteStopId = 5, RouteId = 3, StopLocation = "Omaha, NE", StopOrder = 1 },
        new RouteStop { RouteStopId = 6, RouteId = 3, StopLocation = "Cheyenne, WY", StopOrder = 2 },
        new RouteStop { RouteStopId = 7, RouteId = 4, StopLocation = "San Antonio, TX", StopOrder = 1 },
        new RouteStop { RouteStopId = 8, RouteId = 4, StopLocation = "Waco, TX", StopOrder = 2 },
        new RouteStop { RouteStopId = 9, RouteId = 5, StopLocation = "Tacoma, WA", StopOrder = 1 },
        new RouteStop { RouteStopId = 10, RouteId = 5, StopLocation = "Salem, OR", StopOrder = 2 }
    );

    // Buses
    modelBuilder.Entity<Bus>().HasData(
        new Bus { BusId = 1, BusCode = "10001", BusNumber = "NY123", TypeId = 1, TotalSeats = 40 },
        new Bus { BusId = 2, BusCode = "10002", BusNumber = "CA456", TypeId = 4, TotalSeats = 30 }
    );

    // BusTrips
    modelBuilder.Entity<BusTrip>().HasData(
        new BusTrip { TripId = 1, BusId = 1, RouteId = 1, DepartureTime = new DateTime(2025, 7, 3, 8, 0, 0), ArrivalTime = new DateTime(2025, 7, 3, 12, 0, 0), AvailableSeats = 40, Status = "Scheduled" },
        new BusTrip { TripId = 2, BusId = 1, RouteId = 2, DepartureTime = new DateTime(2025, 7, 3, 14, 0, 0), ArrivalTime = new DateTime(2025, 7, 3, 20, 0, 0), AvailableSeats = 40, Status = "Scheduled" },
        new BusTrip { TripId = 3, BusId = 2, RouteId = 3, DepartureTime = new DateTime(2025, 7, 3, 9, 0, 0), ArrivalTime = new DateTime(2025, 7, 3, 19, 0, 0), AvailableSeats = 30, Status = "Scheduled" },
        new BusTrip { TripId = 4, BusId = 2, RouteId = 4, DepartureTime = new DateTime(2025, 7, 3, 10, 0, 0), ArrivalTime = new DateTime(2025, 7, 3, 12, 0, 0), AvailableSeats = 30, Status = "Scheduled" },
        new BusTrip { TripId = 5, BusId = 1, RouteId = 5, DepartureTime = new DateTime(2025, 7, 3, 15, 0, 0), ArrivalTime = new DateTime(2025, 7, 3, 17, 0, 0), AvailableSeats = 40, Status = "Scheduled" }
    );

    // SeatsInBusTrip
    modelBuilder.Entity<SeatInBusTrip>().HasData(
        // TripId = 1 (40 ghế)
        new SeatInBusTrip { SeatInTripId = 1, TripId = 1, SeatNumber = "A1", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 2, TripId = 1, SeatNumber = "A2", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 3, TripId = 1, SeatNumber = "A3", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 4, TripId = 1, SeatNumber = "A4", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 5, TripId = 1, SeatNumber = "A5", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 6, TripId = 1, SeatNumber = "A6", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 7, TripId = 1, SeatNumber = "A7", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 8, TripId = 1, SeatNumber = "A8", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 9, TripId = 1, SeatNumber = "A9", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 10, TripId = 1, SeatNumber = "A10", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 11, TripId = 1, SeatNumber = "B1", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 12, TripId = 1, SeatNumber = "B2", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 13, TripId = 1, SeatNumber = "B3", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 14, TripId = 1, SeatNumber = "B4", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 15, TripId = 1, SeatNumber = "B5", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 16, TripId = 1, SeatNumber = "B6", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 17, TripId = 1, SeatNumber = "B7", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 18, TripId = 1, SeatNumber = "B8", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 19, TripId = 1, SeatNumber = "B9", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 20, TripId = 1, SeatNumber = "B10", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 21, TripId = 1, SeatNumber = "C1", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 22, TripId = 1, SeatNumber = "C2", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 23, TripId = 1, SeatNumber = "C3", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 24, TripId = 1, SeatNumber = "C4", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 25, TripId = 1, SeatNumber = "C5", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 26, TripId = 1, SeatNumber = "C6", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 27, TripId = 1, SeatNumber = "C7", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 28, TripId = 1, SeatNumber = "C8", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 29, TripId = 1, SeatNumber = "C9", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 30, TripId = 1, SeatNumber = "C10", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 31, TripId = 1, SeatNumber = "D1", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 32, TripId = 1, SeatNumber = "D2", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 33, TripId = 1, SeatNumber = "D3", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 34, TripId = 1, SeatNumber = "D4", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 35, TripId = 1, SeatNumber = "D5", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 36, TripId = 1, SeatNumber = "D6", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 37, TripId = 1, SeatNumber = "D7", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 38, TripId = 1, SeatNumber = "D8", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 39, TripId = 1, SeatNumber = "D9", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 40, TripId = 1, SeatNumber = "D10", IsAvailable = true },

        // TripId = 2 (40 ghế)
        new SeatInBusTrip { SeatInTripId = 41, TripId = 2, SeatNumber = "A1", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 42, TripId = 2, SeatNumber = "A2", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 43, TripId = 2, SeatNumber = "A3", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 44, TripId = 2, SeatNumber = "A4", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 45, TripId = 2, SeatNumber = "A5", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 46, TripId = 2, SeatNumber = "A6", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 47, TripId = 2, SeatNumber = "A7", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 48, TripId = 2, SeatNumber = "A8", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 49, TripId = 2, SeatNumber = "A9", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 50, TripId = 2, SeatNumber = "A10", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 51, TripId = 2, SeatNumber = "B1", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 52, TripId = 2, SeatNumber = "B2", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 53, TripId = 2, SeatNumber = "B3", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 54, TripId = 2, SeatNumber = "B4", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 55, TripId = 2, SeatNumber = "B5", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 56, TripId = 2, SeatNumber = "B6", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 57, TripId = 2, SeatNumber = "B7", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 58, TripId = 2, SeatNumber = "B8", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 59, TripId = 2, SeatNumber = "B9", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 60, TripId = 2, SeatNumber = "B10", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 61, TripId = 2, SeatNumber = "C1", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 62, TripId = 2, SeatNumber = "C2", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 63, TripId = 2, SeatNumber = "C3", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 64, TripId = 2, SeatNumber = "C4", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 65, TripId = 2, SeatNumber = "C5", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 66, TripId = 2, SeatNumber = "C6", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 67, TripId = 2, SeatNumber = "C7", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 68, TripId = 2, SeatNumber = "C8", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 69, TripId = 2, SeatNumber = "C9", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 70, TripId = 2, SeatNumber = "C10", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 71, TripId = 2, SeatNumber = "D1", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 72, TripId = 2, SeatNumber = "D2", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 73, TripId = 2, SeatNumber = "D3", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 74, TripId = 2, SeatNumber = "D4", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 75, TripId = 2, SeatNumber = "D5", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 76, TripId = 2, SeatNumber = "D6", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 77, TripId = 2, SeatNumber = "D7", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 78, TripId = 2, SeatNumber = "D8", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 79, TripId = 2, SeatNumber = "D9", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 80, TripId = 2, SeatNumber = "D10", IsAvailable = true },

        // TripId = 3 (30 ghế)
        new SeatInBusTrip { SeatInTripId = 81, TripId = 3, SeatNumber = "A1", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 82, TripId = 3, SeatNumber = "A2", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 83, TripId = 3, SeatNumber = "A3", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 84, TripId = 3, SeatNumber = "A4", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 85, TripId = 3, SeatNumber = "A5", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 86, TripId = 3, SeatNumber = "A6", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 87, TripId = 3, SeatNumber = "A7", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 88, TripId = 3, SeatNumber = "A8", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 89, TripId = 3, SeatNumber = "A9", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 90, TripId = 3, SeatNumber = "A10", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 91, TripId = 3, SeatNumber = "B1", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 92, TripId = 3, SeatNumber = "B2", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 93, TripId = 3, SeatNumber = "B3", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 94, TripId = 3, SeatNumber = "B4", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 95, TripId = 3, SeatNumber = "B5", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 96, TripId = 3, SeatNumber = "B6", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 97, TripId = 3, SeatNumber = "B7", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 98, TripId = 3, SeatNumber = "B8", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 99, TripId = 3, SeatNumber = "B9", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 100, TripId = 3, SeatNumber = "B10", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 101, TripId = 3, SeatNumber = "C1", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 102, TripId = 3, SeatNumber = "C2", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 103, TripId = 3, SeatNumber = "C3", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 104, TripId = 3, SeatNumber = "C4", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 105, TripId = 3, SeatNumber = "C5", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 106, TripId = 3, SeatNumber = "C6", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 107, TripId = 3, SeatNumber = "C7", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 108, TripId = 3, SeatNumber = "C8", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 109, TripId = 3, SeatNumber = "C9", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 110, TripId = 3, SeatNumber = "C10", IsAvailable = true },

        // TripId = 4 (30 ghế)
        new SeatInBusTrip { SeatInTripId = 111, TripId = 4, SeatNumber = "A1", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 112, TripId = 4, SeatNumber = "A2", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 113, TripId = 4, SeatNumber = "A3", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 114, TripId = 4, SeatNumber = "A4", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 115, TripId = 4, SeatNumber = "A5", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 116, TripId = 4, SeatNumber = "A6", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 117, TripId = 4, SeatNumber = "A7", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 118, TripId = 4, SeatNumber = "A8", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 119, TripId = 4, SeatNumber = "A9", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 120, TripId = 4, SeatNumber = "A10", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 121, TripId = 4, SeatNumber = "B1", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 122, TripId = 4, SeatNumber = "B2", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 123, TripId = 4, SeatNumber = "B3", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 124, TripId = 4, SeatNumber = "B4", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 125, TripId = 4, SeatNumber = "B5", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 126, TripId = 4, SeatNumber = "B6", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 127, TripId = 4, SeatNumber = "B7", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 128, TripId = 4, SeatNumber = "B8", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 129, TripId = 4, SeatNumber = "B9", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 130, TripId = 4, SeatNumber = "B10", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 131, TripId = 4, SeatNumber = "C1", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 132, TripId = 4, SeatNumber = "C2", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 133, TripId = 4, SeatNumber = "C3", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 134, TripId = 4, SeatNumber = "C4", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 135, TripId = 4, SeatNumber = "C5", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 136, TripId = 4, SeatNumber = "C6", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 137, TripId = 4, SeatNumber = "C7", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 138, TripId = 4, SeatNumber = "C8", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 139, TripId = 4, SeatNumber = "C9", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 140, TripId = 4, SeatNumber = "C10", IsAvailable = true },

        // TripId = 5 (40 ghế)
        new SeatInBusTrip { SeatInTripId = 141, TripId = 5, SeatNumber = "A1", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 142, TripId = 5, SeatNumber = "A2", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 143, TripId = 5, SeatNumber = "A3", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 144, TripId = 5, SeatNumber = "A4", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 145, TripId = 5, SeatNumber = "A5", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 146, TripId = 5, SeatNumber = "A6", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 147, TripId = 5, SeatNumber = "A7", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 148, TripId = 5, SeatNumber = "A8", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 149, TripId = 5, SeatNumber = "A9", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 150, TripId = 5, SeatNumber = "A10", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 151, TripId = 5, SeatNumber = "B1", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 152, TripId = 5, SeatNumber = "B2", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 153, TripId = 5, SeatNumber = "B3", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 154, TripId = 5, SeatNumber = "B4", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 155, TripId = 5, SeatNumber = "B5", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 156, TripId = 5, SeatNumber = "B6", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 157, TripId = 5, SeatNumber = "B7", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 158, TripId = 5, SeatNumber = "B8", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 159, TripId = 5, SeatNumber = "B9", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 160, TripId = 5, SeatNumber = "B10", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 161, TripId = 5, SeatNumber = "C1", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 162, TripId = 5, SeatNumber = "C2", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 163, TripId = 5, SeatNumber = "C3", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 164, TripId = 5, SeatNumber = "C4", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 165, TripId = 5, SeatNumber = "C5", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 166, TripId = 5, SeatNumber = "C6", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 167, TripId = 5, SeatNumber = "C7", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 168, TripId = 5, SeatNumber = "C8", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 169, TripId = 5, SeatNumber = "C9", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 170, TripId = 5, SeatNumber = "C10", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 171, TripId = 5, SeatNumber = "D1", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 172, TripId = 5, SeatNumber = "D2", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 173, TripId = 5, SeatNumber = "D3", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 174, TripId = 5, SeatNumber = "D4", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 175, TripId = 5, SeatNumber = "D5", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 176, TripId = 5, SeatNumber = "D6", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 177, TripId = 5, SeatNumber = "D7", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 178, TripId = 5, SeatNumber = "D8", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 179, TripId = 5, SeatNumber = "D9", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 180, TripId = 5, SeatNumber = "D10", IsAvailable = true }
    );

    // Employees
    modelBuilder.Entity<Employee>().HasData(
        new Employee { EmployeeId = 100000, Username = "admin", PasswordHash = "$2a$12$GquGNXIqWkN9fFKR.OmDi.o3DkbahSX2MkVjf..hdeEeBPH6qSO66", Name = "Super Admin", Email = "admin@example.com", PhoneNumber = "001-555-1111", Qualifications = "Bachelor’s Degree", Role = "Admin", Age = 48, Location = "New York, NY" },
        new Employee { EmployeeId = 100001, Username = "john.doe", PasswordHash = "$2a$12$yv5t3yIkt3F9hYGUwiOKe.xFAapbty9TLngpxeQ0KmlbpBYj30Isu", Name = "John Doe", Email = "john.doe@example.com", PhoneNumber = "212-555-1234", Qualifications = "Bachelor’s Degree", Role = "Admin", Age = 35, Location = "New York, NY" },
        new Employee { EmployeeId = 100002, Username = "jane.smith", PasswordHash = "$2a$12$4FleyqJI7PgWsDZ/E1Gf3eocMPjVfQiuZX6ONVx9GKXiSy8JI5AJW", Name = "Jane Smith", Email = "jane.smith@example.com", PhoneNumber = "415-555-5678", Qualifications = "Associate Degree", Role = "Employee", Age = 28, Location = "San Francisco, CA" },
        new Employee { EmployeeId = 100003, Username = "mike.jones", PasswordHash = "$2a$12$8kP9m2nQ3rL6vT8J4yZ9u.eF7pQ8vX2MkYjf..hdeEeBPH6qSO66", Name = "Mike Jones", Email = "mike.jones@example.com", PhoneNumber = "213-555-9012", Qualifications = "High School Diploma", Role = "Employee", Age = 32, Location = "Los Angeles, CA" },
        new Employee { EmployeeId = 100004, Username = "sarah.lee", PasswordHash = "$2a$12$3jK7pL9mQ2rT4vH8yZ6u.eF7pQ8vX2MkYjf..hdeEeBPH6qSO66", Name = "Sarah Lee", Email = "sarah.lee@example.com", PhoneNumber = "206-555-3456", Qualifications = "Bachelor’s Degree", Role = "Employee", Age = 40, Location = "Seattle, WA" }
    );

    // Customers
    modelBuilder.Entity<Customer>().HasData(
        new Customer { CustomerId = 1, Name = "Alice Johnson", DateOfBirth = new DateTime(1990, 5, 15), Email = "alice.johnson@example.com", PhoneNumber = "617-555-9012" },
        new Customer { CustomerId = 2, Name = "Bob Williams", DateOfBirth = new DateTime(2000, 8, 22), Email = "bob.williams@example.com", PhoneNumber = "213-555-3456" },
        new Customer { CustomerId = 3, Name = "Charlie Brown", DateOfBirth = new DateTime(1995, 3, 10), Email = "charlie.brown@example.com", PhoneNumber = "415-555-6789" },
        new Customer { CustomerId = 4, Name = "Dana White", DateOfBirth = new DateTime(1988, 11, 25), Email = "dana.white@example.com", PhoneNumber = "206-555-1234" },
        new Customer { CustomerId = 5, Name = "Evan Davis", DateOfBirth = new DateTime(1992, 7, 12), Email = "evan.davis@example.com", PhoneNumber = "312-555-5678" }
    );

    // Bookings (TotalAmount, TotalTax, RefundAmount in USD)
    modelBuilder.Entity<Booking>().HasData(
        new Booking { BookingId = 1, EmployeeId = 100000, BookingDate = new DateTime(2025, 7, 2, 10, 0, 0), TotalAmount = 50.00m, TotalTax = 5.00m, Status = "Confirmed", CancellationDate = null, RefundAmount = null },
        new Booking { BookingId = 2, EmployeeId = 100001, BookingDate = new DateTime(2025, 7, 2, 11, 0, 0), TotalAmount = 80.00m, TotalTax = 8.00m, Status = "Confirmed", CancellationDate = null, RefundAmount = null },
        new Booking { BookingId = 3, EmployeeId = 100002, BookingDate = new DateTime(2025, 7, 2, 14, 0, 0), TotalAmount = 140.00m, TotalTax = 14.00m, Status = "Cancelled", CancellationDate = new DateTime(2025, 7, 1, 12, 0, 0), RefundAmount = 98.00m },
        new Booking { BookingId = 4, EmployeeId = 100003, BookingDate = new DateTime(2025, 7, 3, 9, 30, 0), TotalAmount = 210.00m, TotalTax = 21.00m, Status = "Confirmed", CancellationDate = null, RefundAmount = null },
        new Booking { BookingId = 5, EmployeeId = 100004, BookingDate = new DateTime(2025, 7, 3, 15, 0, 0), TotalAmount = 45.00m, TotalTax = 4.50m, Status = "Confirmed", CancellationDate = null, RefundAmount = null }
    );

    // BookingDetails (TicketPrice, TicketTax in USD)
    modelBuilder.Entity<BookingDetail>().HasData(
        new BookingDetail { BookingDetailId = 1, BookingId = 1, SeatInTripId = 1, CustomerId = 1, TicketPrice = 50.00m, TicketTax = 5.00m },
        new BookingDetail { BookingDetailId = 2, BookingId = 2, SeatInTripId = 41, CustomerId = 2, TicketPrice = 80.00m, TicketTax = 8.00m },
        new BookingDetail { BookingDetailId = 3, BookingId = 3, SeatInTripId = 81, CustomerId = 3, TicketPrice = 140.00m, TicketTax = 14.00m },
        new BookingDetail { BookingDetailId = 4, BookingId = 4, SeatInTripId = 111, CustomerId = 4, TicketPrice = 210.00m, TicketTax = 21.00m },
        new BookingDetail { BookingDetailId = 5, BookingId = 5, SeatInTripId = 141, CustomerId = 5, TicketPrice = 45.00m, TicketTax = 4.50m }
    );

    // CancellationRules
    modelBuilder.Entity<CancellationRule>().HasData(
        new CancellationRule { RuleId = 1, DaysBeforeDeparture = 2, PenaltyPercentage = 0, Description = "Full refund for cancellations 2+ days before departure" },
        new CancellationRule { RuleId = 2, DaysBeforeDeparture = 1, PenaltyPercentage = 15, Description = "15% penalty for cancellations 1 day before departure" },
        new CancellationRule { RuleId = 3, DaysBeforeDeparture = 0, PenaltyPercentage = 30, Description = "30% penalty for cancellations on departure day" }
    );
    }
}