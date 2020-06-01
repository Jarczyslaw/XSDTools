using System;
using System.Collections.Generic;
using System.Linq;

namespace JToolbox.Core.Results
{
    public class ResultErrors : List<ResultError>
    {
        public ResultErrors()
        {
        }

        public ResultErrors(ResultErrors errors)
        {
            AddRange(errors);
        }

        public void Add(Exception exception)
        {
            Add(new ResultError
            {
                Content = exception.Message,
                Exception = exception
            });
        }

        public void Add(string content)
        {
            Add(new ResultError
            {
                Content = content
            });
        }
    }
}
