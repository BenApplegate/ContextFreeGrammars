namespace LGA12;

public class Program
{

    public static void Main(string[] args)
    {
        HashSet<string> nTerminals = new HashSet<string>();
        HashSet<string> symbols = new HashSet<string>();
        int startRule = -1;
        string input = File.ReadAllText(args[0]);
        string[] tokens = input.Split(new string[]{" ", "\n", "\t", "\r", "\f", "\v"}, StringSplitOptions.RemoveEmptyEntries);

        List<KeyValuePair<string, (List<string> tokens, bool isLambda, bool isStart)>> cfg = new List<KeyValuePair<string, (List<string>, bool, bool)>>();

        int current = -1;
        for(int i = 0; i < tokens.Length; i++)
        {
            string token = tokens[i];
            string next = "";
            if (i != tokens.Length - 1)
            {
                next = tokens[i + 1];
            }
            if (token.Any(char.IsUpper) && next == "->")
            {
                //new production rule
                current++;
                nTerminals.Add(token);
                cfg.Add(new KeyValuePair<string, (List<string> tokens, bool isLambda, bool isStart)>(token, (new List<string>(), false, false)));
                continue;
            }
            if(token == "->") continue;
            if (token == "|")
            {
                cfg.Add(new KeyValuePair<string, (List<string> tokens, bool isLambda, bool isStart)>(cfg[current].Key, (new List<string>(), false, false)));
                current++;
                continue;
            }
            if (token == "lambda")
            {
                cfg[current] =
                    new KeyValuePair<string, (List<string> tokens, bool isLambda, bool isStart)>(cfg[current].Key,
                        (new List<string>(), true, cfg[current].Value.isStart));
                continue;
            }

            if (token == "$")
            {
                cfg[current] =
                    new KeyValuePair<string, (List<string> tokens, bool isLambda, bool isStart)>(cfg[current].Key,
                        (cfg[current].Value.tokens, cfg[current].Value.isLambda, true));
                startRule = current;
            }

            symbols.Add(token);
            cfg[current].Value.tokens.Add(token);
            
        }
        
        Console.WriteLine("Grammar Non-Terminals");
        Console.WriteLine(string.Join(", ", nTerminals));
        Console.WriteLine("Grammar Symbols");
        Console.WriteLine(string.Join(", ", symbols));
        Console.WriteLine("\nGrammar Rules");
        for (int i = 0; i < cfg.Count; i++)
        {
            Console.WriteLine($"({i+1})\t{cfg[i].Key} -> {(cfg[i].Value.isLambda ? "lambda" : string.Join(' ', cfg[i].Value.tokens))}");
        }
        Console.WriteLine($"\nGrammar Start Symbol: {cfg[startRule].Key}");
    }
    
}