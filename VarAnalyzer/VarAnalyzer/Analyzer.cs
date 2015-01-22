using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace VarAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class Analyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor _description = Helper.CreateWarning(
            "You should avoid using var here",
            "You should avoid using var here",
            Category.Readability);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_description);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.VariableDeclaration);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var variableDeclaration = (VariableDeclarationSyntax)context.Node;
            TypeSyntax variableTypeName = variableDeclaration.Type;
            if (!variableTypeName.IsVar)
                return;
            if (variableDeclaration.Variables.Count > 1)
                return;
            IAliasSymbol aliasInfo = context.SemanticModel.GetAliasInfo(variableTypeName);
            if (aliasInfo != null)
                return;
            ITypeSymbol type = context.SemanticModel.GetTypeInfo(variableTypeName).ConvertedType;
            if (type.IsAnonymousType)
                return;
            context.ReportDiagnostic(Diagnostic.Create(_description, variableDeclaration.Type.GetLocation()));
        }
    }
}
