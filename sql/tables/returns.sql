CREATE TABLE returns (
    return_id SERIAL PRIMARY KEY,
    rental_id INT REFERENCES rentals(rental_id),
    return_date DATE NOT NULL,
    condition_description TEXT,
    photo_url VARCHAR(255),
    processed_by INT REFERENCES employees(employee_id),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);