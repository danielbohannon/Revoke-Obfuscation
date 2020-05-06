using System;
using System.Text.RegularExpressions;

public class ConvertToPowerShellExpression
{
    public static void Main(string[] args)
    {
        string encodedCommandRegex = @"^[\u2013\u2014\u2015/-]e(?![px])[a-zA-Z]*";
        string commandRegex = @"^[\u2013\u2014\u2015/-]c[a-zA-Z]*";
        string parameterWithArgumentRegex = @"^[\u2013\u2014\u2015/-](v|config|c|w|f|mod|o|in|if|ex|ep|e|s)";
        string paramRegex = @"^[\u2013\u2014\u2015/-](v|h|\?|noe|imp|nop|nol|noni|so|s|nam|sshs|i|config|c|w|f|w|iss|mod|o|in|if|ex|ep|e|s|mta)";

        bool foundCommand = false;

        // Emit the process name
        Console.Write(args[0] + " ");

        // Process the arguments
        for (int arg = 1; arg < args.Length; arg++)
        {
            if (Regex.IsMatch(args[arg], encodedCommandRegex, RegexOptions.IgnoreCase))
            {
                // Transform EncodedCommand
                foundCommand = true;
                string encodedCommandText = args[arg + 1];
                string newCommandText;

                try
                {
                    newCommandText= System.Text.Encoding.Unicode.GetString(Convert.FromBase64String(encodedCommandText));
                }
                catch (FormatException)
                {
                    newCommandText = "'<Not valid Base64. Original: " + encodedCommandText + ">'";
                }

                Console.Write("-DecodedFromBase64Command  {" + newCommandText + "}");
                arg++;
            }
            else if (Regex.IsMatch(args[arg], commandRegex, RegexOptions.IgnoreCase))
            {
                // Emit -Command when named
                foundCommand = true;

                string originalCommand = args[arg];
                string command = collectArguments(args, arg);
                arg = args.Length;

                Console.Write(originalCommand + " { " + command + " }");
            }
            else if (Regex.IsMatch(args[arg], parameterWithArgumentRegex, RegexOptions.IgnoreCase))
            {
                Console.Write(args[arg] + " " + args[++arg]);
            }
            else
            {
                // Emit the implicit "-Command"
                if (
                    // If we're at the end and haven't found -Command
                    ((arg == args.Length - 1) && (! foundCommand)) ||
                    // Our first parameter that does not look like a known parameter
                    (! Regex.IsMatch(args[arg], paramRegex, RegexOptions.IgnoreCase))
                )
                {
                    Console.Write("{ " + collectArguments(args, arg - 1) + " }");
                    arg = args.Length;
                }
                else
                {
                    Console.Write(args[arg]);
                }
            }

            if (arg < args.Length - 1)
            {
                Console.Write(" ");
            }
        }
   }

   static string collectArguments(string[] args, int currentIndex)
   {
        string command = "";

        do
        {
            currentIndex++;
            command += args[currentIndex];

            if (currentIndex < args.Length - 1)
            {
                command += " ";
            }

        } while (currentIndex < args.Length - 1);

        return command;
   }
}