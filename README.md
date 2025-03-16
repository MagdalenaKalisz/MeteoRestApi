Summary
The aim of this task is to build a simple API (backed by any kind of database). The application should be able to store weather forecast data in the database, based on provided latitude and longitude - you can use https://open-meteo.com/ to get a weather forecast. The API should be able to add (new longitude and latitude), delete or provide weather forecast. It should also provide an endpoint to list previously used longitudes and latitudes from the database, and choose them to provide the newest weather forecast.  

Application specification
It should be a RESTful API
You can use https://open-meteo.com/  for the geolocation of IP addresses and URLs
The application can be built in .net (preferable the newest one)
Usage of any free library which will help implement solution is acceptable (e.g. swagger)
It is preferable that the API operates using JSON (for both input and output)
The solution should also include base specs/tests coverage

How to submit
Create a public Git repository and share the link with us

Notes:
We will run the application on our local machines for testing purposes. This implies that the solution should provide a quick and easy way to get the system up and running, including test data (hint: you can add Docker support so we can run it easily)
We will test the behavior of the system under various "unfortunate" conditions (hint: How will the app behave when we take down the DB? How about the IPStack API?)
After we finish reviewing the solution, we'll invite you to Sofomo's office (or to a Zoom call) for a short discussion about the provided solution. We may also use that as an opportunity to ask questions and drill into the details of your implementation.

Bonus points:
Setup Web App (free version) on Microsoft Azure with your solution. DB can be mocked.


# Sofomo Meteo Rest API - Docker Setup

## Overview
This repository contains the `SofomoMeteoRestAPI` project, a .NET-based REST API with PostgreSQL support. This guide explains how to build, run, and deploy the application using Docker.

---

## Running the Application with Docker

### ** Prerequisites**
Ensure you have the following installed on your system:
- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)

---

### Environment Variables
Before running the application, ensure the following **environment variables** are set for PostgreSQL and PgAdmin (file: .env):

POSTGRES_HOST=db
POSTGRES_PORT=5432
POSTGRES_DB=meteo
POSTGRES_USER=meteo
POSTGRES_PASSWORD=meteo

PGADMIN_EMAIL=admin@admin.com
PGADMIN_PASSWORD=admin

### To run the application using Docker Compose:
docker-compose up --build

### Entrypoint Script
ensures that the database is created before the application starts.
if errors make it executable: chmod +x entrypoint.sh

### Access the Application:
API: Endpoint: http://localhost:5001
To access Scalar: http://localhost:5001/scalar/v1
Every Endpoint has short description here

### Choose the type of database:
In AppSettings.json, you can choose between InMemoryDatabase and PostgreSQL.
  "DatabaseSettings": 
    "UpdatePostgreSqlDb": true - Update PostgreSql - Keep true for first run so it will create all the tables
    "RunPostgreSqlDb": true - Use PostgreSql Database
    "RunJsonDb": false - Use InMemory Database based on Json files. Directory: Meteo.Persistence.JsonDb/MockDatabase

### Tests
To run tests go to the root of the solution and run Dotnet Test
  
