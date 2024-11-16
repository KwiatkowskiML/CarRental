CREATE TABLE cars (
    car_id SERIAL PRIMARY KEY,
    car_provider_id INT REFERENCES car_providers(car_provider_id),
    license_plate VARCHAR(20) UNIQUE NOT NULL,
    brand VARCHAR(100) NOT NULL,
    model VARCHAR(100) NOT NULL,
    year INT NOT NULL,
    status VARCHAR(50) NOT NULL,
    location VARCHAR(255),
    engine_capacity DECIMAL(3, 1) NOT NULL,
    power INT NOT NULL,
    fuel_type VARCHAR(50) NOT NULL,
    base_price DECIMAL(10, 2) NOT NULL,
    description TEXT,
    created_at TIMESTAMP WITH TIME ZONE
);