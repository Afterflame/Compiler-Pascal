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
            if (indent=="")
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
            foreach(Node node in list)
            {
                if(node.Equals(list.Last()))
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
        public Node ParseExpression()
        {
            Node left = ParseTerm();
            Lexem lex = lexer.Token;
            while (lex.Value.Equals(Lexem.SpecialSymbol.Plus) || lex.Value.Equals(Lexem.SpecialSymbol.Minus))
            {
                lexer.NextToken();
                left = new BinOpNode(lex, left, ParseTerm());
                lex = lexer.Token;
            }
            return left;
        }
        public Node ParseTerm()
        {
            Node left = ParseFactor();
            Lexem lex = lexer.Token;
            while (lex.Value.Equals(Lexem.SpecialSymbol.Multiply) || lex.Value.Equals(Lexem.SpecialSymbol.Divide) ||
            lex.Value.Equals(Lexem.KeyWord.DIV) || lex.Value.Equals(Lexem.KeyWord.MOD))
            {
                lexer.NextToken();
                left = new BinOpNode(lex, left, ParseFactor());
                lex = lexer.Token;
            }
            return left;
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
                return new ConstNode(new VariableNode(lex), sign);
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
            if (! lex.Value.Equals(Lexem.SpecialSymbol.RParenthese))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, ")"));
            lexer.NextToken();
            return new ConstChrNode(lex);
        }
        public Node ParseConstDef()
        {
            var identifier = lexer.Token;
            if(identifier.Type != Lexem.Types.Identifier)
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "Identifier"));
            lexer.NextToken();
            if (!lexer.Token.Value.Equals(Lexem.SpecialSymbol.Equal))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "="));
            lexer.NextToken();
            return new ConstDefinitionNode(new VariableNode(identifier), ParseConst());
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
        public Node ParseFactor()
        {
            Lexem lex = lexer.Token;
            if (lex.Type == Lexem.Types.UInteger)
            {
                lexer.NextToken();
                return new NumberNode(lex);
            }
            if (lex.Type == Lexem.Types.Identifier || lex.Value.Equals(Lexem.SpecialSymbol.At))
            {
                return ParseVariable();
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
        public Node ParseVariable()
        {
            bool isRef = false;
            if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.At))
            {
                isRef = true;
                lexer.NextToken();
            }
            if (lexer.Token.Type!= Lexem.Types.Identifier)
                throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "Variable"));
            var lex = lexer.Token;
            lexer.NextToken();
            if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.LBracket))
            {
                List<Node> list = new List<Node>() { ParseExpression() };
                while (!lexer.Token.Value.Equals(Lexem.SpecialSymbol.RBracket))
                {
                    if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Comma)) {
                        lexer.NextToken();
                        list.Add(ParseExpression());
                    }
                }
            }
            return new VariableNode(lex);
        }
    }
}
