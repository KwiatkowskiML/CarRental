CREATE TABLE employees (
    employee_id SERIAL PRIMARY KEY,
    user_id INT REFERENCES users(user_id),
    role VARCHAR(100) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);