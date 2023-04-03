using System.Data;
using System.Text.Json;

namespace CFGS;

public partial class LLParser
{
    private CFG _cfg;
    private Dictionary<String, Dictionary<String, int>> llTable = new Dictionary<string, Dictionary<string, int>>();

    public LLParser(CFG c)
    {
        _cfg = c;

        generateLLTable();
    }

    private void generateLLTable()
    {
        for (int i = 0; i < _cfg.productionRules.Count; i++)
        {
            var prodRule = _cfg.productionRules[i];
            if (!llTable.ContainsKey(prodRule.nonTerminal))
            {
                llTable[prodRule.nonTerminal] = new Dictionary<string, int>();
            }

            var predict = _cfg.PredictSet(i);
            foreach (var terminal in predict)
            {
                if (llTable[prodRule.nonTerminal].ContainsKey(terminal))
                {
                    //We have already found a rule for this non terminal
                    //that predicts this item, LL conflict

                    throw new FormatException(
                        $"An LL conflict occured with non-terminal {prodRule.nonTerminal} on rule {i + 1}");
                }

                llTable[prodRule.nonTerminal][terminal] = i;
            }
        }
    }

    public void printLLTable()
    {
        foreach (var nT in llTable)
        {
            Console.Write($"{nT.Key}: ");
            foreach (var transition in nT.Value)
            {
                Console.Write($"{{{transition.Key}: {transition.Value}}},");
            }
            Console.WriteLine();
        }
    }

    public void parse(string filename, bool useSDT = false)
    {
       string[] lines;
       lines = File.ReadAllLines(filename);

       Queue<(string tokenType, string? value)> tokenStream = new Queue<(string tokenType, string? value)>();
       foreach (var line in lines)
       {
           var split = line.Split(new string[]{" ", "\n", "\t", "\r", "\f", "\v"}, StringSplitOptions.RemoveEmptyEntries);
           if(split.Length == 0) continue;
           (String, String?) newEntry = ("", null);
           newEntry.Item1 = split[0];
           if (split.Length > 1) newEntry.Item2 = split[1];
           
           tokenStream.Enqueue(newEntry);
       }
       tokenStream.Enqueue(("$", null));
       
       Console.WriteLine("Processed token stream:");
       foreach (var t in tokenStream)
       {
           Console.WriteLine($"Token Type: {t.tokenType}, Value: {t.value ?? "null"}");
       }

       var result = parseTokenStream(tokenStream, useSDT);

       result.Print();
    }

    private LLNode parseTokenStream(Queue<(string tokenType, string? value)> tokenStream, bool useSDT)
    {
        LLNode root = new LLNode();
        LLNode current = root;
        
        Stack<String> parseStack = new Stack<string>();
        parseStack.Push(_cfg.startingRule.nonTerminal);

        while (parseStack.Count > 0)
        {
            string currentStack = parseStack.Peek();

            (string tokenType, string? value) currentToken = tokenStream.Count > 0 ? tokenStream.Peek() : ("", null);

            if (_cfg.nonTerminals.Contains(currentStack))
            {
                if (!llTable[currentStack].ContainsKey(currentToken.tokenType))
                {
                    //Syntax error
                    throw new SyntaxErrorException(
                        $"Unexpected token {currentToken.tokenType}, value: {currentToken.value}");
                }

                int nextRule = llTable[currentStack][currentToken.tokenType];
                parseStack.Pop();
                current = current.addChild(NodeType.NON_TERMINAL, currentStack, current);

                parseStack.Push("*");
                int ruleLength = _cfg.productionRules[nextRule].rule.Count;
                for (int i = ruleLength - 1; i >= 0; i--)
                {
                    parseStack.Push(_cfg.productionRules[nextRule].rule[i]);
                }
            }
            else if (_cfg.terminals.Contains(currentStack) || currentStack == "$")
            {
                if (currentStack != currentToken.tokenType)
                {
                    //Syntax error
                    throw new SyntaxErrorException(
                        $"Unexpected token {currentToken.tokenType}, value: {currentToken.value}");
                }

                parseStack.Pop();
                tokenStream.Dequeue();

                current.addChild(NodeType.TERMINAL, currentToken.tokenType, current, currentToken.value);
            }
            else if (currentStack == "*")
            {
                parseStack.Pop();

                if (useSDT)
                {
                    //We are using SDT and should try to find a procedure for the current production rule

                    runSDT(current);
                }
                
                current = current.parent;
            }
            else if (currentStack == "lambda")
            {
                parseStack.Pop();
                current.addChild(NodeType.LAMBDA, "lambda", current);
            }
        }


        return root;
    }

    private void runSDT(LLNode current)
    {
        
    }
}