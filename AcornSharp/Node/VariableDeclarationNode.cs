using JetBrains.Annotations;

namespace AcornSharp.Node
{
    public sealed class VariableDeclarationNode : BaseNode
    {
        public VariableDeclarationNode([NotNull] Parser parser, Position start, Position end) :
            base(parser, start, end)
        {
        }

        public VariableDeclarationNode(SourceLocation location) :
            base(location)
        {
        }

        public override bool TestEquals(BaseNode other)
        {
            if (other is VariableDeclarationNode realOther)
            {
                if (!base.TestEquals(other)) return false;
                return true;
            }
            return false;
        }

        public override bool Equals(BaseNode other)
        {
            if (other is VariableDeclarationNode realOther)
            {
                if (!base.Equals(other)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = base.GetHashCode();
            return hashCode;
        }
    }
}