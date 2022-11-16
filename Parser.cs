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
            if(indent=="")
            {
                Console.WriteLine(indent + " " + tree.getVal());
                indent += last ? " " : "| ";
            }
            else
            {
                Console.WriteLine(indent + "\\-- " + tree.getVal());
                indent += last ? "    " : "|   ";
            }

            if (tree is BinOpNode)
            {
                PrintExpressionTree((tree as BinOpNode).left, indent, false);
                PrintExpressionTree((tree as BinOpNode).right, indent, true);
            }
        }
        public abstract class Node
        {
            public abstract string getVal();
        }
        public class BinOpNode : Node
        {
            public Lexem op;
            public Node left;
            public Node right;
            public override string getVal()
            {
                switch (op.Value)
                {
                    case "Plus":
                        return "+";
                    case "Minus":
                        return "-";
                    case "Multiply":
                        return "*";
                    case "Divide":
                        return "/";
                }

                return op.Value.ToString();
            }
            public BinOpNode(Lexem op, Node left, Node right)
            {
                this.op = op;
                this.left = left;
                this.right = right;
            }
        }
        public class NumberNode : Node
        {
            public Lexem lexem;
            public override string getVal()
            {
                return lexem.Value.ToString();
            }
            public NumberNode(Lexem lexem)
            {
                this.lexem = lexem;
            }
        }
        public class VariableNode : Node
        {
            public Lexem lexem;
            public override string getVal()
            {
                return lexem.Value.ToString();
            }
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
            while (lex.Value == "Plus" || lex.Value == "Minus")
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
            while (lex.Value == "Multiply" || lex.Value == "Divide")
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
            if (lex.Type == "Integer")
            {
                lexer.NextToken();
                return new NumberNode(lex);
            }
            if (lex.Type == "Identifier")
            {
                lexer.NextToken();
                return new VariableNode(lex);
            }
            if (lex.Value != null && lex.Value == "Bracket_open" && lex.Type == "Math")
            {
                lexer.NextToken();
                Node exp = ParseExpression();
                if (lexer.Token.Value == null || !(lexer.Token.Value == "Bracket_closed" && lexer.Token.Type == "Math"))
                    throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.CBracketE));
                lexer.NextToken();
                return exp;
            }
            throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.FactorE));
        }
    }
}
