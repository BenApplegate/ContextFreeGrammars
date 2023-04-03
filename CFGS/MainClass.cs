namespace CFGS;

public class MainClass
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Please enter the path to the input file:");
        string filePath = Console.ReadLine() ?? "";
        if (!File.Exists(filePath))
        {
            Console.Error.WriteLine("The specified file could not be opened");
            Environment.Exit(1);
        }

        CFG cfg = new CFG(File.ReadAllText(filePath));
        cfg.Print();
        
        
        Console.WriteLine("\n\nNow attempting to perform LL parse");
        LLParser parser = new LLParser(cfg);
        parser.printLLTable();
    }
}