namespace Yaroshinski.Blog.Application.Models
{
    public static class Response
    {
        public static Response<T> Fail<T>(string msg, T data = default) =>
            new Response<T>(data, msg, true);

        public static Response<T> Ok<T>(string msg, T data = default) =>
            new Response<T>(data, msg, false);
        
    }

    public class Response<T>
    {
        public Response(T data, string msg, bool error)
        {
            Data = data;
            Message = msg;
            Error = error;
        }

        public T Data { get; set; }

        public string Message { get; set; }

        public bool Error { get; set; }
    }
}
