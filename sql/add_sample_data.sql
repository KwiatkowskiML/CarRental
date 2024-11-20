-- Insert users
INSERT INTO users (email, first_name, last_name, age, location) VALUES
('john.doe@email.com', 'John', 'Doe', 30, 'New York'),
('jane.smith@email.com', 'Jane', 'Smith', 25, 'Los Angeles'),
('mike.wilson@email.com', 'Mike', 'Wilson', 35, 'Chicago');

-- Insert car providers
INSERT INTO car_providers (name, api_key, contact_email, contact_phone) VALUES
('Premium Cars', 'pc_key_123', 'contact@premiumcars.com', '123-456-7890'),
('Luxury Rentals', 'lr_key_456', 'support@luxuryrentals.com', '098-765-4321');

-- Insert employees
INSERT INTO employees (user_id, role) VALUES
(1, 'Manager'),
(2, 'Agent'),
(3, 'Support');

-- Insert customers
INSERT INTO customers (user_id, driving_license_years) VALUES
(1, 5),
(2, 3),
(3, 8);

-- Insert cars
INSERT INTO cars (car_provider_id, license_plate, brand, model, year, status, location, engine_capacity, power, fuel_type, base_price, description) VALUES
(1, 'ABC123', 'BMW', '330i', 2023, 'available', 'New York', 2.0, 255, 'Petrol', 100.00, 'Luxury sedan with premium features'),
(1, 'XYZ789', 'Mercedes', 'C300', 2022, 'available', 'Los Angeles', 2.0, 255, 'Petrol', 200.00,'Executive class vehicle'),
(2, 'DEF456', 'Audi', 'Q5', 2023, 'available', 'Chicago', 2.0, 261, 'Petrol', 150.00, 'Premium SUV');

INSERT INTO insurances (price, name)
VALUES 
    (50.00, 'Standard Insurance'),
    (100.00, 'Full Insurance');

INSERT INTO offers (insurance_id, customer_id, car_id, total_price, start_date, end_date, created_at, has_gps, has_child_seat)
VALUES 
    (1, 1, 1, 1000, '2024-11-19', '2024-11-25', CURRENT_TIMESTAMP, TRUE, FALSE),
    (2, 2, 2, 2000, '2024-11-10', '2024-11-17', CURRENT_TIMESTAMP, FALSE, TRUE);

-- Insert rentals
INSERT INTO rentals (offer_id, status, created_at) VALUES
(1, 'active', CURRENT_TIMESTAMP),
(2, 'completed', CURRENT_TIMESTAMP);

-- Insert returns
INSERT INTO returns (rental_id, return_date, condition_description, photo_url, processed_by) VALUES
(35, '2024-11-17', 'Excellent condition', 'https://storage.example.com/photos/return2.jpg', 2);
