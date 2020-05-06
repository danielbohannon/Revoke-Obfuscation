using System;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CommentMetrics
{
    public static IDictionary AnalyzeAst(Ast ast)
    {
        // Build string list of all AST object values that will be later sent to StringMetricCalculator.
        List<String> stringList = new List<String>();
        // Set entire script into a variable so we can perform regex (if necessary) to remove code signature block.
        string scriptContent = ast.Extent.Text;

        // Only perform regex removal of signature blocks if the signature block header and tail syntax exist.
        if (scriptContent.Contains("# SIG # Begin signature block") && scriptContent.Contains("# SIG # End signature block"))
        {
            string pattern = "(?s)# SIG # Begin signature block.*?# SIG # End signature block\\s*$";
            string replacement = "";
            Regex regExp = new Regex( pattern );
            scriptContent = regExp.Replace( scriptContent, replacement );
        }

        // Create required variables for tokenization.
        Token[] tokens = new Token[]{};
        ParseError[] parseErrors = new ParseError[]{};

        // Tokenize the entire input script. We must tokenize (instead of AST) to retrieve comment tokens.
        Parser.ParseInput( scriptContent, out tokens, out parseErrors );

        // Iterate each token returned from above tokenization.
        foreach (Token token in tokens)
        {
            // If token is a comment then add to stringList.
            if (token.Kind.ToString() == "Comment")
            {
                stringList.Add(token.Text);
            }
        }

        // Return character distribution and additional string metrics across all targeted AST objects across the entire input AST object.
        return RevokeObfuscationHelpers.StringMetricCalculator(stringList, "AstCommentMetrics");
    }
}