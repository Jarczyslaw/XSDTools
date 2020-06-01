using System;

namespace JToolbox.Core.Results
{
    public class ResultError : ResultItem
    {
        public Exception Exception { get; set; }

        public bool IsException => Exception != null;

        public override string Content
        {
            get => IsException ? Exception.Message : base.Content;
            set => base.Content = value;
        }

        public ResultError()
        {
        }

        public ResultError(ResultError resultError)
            : base(resultError)
        {
            Exception = resultError.Exception;
        }
    }
}
