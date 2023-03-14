#region

using System.Net.Http;

using Grpc.Net.Client;
using Grpc.Net.Client.Web;

#endregion

namespace PatternPal.Extension.Grpc
{
    internal static class GrpcChannelHelper
    {
        internal static GrpcChannel Channel { get; }

        static GrpcChannelHelper()
        {
            Channel = GrpcChannel.ForAddress(
                "http://localhost:5000",
                new GrpcChannelOptions
                {
                    HttpHandler = new GrpcWebHandler(new HttpClientHandler()),
                });
        }
    }
}
