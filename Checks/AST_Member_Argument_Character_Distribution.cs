using System;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Collections;
using System.Collections.Generic;

public class MemberArgumentMetrics
{
    public static IDictionary AnalyzeAst(Ast ast)
    {
        // Build string list of all AST object values that will be later sent to StringMetricCalculator.
        List<String> stringList = new List<String>();

        foreach (InvokeMemberExpressionAst targetAst in ast.FindAll(testAst => testAst is InvokeMemberExpressionAst, true))
        {
            if (targetAst.Arguments != null)
            {
                // Extract the AST object value.
                stringList.Add(String.Join(",", targetAst.Arguments));
            }
        }

        // Return character distribution and additional string metrics across all targeted AST objects across the entire input AST object.
        return RevokeObfuscationHelpers.StringMetricCalculator(stringList, "AstMemberArgumentMetrics");
    }
}