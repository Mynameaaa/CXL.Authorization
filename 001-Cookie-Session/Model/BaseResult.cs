namespace _001_Cookie_Session
{
    public class BaseResult
    {

        public string Message { get; set; }

        public bool Success { get; set; } = false;

        public int Code { get; set; } = 500;

        public object Value { get; set; }

    }
}
