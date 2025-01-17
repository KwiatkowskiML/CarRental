#!/bin/bash

# Start the Cloud SQL proxy in the background
/cloud-sql-proxy --unix-socket /cloudsql &

# Start the .NET application
dotnet WebAPI.dll