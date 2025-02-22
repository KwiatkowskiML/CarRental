#!/bin/bash

db_name="car_rental_db"
sql_files_path="/sql_tables"
sql_table_names=("users" "customers" "employees" "car_providers" "cars" "insurances" "offers" "rental_status" "rentals" "returns")

echo "Checking if database $db_name exists"
if psql -U "$POSTGRES_USER" -d postgres -tc "SELECT 1 FROM pg_database WHERE datname = '$db_name';" | grep -q 1; then
    echo "Database $db_name already exists"
else
    echo "Creating database $db_name"
    psql -U "$POSTGRES_USER" -d postgres -c "CREATE DATABASE $db_name;"
fi

# Apply each SQL file to the database
for table in "${sql_table_names[@]}"; do
    echo "Applying $sql_files_path/$table.sql to $db_name"
    psql -U "$POSTGRES_USER" -d "$db_name" -f "$sql_files_path/$table.sql"
done

# Initialize rental_status table
echo "Initializing rental_status table"
psql -U "$POSTGRES_USER" -d "$db_name" -c "
INSERT INTO rental_status (description)
VALUES 
    ('Confirmed'),
    ('Pending return'),
    ('Completed')
ON CONFLICT DO NOTHING;"
