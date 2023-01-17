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

    internal class VarNode : Node
    {
        List<Node> children;
        public override List<Node> GetChildren()
        {
            return children;
        }
        public override string getVal()
        {
            return "transit";
        }
    }
    internal class TransitNode : Node
    {
        List<Node> children;
        public override List<Node> GetChildren()
        {
            return children;
        }
        public override string getVal()
        {
            return "transit";
        }
        public TransitNode(List<Node> children)
        {
            this.children = children;
        }
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
    internal class EmptyStatementNode : Node
    {
        public override List<Node> GetChildren()
        {
            return new List<Node> { };
        }
        public override string getVal()
        {
            return "EmptyStatement";
        }
        public EmptyStatementNode()
        {
        }
    }

    internal class ForStatementNode : Node
    {
        Node identifier;
        Node forList;
        Node statement;
        public override List<Node> GetChildren()
        {
            return new List<Node> { identifier, forList, statement };
        }
        public override string getVal()
        {
            return "ForStatementNode";
        }
        public ForStatementNode(Node identifier, Node forList, Node statement)
        {
            this.identifier = identifier;
            this.forList = forList;
            this.statement = statement;
        }
    }
    internal class WhileStatementNode : Node
    {
        Node condition;
        Node statement;
        public override List<Node> GetChildren()
        {
            return new List<Node> { condition, statement };
        }
        public override string getVal()
        {
            return "WhileStatementNode";
        }
        public WhileStatementNode(Node condition, Node statement)
        {
            this.condition = condition;
            this.statement = statement;
        }
    }
    internal class RepeatStatementNode : Node
    {
        Node condition;
        Node statement;
        public override List<Node> GetChildren()
        {
            return new List<Node> { condition, statement };
        }
        public override string getVal()
        {
            return "RepetetiveStatement";
        }
        public RepeatStatementNode(Node condition, Node statement)
        {
            this.condition = condition;
            this.statement = statement;
        }
    }
        internal class AssignStatementNode : Node
    {
        Node to;
        Node from;
        public override List<Node> GetChildren()
        {
            return new List<Node> { to, from };
        }
        public override string getVal()
        {
            return "AssignStatement";
        }
        public AssignStatementNode(Node to, Node from)
        {
            this.to = to;
            this.from = from;
        }
    }
    internal class ProcedureStatementNode : Node
    {
        Node identifier;
        Node parameterList;
        public override List<Node> GetChildren()
        {
            return new List<Node> { identifier, parameterList };
        }
        public override string getVal()
        {
            return "ProcedureStatement";
        }
        public ProcedureStatementNode(Node identifier, Node parameterList=null)
        {
            this.identifier = identifier;
            this.parameterList = parameterList;
        }
    }
    internal class FunctionDesignatorNode : Node
    {
        Node identifier;
        Node parameterList;
        public override List<Node> GetChildren()
        {
            return new List<Node> { identifier, parameterList };
        }
        public override string getVal()
        {
            return "FuncDesignator";
        }
        public FunctionDesignatorNode(Node identifier, Node parameterList)
        {
            this.identifier = identifier;
            this.parameterList = parameterList;
        }
    }
    internal class ParameterListNode : Node
    {
        List<Node> parameters;
        public override List<Node> GetChildren()
        {
            return parameters;
        }
        public override string getVal()
        {
            return "Parameters";
        }
        public ParameterListNode(List<Node> parameters)
        {
            this.parameters = parameters;
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
            //FIX
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
    internal class RelationalOp : BinOpNode
    {
        public override string getVal()
        {
            switch (op.Value)
            {
                case Lexem.SpecialSymbol.Equal:
                    return "=";
                case Lexem.SpecialSymbol.NotEq:
                    return "!=";
                case Lexem.SpecialSymbol.Less:
                    return "<";
                case Lexem.SpecialSymbol.LessEq:
                    return "<=";
                case Lexem.SpecialSymbol.Greater:
                    return ">";
                case Lexem.SpecialSymbol.GreaterEq:
                    return ">=";
                case Lexem.KeyWord.IN:
                    return "IN";
            }
            return op.Value.ToString();
        }
        public RelationalOp(Lexem op, Node left, Node right) : base(op, left, right) { }
    }
    internal class MultiplicativeOp: BinOpNode
    {
        public override string getVal()
        {
            switch (op.Value)
            {
                case Lexem.SpecialSymbol.Multiply:
                    return "*";
                case Lexem.SpecialSymbol.Divide:
                    return "/";
                case Lexem.KeyWord.DIV:
                    return "DIV";
                case Lexem.KeyWord.MOD:
                    return "MOD";
                case Lexem.KeyWord.AND:
                    return "AND";
            }

            return op.Value.ToString();
        }
        public MultiplicativeOp(Lexem op, Node left, Node right) : base(op, left, right) { }
    }
    internal class AdditiveOp : BinOpNode
    {
        public override string getVal()
        {
            switch (op.Value)
            {
                case Lexem.SpecialSymbol.Plus:
                    return "+";
                case Lexem.SpecialSymbol.Minus:
                    return "-";
                case Lexem.KeyWord.OR:
                    return "OR";
            }

            return op.Value.ToString();
        }
        public AdditiveOp(Lexem op, Node left, Node right) : base(op, left, right) { }
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
    internal class SignedFactorNode:Node
    {
        Node value;
        int sign;
        public override string getVal()
        {
            return sign > 0 ? "+" : "-";
        }
        public SignedFactorNode(int sign, Node value)
        {
            this.sign = sign;
            this.value = value;
        }
        public override List<Node> GetChildren()
        {
            return new List<Node> { value };
        }
    }
    internal class NotFactorNode : Node
    {
        Node value;
        public override string getVal()
        {
            return "NOT";
        }
        public NotFactorNode(Node value)
        {
            this.value = value;
        }
        public override List<Node> GetChildren()
        {
            return new List<Node> { value };
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
        bool isRef;
        public Lexem lexem;
        List<Node> args;
        public override string getVal()
        {
            return lexem.Value.ToString();
        }
        public VariableNode(bool isRef, Lexem lexem, List<Node> args = null)
        {
            this.args = args;
            this.isRef = isRef;
            this.lexem = lexem;
        }
        public override List<Node> GetChildren()
        {
            return args==null ? new List<Node> { } : args;
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
    internal class BoolNode : Node
    {
        public bool value;
        public override string getVal()
        {
            return value.ToString();
        }
        public BoolNode(bool value)
        {
            this.value = value;
        }
        public override List<Node> GetChildren()
        {
            return new List<Node> { };
        }
    }
    internal class SetNode : Node
    {
        List<Node> children;
        public override string getVal()
        {
            return "Set";
        }
        public SetNode(List<Node> children)
        {
            this.children = children;
        }
        public override List<Node> GetChildren()
        {
            return children;
        }
    }
    internal class ElementNode : Node
    {
        public Node left;
        public Node right;
        public override List<Node> GetChildren()
        {
            if (right != null)
                return new List<Node> { left, right };
            return new List<Node> { left };
        }
        public override string getVal()
        {
            return "Element";
        }
        public ElementNode(Node left, Node right=null)
        {
            this.left = left;
            this.right = right;
        }
    }
}
