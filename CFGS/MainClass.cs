using System.Diagnostics;

namespace CFGS;

public class MainClass
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Please enter the path to the input CFG file:");
        string filePath = Console.ReadLine() ?? "";
        if (!File.Exists(filePath))
        {
            Console.Error.WriteLine("The specified file could not be opened");
            Environment.Exit(1);
        }

        CFG cfg = new CFG(File.ReadAllText(filePath));
        cfg.Print();
        
        
        
        Console.WriteLine("\n\nNow attempting to perform LL parse");
        Console.WriteLine("Please provide the location of the input tokens:");
        string filename = Console.ReadLine() ?? "";
        
        Console.WriteLine("\nWould you like to use SDT (Will search assembly for any SDTProc functions and run them)" +
                          "\nType \"yes\" to use SDT type anything else to not use it");

        bool useSDT = (Console.ReadLine() ?? "").ToLower() == "yes";
        
        LLParser parser = new LLParser(cfg);
        parser.printLLTable();
        
        parser.parse(filename, useSDT);
        
        Console.WriteLine("If you would like to save the resulting tree as an SVG file enter the location to save it to, otherwise hit enter" +
                          "\n(Note that dot must be in the PATH for this to work)");
        filename = Console.ReadLine();
        if (!string.IsNullOrEmpty(filename))
        {
           parser.SaveDotFile(filename+".dot");
           var proc = Process.Start("dot", $"-Tsvg {filename + ".dot"} -o {filename}");
           proc.WaitForExit();
           File.Delete(filename + ".dot");
        }
    }
}