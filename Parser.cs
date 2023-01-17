using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Compiler.Program;

namespace Compiler
{
    internal class Parser
    {
        public static void PrintExpressionTree(Node tree, string indent, bool last)
        {
            if (indent == "")
            {
                Console.WriteLine(indent + " " + tree.getVal());
                indent += last ? " " : "│ ";
            }
            else
            {
                Console.WriteLine(indent + (last ? "└── " : "├── ") + tree.getVal());
                indent += last ? "    " : "│   ";
            }

            List<Node> list = tree.GetChildren();
            foreach (Node node in list)
            {
                if (node.Equals(list.Last()))
                {
                    PrintExpressionTree(node, indent, true);
                }
                else
                {
                    PrintExpressionTree(node, indent, false);
                }
            }
        }
        Lexer lexer;
        public Parser(ref Lexer lexer)
        {
            this.lexer = lexer;
            this.lexer.NextToken();
        }

        public Node ParseConst()
        {
            if (lexer.Token.Value.Equals(Lexem.KeyWord.CHR))
            {
                return new ConstNode(ParseConstChr());
            }
            if (lexer.Token.Type == Lexem.Types.Literal)
            {
                var lex = lexer.Token;
                lexer.NextToken();
                return new ConstNode(new StringNode(lex));
            }
            int sign = 1;
            if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Plus) || lexer.Token.Value.Equals(Lexem.SpecialSymbol.Minus))
            {
                if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Minus))
                {
                    sign = -1;
                }
                lexer.NextToken();
            }
            if (lexer.Token.Type == Lexem.Types.UReal || lexer.Token.Type == Lexem.Types.UInteger)
            {
                var lex = lexer.Token;
                lexer.NextToken();
                return new ConstNode(new NumberNode(lex), sign);
            }
            if (lexer.Token.Type == Lexem.Types.Identifier)
            {
                var lex = lexer.Token;
                lexer.NextToken();
                return new ConstNode(new VariableNode(false, lex), sign);
            }
            throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "Constatnt"));
        }
        public Node ParseConstChr()
        {
            if (!lexer.Token.Value.Equals(Lexem.KeyWord.CHR))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "CHR"));
            lexer.NextToken();
            if (!lexer.Token.Value.Equals(Lexem.SpecialSymbol.LParenthese))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "("));
            lexer.NextToken();
            Lexem lex = lexer.Token;
            if (!lexer.Token.Type.Equals(Lexem.Types.UInteger))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "Unsigned Int"));
            lexer.NextToken();
            if (!lex.Value.Equals(Lexem.SpecialSymbol.RParenthese))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, ")"));
            lexer.NextToken();
            return new ConstChrNode(lex);
        }
        public Node ParseConstDef()
        {
            var identifier = lexer.Token;
            if (identifier.Type != Lexem.Types.Identifier)
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "Identifier"));
            lexer.NextToken();
            if (!lexer.Token.Value.Equals(Lexem.SpecialSymbol.Equal))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "="));
            lexer.NextToken();
            return new ConstDefinitionNode(new VariableNode(false, identifier), ParseConst());
        }
        /*public Node ParseFunctionType()
        {
            if (!lexer.Token.Value.Equals(Lexem.KeyWord.FUNCTION))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "FUNCTION"));
            lexer.NextToken();
            if (!lexer.Token.Value.Equals(Lexem.SpecialSymbol.Equal))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "="));
            lexer.NextToken();
            return new ConstDefinitionNode(new VariableNode(identifier), ParseConst());
        }*/
        public Node ParseStatement()
        {

            if (lexer.Token.Type == Lexem.Types.Identifier || lexer.Token.Value.Equals(Lexem.SpecialSymbol.At))
                return ParseSimpleStatement();
            switch (lexer.Token.Value) {
                case Lexem.KeyWord.BEGIN:
                    return ParseCompoundStatement();
                case Lexem.KeyWord.IF:
                    return ParseConditionalStatement();
                case Lexem.KeyWord.WITH:
                    return ParseWithStatement();
                case Lexem.KeyWord.FOR:
                    return ParseForStatement();
                case Lexem.KeyWord.WHILE:
                    return ParseWhileStatement();
                case Lexem.KeyWord.REPEAT:
                    return ParseRepeatStatement();
                default:
                    return new EmptyStatementNode();
            }
        }
        public Node ParseCompoundStatement()
        {
            return new EmptyStatementNode();
        }
        public Node ParseConditionalStatement()
        {
            return new EmptyStatementNode();
        }
        public Node ParseWithStatement()
        {
            return new EmptyStatementNode();
        }
        public Node ParseForStatement()
        {
            return new EmptyStatementNode();
        }
        public Node ParseWhileStatement()
        {
            return new EmptyStatementNode();
        }
        public Node ParseRepeatStatement()
        {
            return new EmptyStatementNode();
        }
        public Node ParseSimpleStatement()
        {
            if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.At))
            {
                return ParseAssignStatement();
            }
            if (lexer.Token.Type != Lexem.Types.Identifier)
                return new EmptyStatementNode();
            var lex = lexer.Token;
            lexer.NextToken();
            if (!lexer.Token.Value.Equals(Lexem.SpecialSymbol.LParenthese))
                return ParseVariable(lex);
            lexer.NextToken();
            List<Node> result = new List<Node>();
            result.Add(ParseExpression());
            while (!lexer.Token.Value.Equals(Lexem.SpecialSymbol.RParenthese))
            {
                if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Comma))
                {
                    lexer.NextToken();
                    result.Add(ParseExpression());
                }
                else throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, ")"));
            }
            return new FunctionDesignatorNode(new VariableNode(false, lex), new ParameterListNode(result));
        }
        public Node ParseAssignStatement(Lexem inputIdentifier = null)
        {
            var lex = lexer.Token;
            if (inputIdentifier != null)
            {
                lex = inputIdentifier;
                goto objecttion;
            }
            if (lexer.Token.Type!=Lexem.Types.Identifier)
                throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "Identifier"));
            lexer.NextToken();
            objecttion:
            if (!lexer.Token.Value.Equals(Lexem.SpecialSymbol.Assign))
                throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, ":="));
            return new ProcedureStatementNode(new VariableNode(false, lex));
        }
        public Node ParseConstDefPart()
        {
            if (!lexer.Token.Value.Equals(Lexem.KeyWord.CONST))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "CONST"));
            lexer.NextToken();
            var answer = new ConstDefinitionPartNode(ParseConstDef());
            if (!lexer.Token.Value.Equals(Lexem.SpecialSymbol.Semicolon))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, ";"));
            lexer.NextToken();
            return answer;
        }
        public Node ParseExpression()
        {
            Node left = ParseTerm();
            Lexem lex = lexer.Token;
            while (lex.Type == Lexem.Types.Additive_Op)
            {
                lexer.NextToken();
                left = new BinOpNode(lex, left, ParseTerm());
                lex = lexer.Token;
            }
            return left;
        }
        public Node ParseTerm()
        {
            Node left = ParseSignedFactor();
            Lexem lex = lexer.Token;
            while (lex.Type == Lexem.Types.Multiplicative_Op)
            {
                lexer.NextToken();
                left = new BinOpNode(lex, left, ParseFactor());
                lex = lexer.Token;
            }
            return left;
        }
        public Node ParseSignedFactor()
        {
            int sign = 1;
            if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Plus)|| lexer.Token.Value.Equals(Lexem.SpecialSymbol.Minus))
            {
                if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Minus))
                {
                    sign = -1;
                }
                lexer.NextToken();
            }
            return new SignedFactorNode(sign, ParseFactor());
        }
        public Node ParseFactor()
        {
            Lexem lex = lexer.Token;
            if (lex.Value.Equals(Lexem.KeyWord.NOT))
            {
                lexer.NextToken();
                return new NotFactorNode(ParseFactor());
            }
            if (lex.Value.Equals(Lexem.KeyWord.TRUE))
            {
                lexer.NextToken();
                return new BoolNode(true);
            }
            if (lex.Value.Equals(Lexem.KeyWord.FALSE))
            {
                lexer.NextToken();
                return new BoolNode(false);
            }
            if (lex.Type == Lexem.Types.UInteger)
            {
                lexer.NextToken();
                return new NumberNode(lex);
            }
            if (lex.Type == Lexem.Types.Identifier || lex.Value.Equals(Lexem.SpecialSymbol.At))
            {
                return ParseVariableOrFunction();
            }
            if (lex.Value != null && lex.Value.Equals(Lexem.SpecialSymbol.LParenthese))
            {
                lexer.NextToken();
                Node exp = ParseExpression();
                if (lexer.Token.Value == null || !(lexer.Token.Value.Equals(Lexem.SpecialSymbol.RParenthese)))
                    throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, ")"));
                lexer.NextToken();
                return exp;
            }
            throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "Factor"));
        }
        public Node ParseVariableOrFunction()
        {
            if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.At))
            {
                return ParseVariable();
            }
            if (lexer.Token.Type != Lexem.Types.Identifier)
                throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "Variable or Function"));
            var lex = lexer.Token;
            lexer.NextToken();
            if (!lexer.Token.Value.Equals(Lexem.SpecialSymbol.LParenthese))
                return ParseVariable(lex);
            lexer.NextToken();
            List<Node> result = new List<Node>();
            result.Add(ParseExpression());
            while (!lexer.Token.Value.Equals(Lexem.SpecialSymbol.RParenthese))
            {
                if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Comma))
                {
                    lexer.NextToken();
                    result.Add(ParseExpression());
                }
                else throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, ")"));
            }
            return new FunctionDesignatorNode(new VariableNode(false, lex), new ParameterListNode(result));
        }
        public Node ParseProcedureStatement(Lexem inputIdentifier = null)
        {

            var lex = lexer.Token;
            if (inputIdentifier != null)
            {
                lex = inputIdentifier;
                goto objecttion;
            }
            if (lexer.Token.Type != Lexem.Types.Identifier)
                throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "Identifier"));
            lexer.NextToken();
            objecttion:
            if (!lexer.Token.Value.Equals(Lexem.SpecialSymbol.LParenthese))
                return new ProcedureStatementNode(new VariableNode(false, lex));
            lexer.NextToken();
            List<Node> result = new List<Node>();
            result.Add(ParseExpression());
            while (!lexer.Token.Value.Equals(Lexem.SpecialSymbol.RParenthese))
            {
                if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Comma))
                {
                    lexer.NextToken();
                    result.Add(ParseExpression());
                }
                else throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, ")"));
            }
            return new ProcedureStatementNode(new VariableNode(false, lex), new ParameterListNode(result));
        }
        public Node ParseVariable(Lexem inputIdentifier = null)
        {
            Lexem lex;
            bool isRef = false;
            if (inputIdentifier != null)
            {
                lex = inputIdentifier;
                goto objecttion;
            }
            if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.At))
            {
                isRef = true;
                lexer.NextToken();
            }
            lex = lexer.Token;
            if (lexer.Token.Type!= Lexem.Types.Identifier)
                throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "Variable"));
            lexer.NextToken();
            objecttion:;
            List<Node> args = new List<Node>();
            while (lexer.Token.Value.Equals(Lexem.SpecialSymbol.LBracket) || lexer.Token.Value.Equals(Lexem.SpecialSymbol.Dot))
            {
                if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.LBracket))
                {
                    List<Node> list = new List<Node>();
                    lexer.NextToken();
                    list.Add(ParseExpression());
                    while (!lexer.Token.Value.Equals(Lexem.SpecialSymbol.RBracket))
                    {
                        if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Comma))
                        {
                            lexer.NextToken();
                            list.Add(ParseExpression());
                            continue;
                        }
                        throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.InvalidArgs));
                    }
                    lexer.NextToken();
                    var transit = new TransitNode(list);
                    args.Add(transit);
                }
                if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Dot))
                {
                    lexer.NextToken();
                    if (lexer.Token.Type != Lexem.Types.Identifier)
                        throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.InvalidArgs));
                    args.Add(new VariableNode(false, lexer.Token));
                    lexer.NextToken();
                }

            }
            return new VariableNode(isRef, lex, args);
        }
    }
}
