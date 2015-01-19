using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

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
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
        }
    }
}
