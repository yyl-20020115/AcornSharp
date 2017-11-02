using JetBrains.Annotations;

namespace AcornSharp.Node
{
    public sealed class ExportAllDeclarationNode : BaseNode
    {
        public BaseNode source;

        internal ExportAllDeclarationNode([NotNull] Parser parser, Position start, Position end) :
            base(parser, start, end)
        {
        }
    }
}