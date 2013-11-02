using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using RDotNet;
using RDotNet.Internals;

namespace WebApplicationRdn
{
    public class Rdn
    {
        public static void Init()
        {
            if (Engine != null)
                return;
            else
            {
                REngine.SetEnvironmentVariables();
                Engine = REngine.CreateInstance("REngine");
                Engine.Initialize();
            }
        }

        public static REngine Engine { get; private set; }

        internal static string Evaluate(string statement)
        {
            try
            {
                var sexp = Engine.Evaluate(statement);
                switch (sexp.Type)
                {
                       case SymbolicExpressionType.CharacterVector:
                            return PrintDisplay(sexp.AsCharacter().ToArrayFast());
                       case SymbolicExpressionType.NumericVector:
                            return PrintDisplay(sexp.AsNumeric().ToArrayFast());
                       case SymbolicExpressionType.IntegerVector:
                            return PrintDisplay(sexp.AsInteger().ToArrayFast());
                       default:
                            return sexp.Type.ToString();
                }
            }
            catch (Exception ex)
            {
                return FormatException(ex);
            }
        }

        public static string FormatException(Exception ex)
        {
            Exception innermost = ex;
            while (innermost.InnerException != null)
                innermost = innermost.InnerException;

            var result = string.Format("Type:    {1}{0}Message: {2}{0}Method:  {3}{0}Stack trace:{0}{4}{0}{0}",
                Environment.NewLine, innermost.GetType(), innermost.Message, innermost.TargetSite, innermost.StackTrace);
            return result;
        }


        private static string PrintDisplay<T>(T[] array)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[ ");
            for (int i = 0; i < (array.Length - 1); i++)
            {
                sb.Append(array[i]);
                sb.Append(", ");
            }
            sb.Append(array[array.Length - 1]);
            sb.Append(" ]");
            return sb.ToString();
        }
    }
}