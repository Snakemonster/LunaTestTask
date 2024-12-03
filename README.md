# Luna test task
## Project overview
This repo implement test task API on C# dotnet 8.0.110. In this project was used libraries:
 - EF(Entity Framework) core for connection to DataBase
 - Npgsql for PostgreSQL
 - JwtBearer for authentication
 - BCrypt for encryption passwords

API that was implemented in this project:
- POST /users/register - create new user
- POST /users/login - send Username or Email and Password, return auth key
- POST /tasks - create new task for user (authenticated)
- GET /tasks - get all user tasks (authenticated), also have Query for sorting by DueDate, Status and Priority
- GET /tasks/{id} - get only one task by it id (authenticated)
- PUT /tasks/{id} - update user task by id (authenticated)
- DELETE /tasks/{id} - delete user task by id (authenticated)

Structure of project:
- Contexts - folder where stores all DbContexts
- Controllers - folder where stores controllers
- Models - folder where stores all kind of models
  - Requests - model for request body
  - And regular models
- Configuration - stores JWT key
- Dockerfile

## How to run project
To run project you should copy this repository and then run it in Rider/Visual stuio/VS Code.

Also there is option to run it in docker containers with database. To do that copy this repo and then run this command:
```shell
docker compose up -d
```
It will automatically create database PostgreSQL with necessarily tables and connections between them. Then it run PostgreSQL web interface (for more comfortable view) and backend container itself. Backend container generates by Dockerfile in LunaTestTask folder.
## Configuring
You can configure some options in docker compose file, such as passwords, ports, JWT secret key.
