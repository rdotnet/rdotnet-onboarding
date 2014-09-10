using System.Collections.Generic;
using System.Linq;
using RDotNet;
using RDotNet.Internals;

namespace WebApplicationRdn.Models
{
    public class RInternalResult
    {
        public string Type { get; set; }
        public List<string> Values { get; set; }
    }

    public class SymbolicExpressionToResultMapper
    {
        private delegate IEnumerable<string> MapExpression(SymbolicExpression sexp);

        private readonly Dictionary<SymbolicExpressionType, MapExpression> _mappers = new Dictionary
            <SymbolicExpressionType, MapExpression>
        {
            {SymbolicExpressionType.Any, MapAsEmpty},
            {SymbolicExpressionType.BuiltinFunction, MapAsEmpty},
            {SymbolicExpressionType.ByteCode, MapAsEmpty},
            {SymbolicExpressionType.CharacterVector, MapCharacterVector},
            {SymbolicExpressionType.Closure, MapAsEmpty},
            {SymbolicExpressionType.ComplexVector, MapComplexVector},
            {SymbolicExpressionType.DotDotDotObject, MapAsEmpty},
            {SymbolicExpressionType.Environment, MapAsEmpty},
            {SymbolicExpressionType.ExpressionVector, MapAsEmpty},
            {SymbolicExpressionType.ExternalPointer, MapAsEmpty},
            {SymbolicExpressionType.IntegerVector, MapIntegerVector},
            {SymbolicExpressionType.InternalCharacterString, MapAsEmpty},
            {SymbolicExpressionType.LanguageObject, MapAsEmpty},
            {SymbolicExpressionType.List, MapAsEmpty},
            {SymbolicExpressionType.LogicalVector, MapAsEmpty},
            {SymbolicExpressionType.Null, MapAsEmpty},
            {SymbolicExpressionType.NumericVector, MapNumericVector},
            {SymbolicExpressionType.Pairlist, MapAsEmpty},
            {SymbolicExpressionType.Promise, MapAsEmpty},
            {SymbolicExpressionType.RawVector, MapAsEmpty},
            {SymbolicExpressionType.S4, MapAsEmpty},
            {SymbolicExpressionType.SpecialFunction, MapAsEmpty},
            {SymbolicExpressionType.Symbol, MapAsEmpty},
            {SymbolicExpressionType.WeakReference, MapAsEmpty},
        };

        public RInternalResult Map(SymbolicExpression sexp)
        {
            //Comments return null instead of a SEXP.
            if (sexp == null) return new RInternalResult();

            var mapper = _mappers[sexp.Type];
            var mapped = mapper(sexp);

            var result = new RInternalResult
            {
                Type = sexp.Type.ToString(),
                Values = mapped.ToList()
            };

            return result;
        }

        //logical, integer, real, complex, character, list?, raw?
        private static IEnumerable<string> MapCharacterVector(SymbolicExpression sexp)
        {
            var mapped = sexp.AsCharacter().Select(s => s.ToString());
            return mapped;
        }

        private static IEnumerable<string> MapIntegerVector(SymbolicExpression sexp)
        {
            var mapped = sexp.AsInteger().Select(s => s.ToString());
            return mapped;
        }

        private static IEnumerable<string> MapNumericVector(SymbolicExpression sexp)
        {
            var mapped = sexp.AsNumeric().Select(s => s.ToString());
            return mapped;
        }

        private static IEnumerable<string> MapComplexVector(SymbolicExpression sexp)
        {
            var mapped = sexp.AsComplex().Select(s => s.ToString());
            return mapped;
        }

        private static IEnumerable<string> MapAsEmpty(SymbolicExpression sexp)
        {
            //TODO: Report missing mappers.
            return Enumerable.Empty<string>();
        }
    }
}
