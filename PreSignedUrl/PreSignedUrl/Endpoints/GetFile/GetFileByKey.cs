using Amazon.S3.Model;
using Amazon.S3;
using FastEndpoints;
using Microsoft.Extensions.Options;

namespace PreSignedUrl.Endpoints.GetFile
{
    public class Request
    {
        public string Key { get; set; }
    }

    public class Response
    {
        public string Url { get; set; }
    }

    public class GetFileByKeyEndpoint : Endpoint<Request, BaseResponse<Response>>
    {
        private readonly AwsConfig _config;
        private readonly IAmazonS3 _amazonS3Client;

        public GetFileByKeyEndpoint(IOptions<AwsConfig> awsOptionsConfig, IAmazonS3 amazonS3Client)
        {
            _config = awsOptionsConfig.Value;
            _amazonS3Client = amazonS3Client;
        }

        public override void Configure()
        {
            Get("/files/{Key}");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            try
            {
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = _config.Bucket,
                    Key = req.Key,
                    Expires = DateTime.UtcNow.AddHours(_config.Duration),
                    Verb = Amazon.S3.HttpVerb.GET
                };

                var url = await _amazonS3Client.GetPreSignedURLAsync(request);

                await SendOkAsync(new BaseResponse<Response>
                {
                    Object = new Response
                    {
                        Url = url
                    }
                });
            }
            catch (Exception ex)
            {
                await SendAsync(new BaseResponse<Response> { Message = "An unexpected error occurred. " }, 500);
            }
        }
    }
}
