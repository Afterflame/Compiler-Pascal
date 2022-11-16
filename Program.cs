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
        public static string fileText;
        public static bool lexOnly;
        public class Lexem
        {
            public int Adress;
            public int Line;
            public int Index;
            public string Type = null;
            public string Value = null;
            public string Input;
            public Lexem(int adress, int line, int index, string type, string value, string input = null)
            {
                this.Adress = adress;
                this.Line = line;
                this.Index = index;
                this.Type = type;
                this.Value = value;
                this.Input = input;
            }
            public string Write()
            {
                if (Input == null || Input.Length == 0)
                    return Line.ToString() + " " + Input.ToString() + " " + Type.ToString() + " " + Value.ToString();
                else
                    return Line.ToString() + " " + Input.ToString() + " " + Type.ToString() + " " + Value.ToString() + " " + Input.ToString();
            }
        }
        public enum ASpecial
        {
            Plus,// +
            Minus,// -
            Multiply,// *
            Divide,// /
            Equal,// =
            Greater,// >
            Less,// <
            GreaterEq,// >=
            LessEq,// <=
            NotEq,// <>
            Becomes,// :=
            Bracket_open,// :=
            Bracket_closed,// :=
        }
        public enum DSpecial
        {
            Space,
            Semicolon,
            EOL,
            EOF,
        }
        public class IntData
        {
            private decimal value = 0;
            private int sign = 1;
            private int c_base = 10;
            public decimal Value
            {
                get
                {
                    return value * sign;
                }
            }
            public int Sign
            {
                get
                {
                    return sign;
                }
                set
                {
                    sign = value;
                }
            }
            public void AddDigit(int a)
            {
                value = value * c_base + a;
            }
            public void SetBase(int a)
            {
                c_base = a;
            }
        }
        public class RealData
        {
            private double value = 0;
            private int sign = 1;
            private int afterDot = 0;
            private int expAddedValue = 0;
            private int expSign = 1;
            public double Value
            {
                get
                {
                    if (sign == 1)
                        return Math.Pow(value, expAddedValue + 1) * sign;
                    else
                        return Math.Pow(value, -expAddedValue) * sign;
                }
            }
            public int ExpSign
            {
                get
                {
                    return expSign;
                }
                set
                {
                    expSign = value;
                }
            }
            public int Sign
            {
                get
                {
                    return sign;
                }
                set
                {
                    sign = value;
                }
            }
            public void AddDigit(int a)
            {
                value = value * 10 + a;
            }
            public void AddDecimal(int a)
            {
                afterDot++;
                value += Math.Pow(0.1, afterDot) * a;
            }
            public void AddExp(int a)
            {
                expAddedValue += a;
            }
        }
        public class LiteralData
        {
            string value = "";
            int lastCharValue = -1;
            public string Value
            {
                get
                {
                    return value;
                }
            }
            public void AddChar(char ch)
            {
                if (lastCharValue != -1)
                    value += (char)lastCharValue;
                lastCharValue = -1;
                value += ch;
            }
            public void IncreaceChar(int a)
            {
                if (lastCharValue * 10 + a > 127) throw new Exception();
                if (lastCharValue == -1)
                    lastCharValue = a;
                else
                    lastCharValue = lastCharValue * 10 + a;
            }
        }
        public class IdentifierData
        {
            string value = "";
            public string Value { get { return value; } }
            public void AddChar(char ch)
            {
                value += ch;
            }
        }
        public class ASpecialData
        {
            public ASpecial Value { get; set; }
        }
        public class DSpecialData
        {
            public DSpecial Value { get; set; }
        }
        
       
        public static void SetupWithArgs(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                fileText = System.IO.File.ReadAllText(@"input.txt");
                lexOnly = true;
            }
            else
            {
                foreach (string arg in args)
                {
                    if (arg == "-l") lexOnly = true;
                    else fileText = System.IO.File.ReadAllText(@arg);
                }
            }
        }
        static void Main(string[] args)
        {
            SetupWithArgs(args);
            Lexer lexer = new Lexer(fileText);
            Parser parser = new Parser(ref lexer);
            Parser.Node exp = parser.ParseExpression();
            Parser.PrintExpressionTree(exp, "", true);
            List<Lexem> answers = new List<Lexem>();
            while (answers.Count == 0 || !(answers.Last().Value == "EOF" && answers.Last().Type == "Divider"))
            {
                answers.Add(lexer.NextToken());
                Console.WriteLine(answers.Last().Value + ' ' + answers.Last().Type);
            }
        }
    }
}
