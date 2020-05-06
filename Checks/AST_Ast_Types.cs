using System;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Collections;
using System.Collections.Generic;

public class GroupedAstTypes
{
    //public static List<KeyValuePair<String, Double>> AnalyzeAst(Ast ast)
    public static IDictionary AnalyzeAst(Ast ast)
    {
        // Initialize Dictionary with all known AST object types initialized to 0.
        Dictionary<String, Double> astTypeDictionary = new Dictionary<String, Double>(StringComparer.OrdinalIgnoreCase);

        astTypeDictionary["Ast"] = 0;
        astTypeDictionary["SequencePointAst"] = 0;
        astTypeDictionary["ErrorStatementAst"] = 0;
        astTypeDictionary["ErrorExpressionAst"] = 0;
        astTypeDictionary["ScriptBlockAst"] = 0;
        astTypeDictionary["ParamBlockAst"] = 0;
        astTypeDictionary["NamedBlockAst"] = 0;
        astTypeDictionary["NamedAttributeArgumentAst"] = 0;
        astTypeDictionary["AttributeBaseAst"] = 0;
        astTypeDictionary["AttributeAst"] = 0;
        astTypeDictionary["TypeConstraintAst"] = 0;
        astTypeDictionary["ParameterAst"] = 0;
        astTypeDictionary["StatementBlockAst"] = 0;
        astTypeDictionary["StatementAst"] = 0;
        astTypeDictionary["TypeDefinitionAst"] = 0;
        astTypeDictionary["UsingStatementAst"] = 0;
        astTypeDictionary["MemberAst"] = 0;
        astTypeDictionary["PropertyMemberAst"] = 0;
        astTypeDictionary["FunctionMemberAst"] = 0;
        astTypeDictionary["CompilerGeneratedMemberFunctionAst"] = 0;
        astTypeDictionary["FunctionDefinitionAst"] = 0;
        astTypeDictionary["IfStatementAst"] = 0;
        astTypeDictionary["DataStatementAst"] = 0;
        astTypeDictionary["LabeledStatementAst"] = 0;
        astTypeDictionary["LoopStatementAst"] = 0;
        astTypeDictionary["ForEachStatementAst"] = 0;
        astTypeDictionary["ForStatementAst"] = 0;
        astTypeDictionary["DoWhileStatementAst"] = 0;
        astTypeDictionary["DoUntilStatementAst"] = 0;
        astTypeDictionary["WhileStatementAst"] = 0;
        astTypeDictionary["SwitchStatementAst"] = 0;
        astTypeDictionary["CatchClauseAst"] = 0;
        astTypeDictionary["TryStatementAst"] = 0;
        astTypeDictionary["TrapStatementAst"] = 0;
        astTypeDictionary["BreakStatementAst"] = 0;
        astTypeDictionary["ContinueStatementAst"] = 0;
        astTypeDictionary["ReturnStatementAst"] = 0;
        astTypeDictionary["ExitStatementAst"] = 0;
        astTypeDictionary["ThrowStatementAst"] = 0;
        astTypeDictionary["PipelineBaseAst"] = 0;
        astTypeDictionary["PipelineAst"] = 0;
        astTypeDictionary["CommandElementAst"] = 0;
        astTypeDictionary["CommandParameterAst"] = 0;
        astTypeDictionary["CommandBaseAst"] = 0;
        astTypeDictionary["CommandAst"] = 0;
        astTypeDictionary["CommandExpressionAst"] = 0;
        astTypeDictionary["RedirectionAst"] = 0;
        astTypeDictionary["MergingRedirectionAst"] = 0;
        astTypeDictionary["FileRedirectionAst"] = 0;
        astTypeDictionary["AssignmentStatementAst"] = 0;
        astTypeDictionary["ConfigurationDefinitionAst"] = 0;
        astTypeDictionary["DynamicKeywordStatementAst"] = 0;
        astTypeDictionary["ExpressionAst"] = 0;
        astTypeDictionary["BinaryExpressionAst"] = 0;
        astTypeDictionary["UnaryExpressionAst"] = 0;
        astTypeDictionary["BlockStatementAst"] = 0;
        astTypeDictionary["AttributedExpressionAst"] = 0;
        astTypeDictionary["ConvertExpressionAst"] = 0;
        astTypeDictionary["MemberExpressionAst"] = 0;
        astTypeDictionary["InvokeMemberExpressionAst"] = 0;
        astTypeDictionary["BaseCtorInvokeMemberExpressionAst"] = 0;
        astTypeDictionary["TypeExpressionAst"] = 0;
        astTypeDictionary["VariableExpressionAst"] = 0;
        astTypeDictionary["ConstantExpressionAst"] = 0;
        astTypeDictionary["StringConstantExpressionAst"] = 0;
        astTypeDictionary["ExpandableStringExpressionAst"] = 0;
        astTypeDictionary["ScriptBlockExpressionAst"] = 0;
        astTypeDictionary["ArrayLiteralAst"] = 0;
        astTypeDictionary["HashtableAst"] = 0;
        astTypeDictionary["ArrayExpressionAst"] = 0;
        astTypeDictionary["ParenExpressionAst"] = 0;
        astTypeDictionary["SubExpressionAst"] = 0;
        astTypeDictionary["UsingExpressionAst"] = 0;
        astTypeDictionary["IndexExpressionAst"] = 0;
        astTypeDictionary["AssignmentTarget"] = 0;
        astTypeDictionary["UNKNOWN"] = 0;

        // Return all targeted AST objects by Count and Percent across the entire input AST object.
        return RevokeObfuscationHelpers.AstValueGrouper(
            ast,
            typeof(Ast),
            astTypeDictionary,
            "AstGroupedAstTypes",
            targetAst => {
                return ((Ast)targetAst).GetType().FullName.Replace("System.Management.Automation.Language.","").Replace("System.","");
            }
        );
    }
}