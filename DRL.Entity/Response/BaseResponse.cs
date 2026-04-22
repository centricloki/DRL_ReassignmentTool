namespace DRL.Entity
{
    public class BaseResponse<T>
    {
        public BaseResponse(bool defalut = false)
        {
            IsSuccess = defalut;
            Message = "";
        }

        public BaseResponse(string message)
        {
            IsSuccess = false;
            Message = message;
        }

        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
