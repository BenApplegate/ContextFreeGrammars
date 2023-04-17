using csdot;
using csdot.Attributes.DataTypes;

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

        public void SaveGraphToFile(string filename)
        {
            //Due to some weird dot thing, each node in the graph needs a unique name
            //I will use i for this
            int i = 0;
            
            Graph graph = new Graph("root");
            graph.type = "graph";

            Node rootNode = new Node($"\"{i}\\n{tokenType}\\n({value})\"");
            graph.AddElement(rootNode);

            foreach (var child in children)
            {
                child.AddChildToGraph(rootNode, graph, ref i);
            }

            new DotDocument().SaveToFile(graph, filename);
        }

        public void AddChildToGraph(Node parentNode, Graph graph, ref int i)
        {
            //We need to increment the node id to keep uniqueness
            i++;
            
            //First we need to create a new node for the child
            Node newNode = new Node($"\"{i}\\n{tokenType}\\n({value})\"");
            
            //Next we need to create the dot transition
            List<Transition> transitions = new List<Transition>()
            {
                new Transition(parentNode, EdgeOp.undirected),
                new Transition(newNode, EdgeOp.unspecified)
            };
            
            //Next we create an edge using our transition
            Edge edge = new Edge(transitions);
            
            //Next we add our new node and edge to the graph
            graph.AddElements(newNode, edge);
            
            //Finally we need to attach our children to us
            foreach (var child in children)
            {
                child.AddChildToGraph(newNode, graph, ref i);
            }
        }
    }
}