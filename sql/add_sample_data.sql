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
INSERT INTO cars (car_provider_id, license_plate, brand, model, year, status, location, engine_capacity, power, fuel_type, description) VALUES
(1, 'ABC123', 'BMW', '330i', 2023, 'available', 'New York', 2.0, 255, 'Petrol', 'Luxury sedan with premium features'),
(1, 'XYZ789', 'Mercedes', 'C300', 2022, 'available', 'Los Angeles', 2.0, 255, 'Petrol', 'Executive class vehicle'),
(2, 'DEF456', 'Audi', 'Q5', 2023, 'available', 'Chicago', 2.0, 261, 'Petrol', 'Premium SUV');

-- Insert rentals
INSERT INTO rentals (customer_id, car_id, start_date, end_date, status, has_gps, has_child_seat, insurance_type) VALUES
(1, 1, '2024-01-01', '2024-01-07', 'completed', true, false, 'Full Coverage'),
(2, 2, '2024-02-01', '2024-02-05', 'completed', true, true, 'Basic'),
(3, 3, '2024-03-01', '2024-03-10', 'active', false, false, 'Premium');

-- Insert returns
INSERT INTO returns (rental_id, return_date, condition_description, photo_url, processed_by) VALUES
(1, '2024-01-07', 'Good condition, minor scratches', 'https://storage.example.com/photos/return1.jpg', 1),
(2, '2024-02-05', 'Excellent condition', 'https://storage.example.com/photos/return2.jpg', 2);