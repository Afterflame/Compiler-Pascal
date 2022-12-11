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
    internal class ConstChrNode : Node
    {
        Lexem lexem;
        public override List<Node> GetChildren()
        {
            return new List<Node> { };
        }
        public override string getVal()
        {
            return ((char)lexem.Value).ToString();
        }
        public ConstChrNode(Lexem lexem)
        {
            this.lexem = lexem;
        }
    }
    internal class ConstNode : Node
    {
        Node value;
        int sign;
        public override List<Node> GetChildren()
        {
            return new List<Node> { value };
        }
        public override string getVal()
        {
            return "Const";
        }
        public ConstNode(Node value, int sign = 1)
        {
            this.sign = sign;
            this.value = value;
        }
    }
    internal class ConstDefinitionPartNode : Node
    {
        Node constDefinition;
        public override List<Node> GetChildren()
        {
            return new List<Node> { constDefinition };
        }
        public override string getVal()
        {
            return "ConstDefPart";
        }
        public ConstDefinitionPartNode(Node constDefinition)
        {
            this.constDefinition = constDefinition;
        }
    }
    internal class ConstDefinitionNode : Node
    {
        Node identifier;
        Node value;
        public override List<Node> GetChildren()
        {
            return new List<Node> { identifier, value };
        }
        public override string getVal()
        {
            return "ConstDef";
        }
        public ConstDefinitionNode(Node identifier, Node value)
        {
            this.identifier = identifier;
            this.value = value;
        }
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
                case Lexem.SpecialSymbol.Plus:
                    return "+";
                case Lexem.SpecialSymbol.Minus:
                    return "-";
                case Lexem.SpecialSymbol.Multiply:
                    return "*";
                case Lexem.SpecialSymbol.Divide:
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
    internal class FunctionTypeNode : Node
    {
        List<Node> formalParameterList;
        Node resultType;
        public override string getVal()
        {
            return "FunctionType";
        }
        public FunctionTypeNode(List<Node> formalParameterList, Node resultType)
        {
            this.formalParameterList = formalParameterList;
            this.resultType = resultType;
        }
        public override List<Node> GetChildren()
        {
            List<Node> children = new List<Node> { resultType };
            children.Concat(formalParameterList);
            return children;
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
    internal class StringNode : Node
    {
        public Lexem lexem;
        public override string getVal()
        {
            return lexem.Value.ToString();
        }
        public StringNode(Lexem lexem)
        {
            this.lexem = lexem;
        }
        public override List<Node> GetChildren()
        {
            return new List<Node> { };
        }
    }
}
