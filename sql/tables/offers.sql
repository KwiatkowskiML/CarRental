CREATE TABLE offers (
    offer_id SERIAL PRIMARY KEY,
    insurance_id INT REFERENCES insurances(insurance_id),
    customer_id INT REFERENCES customers(customer_id),
    car_id INT REFERENCES cars(car_id),
    total_price DECIMAL(10, 2) NOT NULL,
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE,
    has_gps BOOLEAN,
    has_child_seat BOOLEAN
);