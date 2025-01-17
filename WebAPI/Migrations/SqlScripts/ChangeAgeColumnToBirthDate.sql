START TRANSACTION;

ALTER TABLE users ADD birth_date date;

UPDATE users SET birth_date = NOW() - INTERVAL '1 year' * age;

ALTER TABLE users ALTER COLUMN birth_date SET NOT NULL;

ALTER TABLE users DROP COLUMN age;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250117134851_ChangeAgeColumnToBirthDate', '8.0.0');

COMMIT;

