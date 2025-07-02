# BusTicketApi Documentation

## 1. Authentication API (/api/Auth)

- **Description**: Handles login and employee registration (Admin/Employee).

- **Endpoints**:

  - `POST /api/Auth/login`: Authenticate to obtain a JWT token.
  - `POST /api/Auth/register`: Register a new employee (Admin only).

- **Requirements**:

  - Header: No token required for `login`, `Authorization: Bearer <token>` required for `register`.

- **Responses**:

  - `POST /api/Auth/login`:
    - `200 OK`: `{ "token": "string", "role": "string" }`.
    - `401 Unauthorized`: Invalid credentials.
  - `POST /api/Auth/register`:
    - `200 OK`: `"Employee registered successfully"`.
    - `400 Bad Request`: Username already exists.
    - `401 Unauthorized`: Invalid token.
    - `403 Forbidden`: Not an Admin.

- **Testing Instructions**:

  - **Tool**: Use Postman.

  - **Test** `POST /api/Auth/login`:

    1. Method: `POST`.

    2. URL: `http://localhost:7112/api/Auth/login`.

    3. Body: `raw` (JSON):

       ```json
       {
         "username": "admin.doe",
         "password": "password123"
       }
       ```

    4. Expected Result: `200 OK` with a token and role ("Admin").

    5. Error Test: Use incorrect username/password to receive `401 Unauthorized`.

  - **Test** `POST /api/Auth/register`:

    1. Method: `POST`.

    2. URL: `http://localhost:7112/api/Auth/register`.

    3. Header: `Authorization: Bearer <token>` (obtained from login with "Admin" role).

    4. Body: `raw` (JSON):

       ```json
       {
         "username": "new.employee",
         "password": "newpass123",
         "name": "New Employee",
         "email": "new@example.com",
         "phoneNumber": "212-555-0002",
         "qualifications": "Ticketing",
         "role": "Employee",
         "age": 28,
         "location": "New York, NY"
       }
       ```

    5. Expected Result: `200 OK` with success message.

    6. Error Test: Use an existing username to receive `400 Bad Request`.

## 2. Bus Management API (/api/Buses)

- **Description**: Manages bus information (Admin only).

- **Endpoints**:

  - `GET /api/Buses`: Retrieve list of buses.
  - `GET /api/Buses/{id}`: Retrieve bus details by ID.
  - `POST /api/Buses`: Create a new bus.
  - `PUT /api/Buses/{id}`: Update bus by ID.
  - `DELETE /api/Buses/{id}`: Delete bus by ID.

- **Requirements**:

  - Header: `Authorization: Bearer <token>`.

- **Responses**:

  - `200 OK`: List or details of buses.
  - `201 Created`: Bus created successfully.
  - `400 Bad Request`: Invalid data.
  - `401 Unauthorized`: Invalid token.
  - `403 Forbidden`: Not an Admin.
  - `404 Not Found`: Bus not found.
  - `400 Bad Request`: Cannot delete bus with associated trips.

- **Testing Instructions**:

  - **Tool**: Use Postman.

  - **Test** `GET /api/Buses`:

    1. Method: `GET`.
    2. URL: `http://localhost:7112/api/Buses`.
    3. Header: `Authorization: Bearer <token>` (obtained from login with "Admin" role).
    4. Expected Result: `200 OK` with bus list (e.g., `[{"busId": 1, "busCode": "NY123", ...}]`).

  - **Test** `POST /api/Buses`:

    1. Method: `POST`.

    2. URL: `http://localhost:7112/api/Buses`.

    3. Header: `Authorization: Bearer <token>` (role "Admin").

    4. Body: `raw` (JSON):

       ```json
       {
         "busCode": "LA456",
         "busNumber": "LA-45678",
         "typeId": 1,
         "totalSeats": 40
       }
       ```

    5. Expected Result: `201 Created` with new bus.

    6. Error Test: Use invalid data (e.g., negative `totalSeats`) to receive `400 Bad Request`.

## 3. Route Management API (/api/Routes)

- **Description**: Manages route information (Admin only).

- **Endpoints**:

  - `GET /api/Routes`: Retrieve list of routes.
  - `GET /api/Routes/{id}`: Retrieve route details by ID.
  - `POST /api/Routes`: Create a new route.
  - `PUT /api/Routes/{id}`: Update route by ID.
  - `DELETE /api/Routes/{id}`: Delete route by ID.

- **Requirements**:

  - Header: `Authorization: Bearer <token>`.

