namespace DocumentCreator.Model
{
    public class TemplateFieldExpression
    {
        public string Name { get; internal set; }
        public string Expression { get; internal set; }
        public string Parent { get; internal set; }
        public bool IsCollection { get; internal set; }
        public ExpressionResult Result { get; internal set; }
    }
}
