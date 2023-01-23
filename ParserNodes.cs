using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Compiler.Program;

namespace Compiler
{
    public partial class Parser
    {
        public abstract class Node
        {
            public abstract string getStrVal();
            public abstract List<Node> GetChildren();
        }

        public class ProgrammNode : Node
        {
            Node heading;
            List<Node> declarations;
            Node body;
            public override List<Node> GetChildren()
            {
                var list = new List<Node>();
                list.Add(heading);
                list.AddRange(declarations);
                list.Add(body);
                return list;
            }
            public override string getStrVal()
            {
                return "Programm";
            }
            public ProgrammNode(Node heading, List<Node> declarations, Node body)
            {
                this.heading = heading;
                this.declarations = declarations;
                this.body = body;
            }
        }

        public class HeadingNode : Node
        {
            Node identifier;
            public override List<Node> GetChildren()
            {
                return new List<Node> { identifier };
            }
            public override string getStrVal()
            {
                return "Heading";
            }
            public HeadingNode(Node identifier)
            {
                this.identifier = identifier;
            }
        }

        public class TypeDefinitionNode : Node
        {
            public override List<Node> GetChildren()
            {
                return new List<Node> { };
            }
            public override string getStrVal()
            {
                return "ConstDef";
            }
        }
        public class VariableDeclPartNode : Node
        {
            Node listVariableDecls;
            public override List<Node> GetChildren()
            {
                return new List<Node> { listVariableDecls };
            }
            public override string getStrVal()
            {
                return "VariableDeclPart";
            }
            public VariableDeclPartNode(Node listVariableDecls)
            {
                this.listVariableDecls = listVariableDecls;
            }
        }
        public class VariableDeclNode : Node
        {
            public override List<Node> GetChildren()
            {
                return new List<Node> { };
            }
            public override string getStrVal()
            {
                return "VariableDecl";
            }
        }
        public class LabelDeclNode : Node
        {
            public override List<Node> GetChildren()
            {
                return new List<Node> { };
            }
            public override string getStrVal()
            {
                return "LabelDecl";
            }
        }

        public class ProcedureDeclNode : Node
        {
            SymProc proc;
            public override List<Node> GetChildren()
            {
                return new List<Node> { proc.body };
            }
            public override string getStrVal()
            {
                return "ProcedureDecl";
            }
            public ProcedureDeclNode(SymProc proc)
            {
                this.proc = proc;
            }
        }

        public class FunctionDeclNode : Node
        {
            SymFunc func;
            public override List<Node> GetChildren()
            {
                return new List<Node> { func.body };
            }
            public override string getStrVal()
            {
                return "FunctionDecl";
            }
            public FunctionDeclNode(SymFunc func)
            {
                this.func = func;
            }
        }

        public class ListNode : Node
        {
            object type;
            List<Node> children;
            public override List<Node> GetChildren()
            {
                return children;
            }
            public override string getStrVal()
            {
                return "List " + type.ToString();
            }
            public ListNode(object type, List<Node> children)
            {
                this.type = type;
                this.children = children;
            }
        }

        public class BlockNode : Node
        {
            Node declarations;
            Node body;
            public override List<Node> GetChildren()
            {
                return new List<Node> { declarations, body };
            }
            public override string getStrVal()
            {
                return "Block";
            }
            public BlockNode(Node declarations, Node body)
            {
                this.declarations = declarations;
                this.body = body;
            }
        }

