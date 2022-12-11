using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{

    internal class Lexem
    {

        public enum SpecialSymbol
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
            Assign,// :=
            Parenthese_open,// (
            Parenthese_closed,// )
            Bracket_open,// [
            Bracket_closed,// ]
            Dot,
            DotDot,
            Space,
            Semicolon,
            EOL,
            EOF,
        }
        public enum KeyWord
        {
            AND,
            ARRAY,
            BEGIN,
            BOOLEAN,
            CHAR,
            CHR,
            CONST,
            DIV,
            DO,
            DOWNTO,
            ELSE,
            END,
            FOR,
            FUNCTION,
            IF,
            IN,
            INTEGER,
            MOD,
            NIL,
            NOT,
            OF,
            OR,
            PROCEDURE,
            PROGRAM,
            REAL,
            RECORD,
            REPEAT,
            THEN,
            TO,
            TYPE,
            UNTIL,
            VAR,
            WHILE,
            WITH,
            TRUE,
            FALSE,
        }
        public class UIntData
        {
            private decimal value = 0;
            private int c_base = 10;
            public decimal Value
            {
                get
                {
                    return value;
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
        public class URealData
        {
            private double value = 0;
            private int afterDot = 0;
            private int expValue = 0;
            private int expSign = 1;
            public double Value
            {
                get
                {
                    return value*Math.Pow(10, expSign*expValue);
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
                expValue = expValue*10+a;
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
        public class SpecialSymbolData
        {
            public SpecialSymbol Value { get; set; }
        }


        public int Adress;
        public int Line;
        public int Index;
        public Lexer.LexemTypes Type = Lexer.LexemTypes.Null;
        public object Value = null;
        public string Input=null;
        public Lexem(int adress, int line, int index, Lexer.LexemTypes type, object value, string input = null)
        {
            this.Adress = adress;
            this.Line = line;
            this.Index = index;
            this.Type = type;
            this.Value = value;
            switch (Value)
            {
                case Lexem.SpecialSymbol.Space:
                    return;
                case Lexem.SpecialSymbol.EOL:
                    return;
                case Lexem.SpecialSymbol.EOF:
                    return;
            }
            switch (Type)
            {
                case Lexer.LexemTypes.Comment:
                    return;
                case Lexer.LexemTypes.Null:
                    return;
            }
            this.Input = input;
        }
        public string Write()
        {
            if (Input == null || Input.Length == 0)
                return Line.ToString() + " " + Index.ToString() + " " + Type.ToString() + " " + Value.ToString();
            else
                return Line.ToString() + " " + Index.ToString() + " " + Type.ToString() + " " + Value.ToString() + " " + Input.ToString();
        }
    }
}
