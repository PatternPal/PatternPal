# Extension

Currently the only supported IDE is Visual Studio. This section is specific to the extension for
Visual Studio, but it may include information which is also relevant for other extensions.

## Extension Project Structure

The Visual Studio 2022 extension project has only one dependency: `PatternPal.Extension.Protos`. The
sole purpose of this project is to provide a project in which the client side code for the gRPC
background process can be generated. Due to current limitations in the build tooling, it is not
possible to generate the code inside the extension project itself.
