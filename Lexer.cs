using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Compiler.Program;

namespace Compiler
{
    internal class Lexer
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
            Comma, //,
            At, //,
            Dot, // .
            DotDot, // ..
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
            LParenthese, //(
            RParenthese, //(
            LBracket, //[
            RBracket, //]
            GreaterEq,// >=
            LessEq,// <=
            NotEq,// <>
            Colon,// :
            Assign,// :=
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
            LParenthese,// (
            RParenthese,// )
            LCurly,// {
            RCurly,// }
            LBracket,// [
            RBracket,// ]
            Plus,// +
            Minus,// -
            Asterisk,// *
            Slash,// /
            Equal,// =
            Greater,// >
            Less,// <
            Colon,// :
            Comma, //,
            At, //@
            Dot,// .
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
        class LexerStateBackUp
        {
            public Lexem Token { get; private set; }
            public int Idx { get; private set; }
            public int Line { get; private set; }
            public int it { get; private set; }
            public int idx { get; private set; }
            public int line { get; private set; }

            public LexerStateBackUp(Lexem token, int Idx, int Line, int it, int idx, int line)
            {
                Token = token;
                this.Idx = Idx;
                this.Line = Line;
                this.it = it;
                this.idx = idx;
                this.line = line;
            }
        }
        LexerStateBackUp backUp;
        Lexem.UIntData uIntData;
        Lexem.URealData uRealData;
        Lexem.LiteralData literalData;
        Lexem.IdentifierData identifierData;
        Lexem.SpecialSymbolData specialSymbolData;
        public Dictionary<State, Dictionary<Event, Action<char>>> lexerFSM;
        public HashSet<State> currentStates;
        public Dictionary<State, Action> validExits;
        public int Idx;
        public int Line;
        string fileText;
        object exitValue;
        Lexem.Types exitType;
        int it;
        int idx;
        int line;

