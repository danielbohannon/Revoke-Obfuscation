using System;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Collections;
using System.Collections.Generic;

public class CommandParameterNameMetrics
{
    public static IDictionary AnalyzeAst(Ast ast)
    {
        // Build string list of all AST object values that will be later sent to StringMetricCalculator.
        List<string> stringList = new List<string>();
        
        foreach(CommandParameterAst targetAst in ast.FindAll( testAst => testAst is CommandParameterAst, true ))
        {
            // Extract the AST object value.
            string targetName = targetAst.Extent.Text;
            
            // Trim off the single leading dash of the Command Parameter value.
            if(targetName.Length > 1)
            {
                stringList.Add(targetName.Substring(1,targetName.Length-1));
            }
            else
            {
                stringList.Add(targetName);
            }
        }
        
        // Return character distribution and additional string metrics across all targeted AST objects across the entire input AST object.
        return RevokeObfuscationHelpers.StringMetricCalculator(stringList, "AstCommandParameterNameMetrics");
    }
}