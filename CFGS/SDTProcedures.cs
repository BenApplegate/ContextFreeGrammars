namespace CFGS;

public class SDTProcedures
{

    [SDTProc("A")]
    public static void ReverseA(LLParser.LLNode node)
    {
        var children = node.children;
        (children[1], children[2]) = (children[2], children[1]);
    }

    [SDTProc("Y")]
    public static void FlattenRecursion(LLParser.LLNode node)
    {
        if(node.children.Count == 1) return;
        
        //Remove Ys that are just lambdas
        node.children.RemoveAll(child =>
        {
            if (child.type == LLParser.NodeType.TERMINAL) return false;
            if (child.children[0].type == LLParser.NodeType.LAMBDA) return true;
            return false;
        });

        if (node.children.Last().type == LLParser.NodeType.NON_TERMINAL)
        {
            //If we end with a non-terminal Y, remove it, and adopt its children
            var newChildren = node.children.Last().children;
            node.children.Remove(node.children.Last());
            node.children.AddRange(newChildren);
        }
    }
}