# Car Rental Database Documentation

This database is designed to support a car rental platform that manages both direct rentals and integrations with external car providers.

## Core Tables

### 1. Users Table
- Stores basic information about all platform users
- Fields:
  - `user_id`: Unique identifier (SERIAL PRIMARY KEY)
  - `email`: User's email address (UNIQUE)
  - `first_name`, `last_name`: User's name
  - `age`: User's age
  - `location`: User's location
  - `created_at`: Account creation timestamp

### 2. Customers Table
- Extends Users table for rental customers
- Fields:
  - `customer_id`: Unique identifier (SERIAL PRIMARY KEY)
  - `user_id`: Reference to users table
  - `driving_license_years`: Years of driving experience

### 3. Employees Table
- Platform staff information
- Fields:
  - `employee_id`: Unique identifier (SERIAL PRIMARY KEY)
  - `user_id`: Reference to users table
  - `role`: Employee's role in the system
  - `created_at`: Employment start timestamp

### 4. Car Providers Table
- Information about external car rental providers
- Fields:
  - `car_provider_id`: Unique identifier (SERIAL PRIMARY KEY)
  - `name`: Provider company name
  - `api_key`: Unique API key for integration
  - `contact_email`: Primary contact email
  - `contact_phone`: Contact phone number
  - `created_at`: Integration timestamp

### 5. Cars Table
- Comprehensive vehicle inventory
- Fields:
  - `car_id`: Unique identifier (SERIAL PRIMARY KEY)
  - `car_provider_id`: Reference to provider
  - `license_plate`: Unique vehicle identifier
  - `brand`, `model`: Vehicle make and model
  - `year`: Manufacturing year
  - `status`: Current availability status
  - `location`: Current vehicle location
  - `engine_capacity`: Engine size in liters
  - `power`: Engine power
  - `fuel_type`: Type of fuel used
  - `description`: Additional vehicle details
  - `created_at`: Record creation timestamp

### 6. Rentals Table
- Tracks all rental transactions
- Fields:
  - `rental_id`: Unique identifier (SERIAL PRIMARY KEY)
  - `customer_id`: Reference to customer
  - `car_id`: Reference to rented vehicle
  - `start_date`, `end_date`: Rental period
  - `status`: Current rental status
  - `has_gps`: GPS add-on flag
  - `has_child_seat`: Child seat add-on flag
  - `insurance_type`: Selected insurance coverage
  - `created_at`: Booking timestamp

### 7. Returns Table
- Documents vehicle return process
- Fields:
  - `return_id`: Unique identifier (SERIAL PRIMARY KEY)
  - `rental_id`: Reference to rental
  - `return_date`: Actual return date
  - `condition_description`: Vehicle condition notes
  - `photo_url`: Return inspection photos
  - `processed_by`: Employee who handled return
  - `created_at`: Return processing timestamp

## Key Workflows

### 1. Customer Registration
```sql
INSERT INTO users (email, first_name, last_name, age, location)
VALUES (...);
INSERT INTO customers (user_id, driving_license_years)
VALUES (...);
```

### 2. Vehicle Rental Process
```sql
-- Check car availability
SELECT * FROM cars WHERE status = 'available' AND location = ?;

-- Create rental
INSERT INTO rentals (customer_id, car_id, start_date, end_date, status)
VALUES (...);

-- Update car status
UPDATE cars SET status = 'rented' WHERE car_id = ?;
```

### 3. Return Process
```sql
-- Record return
INSERT INTO returns (rental_id, return_date, condition_description, processed_by)
VALUES (...);

-- Update rental status
UPDATE rentals SET status = 'completed' WHERE rental_id = ?;

-- Update car status
UPDATE cars SET status = 'available' WHERE car_id = ?;
```

## Relationships

1. **Users → Customers/Employees**
   - One-to-one relationship through `user_id`
   - Separates user authentication from role-specific data

2. **Cars → Car Providers**
   - Many-to-one relationship through `car_provider_id`
   - Tracks vehicle ownership and provider integration

3. **Rentals → (Customers, Cars)**
   - Links customers to their rented vehicles
   - Maintains rental history and current status

4. **Returns → Rentals**
   - One-to-one relationship through `rental_id`
   - Documents the completion of rental transactions
