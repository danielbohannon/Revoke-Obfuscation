using System;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Collections;
using System.Collections.Generic;

public class FunctionNameMetrics
{
    public static IDictionary AnalyzeAst(Ast ast)
    {
        // Build string list of all AST object values that will be later sent to StringMetricCalculator.
        List<String> stringList = new List<String>();
        
        // Create required variables for tokenization.
        Token[] tokens = new Token[]{};
        ParseError[] parseErrors = new ParseError[]{};

        // Tokenize the entire input script. We must tokenize (instead of AST) to retrieve function values with obfuscation (like ticks) intact.
        Parser.ParseInput( ast.Extent.Text, out tokens, out parseErrors );

        // Iterate each token returned from above tokenization.
        foreach(Token token in tokens)
        {
            // If token is a function name then add to stringList.
            if( (token.Kind.ToString() == "Generic") && (token.TokenFlags.ToString() == "None") )
            {
                stringList.Add(token.Text);
            }
        }
        
        // Return character distribution and additional string metrics across all targeted AST objects across the entire input AST object.
        return RevokeObfuscationHelpers.StringMetricCalculator(stringList, "AstFunctionNameMetrics");
    }
}