        public Lexer(string s)
        {
            uIntData = new Lexem.UIntData();
            uRealData = new Lexem.URealData();
            literalData = new Lexem.LiteralData();
            identifierData = new Lexem.IdentifierData();
            specialSymbolData = new Lexem.SpecialSymbolData();
            this.fileText = s;
            it = 0;
            idx = 1;
            line = 1;
            currentStates = new HashSet<State>();
            validExits = new Dictionary<State, Action>()
            {
                [State.Number_char] = new Action(() => { exitValue = uIntData.Value; exitType = Lexem.Types.UInteger; }),
                [State.Int_char] = new Action(() => { exitValue = uIntData.Value; exitType = Lexem.Types.UInteger; }),
                [State.HexInt_char] = new Action(() => { exitValue = uIntData.Value; exitType = Lexem.Types.UInteger; }),
                [State.Float_char] = new Action(() => { exitValue = uRealData.Value; exitType = Lexem.Types.UReal; }),
                [State.Float_e_power] = new Action(() => { exitValue = uRealData.Value; exitType = Lexem.Types.UReal; }),
                [State.Char_number] = new Action(() => { exitValue = literalData; exitType = Lexem.Types.Literal; }),
                [State.Literal_end] = new Action(() => { exitValue = literalData.Value; exitType = Lexem.Types.Literal; }),
                [State.Identifier_start] = new Action(() => { exitValue = identifierData.Value; exitType = Lexem.Types.Identifier; }),
                [State.Identifier_char] = new Action(() => { exitValue = identifierData.Value; exitType = Lexem.Types.Identifier; }),
                [State.Comment_curly_end] = new Action(() => { exitValue = "-"; exitType = Lexem.Types.Comment; }),
                [State.Comment_double_end] = new Action(() => { exitValue = "-"; exitType = Lexem.Types.Comment; }),
                [State.Comment_slash_end] = new Action(() => { exitValue = "-"; exitType = Lexem.Types.Comment; }),
                [State.Colon] = new Action(() => { exitValue = specialSymbolData.Value; exitType = Lexem.Types.Special; }),
                [State.Comma] = new Action(() => { exitValue = specialSymbolData.Value; exitType = Lexem.Types.Special; }),
                [State.At] = new Action(() => { exitValue = specialSymbolData.Value; exitType = Lexem.Types.Special; }),
                [State.Dot] = new Action(() => { exitValue = specialSymbolData.Value; exitType = Lexem.Types.Special; }),
                [State.DotDot] = new Action(() => { exitValue = specialSymbolData.Value; exitType = Lexem.Types.Special; }),
                [State.Plus] = new Action(() => { exitValue = specialSymbolData.Value; exitType = Lexem.Types.Additive_Op; }),
                [State.Minus] = new Action(() => { exitValue = specialSymbolData.Value; exitType = Lexem.Types.Additive_Op; }),
                [State.Asterisk] = new Action(() => { exitValue = specialSymbolData.Value; exitType = Lexem.Types.Multiplicative_Op; }),
                [State.Slash] = new Action(() => { exitValue = specialSymbolData.Value; exitType = Lexem.Types.Multiplicative_Op; }),
                [State.Equal] = new Action(() => { exitValue = specialSymbolData.Value; exitType = Lexem.Types.Relational_Op; }),
                [State.LParenthese] = new Action(() => { exitValue = specialSymbolData.Value; exitType = Lexem.Types.Special; }),
                [State.RParenthese] = new Action(() => { exitValue = specialSymbolData.Value; exitType = Lexem.Types.Special; }),
                [State.LBracket] = new Action(() => { exitValue = specialSymbolData.Value; exitType = Lexem.Types.Special; }),
                [State.RBracket] = new Action(() => { exitValue = specialSymbolData.Value; exitType = Lexem.Types.Special; }),
                [State.Greater] = new Action(() => { exitValue = specialSymbolData.Value; exitType = Lexem.Types.Relational_Op; }),
                [State.Less] = new Action(() => { exitValue = specialSymbolData.Value; exitType = Lexem.Types.Relational_Op; }),
                [State.GreaterEq] = new Action(() => { exitValue = specialSymbolData.Value; exitType = Lexem.Types.Relational_Op; }),
                [State.LessEq] = new Action(() => { exitValue = specialSymbolData.Value; exitType = Lexem.Types.Relational_Op; }),
                [State.NotEq] = new Action(() => { exitValue = specialSymbolData.Value; exitType = Lexem.Types.Relational_Op; }),
                [State.Assign] = new Action(() => { exitValue = specialSymbolData.Value; exitType = Lexem.Types.Special; }),
                [State.Semicolon] = new Action(() => { exitValue = specialSymbolData.Value; exitType = Lexem.Types.Special; }),
                [State.Space] = new Action(() => { exitValue = specialSymbolData.Value; exitType = Lexem.Types.Special; }),
                [State.EOL] = new Action(() => { exitValue = specialSymbolData.Value; exitType = Lexem.Types.Special; }),
                [State.EOF] = new Action(() => { exitValue = specialSymbolData.Value; exitType = Lexem.Types.Special; }),
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
                    lexerFSM.Add(State.Colon, new Dictionary<Event, Action<char>>());
                    lexerFSM.Add(State.Comma, new Dictionary<Event, Action<char>>());
                    lexerFSM.Add(State.At, new Dictionary<Event, Action<char>>());
                    lexerFSM.Add(State.Dot, new Dictionary<Event, Action<char>>());
                    lexerFSM.Add(State.DotDot, new Dictionary<Event, Action<char>>());
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
                    lexerFSM.Add(State.LParenthese, new Dictionary<Event, Action<char>>());
                    lexerFSM.Add(State.RParenthese, new Dictionary<Event, Action<char>>());
                    lexerFSM.Add(State.LBracket, new Dictionary<Event, Action<char>>());
                    lexerFSM.Add(State.RBracket, new Dictionary<Event, Action<char>>());
                    lexerFSM.Add(State.GreaterEq, new Dictionary<Event, Action<char>>());
                    lexerFSM.Add(State.LessEq, new Dictionary<Event, Action<char>>());
                    lexerFSM.Add(State.NotEq, new Dictionary<Event, Action<char>>());
                    lexerFSM.Add(State.Assign, new Dictionary<Event, Action<char>>());
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
                        uIntData.AddDigit(ch - '0');
                        uRealData.AddDigit(ch - '0');
                    }));
                    lexerFSM[State.Start].Add(Event.Plus, new Action<char>((ch) =>
                    {
                        currentStates.Add(State.Plus);
                        currentStates.Remove(State.Start);
                        specialSymbolData.Value = Lexem.SpecialSymbol.Plus;
                    }));
                    lexerFSM[State.Start].Add(Event.Minus, new Action<char>((ch) =>
                    {
                        currentStates.Add(State.Minus);
                        currentStates.Remove(State.Start);
                        specialSymbolData.Value = Lexem.SpecialSymbol.Minus;
                    }));
                    lexerFSM[State.Start].Add(Event.Comma, new Action<char>((ch) =>
                    {
                        currentStates.Add(State.Comma);
                        currentStates.Remove(State.Start);
                        specialSymbolData.Value = Lexem.SpecialSymbol.Comma;
                    }));
                    lexerFSM[State.Start].Add(Event.Colon, new Action<char>((ch) =>
                    {
                        currentStates.Add(State.Colon);
                        currentStates.Remove(State.Start);
                        specialSymbolData.Value = Lexem.SpecialSymbol.Colon;
                    }));
                    lexerFSM[State.Start].Add(Event.At, new Action<char>((ch) =>
                    {
                        currentStates.Add(State.At);
                        currentStates.Remove(State.Start);
                        specialSymbolData.Value = Lexem.SpecialSymbol.At;
                    }));
                    lexerFSM[State.Start].Add(Event.Dot, new Action<char>((ch) =>
                    {
                        currentStates.Add(State.Dot);
                        currentStates.Remove(State.Start);
                        specialSymbolData.Value = Lexem.SpecialSymbol.Dot;
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
                        specialSymbolData.Value = Lexem.SpecialSymbol.Multiply;
                        currentStates.Add(State.Asterisk);
                        currentStates.Remove(State.Start);
                    }));
                    lexerFSM[State.Start].Add(Event.Slash, new Action<char>((ch) =>
                    {
                        specialSymbolData.Value = Lexem.SpecialSymbol.Divide;
                        currentStates.Add(State.Slash);
                        currentStates.Remove(State.Start);
                    }));
                    lexerFSM[State.Start].Add(Event.Equal, new Action<char>((ch) =>
                    {
                        specialSymbolData.Value = Lexem.SpecialSymbol.Equal;
                        currentStates.Add(State.Equal);
                        currentStates.Remove(State.Start);
                    }));
                    lexerFSM[State.Start].Add(Event.Greater, new Action<char>((ch) =>
                    {
                        specialSymbolData.Value = Lexem.SpecialSymbol.Greater;
                        currentStates.Add(State.Greater);
                        currentStates.Remove(State.Start);
                    }));
                    lexerFSM[State.Start].Add(Event.Less, new Action<char>((ch) =>
                    {
                        specialSymbolData.Value = Lexem.SpecialSymbol.Less;
                        currentStates.Add(State.Less);
                        currentStates.Remove(State.Start);
                    }));
                    lexerFSM[State.Start].Add(Event.Quote, new Action<char>((ch) =>
                    {
                        currentStates.Add(State.Literal_start);
                        currentStates.Remove(State.Char_number);
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
                        uIntData.SetBase(16);
                    }));
                    lexerFSM[State.Start].Add(Event.Percent, new Action<char>((ch) =>
                    {
                        currentStates.Add(State.Int_notation);
                        currentStates.Remove(State.Start);
                        uIntData.SetBase(2);
                    }));
                    lexerFSM[State.Start].Add(Event.Ampersand, new Action<char>((ch) =>
                    {
                        currentStates.Add(State.Int_notation);
                        currentStates.Remove(State.Start);
                        uIntData.SetBase(8);
                    }));
                    lexerFSM[State.Start].Add(Event.LParenthese, new Action<char>((ch) =>
                    {
                        currentStates.Add(State.LParenthese);
                        currentStates.Remove(State.Start);
                        specialSymbolData.Value = Lexem.SpecialSymbol.LParenthese;
                    }));
                    lexerFSM[State.Start].Add(Event.RParenthese, new Action<char>((ch) =>
                    {
                        currentStates.Add(State.RParenthese);
                        currentStates.Remove(State.Start);
                        specialSymbolData.Value = Lexem.SpecialSymbol.RParenthese;
                    }));
                    lexerFSM[State.Start].Add(Event.LBracket, new Action<char>((ch) =>
                    {
                        currentStates.Add(State.LBracket);
                        currentStates.Remove(State.Start);
                        specialSymbolData.Value = Lexem.SpecialSymbol.LBracket;
                    }));
                    lexerFSM[State.Start].Add(Event.RBracket, new Action<char>((ch) =>
                    {
                        currentStates.Add(State.RBracket);
                        currentStates.Remove(State.Start);
                        specialSymbolData.Value = Lexem.SpecialSymbol.RBracket;
                    }));
                    lexerFSM[State.Start].Add(Event.Space, new Action<char>((ch) =>
                    {
                        specialSymbolData.Value = Lexem.SpecialSymbol.Space;
                        currentStates.Add(State.Space);
                        currentStates.Remove(State.Start);
                    }));
                    lexerFSM[State.Start].Add(Event.Semicolon, new Action<char>((ch) =>
                    {
                        specialSymbolData.Value = Lexem.SpecialSymbol.Semicolon;
                        currentStates.Add(State.Semicolon);
                        currentStates.Remove(State.Start);
                    }));
                    lexerFSM[State.Start].Add(Event.EOL, new Action<char>((ch) =>
                    {
                        specialSymbolData.Value = Lexem.SpecialSymbol.EOL;
                        currentStates.Add(State.EOL);
                        currentStates.Remove(State.Start);
                    }));
                    lexerFSM[State.Start].Add(Event.EOF, new Action<char>((ch) =>
                    {
                        specialSymbolData.Value = Lexem.SpecialSymbol.EOF;
                        currentStates.Add(State.EOF);
                        currentStates.Remove(State.Start);
                    }));
                }
                //Number_char
                {
                    lexerFSM[State.Number_char].Add(Event.Number, new Action<char>((ch) =>
                    {
                        uIntData.AddDigit(ch - '0');
                        uRealData.AddDigit(ch - '0');
                    }));
                    lexerFSM[State.Number_char].Add(Event.Dot, new Action<char>((ch) =>
                    {
                        currentStates.Add(State.Float_dot);
                        currentStates.Remove(State.Number_char);
                    }));
                    lexerFSM[State.Number_char].Add(Event.E, new Action<char>((ch) =>
                    {
                        currentStates.Add(State.Float_e);
                        currentStates.Remove(State.Number_char);
                    }));
                }
                //Int_notation
                lexerFSM[State.Int_notation].Add(Event.Number, new Action<char>((ch) =>
                {
                    uIntData.AddDigit(ch - '0');
                    currentStates.Add(State.Int_char);
                    currentStates.Remove(State.Int_notation);
                }));
                //Int_char
                lexerFSM[State.Int_char].Add(Event.Number, new Action<char>((ch) =>
                {
                    uIntData.AddDigit(ch - '0');
                }));
                //HexInt_notation
                lexerFSM[State.HexInt_notation].Add(Event.Number, new Action<char>((ch) =>
                {
                    uIntData.AddDigit(ch - '0');
                    currentStates.Add(State.HexInt_char);
                    currentStates.Remove(State.HexInt_notation);
                }));
                //HexInt_char
                lexerFSM[State.HexInt_char].Add(Event.HexNumber, new Action<char>((ch) =>
                {
                    if (ch >= '0' && ch <= '9')
                        uIntData.AddDigit(ch - '0');
                    if (ch >= 'a' && ch <= 'f')
                        uIntData.AddDigit(ch - 'a');
                }));
                //Float_dot
                {
                    lexerFSM[State.Float_dot].Add(Event.Number, new Action<char>((ch) =>
                    {
                        uRealData.AddDecimal(ch - '0');
                        currentStates.Add(State.Float_char);
                        currentStates.Remove(State.Float_dot);
                    }));
                }
                //Float_char
                {
                    lexerFSM[State.Float_char].Add(Event.Number, new Action<char>((ch) =>
                    {
                        uRealData.AddDecimal(ch - '0');
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
                        uRealData.ExpSign = 1;
                        currentStates.Add(State.Float_e_sign);
                        currentStates.Remove(State.Float_e);
                    }));
                    lexerFSM[State.Float_e].Add(Event.Minus, new Action<char>((ch) =>
                    {
                        uRealData.ExpSign = -1;
                        currentStates.Add(State.Float_e_sign);
                        currentStates.Remove(State.Float_e);
                    }));
                }
                //Float_e_sign
                lexerFSM[State.Float_e_sign].Add(Event.Number, new Action<char>((ch) =>
                {
                    uRealData.AddExp(ch - '0');
                    currentStates.Add(State.Float_e_power);
                    currentStates.Remove(State.Float_e_sign);
                }));
                //Float_e_power
                lexerFSM[State.Float_e_power].Add(Event.Number, new Action<char>((ch) =>
                {
                    uRealData.AddExp(ch - '0');
                }));
                //Char_start
                lexerFSM[State.Char_start].Add(Event.Number, new Action<char>((ch) =>
                {
                    try
                    {

                        literalData.IncreaceChar(ch - '0');
                    }
                    catch
                    {
                        throw new Exception(ErrorConstructor.GetPositionMassage(Line, Idx, Error.CharRange));
                    }
                    currentStates.Add(State.Char_number);
                    currentStates.Remove(State.Char_start);
                }));
                //Char_number
                {
                    lexerFSM[State.Char_number].Add(Event.Number, new Action<char>((ch) =>
                    {
                        try
                        {

                            literalData.IncreaceChar(ch - '0');
                        }
                        catch
                        {
                            throw new Exception(ErrorConstructor.GetPositionMassage(Line, Idx, Error.CharRange));
                        }
                    }));
                    lexerFSM[State.Char_number].Add(Event.Quote, new Action<char>((ch) =>
                    {
                        currentStates.Add(State.Literal_start);
                        currentStates.Remove(State.Char_number);
                    }));
                }
                //Dot
                lexerFSM[State.Dot].Add(Event.Dot, new Action<char>((ch) =>
                {
                    currentStates.Add(State.DotDot);
                    currentStates.Remove(State.Dot);
                    specialSymbolData.Value = Lexem.SpecialSymbol.DotDot;
                }));
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
                lexerFSM[State.LParenthese].Add(Event.Asterisk, new Action<char>((ch) =>
                {
                    currentStates.Add(State.Comment_double);
                    currentStates.Remove(State.LParenthese);
                }));
                //Comment_curly
                {
                    lexerFSM[State.Comment_curly].Add(Event.RCurly, new Action<char>((ch) =>
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
                    lexerFSM[State.Comment_double_preend].Add(Event.RParenthese, new Action<char>((ch) =>
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
                //Colon
                lexerFSM[State.Colon] = new Dictionary<Event, Action<char>>();
                //Comma
                lexerFSM[State.Comma] = new Dictionary<Event, Action<char>>();
                //At
                lexerFSM[State.At] = new Dictionary<Event, Action<char>>();
                //DotDot
                lexerFSM[State.DotDot] = new Dictionary<Event, Action<char>>();
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
                    specialSymbolData.Value = Lexem.SpecialSymbol.GreaterEq;
                }));
                //Less
                lexerFSM[State.Less].Add(Event.Equal, new Action<char>((ch) =>
                {
                    currentStates.Add(State.LessEq);
                    currentStates.Remove(State.Less);
                    specialSymbolData.Value = Lexem.SpecialSymbol.LessEq;
                }));
                //Less
                lexerFSM[State.Less].Add(Event.Greater, new Action<char>((ch) =>
                {
                    currentStates.Add(State.NotEq);
                    currentStates.Remove(State.Less);
                    specialSymbolData.Value = Lexem.SpecialSymbol.NotEq;
                }));
                //Colon
                lexerFSM[State.Colon].Add(Event.Equal, new Action<char>((ch) =>
                {
                    currentStates.Add(State.Assign);
                    currentStates.Remove(State.Colon);
                    specialSymbolData.Value = Lexem.SpecialSymbol.Assign;
                }));
                //Equal
                lexerFSM[State.Equal] = new Dictionary<Event, Action<char>>();
                //GreaterEq
                lexerFSM[State.GreaterEq] = new Dictionary<Event, Action<char>>();
                //RParenthese
                lexerFSM[State.RParenthese] = new Dictionary<Event, Action<char>>();
                //LBracket
                lexerFSM[State.LBracket] = new Dictionary<Event, Action<char>>();
                //RBracket
                lexerFSM[State.RBracket] = new Dictionary<Event, Action<char>>();
                //LessEq
                lexerFSM[State.LessEq] = new Dictionary<Event, Action<char>>();
                //NotEq
                lexerFSM[State.NotEq] = new Dictionary<Event, Action<char>>();
                //Assign
                lexerFSM[State.Assign] = new Dictionary<Event, Action<char>>();
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
                    if (e != Event.RCurly)
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
                    throw new Exception(ErrorConstructor.GetPositionMassage(line, idx, Error.UnknownSymbol));
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
                        case ',':
                            e.Add(Event.Comma);
                            break;
                        case '@':
                            e.Add(Event.Comma);
                            break;
                        case '.':
                            e.Add(Event.Dot);
                            break;
                        case '{':
                            e.Add(Event.LCurly);
                            break;
                        case '}':
                            e.Add(Event.RCurly);
                            break;
                        case '[':
                            e.Add(Event.LBracket);
                            break;
                        case ']':
                            e.Add(Event.RBracket);
                            break;
                        case '(':
                            e.Add(Event.LParenthese);
                            break;
                        case ')':
                            e.Add(Event.RParenthese);
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
                        case '>':
                            e.Add(Event.Greater);
                            break;
                        case '<':
                            e.Add(Event.Less);
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
            while (Token.Type == Lexem.Types.Comment || Token.Value.Equals(Lexem.SpecialSymbol.Space))
                Token = GetNextToken();
            return Token;
        }
        public void SaveState()
        {
            backUp = new LexerStateBackUp(Token, Idx, Line, it, idx, line);
        }
        public void RestoreState()
        {
            Token = backUp.Token;
            Idx = backUp.Idx;
            Line = backUp.Line;
            it = backUp.it;
            idx = backUp.idx;
            line = backUp.line;
        }
        private Lexem GetNextToken()
        {
            uIntData = new Lexem.UIntData();
            uRealData = new Lexem.URealData();
            literalData = new Lexem.LiteralData();
            identifierData = new Lexem.IdentifierData();
            specialSymbolData = new Lexem.SpecialSymbolData();
            specialSymbolData = new Lexem.SpecialSymbolData();
            int start = it;
            Idx = idx;
            Line = line;
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
                        string input;
                        if (fileText.Length > it && it >= start)
                            input = fileText.Substring(start, it - start + 1);
                        else
                            input = null;
                        if(exitType==Lexem.Types.Identifier && Enum.IsDefined(typeof(Lexem.KeyWord), exitValue.ToString().ToUpper()))
                        {
                            exitType=Lexem.Types.KeyWord;
                            exitValue = (Lexem.KeyWord)Enum.Parse(typeof(Lexem.KeyWord), exitValue.ToString().ToUpper());
                        }
                        ans = new Lexem(start, Line, Idx, exitType, exitValue, input);
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
            else
                throw new Exception(ErrorConstructor.GetPositionMassage(line, idx, Error.InvalidSymbol));
        }
    }
}
