CREATE TABLE rentals (
    rental_id SERIAL PRIMARY KEY,
    offer_id INT REFERENCES offers(offer_id),
    rental_status_id  INT REFERENCES rental_status(rental_status_id),
    created_at TIMESTAMP WITH TIME ZONE,
    UNIQUE(offer_id)
);