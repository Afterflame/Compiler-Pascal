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
                    PrintExpressionTree((tree as BinOpNode).right, indent, true);
                }
                else
                {
                    PrintExpressionTree((tree as BinOpNode).left, indent, false);
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
            while (lex.Value.Equals(Lexem.SpecialSymbol.Multiply) || lex.Value.Equals(Lexem.SpecialSymbol.Divide))
            {
                lexer.NextToken();
                left = new BinOpNode(lex, left, ParseFactor());
                lex = lexer.Token;
            }
            return left;
        }
        public Node ParseFactor()
        {
            Lexem lex = lexer.Token;
            if (lex.Type == Lexer.LexemTypes.Integer)
            {
                lexer.NextToken();
                return new NumberNode(lex);
            }
            if (lex.Type == Lexer.LexemTypes.Identifier)
            {
                lexer.NextToken();
                return new VariableNode(lex);
            }
            if (lex.Value != null && lex.Value.Equals(Lexem.SpecialSymbol.Bracket_open))
            {
                lexer.NextToken();
                Node exp = ParseExpression();
                if (lexer.Token.Value == null || !(lexer.Token.Value.Equals(Lexem.SpecialSymbol.Bracket_closed)))
                    throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.CBracketE));
                lexer.NextToken();
                return exp;
            }
            throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.FactorE));
        }
    }
}
