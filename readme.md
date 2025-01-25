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