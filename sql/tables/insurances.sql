CREATE TABLE insurances (
    insurance_id SERIAL PRIMARY KEY,
    price DECIMAL(10, 2) NOT NULL,
    name VARCHAR(100) NOT NULL
);