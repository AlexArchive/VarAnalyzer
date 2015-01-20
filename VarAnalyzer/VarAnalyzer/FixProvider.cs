using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VarAnalyzer
{
    [ExportCodeFixProvider(Constant.DiagnosticId, LanguageNames.CSharp), Shared]
    internal class FixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> GetFixableDiagnosticIds() => ImmutableArray.Create(Constant.DiagnosticId);
        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task ComputeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            Diagnostic diagnostic = context.Diagnostics.First();
            TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;
            VariableDeclarationSyntax variableDeclaration = root
                .FindToken(diagnosticSpan.Start)
                .Parent
                .AncestorsAndSelf()
                .OfType<VariableDeclarationSyntax>()
                .First();

            CodeAction action = CodeAction.Create(
                "Replace var with actual type", 
                cancellationToken => Fix(context.Document, variableDeclaration, cancellationToken));

            context.RegisterFix(action, diagnostic);
        }

        private async Task<Document> Fix(
            Document document,
            VariableDeclarationSyntax variableDeclaration,
            CancellationToken cancellationToken)
        {
            TypeSyntax variableTypeName = variableDeclaration.Type;
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            ITypeSymbol type = semanticModel.GetTypeInfo(variableTypeName).ConvertedType;
            TypeSyntax typeName = SyntaxFactory.ParseTypeName(type.ToDisplayString())
                .WithLeadingTrivia(variableTypeName.GetLeadingTrivia())
                .WithTrailingTrivia(variableTypeName.GetTrailingTrivia());
            TypeSyntax simplifiedTypeName = typeName.WithAdditionalAnnotations(Simplifier.Annotation);
            VariableDeclarationSyntax explicitVariableDeclaration = variableDeclaration
                .WithType(simplifiedTypeName)
                .WithAdditionalAnnotations(Formatter.Annotation);
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);
            SyntaxNode newRoot = root.ReplaceNode(variableDeclaration, explicitVariableDeclaration);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}