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
            InvalidArgs,
            InvalidAction,
            InvalidTypes,
            CantCast,
            XExpexted,
            XExpextedYGot,
            Custom
        }
        public static class ErrorConstructor
        {
            public static string GetPositionMassage(int line, int idx, Error err, string arg1 = null, string arg2 = null)
            {
                var msg = "";
                switch (err)
                {
                    case Error.CharRange:
                        msg = "Expexted number in [0,127] range as char code";
                        break;
                    case Error.UnknownSymbol:
                        msg = "Unknown symbol";
                        break;
                    case Error.InvalidSymbol:
                        msg = "Invalid symbol";
                        break;
                    case Error.InvalidArgs:
                        msg = "Invalid Arguments";
                        break;
                    case Error.UnexpectedSymbol:
                        msg = "Unexpected symbol";
                        break;
                    case Error.InvalidAction:
                        msg = arg1 != null ? String.Format("\"{0}\" action is not valid", arg1) : "Action is not valid, wrong arguments";
                        break;
                    case Error.InvalidTypes:
                        msg = (arg1 != null && arg2 != null) ?
                            String.Format("Operator does not support \"{0}\" and \"{1}\"", arg1 ?? "error", arg2) : "Operator does not support X and Y, wrong arguments";
                        break;
                    case Error.CantCast:
                        msg = (arg1 != null && arg2 != null) ?
                            String.Format("Can not cast \"{0}\" to \"{1}\"", arg1 ?? "error", arg2) : "Can not cast X to Y, wrong arguments";
                        break;
                    case Error.XExpexted:
                        msg = arg1 != null ? String.Format("\"{0}\" expected", arg1) : "X Expected, wrong arguments";
                        break;
                    case Error.XExpextedYGot:
                        msg = (arg1 != null && arg2 != null) ?
                            String.Format("\"{0}\" expected but \"{1}\" got", arg1 ?? "error", arg2) : "X Expected Y Got, wrong arguments";
                        break;
                    case Error.Custom:
                        msg = arg1 != null ? String.Format("{0}", arg1) : "Unknown error";
                        break;
                    default:
                        msg = "Unknown error";
                        break;
                }
                return msg + " at " + line + " " + idx;
            }
        }
        public static string fileText = "";
        public static bool lexerOnly = false;
        public static bool expressionOnly = false;
        public static bool parserOnly = false;
        public static bool semantixOnly = false;
        public static void SetupWithArgs(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                fileText = System.IO.File.ReadAllText(@"input.in");
                lexerOnly = true;
            }
            else
            {
                foreach (string arg in args)
                {
                    if (arg == "-l") lexerOnly = true;
                    else if (arg == "-e") expressionOnly = true;
                    else if (arg == "-p") parserOnly = true;
                    else if (arg == "-s") semantixOnly = true;
                    else fileText = System.IO.File.ReadAllText(@arg);
                }
            }
        }
        public static void WriteLexems()
        {
            Lexer lexer = new Lexer(fileText);
            lexer.NextToken();
            while (!lexer.Token.Value.Equals(Lexem.SpecialSymbol.EOF))
            {
                Console.WriteLine(lexer.Token.Write());
                lexer.NextToken();
            }
            Console.WriteLine(lexer.Token.Write());
        }
        public static void WriteExpression()
        {
            Lexer lexer = new Lexer(fileText);
            SimpleParser parser = new SimpleParser(ref lexer);
            SimpleParser.Node exp = parser.ParseSimpleExpression();
            if ((!lexer.Token.Value.Equals(Lexem.SpecialSymbol.EOF) || !lexer.Token.Value.Equals(Lexem.SpecialSymbol.EOL)))
                throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.UnexpectedSymbol));
            SimpleParser.PrintNodeTree(exp, "", true);
            SimpleParser.PrintNodeTree(exp, "", true);
        }
        public static void ParseTree()
        {
            Lexer lexer = new Lexer(fileText);
            Parser parser = new Parser  (ref lexer);
            Parser.Node exp = parser.ParseProgramm();
            Parser.PrintNodeTree(exp, "", true);
        }
        public static void ParseTables()
        {
            Lexer lexer = new Lexer(fileText);
            Parser parser = new Parser(ref lexer);
            Parser.Node exp = parser.ParseProgramm();
            Parser.PrintNodeTree(exp, "", true);
            Console.WriteLine();
            Parser.PrintSymbolTable(parser.symTableStack.Get("main"), "", true);
        }

        static void Main(string[] args)
        {
            Console.ReadKey();
            SetupWithArgs(args);
            if (args == null || args.Length == 0)
            {
                if (parserOnly) ParseTree();
                if (semantixOnly) ParseTables();
                if (expressionOnly) WriteExpression();
                if (lexerOnly) WriteLexems();
            }
            else
            {
                try
                {
                    if (parserOnly) ParseTree();
                    if (semantixOnly) ParseTables();
                    if (expressionOnly) WriteExpression();
                    if (lexerOnly) WriteLexems();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}