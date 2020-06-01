using System.Linq;

namespace JToolbox.Core.Results
{
    public class Result
    {
        public bool IsSuccess => !Errors.Any();

        public ResultErrors Errors { get; }

        public ResultInfos Infos { get; }

        public Result()
            : this (new ResultErrors(), new ResultInfos())
        {
        }

        public Result(Result result)
            : this (result.Errors, result.Infos)
        {
        }

        public Result(ResultErrors errors, ResultInfos infos)
        {
            Errors = errors;
            Infos = infos;
        }

        public virtual void Clear()
        {
            Errors.Clear();
            Infos.Clear();
        }
    }
}
