using System;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Collections;
using System.Collections.Generic;

public class StringMetrics
{
    public static IDictionary AnalyzeAst(Ast ast)
    {
        // Build string list of all AST object values that will be later sent to StringMetricCalculator.
        List<String> stringList = new List<String>();

        foreach(StringConstantExpressionAst targetAst in ast.FindAll( testAst => testAst is StringConstantExpressionAst, true ))
        {
            if(targetAst.StringConstantType.ToString() != "BareWord")
            {
                // Extract the AST object value.
                String targetName = targetAst.Extent.Text.Replace("\n","");
                
                // Trim off the leading and trailing single- or double-quote for the current string.
                if(targetName.Length > 2)
                {
                    stringList.Add(targetName.Substring(1,targetName.Length-2));
                }
                else
                {
                    stringList.Add(targetName);
                }
            }
        }
        
        // Return character distribution and additional string metrics across all targeted AST objects across the entire input AST object.
        return RevokeObfuscationHelpers.StringMetricCalculator(stringList, "AstStringMetrics");
    }
}