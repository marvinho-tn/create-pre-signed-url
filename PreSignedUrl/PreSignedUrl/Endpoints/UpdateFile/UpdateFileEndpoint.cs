using Amazon.S3;
using Amazon.S3.Model;
using FastEndpoints;
using Microsoft.Extensions.Options;

namespace PreSignedUrl.Endpoints.UpdateFile
{
    public class Request
    {
        public int Latitude { get; set; }
        public int Longitude { get; set; }
    }

    public class Response
    {
        public string Url { get; set; }
    }

    public class UpdateFileEndpoint : Endpoint<Request, BaseResponse<Response>>
    {
        private readonly AwsConfig _config;
        private readonly IAmazonS3 _amazonS3Client;

        public UpdateFileEndpoint(IOptions<AwsConfig> awsOptionsConfig, IAmazonS3 amazonS3Client)
        {
            _config = awsOptionsConfig.Value;
            _amazonS3Client = amazonS3Client;
        }

        public override void Configure()
        {

            Put("/files");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            try
            {
                var objKey = Guid.NewGuid().ToString();
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = _config.Bucket,
                    Key = objKey,
                    Expires = DateTime.UtcNow.AddHours(_config.Duration),
                    Verb = Amazon.S3.HttpVerb.PUT
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
