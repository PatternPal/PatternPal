# Logging Server

The PatternPal extension offers valuable research opportunities, thus
requiring the implementation of an adequate logging solution. The
logging system operates in conjunction with PatternPal's service module
to process source code, recognition scores and other relevant data for a
specific snapshot appropriately.

## System requirements

### Hardware requirements (minimum)

- Processor: Dual-core processor

- Memory: 4GB RAM

- Storage: 32GB

### Software requirements

- Operating system: Linux distribution (Ubuntu, CentOS, or Debian)

- Container manager: Latest stable version of Docker

### Docker images

- .NET SDK image

- ASP.NET Core Runtime image

### Other

- Remote database server: PostgreSQL; A remote PostgreSQL database
server. This can be a cloud-based, managed database service, or a
dedicated server.

## Installation

1. Install Linux: Install a compatible Linux distribution on the target
server. Follow the installation instructions of the chosen Linux
Distribution.

2. Install Docker: Install Docker on the Linux server. Follow the
docker installation instructions for the installed OS
[[here](https://docs.docker.com/desktop/install/linux-install/)]{style="color: blue"}.

3. Pull Docker Image: Pull the Docker image for the application from
the docker repository or Docker Hub. The current Docker repository
is 'siem2l/patternpalloggingserver:latest'

4. Run Docker Container: Create and run the pulled docker container
from step 3.

5. Setup Database: Set up a remote PostgreSQL database server,
Following the instruction provided by your chosen database service
or server.

6. Configuration: Update the application settings or configuration
files to connect to the database using the correct connection
string. This can be done using volume-mounted files.

7. Test Application: Ensure the server is running correctly and
available for remote clients to communicate.

This is the high-level overview on how to install the application. Some
steps require additional guidance that will be explained in the
following sections.

### Pull the Docker Image

For the publishing of our application we use Docker Hub. This
application can host container images and as such it hosts the image of
the Logging server. By using this system continuous deployment is easily
achievable. As of right now the repository is hosted at LINK. To pull
the image the following docker command should be run 'docker pull
siem2l/patternpalloggingserver:latest'

### Network configuration

For the application to be running properly the network settings of the
Linux server need to be configured. This includes opening specific ports
such as 443 and 80 for HTTPS and HTTP incoming traffic. This is usually
done by running the docker instance and exposing the ports as arguments
in step 5. Additionally for outgoing traffic port 5432 should be
configured correctly to connect to the remote database. Furthermore,
configure any necessary firewall rules or security groups to restrict
access only to the necessary IP addresses and networks. This differentiates
per user so this should be considered on an individual scenario basis.

### Run the Docker application

After pulling the docker image and setting up the network configuration
you can start running the docker container containing the logging
server. When running this image it is important to correctly link the
ports between the network configuration and the container. This is done
by port mapping using the -p flag whenever running 'docker run
\<image\>'. This flag can link any port to the correct port inside the
container. In our case the ports relevant are 443 and 80 for a good gRPC
connection. So dependent on what ports you've setup during network
configuration the command should be as following: 'docker run -d -p
8080:80 -p 8443:443 siem2l/patternpalloggingserver:latest'.

### Configuration

After running the application, it's important to update the
configuration files to run the application correctly and be able to
connect to the remote PostgreSQL database server. For the logging server
as of now it only requires the updating of the connection string in the
appsettings.json to match the configuration of the remote database. Make
sure to never publish these configuration files as it can cause security
issues and further exploitation of the application.

## Technical Stack

- Operating system: A Linux distribution that supports Docker and
images for the ASP.NET core runtime and .NET SDK. Examples include
Ubuntu, CentOS, or Debian.

- Containerization: Docker for containerization of the application
server. Making it easier to deploy, scale and manage containers.

- Programming language: C$\#$ is used for all application components
of the logging system.

- Dependency management: NuGet for managing .NET packages.

- Relational database system: PostgreSQL is used as the database
system for the entire application.

- Communication protocol: gRPC is used as the communication protocol
between the server and clients.

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

We use the Progsnap2 model to log information from the extension.
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