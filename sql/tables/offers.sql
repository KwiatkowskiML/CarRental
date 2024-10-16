CREATE TABLE offers (
    offer_id SERIAL PRIMARY KEY,
    car_id INT REFERENCES cars(car_id),
    daily_rate DECIMAL(10, 2) NOT NULL,
    insurance_rate DECIMAL(10, 2) NOT NULL,
    valid_until TIMESTAMP WITH TIME ZONE NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);
