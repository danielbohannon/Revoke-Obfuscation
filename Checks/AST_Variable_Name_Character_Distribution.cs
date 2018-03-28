using System;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Collections;
using System.Collections.Generic;

public class VariableNameMetrics
{
    public static IDictionary AnalyzeAst(Ast ast)
    {
        // Build string list of all AST object values that will be later sent to StringMetricCalculator.
        List<String> stringList = new List<String>();

        foreach(Ast targetAst in ast.FindAll( testAst => testAst is VariableExpressionAst, true ))
        {
            // Extract the AST object value.
            String targetName = targetAst.Extent.Text;
            
            // Trim off the single leading "$" in the variable name.
            if(targetName.Length > 0)
            {
                stringList.Add(targetName.Substring(1,targetName.Length-1));
            }
        }
        
        // Return character distribution and additional string metrics across all targeted AST objects across the entire input AST object.
        return RevokeObfuscationHelpers.StringMetricCalculator(stringList, "AstVariableNameMetrics");
    }
}