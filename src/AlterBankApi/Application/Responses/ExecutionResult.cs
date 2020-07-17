namespace AlterBankApi.Application.Responses
{
    /// <summary>
    /// The execution result
    /// </summary>
    /// <typeparam name="TData">Content</typeparam>
    public class ExecutionResult<TData>
    {
        /// <summary>
        /// Content of response
        /// </summary>
        public TData Data { get; }

        /// <summary>
        /// Declares whether response is error.
        /// </summary>
        public bool IsError { get; }

        /// <summary>
        /// Error message
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// Constructor for response
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="isError">Is error</param>
        /// <param name="errorMessage">Error message</param>
        public ExecutionResult(TData data, bool isError, string errorMessage)
        {
            Data = data;
            IsError = isError;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Constructor for successful execution
        /// </summary>
        /// <param name="data">Data</param>
        public ExecutionResult(TData data)
            : this(data, false, string.Empty) { }
    }
}