        //statements
        public abstract class StatementNode : Node { }
        public class CompoundStatementNode : StatementNode
        {
            List<Node> statements;
            public override List<Node> GetChildren()
            {
                return statements;
            }
            public override string getStrVal()
            {
                return "CompoundStatement";
            }
            public CompoundStatementNode(List<Node> statements)
            {
                this.statements = statements;
            }
        }
        public class EmptyStatementNode : StatementNode
        {
            public override List<Node> GetChildren()
            {
                return new List<Node> { };
            }
            public override string getStrVal()
            {
                return "EmptyStatement";
            }
            public EmptyStatementNode()
            {
            }
        }
        public class LabelStatementNode : StatementNode
        {
            Node label;
            Node statement;
            public override List<Node> GetChildren()
            {
                return new List<Node> { label, statement };
            }
            public override string getStrVal()
            {
                return "LabelStatementNode";
            }
            public LabelStatementNode(Node label, Node statement)
            {
                this.label = label;
                this.statement = statement;
            }
        }
        public class ForStatementNode : StatementNode
        {
            Node identifier;
            Node initial;
            Node final;
            Node statement;
            int direction;
            public override List<Node> GetChildren()
            {
                return new List<Node> { identifier, initial, final, statement };
            }
            public override string getStrVal()
            {
                return "ForStatementNode " + direction;
            }
            public ForStatementNode(Node identifier, Node initial, Node final, Node statement, int direction)
            {
                this.identifier = identifier;
                this.initial = initial;
                this.final = final;
                this.statement = statement;
                this.direction = direction;
            }
        }
        public class WhileStatementNode : StatementNode
        {
            Node condition;
            Node statement;
            public override List<Node> GetChildren()
            {
                return new List<Node> { condition, statement };
            }
            public override string getStrVal()
            {
                return "WhileStatementNode";
            }
            public WhileStatementNode(Node condition, Node statement)
            {
                this.condition = condition;
                this.statement = statement;
            }
        }
        public class IfStatementNode : StatementNode
        {
            Node condition;
            Node statement;
            Node altStatement;
            public override List<Node> GetChildren()
            {
                return new List<Node> { condition, statement, altStatement };
            }
            public override string getStrVal()
            {
                return "IfStatementNode";
            }
            public IfStatementNode(Node condition, Node statement, Node altStatement = null)
            {
                this.condition = condition;
                this.statement = statement;
                this.altStatement = altStatement;
            }
        }
        public class RepeatStatementNode : StatementNode
        {
            Node condition;
            List<Node> statements;
            public override List<Node> GetChildren()
            {
                var list = new List<Node>() { condition };
                list.AddRange(statements);
                return list;
            }
            public override string getStrVal()
            {
                return "RepetetiveStatement";
            }
            public RepeatStatementNode(Node condition, List<Node> statements)
            {
                this.condition = condition;
                this.statements = statements;
            }
        }
        public class WithStatementNode : StatementNode
        {
            Node recordVarList;
            Node statement;
            public override List<Node> GetChildren()
            {
                return new List<Node> { recordVarList, statement };
            }
            public override string getStrVal()
            {
                return "WithStatement";
            }
            public WithStatementNode(Node recordVarList, Node statement)
            {
                this.recordVarList = recordVarList;
                this.statement = statement;
            }
        }
        public class AssignStatementNode : StatementNode
        {
            Node left;
            Node right;
            public override List<Node> GetChildren()
            {
                return new List<Node> { left, right };
            }
            public override string getStrVal()
            {
                return "AssignStatement";
            }
            public AssignStatementNode(Node left, Node right)
            {
                this.left = left;
                this.right = right;
            }
        }
        public class ProcedureStatementNode : StatementNode
        {
            Node identifier;
            List<ExprNode> parameterList;
            public override List<Node> GetChildren()
            {
                var list = new List<Node>();
                list.Add(identifier);
                list.AddRange(parameterList);
                return list;
            }
            public override string getStrVal()
            {
                return "ProcedureStatement";
            }
            public ProcedureStatementNode(Node identifier, List<ExprNode> parameterList = null)
            {
                this.identifier = identifier;
                this.parameterList = parameterList;
            }
        }
        public class ArrayAccessNode : Node
        {
            public Node left;
            public Node right;
            public override List<Node> GetChildren()
            {
                return new List<Node> { left, right };
            }
            public override string getStrVal()
            {
                return "ArrayAccess";
            }
            public ArrayAccessNode(Node left, Node right)
            {
                this.left = left;
                this.right = right;
            }
        }
        public class RecordAccessNode : Node
        {
            public Node left;
            public Node right;
            public override List<Node> GetChildren()
            {
                return new List<Node> { left, right };
            }
            public override string getStrVal()
            {
                return "RecordAccess";
            }
            public RecordAccessNode(Node left, Node right)
            {
                this.left = left;
                this.right = right;
            }
        }
        public class FunctionCallNode : ExprNode
        {
            SymFunc func;
            List<ExprNode> parameterList;
            public override List<Node> GetChildren()
            {
                var list = new List<Node>();
                list.AddRange(parameterList);
                return list;
            }
            public override string getStrVal()
            {
                return func.name;
            }
            public FunctionCallNode(SymFunc func, SymType type, List<ExprNode> parameterList = null) : base(type)
            {
                this.func = func;
                this.parameterList = parameterList;
            }
        }
        public class ParameterListNode : Node
        {
            List<Node> parameters;
            public override List<Node> GetChildren()
            {
                return parameters;
            }
            public override string getStrVal()
            {
                return "Parameters";
            }
            public ParameterListNode(List<Node> parameters)
            {
                this.parameters = parameters;
            }
        }

