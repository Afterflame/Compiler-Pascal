using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Compiler.Program;

namespace Compiler
{
    internal abstract class Node
    {
        public abstract string getVal();
        public abstract List<Node> GetChildren();
    }
    internal class BinOpNode : Node
    {
        public Lexem op;
        public Node left;
        public Node right;
        public override List<Node> GetChildren()
        {
            return new List<Node> { left, right };
        }
        public override string getVal()
        {
            switch (op.Value)
            {
                case Lexem.ASpecial.Plus:
                    return "+";
                case Lexem.ASpecial.Minus:
                    return "-";
                case Lexem.ASpecial.Multiply:
                    return "*";
                case Lexem.ASpecial.Divide:
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
    internal class NumberNode : Node
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
        public override List<Node> GetChildren()
        {
            return new List<Node> { };
        }
    }
    internal class VariableNode : Node
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
        public override List<Node> GetChildren()
        {
            return new List<Node> { };
        }
    }
}
