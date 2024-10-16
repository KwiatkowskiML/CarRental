CREATE TABLE cars (
    car_id SERIAL PRIMARY KEY,
    brand VARCHAR(100) NOT NULL,
    model VARCHAR(100) NOT NULL,
    year INT NOT NULL,
    status VARCHAR(50) NOT NULL, -- Available, Rented, Maintenance, etc.
    location VARCHAR(255),
    engine_capacity DECIMAL(3, 1) NOT NULL, -- in liters, e.g., 2.0, 3.5
    power INT NOT NULL, -- in horsepower
    fuel_type VARCHAR(50) NOT NULL, -- e.g., Petrol, Diesel, Electric, Hybrid
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);