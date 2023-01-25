using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{

    public class Lexem
    {
        public enum Types
        {
            Null,
            UInteger,
            UReal,
            Literal,
            Identifier,
            Comment,
            KeyWord,
            Divider,
            Additive_Op,
            Multiplicative_Op,
            Relational_Op,
            Bool_,
            Sign,
            Special,
        }
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
            LParenthese,// (
            RParenthese,// )
            LBracket,// [
            RBracket,// ]
            At,// @
            Colon,
            Comma,
            Dot,
            DotDot,
            Space,
            Semicolon,
            EOL,
            EOF,
        }
        public enum KeyWord
        {
            LABEL,
            CASE,
            STRING,
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
            public int Value
            {
                get
                {
                        return Decimal.ToInt32(value);
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
            public void SetAsBase()
            {
                c_base = Decimal.ToInt32(value);
                value = 0;
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
                    if (lastCharValue > 127) throw new Exception();
                    if(lastCharValue!=-1)
                        return value + Convert.ToChar(lastCharValue).ToString();
                    return value;
                }
            }
            public void AddChar(char ch)
            {
                if (lastCharValue != -1)
                    value += Convert.ToChar(lastCharValue).ToString();
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
            public void FinishChar()
            {
                if (lastCharValue != -1)
                    value += (char)lastCharValue;
                lastCharValue = -1;
            }
        }
        public class IdentifierData
        {
            string value = "";
            public string Value { get { return value.ToUpper(); } }
            public void AddChar(char ch)
            {
                if(value.Length<32)
                    value += ch;
                value = value.ToUpper();
            }
        }
        public class SpecialSymbolData
        {
            public SpecialSymbol Value { get; set; }
        }


        public int Adress;
        public int Line;
        public int Index;
        public Types Type = Types.Null;
        public object Value = null;
        public string Input=null;
        public Lexem(int adress, int line, int index, Types type, object value, string input = null)
        {
            Adress = adress;
            Line = line;
            Index = index;
            Type = type;
            Value = value;
            switch (Value)
            {
                case SpecialSymbol.Space:
                    return;
                case SpecialSymbol.EOL:
                    return;
                case SpecialSymbol.EOF:
                    return;
                case KeyWord.DIV:
                    Type = Types.Multiplicative_Op;
                    break;
                case KeyWord.MOD:
                    Type = Types.Multiplicative_Op;
                    return;
                case KeyWord.AND:
                    Type = Types.Multiplicative_Op;
                    break;
                case KeyWord.OR:
                    Type = Types.Multiplicative_Op;
                    return;
            }
            switch (Type)
            {
                case Types.Comment:
                    return;
                case Types.Null:
                    return;
            }
            this.Input = input;
        }
        public string Write()
        {
            if (Input == null || Input.Length == 0)
                return String.Format("{0,5} {1,5} {2,20} {3,35}", Line.ToString(), Index.ToString(), Type.ToString(), Value.ToString());
            else
                return String.Format("{0,5} {1,5} {2,20} {3,35}  {4,35}", Line.ToString(), Index.ToString(), Type.ToString(), Value.ToString(), Input.ToString());
        }
    }
}
