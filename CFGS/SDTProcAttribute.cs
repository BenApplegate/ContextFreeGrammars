namespace CFGS;

[AttributeUsage(AttributeTargets.Method)]
public class SDTProcAttribute : Attribute
{
    public string nonTerminal;
    public SDTProcAttribute(string nonTerminal)
    {
        this.nonTerminal = nonTerminal;
    }
}