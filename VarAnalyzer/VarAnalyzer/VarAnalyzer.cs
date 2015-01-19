using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VarAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class VarAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "Var1";
        internal const string Title = "Abstain from using implicitly typed variables";
        internal const string MessageFormat = "Abstain from using implicitly typed variables";
        internal const string Category = "Readability";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId, 
            Title, 
            MessageFormat, 
            Category, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(Rule); }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.VariableDeclaration);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            VariableDeclarationSyntax variableDeclaration = (VariableDeclarationSyntax)context.Node;
            TypeSyntax variableTypeName = variableDeclaration.Type;
            if (variableTypeName.IsVar)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, variableDeclaration.Type.GetLocation()));
            }
        }
    }
}
