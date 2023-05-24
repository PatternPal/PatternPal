# Installation

## Software requirements

- Operating system: Linux distribution (Ubuntu, CentOS, or Debian)

- Container manager: Latest stable version of Docker

### Docker images

- .NET SDK image

- ASP.NET Core Runtime image

### Other

- Remote database server: PostgreSQL; A remote PostgreSQL database
server. This can be a cloud-based, managed database service, or a
dedicated server.

--- 

## Installation

1. Install Linux: Install a compatible Linux distribution on the target
server. Follow the installation instructions of the chosen Linux
Distribution.

2. Install Docker: Install Docker on the Linux server. Follow the
docker installation instructions for the installed OS
[here](https://docs.docker.com/desktop/install/linux-install/).

3. Pull Docker Image: Pull the Docker image for the application from
the docker repository or Docker Hub. The current Docker repository
is `siem2l/patternpalloggingserver:latest`.

4. Run Docker Container: Create and run the pulled docker container
from step 3.

5. Setup Database: Set up a remote PostgreSQL database server,
Following the instruction provided by your chosen database service
or server.

6. Configuration: Update the application settings or configuration
files to connect to the database using the correct connection
string.

7. Test Application: Ensure the server is running correctly and
available for remote clients to communicate.

This is the high-level overview on how to install the application. Some
steps require additional guidance that will be explained in the
following sections.

---

## Pull the Docker Image

For the publishing of our application we use Docker Hub. This
application can host container images and as such it hosts the image of
the Logging server. By using this system continuous deployment is easily
achievable. As of right now the repository is hosted at LINK. To pull
the image, use the following command: `docker pull
siem2l/patternpalloggingserver:latest`.

## Network configuration

For the application to be running properly the network settings of the
Linux server need to be configured. This includes opening specific ports
such as `443` and `80` for HTTPS and HTTP incoming traffic. This is usually
done by running the docker instance and exposing the ports as arguments
in step 5. Additionally for outgoing traffic port `5432` should be
configured correctly to connect to the remote database. Furthermore,
configure any necessary firewall rules or security groups to restrict
access only to the necessary IP addresses and networks. This differentiates
per user so this should be considered on an individual scenario basis.

## Run the Docker application

After pulling the docker image and setting up the network configuration
you can start running the docker container containing the logging
server. When running this image it is important to correctly link the
ports between the network configuration and the container. This is done
by port mapping using the -p flag whenever running `docker run
\<image\>`. This flag can link any port to the correct port inside the
container. In our case the ports relevant are 443 and 80 for a good gRPC
connection. So dependent on what ports you've setup during network
configuration the command should be as following: 'docker run -d -p
8080:80 -p 8443:443 --name patternpal siem2l/patternpalloggingserver:latest'.

### Configuration

After running the application, it's important to update the
configuration files to run the application correctly and be able to
connect to the remote PostgreSQL database server. For the logging server
as of now it only requires the updating of the connection string in the
appsettings.json to match the configuration of the remote database. Make
sure to never publish these configuration files as it can cause security
issues and further exploitation of the application.