# Employee Pairs Analyzer

## Overview

This application identifies the pair of employees who have worked together on common projects for the longest period of time.

It processes a CSV file, calculates overlapping working days between employees on common projects, and displays the results in a user-friendly UI.

## Features

- Upload CSV file from the UI
- Supports multiple date formats
- Handles `NULL` in `DateTo` as today's date
- Calculates overlapping working days between employees
- Identifies the top pair with the longest collaboration time
- Displays:
  - Common projects
  - Days worked together
  - Top pair summary
- Handles invalid data:
  - Collects row-level validation errors
  - Continues processing valid rows
- UI enhancements:
  - Loading spinner
  - Error messages
  - Pagination
  - Sorting for validation errors

## Tech Stack

### Backend
- ASP.NET Core Web API
- C#
- Layered architecture: Controller → Service → DTOs

### Frontend
- Angular
- TypeScript
- Angular Material

## Input Format

The input data must be provided in a CSV file with the following structure:

## How to Run
Make sure you have installed:

* .NET SDK
* Node.js
* Angular CLI

=> Backend:
1) cd EmployeePairs.Api 
2) dotnet restore
3) dotnet run

=> Frontend
1) cd employee-pairs-ui
2) npm install
3) ng serve

You can test with the exmaple file
