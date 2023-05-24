# Overview

The PatternPal extension offers valuable research opportunities, thus
requiring the implementation of an adequate logging solution. The
logging system operates in conjunction with PatternPal's service module
to process source code, recognition scores and other relevant data for a
specific snapshot appropriately.

## Technical Stack

- Operating system: A Linux distribution that supports Docker and
images for the ASP.NET core runtime and .NET SDK. Examples include
Ubuntu, CentOS, or Debian.

- Containerization: Docker for containerization of the application
server. Making it easier to deploy, scale and manage containers.

- Programming language: C# is used for all application components
of the logging system.

- Dependency management: NuGet for managing .NET packages.

- Relational database system: PostgreSQL is used as the database
system for the entire application.

- Communication protocol: gRPC is used as the communication protocol
between the server and clients.

---

## Logging server structure

### ASP.NET API Structure

For the logging server, we use a generic ASP.NET API structure. This
means separating the routing from the business logic by writing it into
a separate service class. Each time the server receives a request, a new
class instance will be initialized and can implement different classes
through dependency injection.

### Repository Pattern

We use the Repository pattern to access our PostgreSQL database in order
to decouple the logic from the other parts. A software architecture
pattern called Repository offers a neat and modular way to communicate
with databases. It achieves separation of concerns and maintainability
in our application by using repository classes in a separate layer that
are in charge of querying and updating the database. Furthermore this
prevents redundancy and a single place for all communication.

### Progsnap2

We use the [Progsnap2](https://cssplice.github.io/progsnap2/) model to log information from the extension.
Progsnap2 is a specification for datasets that record information about
the programming process, such as modifications to the source code and
debugging details during the building of projects. This enables us to
gather extensive and comprehensive data about programmers' work
processes, which can be useful for examining and enhancing software
development procedures.

### Database & ORM

A PostgreSQL database is used by our logging server to store the data
that is logged. We employ the ORM (Object-Relational Mapping) tool
Entity Framework (EF) to represent and communicate with our PostgreSQL
database. Using EF, you can easily conduct Create, Read, Update, Delete
activities on the data by mapping database tables to classes in the
code.

### CSV vs. SQL format

Although the Progsnap2 model suggests utilizing CSV format for logging
data, we choose to store the information in SQL format, specifically
using PostgreSQL, for simpler reads and writes. Large datasets can be
handled with ease by SQL's robust querying features. Additionally, SQL
data is adaptable for many use cases and can be easily converted to a
CSV format for research purposes.