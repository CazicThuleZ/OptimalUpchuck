#!/bin/bash
set -e

# Enable UUID extension for UUID primary keys
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    -- Enable UUID extension for generating UUID primary keys
    CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
    
    -- Create schema for application (optional but good practice)
    CREATE SCHEMA IF NOT EXISTS optimal_upchuck;
    
    -- Grant all permissions on schema to application user
    GRANT ALL PRIVILEGES ON SCHEMA optimal_upchuck TO $POSTGRES_USER;
    
    -- Set default schema search path to include our schema
    ALTER DATABASE $POSTGRES_DB SET search_path TO optimal_upchuck, public;
    
    -- Log successful initialization
    SELECT 'Database initialized successfully' AS status;
EOSQL

echo "PostgreSQL database initialization completed successfully"