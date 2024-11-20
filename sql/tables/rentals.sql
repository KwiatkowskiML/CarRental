CREATE TABLE rentals (
    rental_id SERIAL PRIMARY KEY,
    offer_id INT REFERENCES offers(offer_id),
    status VARCHAR(50) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE
);