- **Responses**:

  - `200 OK`: List or details of routes.
  - `201 Created`: Route created successfully.
  - `400 Bad Request`: Invalid data.
  - `401 Unauthorized`: Invalid token.
  - `403 Forbidden`: Not an Admin.
  - `404 Not Found`: Route not found.
  - `400 Bad Request`: Cannot delete route with associated trips.

- **Testing Instructions**:

  - **Tool**: Use Postman.

  - **Test** `GET /api/Routes`:

    1. Method: `GET`.
    2. URL: `http://localhost:7112/api/Routes`.
    3. Header: `Authorization: Bearer <token>` (role "Admin").
    4. Expected Result: `200 OK` with route list (e.g., `[{"routeId": 1, "routeCode": "NY-BOS", ...}]`).

  - **Test** `POST /api/Routes`:

    1. Method: `POST`.

    2. URL: `http://localhost:7112/api/Routes`.

    3. Header: `Authorization: Bearer <token>` (role "Admin").

    4. Body: `raw` (JSON):

       ```json
       {
         "routeCode": "LA-SF",
         "startLocation": "Los Angeles, CA",
         "endLocation": "San Francisco, CA",
         "distance": 380.0,
         "basePrice": 70.0
       }
       ```

    5. Expected Result: `201 Created` with new route.

    6. Error Test: Use invalid data (e.g., negative `basePrice`) to receive `400 Bad Request`.

## 4. Employee Management API (/api/Employees)

- **Description**: Manages employee information (Admin only).

- **Endpoints**:

  - `GET /api/Employees`: Retrieve list of employees.
  - `GET /api/Employees/{id}`: Retrieve employee details by ID.
  - `POST /api/Employees`: Create a new employee.
  - `PUT /api/Employees/{id}`: Update employee by ID.
  - `DELETE /api/Employees/{id}`: Delete employee by ID.

- **Requirements**:

  - Header: `Authorization: Bearer <token>`.

- **Responses**:

  - `200 OK`: List or details of employees.
  - `201 Created`: Employee created successfully.
  - `400 Bad Request`: Invalid data.
  - `401 Unauthorized`: Invalid token.
  - `403 Forbidden`: Not an Admin.
  - `404 Not Found`: Employee not found.
  - `400 Bad Request`: Cannot delete employee with associated bookings.

- **Testing Instructions**:

  - **Tool**: Use Postman.

  - **Test** `GET /api/Employees`:

    1. Method: `GET`.
    2. URL: `http://localhost:7112/api/Employees`.
    3. Header: `Authorization: Bearer <token>` (role "Admin").
    4. Expected Result: `200 OK` with employee list (e.g., `[{"employeeId": 1, "username": "admin.doe", ...}]`).

  - **Test** `POST /api/Employees`:

    1. Method: `POST`.

    2. URL: `http://localhost:7112/api/Employees`.

    3. Header: `Authorization: Bearer <token>` (role "Admin").

    4. Body: `raw` (JSON):

       ```json
       {
         "username": "new.admin",
         "password": "newpass123",
         "name": "New Admin",
         "email": "newadmin@example.com",
         "phoneNumber": "213-555-0002",
         "qualifications": "Management",
         "role": "Admin",
         "age": 40,
         "location": "Los Angeles, CA"
       }
       ```

    5. Expected Result: `201 Created` with new employee.

    6. Error Test: Use an existing username to receive `400 Bad Request`.

## 5. Revenue Report API (/api/Reports)

- **Description**: Provides total revenue report (Admin only).
- **Endpoints**:
  - `GET /api/Reports`: Retrieve total revenue.
- **Requirements**:
  - Header: `Authorization: Bearer <token>`.
- **Responses**:
  - `200 OK`: `{ "totalRevenue": double }`.
  - `401 Unauthorized`: Invalid token.
  - `403 Forbidden`: Not an Admin.
- **Testing Instructions**:
  - **Tool**: Use Postman.
  - **Test** `GET /api/Reports`:
    1. Method: `GET`.
    2. URL: `http://localhost:7112/api/Reports`.
    3. Header: `Authorization: Bearer <token>` (role "Admin").
    4. Expected Result: `200 OK` with `{ "totalRevenue": 100.00 }` (based on sample data).
    5. Error Test: Use an invalid token to receive `401 Unauthorized`.

## 6. Booking Management API (/api/Bookings)

- **Description**: Manages booking and ticket generation (Employee only).

- **Endpoints**:

  - `POST /api/Bookings`: Create a new booking.
  - `PUT /api/Bookings/{id}/cancel`: Cancel a booking.
  - `GET /api/Bookings/{id}/ticket`: Generate a PDF ticket.

- **Requirements**:

  - Header: `Authorization: Bearer <token>`.

