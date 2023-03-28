#region

using System.Net.Http;

using Grpc.Net.Client;
using Grpc.Net.Client.Web;

using PatternPal.Protos;

#endregion

namespace PatternPal.Extension.Grpc
{
    internal static class GrpcHelper
    {
        internal static GrpcChannel Channel { get; }
        internal static RecognizerService.RecognizerServiceClient RecognizerClient { get; }
        internal static StepByStepService.StepByStepServiceClient StepByStepClient { get; }

        static GrpcHelper()
        {
            Channel = GrpcChannel.ForAddress(
                "http://localhost:5000",
                new GrpcChannelOptions
                {
                    HttpHandler = new GrpcWebHandler(new HttpClientHandler()),
                });

            RecognizerClient = new RecognizerService.RecognizerServiceClient(Channel);
            StepByStepClient = new StepByStepService.StepByStepServiceClient(Channel);
        }
    }
}
