using Microsoft.CodeAnalysis;

namespace VarAnalyzer
{
    internal static class Helper
    {
        internal static DiagnosticDescriptor CreateWarning(
            string title, 
            string messageFormat, 
            Category category, 
            string id = "test")
        {
            return new DiagnosticDescriptor(
                id, 
                title, 
                messageFormat, 
                category.ToString(),
                DiagnosticSeverity.Warning, 
                isEnabledByDefault: true);
        }
    }
}
