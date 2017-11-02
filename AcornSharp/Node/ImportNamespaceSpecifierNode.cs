using JetBrains.Annotations;

namespace AcornSharp.Node
{
    public sealed class ImportNamespaceSpecifierNode : BaseNode
    {
        public IdentifierNode local;

        internal ImportNamespaceSpecifierNode([NotNull] Parser parser, Position start, Position end) :
            base(parser, start, end)
        {
        }
    }
}