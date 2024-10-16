CREATE TABLE rentals (
    rental_id SERIAL PRIMARY KEY,
    user_id INT REFERENCES users(user_id),
    car_id INT REFERENCES cars(car_id),
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    status VARCHAR(50) NOT NULL, -- Active, Completed, Cancelled, etc.
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);