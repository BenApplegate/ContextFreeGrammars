namespace CFGS;

public partial class LLParser
{
    public enum NodeType
    {
        ROOT,
        NON_TERMINAL,
        TERMINAL,
        LAMBDA,
        END_OF_INPUT
    }

    public class LLNode
    {
        public NodeType type;
        public string tokenType;
        public string? value;
        public List<LLNode> children;
        public LLNode parent;

        public LLNode()
        {
            type = NodeType.ROOT;
            tokenType = "ROOT";
            value = null;
            children = new List<LLNode>();
        }

        public LLNode(NodeType type, string tokenType, LLNode parent, string? value = null)
        {
            this.type = type;
            this.tokenType = tokenType;
            this.parent = parent;
            this.children = new List<LLNode>();
            this.value = value;
        }

        public LLNode addChild(NodeType type, string tokenType, LLNode parent, string? value = null)
        {
            LLNode newNode = new LLNode(type, tokenType, parent, value);
            this.children.Add(newNode);
            return newNode;
        }

        public void Print()
        {
            Console.Write(tokenType);
            if (value != null)
            {
                Console.Write($" ({value})");
            }
            Console.WriteLine();
            
            
            
            for (int i = 0; i < children.Count; i++)
            {
                children[i].Print(1, i == children.Count-1, new List<bool>());
            }
        }

        public void Print(int depth, bool lastChild, List<bool> printPipe)
        {
            for (int i = 0; i < depth - 1; i++)
            {
                if (!printPipe[i])
                {
                    Console.Write(" ");
                }
                else
                {
                    Console.Write("│");
                }
            }

            if (!lastChild)
            {
                Console.Write("├");
                printPipe.Add(true);
            }
            else
            {
                Console.Write("└");
                printPipe.Add(false);
            }
            
            Console.Write(tokenType);
            if (value != null)
            {
                Console.Write($" ({value})");
            }
            Console.WriteLine();

            for (int i = 0; i < children.Count; i++)
            {
                children[i].Print(depth + 1, i == children.Count-1, new List<bool>(printPipe));
            }
        }
    }
}