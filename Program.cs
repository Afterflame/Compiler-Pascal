using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    internal class Program
    {
        public enum Error
        {
            CharRange,
            UnknownSymbol,
            UnexpectedSymbol,
            InvalidSymbol,
            FactorE,
            OBracketE,
            CBracketE,
        }
        public static class ErrorConstructor
        {
            public static string GetPositionMassage(int line, int idx, Error err)
            {
                string text;
                switch (err)
                {
                    case Error.CharRange:
                        text = "Expexted number in [0,127] range as char code";
                        break;
                    case Error.UnknownSymbol:
                        text = "Unknown symbol";
                        break;
                    case Error.InvalidSymbol:
                        text = "Invalid symbol";
                        break;
                    case Error.UnexpectedSymbol:
                        text = "Unexpected symbol";
                        break;
                    case Error.FactorE:
                        text = "Factor expected";
                        break;
                    case Error.OBracketE:
                        text = "Opening bracket expected";
                        break;
                    case Error.CBracketE:
                        text = "Closing bracket expected";
                        break;
                    default:
                        text = "Unknown error";
                        break;
                }
                return text + " at " + line + " " + idx;
            }
        }
        public static string fileText="";
        public static bool lexerOnly = false;
        public static bool expressionOnly = false;
        public static bool syntaxOnly = false;
        public static void SetupWithArgs(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                fileText = System.IO.File.ReadAllText(@"input.in");
                expressionOnly = true;
            }
            else
            {
                foreach (string arg in args)
                {
                    if (arg == "-l") lexerOnly = true;
                    else if (arg == "-e") expressionOnly = true;
                    else if (arg == "-s") syntaxOnly = true;
                    else fileText = System.IO.File.ReadAllText(@arg);
                }
            }
        }
        public static void WriteExpression()
        {
            try
            {
                Lexer lexer = new Lexer(fileText);
                Parser parser = new Parser(ref lexer);
                Node exp = parser.ParseExpression();
                if (lexer.Token.Type != Lexer.LexemTypes.Divider && (lexer.Token.Value != (object)Lexem.DSpecial.EOF || lexer.Token.Value != (object)Lexem.DSpecial.EOL))
                    throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.UnexpectedSymbol));
                Parser.PrintExpressionTree(exp, "", true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        static void Main(string[] args)
        {
            SetupWithArgs(args);
            if (expressionOnly) WriteExpression();
            Console.ReadKey();
        }
    }
}
