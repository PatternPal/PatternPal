# Extension Project Structure

The Visual Studio 2022 extension project has only one dependency: `PatternPal.Extension.Protos`. The
sole purpose of this project is to provide a project in which the client side code for the gRPC
background service can be generated. Due to current limitations in the build tooling, it is not
possible to generate the code inside the extension project itself.
