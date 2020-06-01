namespace JToolbox.Core.Results
{
    public class ValueResult<T> : Result
    {
        public T Value { get; set; }

        public ValueResult()
        {
        }

        public ValueResult(T value)
        {
            Value = value;
        }

        public ValueResult(Result result)
            : base(result)
        {
        }

        public ValueResult(ValueResult<T> valueResult)
            : base(valueResult)
        {
            Value = valueResult.Value;
        }

        public override void Clear()
        {
            Value = default(T);
            base.Clear();
        }
    }
}