- **Responses**:

  - `200 OK`: Booking data or PDF file.
  - `201 Created`: Booking created successfully.
  - `400 Bad Request`: Invalid data.
  - `401 Unauthorized`: Invalid token.
  - `403 Forbidden`: Not an Employee or Admin.
  - `404 Not Found`: Booking not found.
  - `500 Internal Server Error`: Error generating PDF.

- **Testing Instructions**:

  - **Tool**: Use Postman.

  - **Test** `POST /api/Bookings`:

    1. Method: `POST`.

    2. URL: `http://localhost:7112/api/Bookings`.

    3. Header: `Authorization: Bearer <token>` (role "Employee" or "Admin").

    4. Body: `raw` (JSON):

       ```json
       {
         "employeeId": 2,
         "bookingDate": "2025-07-02T12:00:00",
         "customerDetails": {
           "name": "Charlie Brown",
           "dateOfBirth": "1995-03-10",
           "email": "charlie@example.com",
           "phoneNumber": "213-555-1234"
         },
         "seatInTripId": 2
       }
       ```

    5. Expected Result: `201 Created` with new booking.

    6. Error Test: Use a non-existent `seatInTripId` to receive `400 Bad Request`.

  - **Test** `PUT /api/Bookings/{id}/cancel`:

    1. Method: `PUT`.
    2. URL: `http://localhost:7112/api/Bookings/1/cancel`.
    3. Header: `Authorization: Bearer <token>` (role "Employee" or "Admin").
    4. Expected Result: `200 OK` with cancellation success message.
    5. Error Test: Use a non-existent ID to receive `404 Not Found`.

  - **Test** `GET /api/Bookings/{id}/ticket`:

    1. Method: `GET`.
    2. URL: `http://localhost:7112/api/Bookings/1/ticket`.
    3. Header: `Authorization: Bearer <token>` (role "Employee" or "Admin").
    4. Expected Result: `200 OK` with PDF file (Ticket_1.pdf).
    5. Error Test: Use a non-existent ID to receive `404 Not Found`.

## 7. Seat Management API (/api/SeatsInBusTrip)

- **Description**: Manages seat information for trips (Admin only).

- **Endpoints**:

  - `GET /api/SeatsInBusTrip`: Retrieve list of seats.
  - `GET /api/SeatsInBusTrip/{id}`: Retrieve seat details by ID.
  - `POST /api/SeatsInBusTrip`: Create a new seat.
  - `POST /api/SeatsInBusTrip/bulkByTrip`: Create multiple seats for a trip.
  - `PUT /api/SeatsInBusTrip/{id}`: Update seat by ID.
  - `DELETE /api/SeatsInBusTrip/{id}`: Delete seat by ID.

- **Requirements**:

  - Header: `Authorization: Bearer <token>`.

- **Responses**:

  - `200 OK`: List or details of seats.
  - `201 Created`: Seat created successfully.
  - `400 Bad Request`: Invalid data or seat already exists.
  - `401 Unauthorized`: Invalid token.
  - `403 Forbidden`: Not an Admin.
  - `404 Not Found`: Seat not found.
  - `400 Bad Request`: Cannot delete seat with associated bookings.

- **Testing Instructions**:

  - **Tool**: Use Postman.

  - **Test** `GET /api/SeatsInBusTrip`:

    1. Method: `GET`.
    2. URL: `http://localhost:7112/api/SeatsInBusTrip`.
    3. Header: `Authorization: Bearer <token>` (role "Admin").
    4. Expected Result: `200 OK` with seat list (e.g., `[{"seatInTripId": 1, "tripId": 1, ...}]`).

  - **Test** `POST /api/SeatsInBusTrip`:

    1. Method: `POST`.

    2. URL: `http://localhost:7112/api/SeatsInBusTrip`.

    3. Header: `Authorization: Bearer <token>` (role "Admin").

    4. Body: `raw` (JSON):

       ```json
       {
         "tripId": 1,
         "seatNumber": "A3",
         "isAvailable": true
       }
       ```

    5. Expected Result: `201 Created` with new seat.

    6. Error Test: Use an existing `seatNumber` to receive `400 Bad Request`.

  - **Test** `POST /api/SeatsInBusTrip/bulkByTrip`:

    1. Method: `POST`.

    2. URL: `http://localhost:7112/api/SeatsInBusTrip/bulkByTrip`.

    3. Header: `Authorization: Bearer <token>` (role "Admin").

    4. Body: `raw` (JSON):

       ```json
       {
         "tripId": 1,
         "seatNumbers": ["A4", "A5", "A6"]
       }
       ```

    5. Expected Result: `201 Created` with new seat list.

# Error Test: Use existing `seatNumbers` to receive `400 Bad Request`.
