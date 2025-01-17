CREATE TABLE car_providers (
    car_provider_id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    api_key VARCHAR(255) UNIQUE NOT NULL,
    contact_email VARCHAR(255) NOT NULL,
    contact_phone VARCHAR(50),
    created_at TIMESTAMP WITH TIME ZONE
);