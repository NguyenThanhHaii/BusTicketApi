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

    // Routes (BasePrice in USD)
    modelBuilder.Entity<Route>().HasData(
        new Route { RouteId = 1, RouteCode = "R001", StartLocation = "New York, NY", EndLocation = "Boston, MA", Distance = 215.0m, BasePrice = 50.00m }, // $50.00
        new Route { RouteId = 2, RouteCode = "R002", StartLocation = "Los Angeles, CA", EndLocation = "San Francisco, CA", Distance = 380.0m, BasePrice = 80.00m } // $80.00
    );

    // RouteStops
    modelBuilder.Entity<RouteStop>().HasData(
        new RouteStop { RouteStopId = 1, RouteId = 1, StopLocation = "Hartford, CT", StopOrder = 1 },
        new RouteStop { RouteStopId = 2, RouteId = 1, StopLocation = "Providence, RI", StopOrder = 2 },
        new RouteStop { RouteStopId = 3, RouteId = 2, StopLocation = "Santa Barbara, CA", StopOrder = 1 },
        new RouteStop { RouteStopId = 4, RouteId = 2, StopLocation = "Monterey, CA", StopOrder = 2 }
    );

    // Buses
    modelBuilder.Entity<Bus>().HasData(
        new Bus { BusId = 1, BusCode = "10001", BusNumber = "NY123", TypeId = 1, TotalSeats = 40 },
        new Bus { BusId = 2, BusCode = "10002", BusNumber = "CA456", TypeId = 4, TotalSeats = 30 }
    );

    // BusTrips
    modelBuilder.Entity<BusTrip>().HasData(
        new BusTrip { TripId = 1, BusId = 1, RouteId = 1, DepartureTime = new DateTime(2025, 7, 3, 8, 0, 0), ArrivalTime = new DateTime(2025, 7, 3, 12, 0, 0), AvailableSeats = 40, Status = "Scheduled" },
        new BusTrip { TripId = 2, BusId = 2, RouteId = 2, DepartureTime = new DateTime(2025, 7, 3, 9, 0, 0), ArrivalTime = new DateTime(2025, 7, 3, 16, 0, 0), AvailableSeats = 30, Status = "Scheduled" }
    );

    // SeatsInBusTrip
    modelBuilder.Entity<SeatInBusTrip>().HasData(
        new SeatInBusTrip { SeatInTripId = 1, TripId = 1, SeatNumber = "A1", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 2, TripId = 1, SeatNumber = "A2", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 3, TripId = 2, SeatNumber = "B1", IsAvailable = true },
        new SeatInBusTrip { SeatInTripId = 4, TripId = 2, SeatNumber = "B2", IsAvailable = true }
    );

    // Employees
    modelBuilder.Entity<Employee>().HasData(
        new Employee { EmployeeId = 100000, Username = "admin", PasswordHash = "$2a$12$GquGNXIqWkN9fFKR.OmDi.o3DkbahSX2MkVjf..hdeEeBPH6qSO66", Name = "Super Admin", Email = "admin@example.com", PhoneNumber = "001-555-1111", Qualifications = "Bachelor’s Degree", Role = "Admin", Age = 48, Location = "New York, NY" },
        new Employee { EmployeeId = 100001, Username = "john.doe", PasswordHash = "$2a$12$yv5t3yIkt3F9hYGUwiOKe.xFAapbty9TLngpxeQ0KmlbpBYj30Isu", Name = "John Doe", Email = "john.doe@example.com", PhoneNumber = "212-555-1234", Qualifications = "Bachelor’s Degree", Role = "Admin", Age = 35, Location = "New York, NY" },
        new Employee { EmployeeId = 100002, Username = "jane.smith", PasswordHash = "$2a$12$4FleyqJI7PgWsDZ/E1Gf3eocMPjVfQiuZX6ONVx9GKXiSy8JI5AJW", Name = "Jane Smith", Email = "jane.smith@example.com", PhoneNumber = "415-555-5678", Qualifications = "Associate Degree", Role = "Employee", Age = 28, Location = "San Francisco, CA" }
    );

    // Customers
    modelBuilder.Entity<Customer>().HasData(
        new Customer { CustomerId = 1, Name = "Alice Johnson", DateOfBirth = new DateTime(1990, 5, 15), Email = "alice.johnson@example.com", PhoneNumber = "617-555-9012" },
        new Customer { CustomerId = 2, Name = "Bob Williams", DateOfBirth = new DateTime(2000, 8, 22), Email = "bob.williams@example.com", PhoneNumber = "213-555-3456" }
    );

    // Bookings (TotalAmount, TotalTax, RefundAmount in USD)
    modelBuilder.Entity<Booking>().HasData(
        new Booking { BookingId = 1, EmployeeId = 100000, BookingDate = new DateTime(2025, 7, 2, 10, 0, 0), TotalAmount = 50.00m, TotalTax = 5.00m, Status = "Confirmed", CancellationDate = null, RefundAmount = null }, // $50.00 + $5.00 tax
        new Booking { BookingId = 2, EmployeeId = 100001, BookingDate = new DateTime(2025, 7, 2, 11, 0, 0), TotalAmount = 80.00m, TotalTax = 8.00m, Status = "Confirmed", CancellationDate = null, RefundAmount = null } // $80.00 + $8.00 tax
    );

    // BookingDetails (TicketPrice, TicketTax in USD)
    modelBuilder.Entity<BookingDetail>().HasData(
        new BookingDetail { BookingDetailId = 1, BookingId = 1, SeatInTripId = 1, CustomerId = 1, TicketPrice = 50.00m, TicketTax = 5.00m }, // $50.00 + $5.00 tax
        new BookingDetail { BookingDetailId = 2, BookingId = 2, SeatInTripId = 3, CustomerId = 2, TicketPrice = 80.00m, TicketTax = 8.00m } // $80.00 + $8.00 tax
    );

    // CancellationRules
    modelBuilder.Entity<CancellationRule>().HasData(
        new CancellationRule { RuleId = 1, DaysBeforeDeparture = 2, PenaltyPercentage = 0, Description = "Full refund for cancellations 2+ days before departure" },
        new CancellationRule { RuleId = 2, DaysBeforeDeparture = 1, PenaltyPercentage = 15, Description = "15% penalty for cancellations 1 day before departure" },
        new CancellationRule { RuleId = 3, DaysBeforeDeparture = 0, PenaltyPercentage = 30, Description = "30% penalty for cancellations on departure day" }
    );
    }
}