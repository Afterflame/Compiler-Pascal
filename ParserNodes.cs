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
            public abstract string GetStrVal();
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
            public override string GetStrVal()
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
            public override string GetStrVal()
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
            public override string GetStrVal()
            {
                return "TypeDef";
            }
        }
        public class VariableDeclPartNode : Node
        {
            Node listVariableDecls;
            public override List<Node> GetChildren()
            {
                return new List<Node> { listVariableDecls };
            }
            public override string GetStrVal()
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
            public override string GetStrVal()
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
            public override string GetStrVal()
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
            public override string GetStrVal()
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
            public override string GetStrVal()
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
            public override string GetStrVal()
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
            public override string GetStrVal()
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
            public override string GetStrVal()
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
            public override string GetStrVal()
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
            public override string GetStrVal()
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
            public override string GetStrVal()
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
            public override string GetStrVal()
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
            public override string GetStrVal()
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
            public override string GetStrVal()
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
            public override string GetStrVal()
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
            public ExprNode left;
            public ExprNode right;
            public override List<Node> GetChildren()
            {
                return new List<Node> { left, right };
            }
            public override string GetStrVal()
            {
                return "AssignStatement";
            }
            public AssignStatementNode(ExprNode left, ExprNode right)
            {
                this.left = left;
                this.right = right;
            }
        }
        public class ProcedureStatementNode : StatementNode
        {
            SymProc proc;
            List<ExprNode> parameterList;
            public override List<Node> GetChildren()
            {
                var list = new List<Node>();
                list.AddRange(parameterList);
                return list;
            }
            public override string GetStrVal()
            {
                return proc.name;
            }
            public ProcedureStatementNode(SymProc proc, List<ExprNode> parameterList = null)
            {
                this.proc = proc;
                this.parameterList = parameterList;
            }
        }
        public class ArrayAccessNode : ExprNode
        {
            public ExprNode array;
            public ExprNode index;
            public override List<Node> GetChildren()
            {
                return new List<Node> { array, index };
            }
            public override string GetStrVal()
            {
                return "ArrayAccess";
            }
            public ArrayAccessNode(ExprNode array, ExprNode index)
            {
                this.array = array;
                this.index = index;
            }
            public override SymType GetNodeType()
            {
                if (cachedType == null)
                    cachedType = array.GetNodeType().AsArray().type;
                return cachedType;
            }
        }
        public class RecordAccessNode : ExprNode
        {
            public ExprNode record;
            public SymVar field;
            public override List<Node> GetChildren()
            {
                return new List<Node> { record, new IdentifierNode(field.name) };
            }
            public override string GetStrVal()
            {
                return "RecordAccess";
            }
            public RecordAccessNode(ExprNode record, SymVar field)
            {
                this.record = record;
                this.field = field;
            }
            public override SymType GetNodeType()
            {
                return field.type;
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
            public override string GetStrVal()
            {
                return func.name;
            }
            public FunctionCallNode(SymFunc func, List<ExprNode> parameterList = null, SymType type=null) : base(type)
            {
                this.func = func;
                this.parameterList = parameterList;
            }
            public override SymType GetNodeType()
            {
                return func.type_;
            }
        }
        public class ParameterListNode : Node
        {
            List<Node> parameters;
            public override List<Node> GetChildren()
            {
                return parameters;
            }
            public override string GetStrVal()
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
            public SymType cachedType;
            public ExprNode(SymType type = null)
            {
                cachedType = type;
            }
            public abstract SymType GetNodeType();
        }
        public abstract class BinOpNode : ExprNode
        {
            public Lexem op;
            public ExprNode left;
            public ExprNode right;
            public override List<Node> GetChildren()
            {
                return new List<Node> { left, right };
            }
            public override string GetStrVal()
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
            public BinOpNode(Lexem op, ExprNode left, ExprNode right, SymType type) : base(type)
            {
                this.op = op;
                this.left = left;
                this.right = right;
            }
            public override SymType GetNodeType()
            {
                if (cachedType == null)
                    throw new Exception("Type was not declared");
                return cachedType;
            }
        }
        public class RelationalOp : BinOpNode
        {
            public override string GetStrVal()
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
            public RelationalOp(Lexem op, ExprNode left, ExprNode right, SymType type) : base(op, left, right, type) { }
        }
        public class MultiplicativeOp : BinOpNode
        {
            public override string GetStrVal()
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
            public MultiplicativeOp(Lexem op, ExprNode left, ExprNode right, SymType type) : base(op, left, right, type) { }
        }
        public class AdditiveOp : BinOpNode
        {
            public override string GetStrVal()
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
            public AdditiveOp(Lexem op, ExprNode left, ExprNode right, SymType type) : base(op, left, right, type) { }
        }

        public class CastNode : ExprNode
        {
            ExprNode expr;
            public override string GetStrVal()
            {
                return cachedType.name;
            }
            public override SymType GetNodeType()
            {
                return cachedType;
            }
            public CastNode(ExprNode expr, SymType type) : base(type)
            {
                this.expr = expr;
                if (type == null)
                    throw new Exception("Type must be declared on creation");
            }
            public override List<Node> GetChildren()
            {
                return new List<Node> { expr };
            }
        }
        public class SignedFactorNode : ExprNode
        {
            ExprNode expr;
            int sign;
            public override string GetStrVal()
            {
                return String.Format("Signed Factor({0})", sign > 0 ? "+" : "-");
            }
            public SignedFactorNode(int sign, ExprNode expr, SymType type) : base(type)
            {
                this.sign = sign;
                this.expr = expr;
            }
            public override List<Node> GetChildren()
            {
                return new List<Node> { expr };
            }
            public override SymType GetNodeType()
            {
                if (cachedType == null)
                    cachedType = expr.GetNodeType();
                return cachedType;
            }
        }
        public class VariableNode : ExprNode
        {
            public bool isRef;
            public SymVar variable;
            public override string GetStrVal()
            {
                return String.Format("{0} : {1}", variable.name, isRef ? "Var Ref" : "Var");
            }
            public VariableNode(bool isRef, SymVar variable)
            {
                this.isRef = isRef;
                this.variable = variable;
            }
            public override List<Node> GetChildren()
            {
                return new List<Node> { };
            }
            public override SymType GetNodeType()
            {
                if (cachedType == null)
                    cachedType = variable.type;
                return cachedType;
            }
        }
        public class ConstDNode : VariableNode
        {
            public ConstNode value;
            public ConstDNode(SymVar variable, ConstNode value, bool isRef = false) : base(isRef, variable) 
            {
                this.value = value;
            }
            public override string GetStrVal()
            {
                return "ConstDef";
            }
            public override List<Node> GetChildren()
            {
                return new List<Node> { value };
            }
            public override SymType GetNodeType()
            {
                if (cachedType == null)
                    cachedType = variable.type;
                return cachedType;
            }
        }
        public class IdentifierNode : Node
        {
            public string value;
            public override string GetStrVal()
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
            ExprNode factor;
            public override string GetStrVal()
            {
                return "NOT";
            }
            public NotFactorNode(ExprNode factor, SymType type = null) : base(type)
            {
                this.factor = factor;
            }
            public override List<Node> GetChildren()
            {
                return new List<Node> { factor };
            }
            public override SymType GetNodeType()
            {
                if (cachedType == null)
                    cachedType = factor.GetNodeType();
                return cachedType;
            }
        }

        public abstract class ConstNode : ExprNode
        {
            public abstract object getVal();
            public ConstNode(SymType type) : base(type)
            {
                if (type == null)
                    throw new Exception("Type must be declared on creation");
            }
            public override SymType GetNodeType()
            {
                return cachedType;
            }
        }
        public class IntNode : ConstNode
        {
            public int value;
            public IntNode(int value, SymType type) : base(type)
            {
                this.value = value;
            }
            public override string GetStrVal()
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
        public class RealNode : ConstNode
        {
            public double value;
            public override string GetStrVal()
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
        public class StringNode : ConstNode
        {
            public string value;
            public override string GetStrVal()
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
        public class BoolNode : ConstNode
        {
            public bool value;
            public override string GetStrVal()
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
        public class UConstNode : ConstNode
        {
            public ConstNode value;
            public override List<Node> GetChildren()
            {
                return new List<Node> { value };
            }
            public override string GetStrVal()
            {
                return "UConst";
            }
            public override object getVal()
            {
                return value.getVal();
            }
            public UConstNode(ConstNode value, SymType type) : base(type)
            {
                this.value = value;
            }
        }
        public class SignedConstNode : ConstNode
        {
            ConstNode value;
            int sign;
            public override List<Node> GetChildren()
            {
                return new List<Node> { value };
            }
            public override string GetStrVal()
            {
                return "Const" + (sign > 0 ? "+" : "-");
            }
            public override object getVal()
            {
                return value.getVal();
            }
            public SignedConstNode(ConstNode value, int sign, SymType type):base(type)
            {
                this.value = value;
                this.sign = sign;
            }
        }
        public class NILNode : ConstNode
        {
            public override string GetStrVal()
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
            public override string GetStrVal()
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
            public override string GetStrVal()
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
            public override string GetStrVal()
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
            public override string GetStrVal()
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
            public override string GetStrVal()
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
            public override string GetStrVal()
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
            public override string GetStrVal()
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
