namespace CFGS;

public partial class CFG
{
    public List<(string nonTerminal, List<string> rule)> productionRules = new List<(string nonTerminal, List<string> rule)>();
    public Dictionary<string, HashSet<int>> productionMap = new Dictionary<string, HashSet<int>>();
    public HashSet<string> terminals = new HashSet<string>();
    public HashSet<string> nonTerminals = new HashSet<string>();
    private (int ruleNum, string nonTerminal) startingRule = (-1, "");

    public bool ruleIsLambda(int ruleNum)
    {
        if (productionRules[ruleNum].rule[0] == "lambda")
        {
            return true;
        }

        return false;
    }
    
    public CFG(string fileText)
    {
        //Get tokens by splitting on whitespace removing any empty entries 
        string[] tokens = fileText.Split(new string[]{" ", "\n", "\t", "\r", "\f", "\v"}, StringSplitOptions.RemoveEmptyEntries);

        //Note that we are just ignoring lambda as we are going to store it in the production rules like a terminal
        
        int currentRule = -1;
        string currentNonTerminal = "";
        for (int i = 0; i < tokens.Length; i++)
        {
            string token = tokens[i];
            string next = "";
            if (i != tokens.Length - 1)
            {
                next = tokens[i + 1];
            }
            
            //Next we need to check for a new production rule
            if (token.Any(char.IsUpper) && next == "->")
            {
                //Init new rule
                currentRule++;
                nonTerminals.Add(token);
                currentNonTerminal = token;
                productionRules.Add((token, new List<string>()));
                if (!productionMap.ContainsKey(token))
                {
                    productionMap.Add(token, new HashSet<int>());
                }

                productionMap[token].Add(currentRule);
                continue;
            }
            
            //Skip the token if it is ->
            if (token == "->") continue;
            
            //If the token is a | create a new production rule under the same nonTerminal
            if (token == "|")
            {
                currentRule++;
                productionRules.Add((currentNonTerminal, new List<string>()));
                productionMap[currentNonTerminal].Add(currentRule);
                continue;
            }
            
            //Mark $ tokens as starting rule
            if (token == "$")
            {
                if (startingRule.ruleNum != -1)
                {
                    Console.Error.WriteLine("Existing starting rule found (Duplicate $) keeping first starting rule");
                    continue;
                }

                startingRule = (currentRule, currentNonTerminal);
            }
            
            productionRules[currentRule].rule.Add(token);
            if (token is not ("$" or "lambda"))
            {
                if(!token.Any(char.IsUpper))
                    terminals.Add(token);
            }

        }

    }

    public void Print()
    {
        Console.WriteLine("Grammar Non-Terminals");
        Console.WriteLine(string.Join(", ", nonTerminals));
        Console.WriteLine("Grammar Terminals");
        Console.WriteLine(string.Join(", ", terminals));
        Console.WriteLine("\nGrammar Rules");
        for (int i = 0; i < productionRules.Count; i++)
        {
            Console.WriteLine(
                $"({i + 1})\t{productionRules[i].nonTerminal} -> {String.Join(" ", productionRules[i].rule)}\t\t " +
                $"DerivesToLambda: {DerivesToLambda(i)} \t\tFirstSet:{{{string.Join(',', FirstSet(i))}}}");
        }
        Console.WriteLine($"\nGrammar Start Symbol: {startingRule.nonTerminal}");
        
        Console.WriteLine("FollowSets:\n");
        foreach (var nt in nonTerminals)
        {
            Console.WriteLine($"{nt}:{{{string.Join(',', FollowSet(nt))}}}");
        }
    }
}