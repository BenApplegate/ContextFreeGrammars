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
}