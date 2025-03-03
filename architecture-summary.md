# Service Provision System - Architecture Summary

## Overview

This application is a full-stack solution for connecting service providers with consumers. It follows a modern architecture with a C#/.NET API backend and a React single-page application (SPA) frontend.

## Technology Stack

### Backend
- **Framework**: ASP.NET Core 6.0 Web API
- **Language**: C# 10
- **Database**: Microsoft SQL Server
- **ORM**: Entity Framework Core
- **Authentication**: JWT (JSON Web Tokens)
- **Project Structure**: Clean architecture with separation of concerns

### Frontend
- **Framework**: React 18
- **Routing**: React Router v6
- **HTTP Client**: Axios
- **CSS Framework**: Tailwind CSS
- **State Management**: React Context API
- **Design**: Mobile-first, responsive design

## Architecture

### Backend Architecture

The backend follows a layered architecture with these projects:

1. **ServiceProvisionSystem.API**
   - Entry point for the application
   - Controllers handling HTTP requests
   - Middleware configuration
   - JWT authentication
   - CORS configuration
   - Swagger API documentation

2. **ServiceProvisionSystem.Core**
   - Domain models
   - Interface definitions
   - Business logic abstractions

3. **ServiceProvisionSystem.Data**
   - Entity Framework DbContext
   - Data access layer
   - Migrations
   - Data seeding
   - Repository implementations

4. **ServiceProvisionSystem.Services**
   - Business logic implementation
   - Service layer orchestrating operations

### Database Schema

The database consists of three primary entities:

1. **Person** (extends IdentityUser)
   - Basic user information
   - Account type (Consumer or Provider)
   - Profile details

2. **Service**
   - Service details
   - Pricing information
   - Categorization

3. **Provision**
   - Links Person and Service
   - Tracks status and request details
   - Manages the service lifecycle

### Frontend Architecture

The React frontend follows a component-based architecture:

1. **Core Structure**
   - App entry point with routing
   - Context providers for state management
   - Layout components for consistent UI

2. **Components**
   - Common UI components (buttons, cards, forms)
   - Feature-specific components for services and provisions
   - Authentication components
   - Layout components (header, footer)

3. **Pages**
   - Container components for each route
   - Composition of smaller components
   - Data fetching and state management

4. **Services/API**
   - Axios-based API client
   - Service-specific API modules
   - Authentication handling

5. **Utilities**
   - Formatting helpers
   - Validation utilities
   - Custom hooks

## Authentication Flow

1. User registers with email, password, and user type (Consumer/Provider)
2. User logs in with credentials
3. Backend validates credentials and issues a JWT token
4. Frontend stores the token in localStorage
5. Token is included in request headers for authenticated API calls
6. Protected routes check for token presence and validity

## Key Features

1. **User Management**
   - Registration and login
   - Profile management
   - Role-based authorization

2. **Service Management**
   - Service browsing and filtering
   - Service details view
   - Service request creation

3. **Provision Management**
   - Service request tracking
   - Status updates
   - Completion workflow

## Security Considerations

1. **Authentication**
   - JWT with secure key storage
   - Token expiration and refresh
   - Password hashing with Identity

2. **Authorization**
   - Role-based access control
   - Route protection on both frontend and backend
   - Data access restrictions

3. **Data Protection**
   - Input validation
   - HTTPS enforcement
   - SQL protection via EF Core

## Deployment Architecture

1. **Backend**
   - .NET API deployed to IIS or Azure App Service
   - Database on SQL Server (local or Azure SQL)

2. **Frontend**
   - Static files served from CDN or web server
   - Client-side routing with history API fallback
