# Error handling

Calls to the background process can fail. Because the way these failures should be handled depends
on the call being made, the caller is responsible for handling the failure. To present an error
message to the user, use the `GrpcHelper.ShowErrorMessage` method.

If the background process has crashed, a notification is shown to the user which includes an option
to restart the background service.

# Extension Project Structure

The Visual Studio 2022 extension project has only one dependency: `PatternPal.Extension.Protos`. The
sole purpose of this project is to provide a project in which the client side code for the gRPC
background service can be generated. Due to current limitations in the build tooling, it is not
possible to generate the code inside the extension project itself.
