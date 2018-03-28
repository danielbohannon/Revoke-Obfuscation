using System;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Collections;
using System.Collections.Generic;

public class GroupedAssignmentStatements
{
    public static IDictionary AnalyzeAst(Ast ast)
    {
        // Initialize Dictionary with common array counts initialized to 0.
        Dictionary<String, Double> astGroupedAssignmentStatementsDictionary = new Dictionary<String, Double>(StringComparer.OrdinalIgnoreCase);
        
        astGroupedAssignmentStatementsDictionary["Equals"] = 0;
        astGroupedAssignmentStatementsDictionary["PlusEquals"] = 0;
        astGroupedAssignmentStatementsDictionary["MinusEquals"] = 0;
        astGroupedAssignmentStatementsDictionary["MultiplyEquals"] = 0;
        astGroupedAssignmentStatementsDictionary["DivideEquals"] = 0;
        astGroupedAssignmentStatementsDictionary["RemainderEquals"] = 0;
        astGroupedAssignmentStatementsDictionary["UNKNOWN"] = 0;
        
        // Return all targeted AST objects by Count and Percent across the entire input AST object.
        return RevokeObfuscationHelpers.AstValueGrouper(ast, typeof(AssignmentStatementAst), astGroupedAssignmentStatementsDictionary, "AstGroupedAssignmentStatements",
            targetAst => { return ((AssignmentStatementAst) targetAst).Operator.ToString(); } );
    }
}