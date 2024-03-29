﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    internal class Program
    {
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
        public static string fileText;
        public static bool lexOnly;
        public enum ASpecial
        {
            Plus,// +
            Minus,// -
            Asterisk,// *
            Slash,// /
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
                if (lastCharValue * 10 + a > 127) throw new ArgumentException("Expexted number in [0,127] range as char code");
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
        public class Lexer
        {
            public Lexem Token;
            public enum State
            {
                Start, //
                Number_char, // 0-9
                Int_notation, //%&
                HexInt_notation, //%
                Int_char, // 0-9
                HexInt_char, //a-f, 0-9
                Float_dot, // .
                Float_char, // 0-9
                Float_e, // e
                Float_e_sign, // +-
                Float_e_power, // 0-9
                Char_start, // #
                Char_number, // 0-255
                Literal_start, // '
                Literal_char, //  any char, no \n or ' 
                Literal_special,// '
                Literal_end,// '
                Identifier_start,// _,a-z
                Identifier_char,// _,a-z,0-9
                Comment_curly,// {
                Comment_double,// (*
                Comment_slash,// //
                Comment_curly_end,// }
                Comment_double_preend,// *
                Comment_double_end,// )
                Comment_slash_end,// \n
                Plus,// +
                Minus,// -
                Asterisk,// *
                Slash,// /
                Equal,// =
                Greater,// >
                Less,// <
                Bracket_open, //(
                Bracket_closed, //(
                GreaterEq,// >=
                LessEq,// <=
                NotEq,// <>
                Colon,// :
                Becomes,// :=
                Semicolon,// ;
                Space,// ' '
                EOL,// \n
                EOF,// i = length
            }
            public enum Event
            {
                Number, // 0-9
                Character, // a-z
                HexNumber, // 0-9, a-f
                E, // e E
                Underscore,// _
                Quote, // '
                Curly_open,// {
                Bracket_open,// (
                Square_open,// [
                Curly_closed,// }
                Bracket_closed,// )
                Square_closed,// ]
                Plus,// +
                Minus,// -
                Asterisk,// *
                Slash,// /
                Equal,// =
                Greater,// >
                Less,// <
                Colon,// :
                Period,// .
                Hash,// #
                Dollar,// $
                Percent,// %
                Ampersand,// &
                Semicolon,// ;
                Other_symbol,// ...
                Space,// ' '
                EOL,// \n
                EOF,// i = length
            }
            IntData intData;
            RealData realData;
            LiteralData literalData;
            IdentifierData identifierData;
            ASpecialData aSpecialData;
            DSpecialData dSpecialData;
            public Dictionary<State, Dictionary<Event, Action<char>>> lexerFSM;
            public HashSet<State> currentStates;
            public Dictionary<State, Action> validExits;
            string fileText;
            string exitValue;
            string exitType;
            long exitIdx;
            long exitLine;
            int it;
            long idx;
            long line;

            public Lexer(string s)
            {
                intData = new IntData();
                realData = new RealData();
                literalData = new LiteralData();
                identifierData = new IdentifierData();
                aSpecialData = new ASpecialData();
                dSpecialData = new DSpecialData();
                this.fileText = s;
                it = 0;
                idx = 1;
                line = 1;
                currentStates = new HashSet<State>();
                validExits = new Dictionary<State, Action>()
                {
                    [State.Number_char] = new Action(() => { exitValue = intData.Value.ToString(); exitType = "Integer"; }),
                    [State.Int_char] = new Action(() => { exitValue = intData.Value.ToString(); exitType = "Integer"; }),
                    [State.HexInt_char] = new Action(() => { exitValue = intData.Value.ToString(); exitType = "Integer"; }),
                    [State.Float_char] = new Action(() => { exitValue = realData.Value.ToString(); exitType = "Real"; }),
                    [State.Float_e_power] = new Action(() => { exitValue = realData.Value.ToString(); exitType = "Real"; }),
                    [State.Char_number] = new Action(() => { exitValue = literalData.ToString(); exitType = "Literal"; }),
                    [State.Literal_end] = new Action(() => { exitValue = literalData.Value.ToString(); exitType = "Literal"; }),
                    [State.Identifier_start] = new Action(() => { exitValue = identifierData.Value.ToString(); exitType = "Identifier"; }),
                    [State.Identifier_char] = new Action(() => { exitValue = identifierData.Value.ToString(); exitType = "Identifier"; }),
                    [State.Comment_curly_end] = new Action(() => { exitValue = "-"; exitType = "Comment"; }),
                    [State.Comment_double_end] = new Action(() => { exitValue = "-"; exitType = "Comment"; }),
                    [State.Comment_slash_end] = new Action(() => { exitValue = "-"; exitType = "Comment"; }),
                    [State.Plus] = new Action(() => { exitValue = aSpecialData.Value.ToString(); exitType = "Math"; }),
                    [State.Minus] = new Action(() => { exitValue = aSpecialData.Value.ToString(); exitType = "Math"; }),
                    [State.Asterisk] = new Action(() => { exitValue = aSpecialData.Value.ToString(); exitType = "Math"; }),
                    [State.Slash] = new Action(() => { exitValue = aSpecialData.Value.ToString(); exitType = "Math"; }),
                    [State.Equal] = new Action(() => { exitValue = aSpecialData.Value.ToString(); exitType = "Math"; }),
                    [State.Bracket_open] = new Action(() => { exitValue = aSpecialData.Value.ToString(); exitType = "Math"; }),
                    [State.Bracket_closed] = new Action(() => { exitValue = aSpecialData.Value.ToString(); exitType = "Math"; }),
                    [State.Equal] = new Action(() => { exitValue = aSpecialData.Value.ToString(); exitType = "Math"; }),
                    [State.Greater] = new Action(() => { exitValue = aSpecialData.Value.ToString(); exitType = "Math"; }),
                    [State.Less] = new Action(() => { exitValue = aSpecialData.Value.ToString(); exitType = "Math"; }),
                    [State.GreaterEq] = new Action(() => { exitValue = aSpecialData.Value.ToString(); exitType = "Math"; }),
                    [State.LessEq] = new Action(() => { exitValue = aSpecialData.Value.ToString(); exitType = "Math"; }),
                    [State.NotEq] = new Action(() => { exitValue = aSpecialData.Value.ToString(); exitType = "Math"; }),
                    [State.Becomes] = new Action(() => { exitValue = aSpecialData.Value.ToString(); exitType = "Math"; }),
                    [State.Semicolon] = new Action(() => { exitValue = dSpecialData.Value.ToString(); exitType = "Divider"; }),
                    [State.Space] = new Action(() => { exitValue = dSpecialData.Value.ToString(); exitType = "Divider"; }),
                    [State.EOL] = new Action(() => { exitValue = dSpecialData.Value.ToString(); exitType = "Divider"; }),
                    [State.EOF] = new Action(() => { exitValue = dSpecialData.Value.ToString(); exitType = "Divider"; }),
                };
                lexerFSM = new Dictionary<State, Dictionary<Event, Action<char>>>();
                {
                    //creating blank states
                    {
                        lexerFSM.Add(State.Start, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Number_char, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Int_notation, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.HexInt_notation, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Int_char, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.HexInt_char, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Float_dot, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Float_char, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Float_e, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Float_e_sign, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Float_e_power, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Char_start, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Char_number, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Literal_start, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Literal_char, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Literal_special, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Literal_end, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Identifier_start, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Identifier_char, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Comment_curly, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Comment_double, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Comment_slash, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Comment_curly_end, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Comment_double_preend, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Comment_double_end, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Comment_slash_end, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Plus, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Minus, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Asterisk, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Slash, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Equal, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Greater, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Less, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Bracket_open, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Bracket_closed, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.GreaterEq, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.LessEq, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.NotEq, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Colon, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Becomes, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.Space, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.EOL, new Dictionary<Event, Action<char>>());
                        lexerFSM.Add(State.EOF, new Dictionary<Event, Action<char>>());
                    }
                    //State.Start
                    {
                        lexerFSM[State.Start].Add(Event.Number, new Action<char>((ch) =>
                        {
                            currentStates.Add(State.Number_char);
                            currentStates.Remove(State.Start);
                            intData.AddDigit(ch - '0');
                            realData.AddDigit(ch - '0');
                        }));
                        lexerFSM[State.Start].Add(Event.Plus, new Action<char>((ch) =>
                        {
                            currentStates.Add(State.Plus);
                            currentStates.Remove(State.Start);
                            aSpecialData.Value = ASpecial.Plus;
                            intData.Sign = 1;
                            realData.Sign = 1;
                        }));
                        lexerFSM[State.Start].Add(Event.Minus, new Action<char>((ch) =>
                        {
                            currentStates.Add(State.Minus);
                            currentStates.Remove(State.Start);
                            aSpecialData.Value = ASpecial.Minus;
                            intData.Sign = -1;
                            realData.Sign = -1;
                        }));
                        lexerFSM[State.Start].Add(Event.Character, new Action<char>((ch) =>
                        {
                            identifierData.AddChar(ch);
                            currentStates.Add(State.Identifier_start);
                            currentStates.Remove(State.Start);
                        }));
                        lexerFSM[State.Start].Add(Event.Underscore, new Action<char>((ch) =>
                        {
                            identifierData.AddChar(ch);
                            currentStates.Add(State.Identifier_start);
                            currentStates.Remove(State.Start);
                        }));
                        lexerFSM[State.Start].Add(Event.Asterisk, new Action<char>((ch) =>
                        {
                            aSpecialData.Value = ASpecial.Asterisk;
                            currentStates.Add(State.Asterisk);
                            currentStates.Remove(State.Start);
                        }));
                        lexerFSM[State.Start].Add(Event.Slash, new Action<char>((ch) =>
                        {
                            aSpecialData.Value = ASpecial.Slash;
                            currentStates.Add(State.Slash);
                            currentStates.Remove(State.Start);
                        }));
                        lexerFSM[State.Start].Add(Event.Equal, new Action<char>((ch) =>
                        {
                            aSpecialData.Value = ASpecial.Equal;
                            currentStates.Add(State.Equal);
                            currentStates.Remove(State.Start);
                        }));
                        lexerFSM[State.Start].Add(Event.Greater, new Action<char>((ch) =>
                        {
                            aSpecialData.Value = ASpecial.Greater;
                            currentStates.Add(State.Greater);
                            currentStates.Remove(State.Start);
                        }));
                        lexerFSM[State.Start].Add(Event.Less, new Action<char>((ch) =>
                        {
                            aSpecialData.Value = ASpecial.Less;
                            currentStates.Add(State.Less);
                            currentStates.Remove(State.Start);
                        }));
                        lexerFSM[State.Start].Add(Event.Colon, new Action<char>((ch) =>
                        {
                            currentStates.Add(State.Colon);
                            currentStates.Remove(State.Start);
                        }));
                        lexerFSM[State.Start].Add(Event.Hash, new Action<char>((ch) =>
                        {
                            currentStates.Add(State.Char_start);
                            currentStates.Remove(State.Start);
                        }));
                        lexerFSM[State.Start].Add(Event.Dollar, new Action<char>((ch) =>
                        {
                            currentStates.Add(State.HexInt_notation);
                            currentStates.Remove(State.Start);
                            intData.SetBase(16);
                        }));
                        lexerFSM[State.Start].Add(Event.Percent, new Action<char>((ch) =>
                        {
                            currentStates.Add(State.Int_notation);
                            currentStates.Remove(State.Start);
                            intData.SetBase(2);
                        }));
                        lexerFSM[State.Start].Add(Event.Ampersand, new Action<char>((ch) =>
                        {
                            currentStates.Add(State.Int_notation);
                            currentStates.Remove(State.Start);
                            intData.SetBase(8);
                        }));
                        lexerFSM[State.Start].Add(Event.Bracket_open, new Action<char>((ch) =>
                        {
                            currentStates.Add(State.Bracket_open);
                            currentStates.Remove(State.Start);
                            aSpecialData.Value = ASpecial.Bracket_open;
                        }));
                        lexerFSM[State.Start].Add(Event.Bracket_closed, new Action<char>((ch) =>
                        {
                            currentStates.Add(State.Bracket_closed);
                            currentStates.Remove(State.Start);
                            aSpecialData.Value = ASpecial.Bracket_closed;
                        }));
                        lexerFSM[State.Start].Add(Event.Space, new Action<char>((ch) =>
                        {
                            dSpecialData.Value = DSpecial.Space;
                            currentStates.Add(State.Space);
                            currentStates.Remove(State.Start);
                        }));
                        lexerFSM[State.Start].Add(Event.Semicolon, new Action<char>((ch) =>
                        {
                            dSpecialData.Value = DSpecial.Semicolon;
                            currentStates.Add(State.Semicolon);
                            currentStates.Remove(State.Start);
                        }));
                        lexerFSM[State.Start].Add(Event.EOL, new Action<char>((ch) =>
                        {
                            dSpecialData.Value = DSpecial.EOL;
                            currentStates.Add(State.EOL);
                            currentStates.Remove(State.Start);
                        }));
                        lexerFSM[State.Start].Add(Event.EOF, new Action<char>((ch) =>
                        {
                            dSpecialData.Value = DSpecial.EOF;
                            currentStates.Add(State.EOF);
                            currentStates.Remove(State.Start);
                        }));
                    }
                    //Number_char
                    {
                        lexerFSM[State.Number_char].Add(Event.Number, new Action<char>((ch) =>
                        {
                            intData.AddDigit(ch - '0');
                            realData.AddDigit(ch - '0');
                        }));
                        lexerFSM[State.Number_char].Add(Event.Period, new Action<char>((ch) =>
                        {
                            currentStates.Add(State.Float_dot);
                            currentStates.Remove(State.Number_char);
                        }));
                    }
                    //Int_notation
                    lexerFSM[State.Int_notation].Add(Event.Number, new Action<char>((ch) =>
                    {
                        intData.AddDigit(ch - '0');
                        currentStates.Add(State.Int_char);
                        currentStates.Remove(State.Int_notation);
                    }));
                    //Int_char
                    lexerFSM[State.Int_char].Add(Event.Number, new Action<char>((ch) =>
                    {
                        intData.AddDigit(ch - '0');
                    }));
                    //HexInt_notation
                    lexerFSM[State.HexInt_notation].Add(Event.Number, new Action<char>((ch) =>
                    {
                        intData.AddDigit(ch - '0');
                        currentStates.Add(State.HexInt_char);
                        currentStates.Remove(State.HexInt_notation);
                    }));
                    //HexInt_char
                    lexerFSM[State.HexInt_char].Add(Event.HexNumber, new Action<char>((ch) =>
                    {
                        if (ch >= '0' && ch <= '9')
                            intData.AddDigit(ch - '0');
                        if (ch >= 'a' && ch <= 'f')
                            intData.AddDigit(ch - 'a');
                    }));
                    //Float_char
                    {
                        lexerFSM[State.Float_char].Add(Event.Number, new Action<char>((ch) =>
                        {
                            realData.AddDecimal(ch - '0');
                        }));
                        lexerFSM[State.Float_char].Add(Event.E, new Action<char>((ch) =>
                        {
                            currentStates.Add(State.Float_e);
                            currentStates.Remove(State.Float_char);
                        }));
                    }
                    //Float_e
                    {
                        lexerFSM[State.Float_e].Add(Event.Plus, new Action<char>((ch) =>
                        {
                            realData.ExpSign = 1;
                            currentStates.Add(State.Float_e_sign);
                            currentStates.Remove(State.Float_e);
                        }));
                        lexerFSM[State.Float_e].Add(Event.Minus, new Action<char>((ch) =>
                        {
                            realData.ExpSign = -1;
                            currentStates.Add(State.Float_e_sign);
                            currentStates.Remove(State.Float_e);
                        }));
                    }
                    //Float_e_sign
                    lexerFSM[State.Float_e_sign].Add(Event.Number, new Action<char>((ch) =>
                    {
                        realData.AddExp(ch - '0');
                        currentStates.Add(State.Float_e_power);
                        currentStates.Remove(State.Float_e_sign);
                    }));
                    //Float_e_power
                    lexerFSM[State.Float_e_power].Add(Event.Number, new Action<char>((ch) =>
                    {
                        realData.AddExp(ch - '0');
                    }));
                    //Char_start
                    lexerFSM[State.Char_start].Add(Event.Number, new Action<char>((ch) =>
                    {
                        literalData.IncreaceChar(ch - '0');
                        currentStates.Add(State.Char_number);
                        currentStates.Remove(State.Char_start);
                    }));
                    //Char_number
                    {
                        lexerFSM[State.Char_number].Add(Event.Number, new Action<char>((ch) =>
                        {
                            literalData.IncreaceChar(ch - '0');
                        }));
                        lexerFSM[State.Char_number].Add(Event.Quote, new Action<char>((ch) =>
                        {
                            currentStates.Add(State.Literal_start);
                            currentStates.Remove(State.Char_number);
                        }));
                    }
                    //Literal_start
                    {
                        lexerFSM[State.Literal_start].Add(Event.Quote, new Action<char>((ch) =>
                        {
                            currentStates.Add(State.Literal_end);
                            currentStates.Remove(State.Literal_start);
                        }));
                        //runtime fill
                    }
                    //Literal_char
                    {
                        lexerFSM[State.Literal_char].Add(Event.Quote, new Action<char>((ch) =>
                        {
                            currentStates.Add(State.Literal_end);
                            currentStates.Remove(State.Literal_char);
                        }));
                        //runtime fill
                    }
                    //Literal_special
                    {
                        lexerFSM[State.Literal_special].Add(Event.Quote, new Action<char>((ch) =>
                        {
                            currentStates.Add(State.Literal_end);
                            currentStates.Remove(State.Literal_special);
                        }));
                        //runtime fill
                    }
                    //Literal_end
                    {
                        lexerFSM[State.Literal_end].Add(Event.Quote, new Action<char>((ch) =>
                        {
                            literalData.AddChar('\'');
                            currentStates.Add(State.Literal_special);
                            currentStates.Remove(State.Literal_end);
                        }));
                        lexerFSM[State.Literal_end].Add(Event.Hash, new Action<char>((ch) =>
                        {
                            currentStates.Add(State.Char_start);
                            currentStates.Remove(State.Literal_end);
                        }));
                    }
                    //Identifier_start
                    {
                        lexerFSM[State.Identifier_start].Add(Event.Number, new Action<char>((ch) =>
                        {
                            identifierData.AddChar(ch);
                        }));
                        lexerFSM[State.Identifier_start].Add(Event.Character, new Action<char>((ch) =>
                        {
                            identifierData.AddChar(ch);
                        }));
                        lexerFSM[State.Identifier_start].Add(Event.Underscore, new Action<char>((ch) =>
                        {
                            identifierData.AddChar(ch);
                        }));
                    }
                    //Identifier_char
                    {
                        lexerFSM[State.Identifier_char].Add(Event.Number, new Action<char>((ch) =>
                        {
                            identifierData.AddChar(ch);
                        }));
                        lexerFSM[State.Identifier_char].Add(Event.Character, new Action<char>((ch) =>
                        {
                            identifierData.AddChar(ch);
                        }));
                        lexerFSM[State.Identifier_char].Add(Event.Underscore, new Action<char>((ch) =>
                        {
                            identifierData.AddChar(ch);
                        }));
                    }
                    lexerFSM[State.Bracket_open].Add(Event.Asterisk, new Action<char>((ch) =>
                    {
                        currentStates.Add(State.Comment_double);
                        currentStates.Remove(State.Bracket_open);
                    }));
                    //Comment_curly
                    {
                        lexerFSM[State.Comment_curly].Add(Event.Curly_closed, new Action<char>((ch) =>
                        {
                            currentStates.Add(State.Comment_curly_end);
                            currentStates.Remove(State.Comment_curly);
                        }));
                        //runtime fill
                    }
                    //Comment_double
                    {
                        lexerFSM[State.Comment_double].Add(Event.Asterisk, new Action<char>((ch) =>
                        {
                            currentStates.Add(State.Comment_double_preend);
                            currentStates.Remove(State.Comment_double);
                        }));
                        //runtime fill
                    }
                    //Comment_slash
                    {
                        lexerFSM[State.Comment_slash].Add(Event.EOL, new Action<char>((ch) =>
                        {
                            currentStates.Add(State.Comment_slash_end);
                            currentStates.Remove(State.Comment_slash);
                        }));
                        //runtime fill
                    }
                    //Comment_double_preend
                    {
                        lexerFSM[State.Comment_double_preend].Add(Event.Bracket_closed, new Action<char>((ch) =>
                        {
                            currentStates.Add(State.Comment_double_end);
                            currentStates.Remove(State.Comment_double_preend);
                        }));
                        //runtime fill
                    }
                    //Comment_curly_end
                    lexerFSM[State.Comment_curly_end] = new Dictionary<Event, Action<char>>();
                    //Comment_double_end
                    lexerFSM[State.Comment_double_end] = new Dictionary<Event, Action<char>>();
                    //Comment_slash_end
                    lexerFSM[State.Comment_slash_end] = new Dictionary<Event, Action<char>>();
                    //Plus
                    lexerFSM[State.Plus] = new Dictionary<Event, Action<char>>();
                    //Minus
                    lexerFSM[State.Minus] = new Dictionary<Event, Action<char>>();
                    //Asterisk
                    lexerFSM[State.Asterisk] = new Dictionary<Event, Action<char>>();
                    //Slash
                    lexerFSM[State.Slash].Add(Event.Slash, new Action<char>((ch) =>
                    {
                        currentStates.Add(State.Comment_slash);
                        currentStates.Remove(State.Slash);
                    }));
                    //Greater
                    lexerFSM[State.Greater].Add(Event.Equal, new Action<char>((ch) =>
                    {
                        currentStates.Add(State.GreaterEq);
                        currentStates.Remove(State.Greater);
                        aSpecialData.Value = ASpecial.GreaterEq;
                    }));
                    //Less
                    lexerFSM[State.Less].Add(Event.Equal, new Action<char>((ch) =>
                    {
                        currentStates.Add(State.LessEq);
                        currentStates.Remove(State.Less);
                        aSpecialData.Value = ASpecial.LessEq;
                    }));
                    //Less
                    lexerFSM[State.Less].Add(Event.Greater, new Action<char>((ch) =>
                    {
                        currentStates.Add(State.NotEq);
                        currentStates.Remove(State.Less);
                        aSpecialData.Value = ASpecial.NotEq;
                    }));
                    //Colon
                    lexerFSM[State.Colon].Add(Event.Equal, new Action<char>((ch) =>
                    {
                        currentStates.Add(State.Becomes);
                        currentStates.Remove(State.Colon);
                        aSpecialData.Value = ASpecial.Becomes;
                    }));
                    //Equal
                    lexerFSM[State.Equal] = new Dictionary<Event, Action<char>>();
                    //GreaterEq
                    lexerFSM[State.GreaterEq] = new Dictionary<Event, Action<char>>();
                    //Bracket_closed
                    lexerFSM[State.Bracket_closed] = new Dictionary<Event, Action<char>>();
                    //LessEq
                    lexerFSM[State.LessEq] = new Dictionary<Event, Action<char>>();
                    //NotEq
                    lexerFSM[State.NotEq] = new Dictionary<Event, Action<char>>();
                    //Becomes
                    lexerFSM[State.Becomes] = new Dictionary<Event, Action<char>>();
                    //Semicolon
                    lexerFSM[State.Semicolon] = new Dictionary<Event, Action<char>>();
                    //Space
                    lexerFSM[State.Space] = new Dictionary<Event, Action<char>>();
                    //EOL
                    lexerFSM[State.EOL] = new Dictionary<Event, Action<char>>();
                }
                foreach (Event e in (Event[])Enum.GetValues(typeof(Event)))
                {
                    if (e != Event.HexNumber)
                    {
                        if (e != Event.Asterisk)
                            lexerFSM[State.Comment_double][e] = new Action<char>((ch) => { });
                        if (e != Event.Curly_closed)
                            lexerFSM[State.Comment_curly][e] = new Action<char>((ch) => { });
                        if (e != Event.EOL)
                        {
                            lexerFSM[State.Comment_curly][e] = new Action<char>((ch) => { });
                            if (e != Event.Quote)
                            {
                                lexerFSM[State.Literal_char][e] = new Action<char>((ch) =>
                                {
                                    literalData.AddChar(ch);
                                });
                                lexerFSM[State.Literal_special][e] = new Action<char>((ch) =>
                                {
                                    currentStates.Add(State.Literal_char);
                                    currentStates.Remove(State.Literal_special);
                                    literalData.AddChar(ch);
                                });
                                lexerFSM[State.Literal_start][e] = new Action<char>((ch) =>
                                {
                                    currentStates.Add(State.Literal_char);
                                    currentStates.Remove(State.Literal_start);
                                    literalData.AddChar(ch);
                                });
                            }
                        }
                    }
                }
            }
            private HashSet<Event> DeduceEvents()
            {
                HashSet<Event> e = new HashSet<Event>();
                if (fileText.Length <= it)
                {
                    e.Add(Event.EOF);
                }
                else
                {

                    while (fileText[it] == '\r')
                    {
                        idx++;
                        it++;
                    }
                    if (fileText[it] >= '0' && fileText[it] <= '9')
                    {
                        e.Add(Event.Number);
                        e.Add(Event.HexNumber);
                    }
                    else
                if ((fileText[it] >= 'a' && fileText[it] <= 'z') || (fileText[it] >= 'A' && fileText[it] <= 'Z'))
                    {
                        e.Add(Event.Character);
                        if ((fileText[it] >= 'a' && fileText[it] <= 'z') || (fileText[it] >= 'A' && fileText[it] <= 'Z'))
                        {
                            e.Add(Event.HexNumber);
                        }
                        if (fileText[it] == 'E' || fileText[it] == 'e')
                            e.Add(Event.E);
                    }
                    else
                    if (fileText[it] > 127)
                    {
                        throw new ArgumentException("unknown symbol at " + line + " " + idx);
                    }
                    else
                        switch (fileText[it])
                        {
                            case '_':
                                e.Add(Event.Underscore);
                                break;
                            case '\'':
                                e.Add(Event.Quote);
                                break;
                            case '{':
                                e.Add(Event.Curly_open);
                                break;
                            case '}':
                                e.Add(Event.Curly_closed);
                                break;
                            case '[':
                                e.Add(Event.Square_open);
                                break;
                            case ']':
                                e.Add(Event.Square_closed);
                                break;
                            case '(':
                                e.Add(Event.Bracket_open);
                                break;
                            case ')':
                                e.Add(Event.Bracket_closed);
                                break;
                            case '+':
                                e.Add(Event.Plus);
                                break;
                            case '-':
                                e.Add(Event.Minus);
                                break;
                            case '*':
                                e.Add(Event.Asterisk);
                                break;
                            case '/':
                                e.Add(Event.Slash);
                                break;
                            case '=':
                                e.Add(Event.Equal);
                                break;
                            case ':':
                                e.Add(Event.Colon);
                                break;
                            case '$':
                                e.Add(Event.Dollar);
                                break;
                            case '%':
                                e.Add(Event.Percent);
                                break;
                            case '&':
                                e.Add(Event.Ampersand);
                                break;
                            case '#':
                                e.Add(Event.Hash);
                                break;
                            case ' ':
                                e.Add(Event.Space);
                                break;
                            case ';':
                                e.Add(Event.Semicolon);
                                break;
                            case '\n':
                                e.Add(Event.EOL);
                                break;
                            default:
                                e.Add(Event.Other_symbol);
                                break;
                        }
                }
                return e;
            }
            public Lexem NextToken()
            {
                Token = GetNextToken();
                return Token;
            }
                private Lexem GetNextToken()
            {
                intData = new IntData();
                realData = new RealData();
                literalData = new LiteralData();
                identifierData = new IdentifierData();
                aSpecialData = new ASpecialData();
                dSpecialData = new DSpecialData();
                int start = it;
                exitIdx = idx;
                exitLine = line;
                Lexem ans = null;
                currentStates.Add(State.Start);
                while (currentStates.Count > 0)
                {
                    HashSet<Event> evs = DeduceEvents();
                    List<Tuple<Action<char>, char, State>> acts = new List<Tuple<Action<char>, char, State>>();
                    foreach (Event ev in evs)
                    {
                        foreach (State st in currentStates)
                        {
                            if (lexerFSM[st].ContainsKey(ev))
                            {
                                acts.Add(new Tuple<Action<char>, char, State>(lexerFSM[st][ev], fileText.Length == it ? ' ' : fileText[it], st));
                            }
                        }
                    }
                    foreach (State st in currentStates)
                    {
                        bool stateBlank = true;
                        foreach (var act in acts)
                        {
                            if (act.Item3 == st)
                            {
                                stateBlank = false;
                            }
                        }
                        if (stateBlank)
                        {
                            acts.Add(new Tuple<Action<char>, char, State>(new Action<char>((ch) => { currentStates.Remove(st); }), '0', st));
                        }
                    }
                    foreach (var act in acts)
                    {
                        act.Item1.Invoke(act.Item2);
                    }
                    if (currentStates.Count != 0) ans = null;
                    foreach (State st in currentStates)
                    {
                        if (validExits.ContainsKey(st))
                        {
                            validExits[st].Invoke();
                            ans = new Lexem(start, (int)exitLine, (int)exitIdx, exitType.ToString(), exitValue.ToString());
                        }
                    }
                    if (currentStates.Count > 0 && currentStates.Last() != State.EOF)
                    {
                        if (fileText[it] == '\n')
                        {
                            line++;
                            idx = 1;
                        }
                        else
                        {
                            idx++;
                        }
                        it++;
                    }
                }
                if (ans != null) return ans;
                else throw new ArgumentException("Invalid symbol at " + line + " " + idx);
            }
        }
        
        public class Parser
        {
            public class Node
            {

            }
            public class BinOpNode : Node
            {
                Lexem op;
                Node left;
                Node right;
                public BinOpNode(Lexem op, Node left, Node right)
                {
                    this.op = op;
                    this.left = left;
                    this.right = right;
                }
            }
            public class NumberNode : Node
            {
                Lexem lexem;
                public NumberNode(Lexem lexem)
                {
                    this.lexem = lexem;
                }
            }
            public class VariableNode : Node
            {
                Lexem lexem;
                public VariableNode(Lexem lexem)
                {
                    this.lexem = lexem;
                }
            }
            Lexer lexer;
            public Parser(ref Lexer lexer)
            {
                this.lexer = lexer;
                this.lexer.NextToken();
            }
            public Node ParseExpression()
            {
                Node left = ParseTerm();
                Lexem lex = lexer.Token;
                if (lex.Value == "Plus" || lex.Value == "Minus")
                {
                    lexer.NextToken();
                    return new BinOpNode(lex, left, ParseExpression());
                }
                return left;
            }
            public Node ParseTerm()
            {
                Node left = ParseFactor();
                Lexem lex = lexer.Token;
                if (lex.Value == "Asterisk" || lex.Value == "Slash")
                {
                    lexer.NextToken();
                    return new BinOpNode(lex, left, ParseTerm());
                }
                return left;
            }
            public Node ParseFactor()
            {
                Lexem lex = lexer.Token;
                if (lex.Type == "Integer")
                {
                    lexer.NextToken();
                    return new NumberNode(lexer.Token);
                }
                if (lex.Type == "Identifier")
                {
                    lexer.NextToken();
                    return new VariableNode(lexer.Token);
                }
                if (lex.Value != null && lex.Value == "Bracket_open" && lex.Type == "Math")
                {
                    lexer.NextToken();
                    Node exp = ParseExpression();
                    if (lexer.Token.Value == null || !(lexer.Token.Value == "Bracket_closed" && lexer.Token.Type == "Math"))
                        throw new ArgumentException("Closing bracket expected");
                    lexer.NextToken();
                    return exp;
                }
                throw new ArgumentException("Factor expected");
            }
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
            List<Lexem> answers = new List<Lexem>();
            while (answers.Count == 0 || !(answers.Last().Value == "EOF" && answers.Last().Type == "Divider"))
            {
                answers.Add(lexer.NextToken());
                Console.WriteLine(answers.Last().Value + ' ' + answers.Last().Type);
            }
        }
    }
}
