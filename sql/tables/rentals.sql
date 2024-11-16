CREATE TABLE rentals (
    rental_id SERIAL PRIMARY KEY,
    customer_id INT REFERENCES customers(customer_id),
    insurance_id INT REFERENCES insurances(insurance_id),
    car_id INT REFERENCES cars(car_id),
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    status VARCHAR(50) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE,
    has_gps BOOLEAN,
    has_child_seat BOOLEAN
);