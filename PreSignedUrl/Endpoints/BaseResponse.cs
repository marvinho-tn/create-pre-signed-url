namespace PreSignedUrl.Endpoints
{
    public class BaseResponse<T>
    {
        public T Object { get; set; }
        public string Message { get; set; }
    }
}