        public abstract class ExprNode : Node
        {
            public SymType type;

            public ExprNode(SymType type)
            {
                this.type = type;
            }
        }
        public abstract class BinOpNode : ExprNode
        {
            public Lexem op;
            public Node left;
            public Node right;
            public override List<Node> GetChildren()
            {
                return new List<Node> { left, right };
            }
            public override string getStrVal()
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
            public BinOpNode(Lexem op, Node left, Node right, SymType type) : base(type)
            {
                this.op = op;
                this.left = left;
                this.right = right;
            }
        }
        public class RelationalOp : BinOpNode
        {
            public override string getStrVal()
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
                }
                return op.Value.ToString();
            }
            public RelationalOp(Lexem op, Node left, Node right, SymType type) : base(op, left, right, type) { }
        }
        public class MultiplicativeOp : BinOpNode
        {
            public override string getStrVal()
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
            public MultiplicativeOp(Lexem op, Node left, Node right, SymType type) : base(op, left, right, type) { }
        }
        public class AdditiveOp : BinOpNode
        {
            public override string getStrVal()
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
            public AdditiveOp(Lexem op, Node left, Node right, SymType type) : base(op, left, right, type) { }
        }


        public class SignedFactorNode : ExprNode
        {
            Node value;
            int sign;
            public override string getStrVal()
            {
                return String.Format("Signed Factor({0})", sign > 0 ? "+" : "-");
            }
            public SignedFactorNode(int sign, Node value, SymType type) : base(type)
            {
                this.sign = sign;
                this.value = value;
            }
            public override List<Node> GetChildren()
            {
                return new List<Node> { value };
            }
        }
        public class VariableNode : ExprNode
        {
            bool isRef;
            Node identifier;
            public override string getStrVal()
            {
                return (isRef ? "Ref var" : "var");
            }
            public VariableNode(bool isRef, Node identifier, SymType symType) :base(symType)
            {
                this.isRef = isRef;
                this.identifier = identifier;
            }
            public override List<Node> GetChildren()
            {
                return new List<Node> { identifier };
            }
        }
        public class IdentifierNode : Node
        {
            public string value;
            public override string getStrVal()
            {
                return value;
            }
            public IdentifierNode(string value)
            {
                this.value = value;
            }
            public override List<Node> GetChildren()
            {
                return new List<Node> { };
            }
        }
        public class NotFactorNode : ExprNode
        {
            Node value;
            public override string getStrVal()
            {
                return "NOT";
            }
            public NotFactorNode(Node value, SymType type) : base(type)
            {
                this.value = value;
            }
            public override List<Node> GetChildren()
            {
                return new List<Node> { value };
            }
        }

        public abstract class ValueNode : ExprNode
        {
            public abstract object getVal();
            public ValueNode(SymType type) : base(type) { }
        }
        public class IntNode : ValueNode
        {
            public int value;
            public IntNode(int value, SymType type) : base(type)
            {
                this.value = value;
            }
            public override string getStrVal()
            {
                return value.ToString();
            }
            public override object getVal()
            {
                return value;
            }
            public override List<Node> GetChildren()
            {
                return new List<Node> { };
            }
        }
        public class RealNode : ValueNode
        {
            public double value;
            public override string getStrVal()
            {
                return value.ToString();
            }
            public RealNode(double value, SymType type) : base(type)
            {
                this.value = value;
            }
            public override object getVal()
            {
                return value;
            }
            public override List<Node> GetChildren()
            {
                return new List<Node> { };
            }
        }
        public class StringNode : ValueNode
        {
            public string value;
            public override string getStrVal()
            {
                return value;
            }
            public StringNode(string value, SymType type) : base(type)
            {
                this.value = value;
            }
            public override object getVal()
            {
                return value;
            }
            public override List<Node> GetChildren()
            {
                return new List<Node> { };
            }
        }
        public class BoolNode : ValueNode
        {
            public bool value;
            public override string getStrVal()
            {
                return value.ToString();
            }
            public BoolNode(bool value, SymType type) : base(type)
            {
                this.value = value;
            }
            public override object getVal()
            {
                return value;
            }
            public override List<Node> GetChildren()
            {
                return new List<Node> { };
            }
        }
        public class UConstNode : ValueNode
        {
            public ValueNode value;
            public override List<Node> GetChildren()
            {
                return new List<Node> { value };
            }
            public override string getStrVal()
            {
                return "UConst";
            }
            public override object getVal()
            {
                return value.getVal();
            }
            public UConstNode(ValueNode value, SymType type) : base(type)
            {
                this.value = value;
            }
        }
        public class ConstNode : Node
        {
            Node value;
            int sign;
            public override List<Node> GetChildren()
            {
                return new List<Node> { value };
            }
            public override string getStrVal()
            {
                return "Const" + (sign > 0 ? "+" : "-");
            }
            public ConstNode(Node value, int sign)
            {
                this.value = value;
                this.sign = sign;
            }
        }
        public class NILNode : ValueNode
        {
            public override string getStrVal()
            {
                return "NIL";
            }
            public override object getVal()
            {
                return null;
            }
            public override List<Node> GetChildren()
            {
                return new List<Node> { };
            }
            public NILNode(SymType type) : base(type) { }
        }
        public class RecordTypeNode : Node
        {
            public SymRecord symbol;
            public Node recordList;
            public override List<Node> GetChildren()
            {
                return new List<Node>() { recordList };
            }
            public override string getStrVal()
            {
                return "RecordList";
            }
            public RecordTypeNode(Node recordList)
            {
                this.recordList = recordList;
            }
        }
        public class SubrangeTypeNode : Node
        {
            public IntNode initial;
            public IntNode final;
            public override List<Node> GetChildren()
            {
                return new List<Node>() { initial, final };
            }
            public override string getStrVal()
            {
                return "SubrangeType";
            }
            public SubrangeTypeNode(IntNode initial, IntNode final)
            {
                this.initial = initial;
                this.final = final;
            }
        }
        public class TypeIdentifierNode : Node
        {
            public SymType type;
            public override List<Node> GetChildren()
            {
                return new List<Node>() { };
            }
            public override string getStrVal()
            {
                return type.name.ToString();
            }
            public TypeIdentifierNode(SymType type)
            {
                this.type = type;
            }
        }
        public class ArrayTypeNode : Node
        {
            public SymType conponentType;
            public List<SubrangeTypeNode> subranges;
            public override List<Node> GetChildren()
            {
                var list = new List<Node>() {  };
                list.AddRange(subranges);
                return list;
            }
            public override string getStrVal()
            {
                return "ArrayType";
            }
            public ArrayTypeNode(SymType conponentType, List<SubrangeTypeNode> subranges)
            {
                this.conponentType = conponentType;
                this.subranges = subranges;
            }
        }
        public class ElementNode : Node
        {
            public Node left;
            public Node right;
            public override List<Node> GetChildren()
            {
                return new List<Node> { left, right };
            }
            public override string getStrVal()
            {
                return "Element";
            }
            public ElementNode(Node left, Node right = null)
            {
                this.left = left;
                this.right = right;
            }
        }
        public class FieldListNode : Node
        {
            public Node fixedP;
            public override List<Node> GetChildren()
            {
                return new List<Node>() { fixedP };
            }
            public override string getStrVal()
            {
                return "FieldList";
            }
            public FieldListNode(Node fixedP)
            {
                this.fixedP = fixedP;
            }
        }
        public class RecordSectionNode : Node
        {
            public Node identifiers;
            public SymType type;
            public override List<Node> GetChildren()
            {
                return new List<Node>() { };
            }
            public override string getStrVal()
            {
                return "RecordSection";
            }
            public RecordSectionNode(Node identifiers, SymType type)
            {
                this.identifiers = identifiers;
                this.type = type;
            }
        }
    }
}
