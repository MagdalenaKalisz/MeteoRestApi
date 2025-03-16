#!/bin/sh
set -e

# Wait until PostgreSQL is fully ready
echo "Waiting for PostgreSQL to be ready..."
until pg_isready -h db -p 5432 -U meteo; do
  echo "PostgreSQL is not ready yet. Retrying..."
  sleep 2
done
echo "PostgreSQL is ready."

# Ensure the DB is fully initialized before applying migrations
echo "Checking if database is ready to accept commands..."
until PGPASSWORD=meteo psql -h db -U meteo -d meteo -c "SELECT 1" >/dev/null 2>&1; do
  echo "Database is up, but still initializing. Waiting..."
  sleep 2
done
echo "Database is fully initialized."

# Apply EF Core migrations
echo "Running database migrations..."
exec dotnet Meteo.Api.dll
