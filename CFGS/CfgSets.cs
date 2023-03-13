namespace CFGS;

public partial class CFG
{
    public bool DerivesToLambda(string token, Stack<int>? T = null)
    {
        if (T == null) T = new Stack<int>();

        if (token is "$" or "lambda") return true;
        if (terminals.Contains(token)) return false;
        
        var rules = productionMap[token];
        foreach (var rule in rules)
        {
            if(T.Contains(rule)) continue;

            if (ruleIsLambda(rule)) return true;
            
            var production = productionRules[rule].rule;
            
            if(production.Any(s => terminals.Contains(s))) continue;

            bool allDeriveToLambda = true;
            foreach (var r in production)
            {
                T.Push(rule);
                allDeriveToLambda = DerivesToLambda(r, T);
                T.Pop();
                if(!allDeriveToLambda) break;
            }

            if (allDeriveToLambda) return true;
        }

        return false;
    }

    public bool DerivesToLambda(int rule)
    {
        var T = new Stack<int>();

        if (ruleIsLambda(rule)) return true;
            
        var production = productionRules[rule].rule;
            
        if(production.Any(s => terminals.Contains(s))) return false;

        bool allDeriveToLambda = true;
        foreach (var r in production)
        {
            T.Push(rule);
            allDeriveToLambda = DerivesToLambda(r, T);
            T.Pop();
            if(!allDeriveToLambda) break;
        }

        if (allDeriveToLambda) return true;
        return false;
    }

    HashSet<string> FirstSet(List<string> rule, HashSet<string>? T = null)
    {
        if (T is null) T = new HashSet<string>();
        if (terminals.Contains(rule[0]))
        {
            return new HashSet<string>() { rule[0] };
        }

        if (rule[0] is "$")
        {
            return new HashSet<string>() { rule[0] };
        }

        HashSet<string> first = new HashSet<string>();
        if (!T.Contains(rule[0]))
        {
            T.Add(rule[0]);
            foreach (var p in productionRules.Where(p=> p.nonTerminal == rule[0]))
            {
                var R = p.rule;
                var G = FirstSet(R, T);
                first.UnionWith(G);
            }
        }

        if (DerivesToLambda(rule[0]))
        {
            if (rule.Count > 1)
            {
                var copy = new List<string>(rule);
                copy.RemoveAt(0);
                var G = FirstSet(copy);
                first.UnionWith(G);
            }
        }

        return first;
    }

    HashSet<string> FirstSet(int rule)
    {
        return FirstSet(productionRules[rule].rule);
    }

    HashSet<string> FollowSet(string A, HashSet<string>? T = null)
    {
        if (T is null) T = new HashSet<string>();
        
        if (T.Contains(A))
        {
            return new HashSet<string>();
        }

        T.Add(A);
        HashSet<string> follow = new HashSet<string>();
        var productions = from production in productionRules
                                           where production.rule.Contains(A)
                                           select production;
        foreach (var productionRule in productions)
        {
            var production = productionRule.rule;
            if(production is null) continue;
            for (int i = 0; i < production.Count; i++)
            {
                if(production[i] != A) continue;
                List<string> remaining = new List<string>();
                for (int j = i + 1; j < production.Count; j++)
                {
                    remaining.Add(production[j]);
                }

                if (remaining.Count > 0)
                {
                    var G = FirstSet(remaining);
                    follow.UnionWith(G);
                }

                bool allDeriveToLambda = true;
                foreach (var s in remaining)
                {
                    if (!DerivesToLambda(s))
                    {
                        allDeriveToLambda = false;
                        break;
                    }
                }

                if (remaining.Count == 0 || allDeriveToLambda)
                {
                    var G = FollowSet(productionRule.nonTerminal, T);
                    follow.UnionWith(G);
                }
            }
        }

        return follow;

    }

}