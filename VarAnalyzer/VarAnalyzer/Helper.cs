using Microsoft.CodeAnalysis;

namespace VarAnalyzer
{
    internal static class Helper
    {
        internal static DiagnosticDescriptor CreateWarning(
            string title, 
            string messageFormat, 
            Category category)
        {
            return new DiagnosticDescriptor(
                Constant.DiagnosticId,
                title, 
                messageFormat, 
                category.ToString(),
                DiagnosticSeverity.Warning, 
                isEnabledByDefault: true);
        }
    }
}
