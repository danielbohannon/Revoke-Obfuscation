using System;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Collections;
using System.Collections.Generic;

public class GroupedUnaryExpressionOperators
{
    public static IDictionary AnalyzeAst(Ast ast)
    {
        // Initialize Dictionary with common array counts initialized to 0.
        Dictionary<String, Double> astGroupedUnaryExpressionOperatorsDictionary = new Dictionary<String, Double>(StringComparer.OrdinalIgnoreCase);

        astGroupedUnaryExpressionOperatorsDictionary["Exclaim"] = 0;
        astGroupedUnaryExpressionOperatorsDictionary["Not"] = 0;
        astGroupedUnaryExpressionOperatorsDictionary["Minus"] = 0;
        astGroupedUnaryExpressionOperatorsDictionary["Plus"] = 0;
        astGroupedUnaryExpressionOperatorsDictionary["Bnot"] = 0;
        astGroupedUnaryExpressionOperatorsDictionary["PlusPlus"] = 0;
        astGroupedUnaryExpressionOperatorsDictionary["MinusMinus"] = 0;
        astGroupedUnaryExpressionOperatorsDictionary["PostfixPlusPlus"] = 0;
        astGroupedUnaryExpressionOperatorsDictionary["PostfixMinusMinus"] = 0;
        astGroupedUnaryExpressionOperatorsDictionary["Join"] = 0;
        astGroupedUnaryExpressionOperatorsDictionary["Isplit"] = 0;
        astGroupedUnaryExpressionOperatorsDictionary["Csplit"] = 0;
        astGroupedUnaryExpressionOperatorsDictionary["UNKNOWN"] = 0;

        // Return all targeted AST objects by Count and Percent across the entire input AST object.
        return RevokeObfuscationHelpers.AstValueGrouper(
            ast,
            typeof(UnaryExpressionAst),
            astGroupedUnaryExpressionOperatorsDictionary,
            "AstGroupedUnaryExpressionOperators",
            targetAst => {
                return ((UnaryExpressionAst) targetAst).TokenKind.ToString();
            }
        );
    }
}