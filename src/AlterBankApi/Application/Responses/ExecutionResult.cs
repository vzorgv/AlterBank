namespace AlterBankApi.Application.Responses
{
    /// <summary>
    /// The execution result base implementation
    /// </summary>
    public class ExecutionResult<TData>
    {
        /// <summary>
        /// Content of response
        /// </summary>
        public virtual TData Data { get; }

        /// <summary>
        /// Declares whether response is error.
        /// </summary>
        public virtual bool IsError { get; }

        /// <summary>
        /// Description
        /// </summary>
        public IExecutionResultDescription Description { get; }

        /// <summary>
        /// Constructor for response
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="isError">Is error</param>
        /// <param name="description">Description</param>
        protected ExecutionResult(TData data, bool isError, IExecutionResultDescription description)
        {
            Data = data;
            IsError = isError;
            Description = description;
        }

        /// <summary>
        /// Constructor with data only
        /// </summary>
        /// <param name="data">Data</param>
        public ExecutionResult(TData data)
            : this(data, false, null) { }

        /// <summary>
        /// Constructor with error
        /// </summary>
        /// <param name="errorDescription">Error description</param>
        public ExecutionResult(IExecutionResultDescription errorDescription)
            : this(default, true, errorDescription) { }
    }
}
