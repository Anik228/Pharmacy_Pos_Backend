## Pharmacy POS System (ASP.NET Core Web API)

## Overview

Pharmacy POS System is a backend API developed using ASP.NET Core Web API with PostgreSQL as the database. It includes User Management and Medicine Management modules. Authentication and authorization are handled using JWT tokens and the [Authorize] attribute in ASP.NET.

## Features

# User Module

Create User (Admin)POST https://localhost:5001/api/User/add-user

Get All Users (Admin)GET https://localhost:5001/api/User/User

Delete User (Admin)DELETE https://localhost:5001/api/User/delete-user/{id}

Update User (Admin, User)PUT https://localhost:5001/api/User/update-user/{id}

Update Password (Admin, User)PUT https://localhost:5001/api/User/update-user-password/{id}

Find User (Admin, User)GET https://localhost:5001/api/User/find-a-user/{id}

Login (Admin, User)POST https://localhost:5001/api/User/login


## Medicine Module

Create Medicine (Admin)POST https://localhost:5001/api/Medicine/add-medicine

Get All Medicines (Admin)GET https://localhost:5001/api/Medicine/medicines

Delete Medicine (Admin)DELETE https://localhost:5001/api/User/delete-medicine/{id}

Update Medicine (Admin)PUT https://localhost:5001/api/Medicine/update-medicine/{id}

Find Medicine (Admin, User)GET https://localhost:5001/api/Medicine/find-a-medicine/{id}

Find Medicine by name (Admin, User)GET https://localhost:5001/api/Medicine/find-a-medicine-by-name/{name}

## Authentication & Authorization

JWT Token is used for authentication.

Users must include a valid JWT Token in the Authorization header for all requests.

[Authorize] attribute is used to restrict access to authorized users only.

## Database

PostgreSQL is used as the database.

Ensure PostgreSQL is installed and configured before running the API

## Technologies Used

ASP.NET Core Web API

Entity Framework Core

JWT Authentication

PostgreSQL

## Contact

For any inquiries, please contact aniklal2020@gmail.com.

