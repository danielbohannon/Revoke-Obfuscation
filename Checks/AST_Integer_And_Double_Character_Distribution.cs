using System;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Collections;
using System.Collections.Generic;

public class IntegerAndDoubleMetrics
{
    public static IDictionary AnalyzeAst(Ast ast)
    {
        // Build string list of all AST object values that will be later sent to StringMetricCalculator.
        List<string> stringList = new List<string>();
        
        foreach(ConstantExpressionAst targetAst in ast.FindAll( testAst => testAst is ConstantExpressionAst, true ))
        {
            // Extract the AST object value.
            // If StaticType name starts with "Int" or "Double" then extract the value for metrics.
            // We use .Extent.Text for value since it preserves non-numerical representations (e.g., 0x001F0FFF --> 2035711, 0x52 --> 82).
            if((targetAst.StaticType.Name.Substring(0,3) == "Int") || (targetAst.StaticType.Name.Substring(0,6) == "Double"))
            {
                stringList.Add(targetAst.Extent.Text);
            }
        }
        
        // Return character distribution and additional string metrics across all targeted AST objects across the entire input AST object.
        return RevokeObfuscationHelpers.StringMetricCalculator(stringList, "AstIntegerAndDoubleMetrics");
    }
}