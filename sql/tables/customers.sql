CREATE TABLE customers (
    customer_id SERIAL PRIMARY KEY,
    user_id INT,
    driving_license_years INT NOT NULL
);