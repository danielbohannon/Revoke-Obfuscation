using System;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Collections;
using System.Collections.Generic;

public class ArrayElementMetrics
{
    public static IDictionary AnalyzeAst(Ast ast)
    {
        // Build string list of all AST object values that will be later sent to StringMetricCalculator.
        List<String> stringList = new List<String>();

        foreach(Ast targetAst in ast.FindAll( testAst => testAst is ArrayLiteralAst, true ))
        {
            // Extract the AST object value.
            String targetName = targetAst.Extent.Text;
            
            stringList.Add(targetName);
        }
        
        // Return character distribution and additional string metrics across all targeted AST objects across the entire input AST object.
        return RevokeObfuscationHelpers.StringMetricCalculator(stringList, "AstArrayElementMetrics");
    }
}