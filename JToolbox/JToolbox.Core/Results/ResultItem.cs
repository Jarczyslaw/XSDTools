namespace JToolbox.Core.Results
{
    public abstract class ResultItem
    {
        public int Code { get; set; }

        public virtual string Content { get; set; }

        protected ResultItem()
        {
        }

        protected ResultItem(ResultItem item)
        {
            Code = item.Code;
            Content = item.Content;
        }
    }
}
