using System;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Collections;
using System.Collections.Generic;

public class GroupedBinaryExpressionOperators
{
    public static IDictionary AnalyzeAst(Ast ast)
    {
        // Initialize Dictionary with common array counts initialized to 0.
        Dictionary<String, Double> astGroupedBinaryExpressionOperatorsDictionary = new Dictionary<String, Double>(StringComparer.OrdinalIgnoreCase);
        
        astGroupedBinaryExpressionOperatorsDictionary["And"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Or"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Is"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["IsNot"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["As"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["DotDot"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Multiply"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Divide"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Rem"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Plus"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Minus"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Format"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Xor"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Shl"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Shr"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Band"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Bor"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Bxor"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Join"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Ieq"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Ine"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Ige"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Igt"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Ilt"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Ile"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Ilike"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Inotlike"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Imatch"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Inotmatch"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Ireplace"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Icontains"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Inotcontains"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Iin"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Inotin"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Isplit"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Ceq"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Cne"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Cge"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Cgt"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Clt"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Cle"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Clike"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Cnotlike"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Cmatch"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Cnotmatch"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Creplace"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Ccontains"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Cnotcontains"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Cin"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Cnotin"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["Csplit"] = 0;
        astGroupedBinaryExpressionOperatorsDictionary["UNKNOWN"] = 0;
        
        // Return all targeted AST objects by Count and Percent across the entire input AST object.
        return RevokeObfuscationHelpers.AstValueGrouper(ast, typeof(BinaryExpressionAst), astGroupedBinaryExpressionOperatorsDictionary, "AstGroupedBinaryExpressionOperators",
            targetAst => { return ((BinaryExpressionAst) targetAst).Operator.ToString(); } );
    }
}