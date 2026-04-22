namespace DRL.Library
{
    public class ResultStatus<T> where T : class
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
        public string MessageDetail { get; set; }
    }
}