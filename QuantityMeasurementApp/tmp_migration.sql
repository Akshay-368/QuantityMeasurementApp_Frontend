CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260331111719_InitialPostgres') THEN
    CREATE TABLE "Histories" (
        "Id" uuid NOT NULL,
        "Operation" text NOT NULL,
        "Value1" double precision NOT NULL,
        "Unit1" text NOT NULL,
        "Value2" double precision,
        "Unit2" text,
        "TargetUnit" text,
        "Scalar" double precision,
        "Result" double precision NOT NULL,
        "ResultUnit" text NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_Histories" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260331111719_InitialPostgres') THEN
    CREATE TABLE "Users" (
        "Id" uuid NOT NULL,
        "Username" text NOT NULL,
        "PasswordHash" bytea NOT NULL,
        "PasswordSalt" bytea NOT NULL,
        "Created" timestamp with time zone NOT NULL,
        "RefreshToken" text NOT NULL,
        "TokenCreated" timestamp with time zone NOT NULL,
        "TokenExpires" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260331111719_InitialPostgres') THEN
    CREATE UNIQUE INDEX "IX_Users_Username" ON "Users" ("Username");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260331111719_InitialPostgres') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260331111719_InitialPostgres', '8.0.8');
    END IF;
END $EF$;
COMMIT;

