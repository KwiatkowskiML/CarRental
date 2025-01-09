CREATE TABLE customers (
    customer_id SERIAL PRIMARY KEY,
    user_id INT REFERENCES users(user_id),
    driving_license_years INT NOT NULL
);