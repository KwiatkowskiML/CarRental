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
(2, 'DEF456', 'Audi', 'Q5', 2023, 'available', 'Chicago', 2.0, 261, 'Petrol', 150.00, 'Premium SUV'),
(1, 'GHI789', 'Tesla', 'Model 3', 2024, 'available', 'New York', 0.0, 283, 'Electric', 180.00, 'Electric performance sedan'),
(2, 'JKL012', 'Porsche', 'Cayenne', 2023, 'available', 'Los Angeles', 3.0, 335, 'Petrol', 250.00, 'Luxury SUV'),
(1, 'MNO345', 'Toyota', 'Camry', 2023, 'available', 'Chicago', 2.5, 203, 'Hybrid', 80.00, 'Reliable hybrid sedan'),
(2, 'PQR678', 'Lexus', 'RX350', 2024, 'available', 'New York', 3.5, 295, 'Petrol', 170.00, 'Premium crossover SUV');


INSERT INTO insurances (price, name)
VALUES 
    (50.00, 'Standard Insurance'),
    (100.00, 'Full Insurance');

INSERT INTO offers (insurance_id, customer_id, car_id, total_price, start_date, end_date, created_at, has_gps, has_child_seat)
VALUES 
    (1, 1, 1, 1000, '2024-11-19', '2024-11-25', CURRENT_TIMESTAMP, TRUE, FALSE),
    (2, 2, 2, 2000, '2024-11-10', '2024-11-17', CURRENT_TIMESTAMP, FALSE, TRUE),
    (1, 3, 4, 1500, '2024-12-01', '2024-12-07', CURRENT_TIMESTAMP, TRUE, TRUE),
    (2, 1, 5, 2500, '2024-12-15', '2024-12-22', CURRENT_TIMESTAMP, TRUE, FALSE);

INSERT INTO rental_status (description)
VALUES 
    ('Confirmed'),
    ('Pending return'),
    ('Completed')
ON CONFLICT DO NOTHING;

INSERT INTO rentals (offer_id, rental_status_id, created_at) VALUES
(1, 3, CURRENT_TIMESTAMP - INTERVAL '30 days'),
(2, 2, CURRENT_TIMESTAMP - INTERVAL '7 days'),
(3, 1, CURRENT_TIMESTAMP),
(4, 1, CURRENT_TIMESTAMP);

UPDATE cars 
SET images = ARRAY['https://storage.googleapis.com/car-images-dev-0/Audi_Q5_FY_50_TFSI_e_Facelift_IMG_5931.jpg']
WHERE brand ILIKE 'Audi%';

UPDATE cars 
SET images = ARRAY['https://storage.googleapis.com/car-images-dev-0/Mercedes-Benz-C300-Coupe-3dosetki.pl-3.jpg']
WHERE brand ILIKE 'Mercedes%';

UPDATE cars 
SET images = ARRAY['https://storage.googleapis.com/car-images-dev-0/bmw-330.jpeg']
WHERE brand ILIKE 'BMW%';

UPDATE cars 
SET images = ARRAY['https://storage.googleapis.com/car-images-dev-0/porsche.jpeg']
WHERE brand ILIKE 'Porsche%';

UPDATE cars 
SET images = ARRAY['https://storage.googleapis.com/car-images-dev-0/toyota.jpeg']
WHERE brand ILIKE 'Toyota%';

UPDATE cars
SET images = ARRAY['https://storage.googleapis.com/car-images-dev-0/tesla.jpg']
WHERE brand ILIKE 'Tesla%';

UPDATE cars
SET images = ARRAY['https://storage.googleapis.com/car-images-dev-0/lexus.jpg']
WHERE brand ILIKE 'Lexus%';