namespace IA.Rest
{
    class RestResponse<T>
    {
        public bool Success { get; internal set; }
        public T Data { get; internal set; }
    }
}
