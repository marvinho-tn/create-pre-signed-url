using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using FastEndpoints;
using Microsoft.Extensions.Options;
using PreSignedUrl.Endpoints.GetFile;

namespace PreSignedUrl.Endpoints.UpdateFile
{
    public class Request
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class Response
    {
        public string Key { get; set; }
        public string Url { get; set; }
    }

    public class UpdateFileEndpoint : Endpoint<Request, BaseResponse<Response>>
    {
        private readonly AwsConfig _config;
        private readonly IAmazonS3 _amazonS3Client;
        private readonly ILogger<GetFileByKeyEndpoint> _logger;

        public UpdateFileEndpoint(IOptions<AwsConfig> awsOptionsConfig, IAmazonS3 amazonS3Client, ILogger<GetFileByKeyEndpoint> logger)
        {
            _config = awsOptionsConfig.Value;
            _amazonS3Client = amazonS3Client;
            _logger = logger;
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
                    Expires = DateTime.UtcNow.AddMinutes(_config.Duration),
                    Verb = HttpVerb.PUT
                };

                request.Metadata.Add(nameof(Request.Latitude), req.Latitude.ToString());
                request.Metadata.Add(nameof(Request.Longitude), req.Longitude.ToString());

                AWSConfigsS3.UseSignatureVersion4 = true;

                var url = await _amazonS3Client.GetPreSignedURLAsync(request);

                await SendOkAsync(new BaseResponse<Response>
                {
                    Object = new Response
                    {
                        Key = objKey,
                        Url = url
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                await SendAsync(new BaseResponse<Response> { Message = "An unexpected error occurred. " }, 500);
            }
        }
    }
}
