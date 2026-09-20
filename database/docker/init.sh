#!/bin/sh
set -e
cd /docker-init
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" -f install.sql
