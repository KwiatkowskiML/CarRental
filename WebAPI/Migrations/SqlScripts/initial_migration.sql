CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE car_providers (
    car_provider_id integer GENERATED BY DEFAULT AS IDENTITY,
    name character varying(255) NOT NULL,
    api_key character varying(255) NOT NULL,
    contact_email character varying(255) NOT NULL,
    contact_phone character varying(50),
    created_at timestamp with time zone DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT car_providers_pkey PRIMARY KEY (car_provider_id)
);

CREATE TABLE insurances (
    insurance_id integer GENERATED BY DEFAULT AS IDENTITY,
    price numeric NOT NULL,
    name text NOT NULL,
    CONSTRAINT insurances_pkey PRIMARY KEY (insurance_id)
);

CREATE TABLE rental_status (
    rental_status_id integer GENERATED BY DEFAULT AS IDENTITY,
    description text NOT NULL,
    CONSTRAINT rental_status_pkey PRIMARY KEY (rental_status_id)
);

CREATE TABLE users (
    user_id integer GENERATED BY DEFAULT AS IDENTITY,
    email character varying(255) NOT NULL,
    first_name character varying(100) NOT NULL,
    last_name character varying(100) NOT NULL,
    age integer NOT NULL,
    location character varying(255),
    created_at timestamp with time zone DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT users_pkey PRIMARY KEY (user_id)
);

CREATE TABLE cars (
    car_id integer GENERATED BY DEFAULT AS IDENTITY,
    car_provider_id integer NOT NULL,
    license_plate character varying(20) NOT NULL,
    brand character varying(100) NOT NULL,
    model character varying(100) NOT NULL,
    year integer NOT NULL,
    status character varying(50) NOT NULL,
    location character varying(255),
    engine_capacity numeric(3,1) NOT NULL,
    power integer NOT NULL,
    fuel_type character varying(50) NOT NULL,
    description text,
    created_at timestamp with time zone DEFAULT (CURRENT_TIMESTAMP),
    base_price numeric(10,2) NOT NULL,
    images text[],
    CONSTRAINT cars_pkey PRIMARY KEY (car_id),
    CONSTRAINT cars_car_provider_id_fkey FOREIGN KEY (car_provider_id) REFERENCES car_providers (car_provider_id) ON DELETE CASCADE
);

CREATE TABLE customers (
    customer_id integer GENERATED BY DEFAULT AS IDENTITY,
    user_id integer NOT NULL,
    driving_license_years integer NOT NULL,
    CONSTRAINT customers_pkey PRIMARY KEY (customer_id),
    CONSTRAINT "FK_customers_users_user_id" FOREIGN KEY (user_id) REFERENCES users (user_id) ON DELETE RESTRICT
);

CREATE TABLE employees (
    employee_id integer GENERATED BY DEFAULT AS IDENTITY,
    user_id integer,
    role character varying(100) NOT NULL,
    CONSTRAINT employees_pkey PRIMARY KEY (employee_id),
    CONSTRAINT employees_user_id_fkey FOREIGN KEY (user_id) REFERENCES users (user_id)
);

CREATE TABLE offers (
    offer_id integer GENERATED BY DEFAULT AS IDENTITY,
    total_price numeric(10,2) NOT NULL,
    customer_id integer NOT NULL,
    car_id integer NOT NULL,
    insurance_id integer NOT NULL,
    start_date date NOT NULL,
    end_date date NOT NULL,
    created_at timestamp with time zone DEFAULT (CURRENT_TIMESTAMP),
    has_gps boolean NOT NULL,
    has_child_seat boolean NOT NULL,
    CONSTRAINT offers_pkey PRIMARY KEY (offer_id),
    CONSTRAINT "FK_offers_customers_customer_id" FOREIGN KEY (customer_id) REFERENCES customers (customer_id) ON DELETE CASCADE,
    CONSTRAINT offers_car_id_fkey FOREIGN KEY (car_id) REFERENCES cars (car_id) ON DELETE CASCADE,
    CONSTRAINT offers_insurance_id_fkey FOREIGN KEY (insurance_id) REFERENCES insurances (insurance_id) ON DELETE CASCADE
);

CREATE TABLE rentals (
    rental_id integer GENERATED BY DEFAULT AS IDENTITY,
    offer_id integer NOT NULL,
    rental_status_id integer NOT NULL,
    created_at timestamp with time zone DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT rentals_pkey PRIMARY KEY (rental_id),
    CONSTRAINT rentals_offer_id_fkey FOREIGN KEY (offer_id) REFERENCES offers (offer_id),
    CONSTRAINT rentals_rental_status_id_fkey FOREIGN KEY (rental_status_id) REFERENCES rental_status (rental_status_id) ON DELETE CASCADE
);

CREATE TABLE returns (
    return_id integer GENERATED BY DEFAULT AS IDENTITY,
    rental_id integer,
    return_date date NOT NULL,
    condition_description text,
    photo_url character varying(255),
    processed_by integer,
    created_at timestamp with time zone DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT returns_pkey PRIMARY KEY (return_id),
    CONSTRAINT returns_rental_id_fkey FOREIGN KEY (rental_id) REFERENCES rentals (rental_id)
);

CREATE UNIQUE INDEX car_providers_api_key_key ON car_providers (api_key);

CREATE INDEX "IX_cars_car_provider_id" ON cars (car_provider_id);

CREATE UNIQUE INDEX cars_license_plate_key ON cars (license_plate);

CREATE UNIQUE INDEX "IX_customers_user_id" ON customers (user_id);

CREATE INDEX "IX_employees_user_id" ON employees (user_id);

CREATE INDEX "IX_offers_car_id" ON offers (car_id);

CREATE INDEX "IX_offers_customer_id" ON offers (customer_id);

CREATE INDEX "IX_offers_insurance_id" ON offers (insurance_id);

CREATE UNIQUE INDEX "IX_rentals_offer_id" ON rentals (offer_id);

CREATE INDEX "IX_rentals_rental_status_id" ON rentals (rental_status_id);

CREATE INDEX "IX_returns_rental_id" ON returns (rental_id);

CREATE UNIQUE INDEX users_email_key ON users (email);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250117124300_InitialCreate', '8.0.0');

COMMIT;


