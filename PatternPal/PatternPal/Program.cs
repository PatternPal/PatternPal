namespace PatternPal;

internal static class Program
{
    internal static void Main()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

        builder.Services.AddGrpc();

        WebApplication app = builder.Build();

        app.UseGrpcWeb(
            new GrpcWebOptions
            {
                DefaultEnabled = true
            });
        app.MapGrpcService< RecognizerService >();
        app.MapGrpcService< StepByStepService >();

        app.Run();
    }
}
