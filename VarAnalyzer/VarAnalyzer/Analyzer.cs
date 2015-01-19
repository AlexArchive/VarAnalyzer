using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace VarAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class Analyzer : DiagnosticAnalyzer
    {
        private static DiagnosticDescriptor description = Helper.CreateWarning(
            "You should avoid using var here", 
            "You should avoid using var here", 
            Category.Readability);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(description);

        public override void Initialize(AnalysisContext context) => 
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.VariableDeclaration);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            VariableDeclarationSyntax variableDeclaration = (VariableDeclarationSyntax)context.Node;
            TypeSyntax variableTypeName = variableDeclaration.Type;
            if (variableTypeName.IsVar)
                context.ReportDiagnostic(Diagnostic.Create(description, variableDeclaration.Type.GetLocation()));
        }
    }
}
