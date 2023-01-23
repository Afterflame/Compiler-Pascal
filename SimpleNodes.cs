using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Compiler.Program;

namespace Compiler
{

    public partial class SimpleParser
    {
        public abstract class Node
        {
            public abstract string getStrVal();
            public abstract List<Node> GetChildren();
        }

        public class ProgrammNode : Node
        {
            Node heading;
            Node body;
            public override List<Node> GetChildren()
            {
                return new List<Node> { heading, body };
            }
            public override string getStrVal()
            {
                return "Programm";
            }
            public ProgrammNode(Node heading, Node body)
            {
                this.heading = heading;
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


        public class ConstDefinitionNode : Node
        {
            Node identifier;
            Node value;
            public override List<Node> GetChildren()
            {
                return new List<Node> { identifier, value };
            }
            public override string getStrVal()
            {
                return "ConstDef";
            }
            public ConstDefinitionNode(Node identifier, Node value)
            {
                this.identifier = identifier;
                this.value = value;
            }
        }
        public class TypeDefinitionNode : Node
        {
            Node identifier;
            Node value;
            public override List<Node> GetChildren()
            {
                return new List<Node> { identifier, value };
            }
            public override string getStrVal()
            {
                return "ConstDef";
            }
            public TypeDefinitionNode(Node identifier, Node value)
            {
                this.identifier = identifier;
                this.value = value;
            }
        }
        public class ProcedureTypeNode : Node
        {
            Node args;
            public override List<Node> GetChildren()
            {
                return new List<Node> { args };
            }
            public override string getStrVal()
            {
                return "ProcedureType";
            }
            public ProcedureTypeNode(Node args)
            {
                this.args = args;
            }
        }
        public class FunctionTypeNode : Node
        {
            Node args;
            Node resultType;
            public override List<Node> GetChildren()
            {
                return new List<Node> { args, resultType };
            }
            public override string getStrVal()
            {
                return "FunctionType";
            }
            public FunctionTypeNode(Node args, Node resultType)
            {
                this.args = args;
                this.resultType = resultType;
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
            Node identifier;
            Node type;
            public override List<Node> GetChildren()
            {
                return new List<Node> { identifier, type };
            }
            public override string getStrVal()
            {
                return "VariableDecl";
            }
            public VariableDeclNode(Node identifier, Node type)
            {
                this.identifier = identifier;
                this.type = type;
            }
        }

        public class ProcedureDeclNode : Node
        {
            Node identifier;
            Node fParameterList;
            Node block;
            public override List<Node> GetChildren()
            {
                return new List<Node> { identifier, fParameterList, block };
            }
            public override string getStrVal()
            {
                return "ProcedureDecl";
            }
            public ProcedureDeclNode(Node identifier, Node fParameterList, Node block)
            {
                this.identifier = identifier;
                this.fParameterList = fParameterList;
                this.block = block;
            }
        }

        public class FunctionDeclNode : Node
        {
            Node identifier;
            Node fParameterList;
            Node resultType;
            Node block;
            public override List<Node> GetChildren()
            {
                return new List<Node> { identifier, fParameterList, resultType, block };
            }
            public override string getStrVal()
            {
                return "FunctionDecl";
            }
            public FunctionDeclNode(Node identifier, Node fParameterList, Node resultType, Node block)
            {
                this.identifier = identifier;
                this.fParameterList = fParameterList;
                this.resultType = resultType;
                this.block = block;
            }
        }
        public class LabelDeclNode : Node
        {
            Node list;
            public override List<Node> GetChildren()
            {
                return new List<Node> { list };
            }
            public override string getStrVal()
            {
                return "LabelDecl";
            }
            public LabelDeclNode(Node list)
            {
                this.list = list;
            }
        }

        public class FParameterGroupNode : Node
        {
            bool isVar;
            Node identifierList;
            Node type;
            public override List<Node> GetChildren()
            {
                return new List<Node> { identifierList, type };
            }
            public override string getStrVal()
            {
                return "FParameterGroup" + (isVar ? ", var" : "");
            }
            public FParameterGroupNode(Node identifierList, Node type, bool isVar)
            {
                this.identifierList = identifierList;
                this.type = type;
                this.isVar = isVar;
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
        public class ConstChrNode : Node
        {
            Lexem lexem;
            public override List<Node> GetChildren()
            {
                return new List<Node> { };
            }
            public override string getStrVal()
            {
                return ((char)lexem.Value).ToString();
            }
            public ConstChrNode(Lexem lexem)
            {
                this.lexem = lexem;
            }
        }
        public class UConstNode : Node
        {
            Node value;
            public override List<Node> GetChildren()
            {
                return new List<Node> { value };
            }
            public override string getStrVal()
            {
                return "UConst";
            }
            public UConstNode(Node value)
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

        //statements
        public abstract class StatementNode : Node
        { }
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
            Node statements;
            public override List<Node> GetChildren()
            {
                return new List<Node> { condition, statements };
            }
            public override string getStrVal()
            {
                return "RepetetiveStatement";
            }
            public RepeatStatementNode(Node condition, Node statements)
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
            Node parameterList;
            public override List<Node> GetChildren()
            {
                return new List<Node> { identifier, parameterList };
            }
            public override string getStrVal()
            {
                return "ProcedureStatement";
            }
            public ProcedureStatementNode(Node identifier, Node parameterList = null)
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
        public class FunctionDesignatorNode : Node
        {
            Node identifier;
            Node parameterList;
            public override List<Node> GetChildren()
            {
                return new List<Node> { identifier, parameterList };
            }
            public override string getStrVal()
            {
                return "FuncDesignator";
            }
            public FunctionDesignatorNode(Node identifier, Node parameterList = null)
            {
                this.identifier = identifier;
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
        public class BinOpNode : Node
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
            public BinOpNode(Lexem op, Node left, Node right)
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
                    case Lexem.KeyWord.IN:
                        return "IN";
                }
                return op.Value.ToString();
            }
            public RelationalOp(Lexem op, Node left, Node right) : base(op, left, right) { }
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
            public MultiplicativeOp(Lexem op, Node left, Node right) : base(op, left, right) { }
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
            public AdditiveOp(Lexem op, Node left, Node right) : base(op, left, right) { }
        }


        public class SignedFactorNode : Node
        {
            Node value;
            int sign;
            public override string getStrVal()
            {
                return String.Format("Signed Factor({0})", sign > 0 ? "+" : "-");
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
        public class VariableNode : Node
        {
            bool isRef;
            Node identifier;
            public override string getStrVal()
            {
                return (isRef ? "Ref var" : "var");
            }
            public VariableNode(bool isRef, Node identifier)
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
            public Lexem lexem;
            public override string getStrVal()
            {
                return lexem.Value.ToString();
            }
            public IdentifierNode(Lexem lexem)
            {
                this.lexem = lexem;
            }
            public override List<Node> GetChildren()
            {
                return new List<Node> { };
            }
        }
        public class NotFactorNode : Node
        {
            Node value;
            public override string getStrVal()
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
        public class NumberNode : Node
        {
            public Lexem lexem;
            public override string getStrVal()
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
        public class StringNode : Node
        {
            public Lexem lexem;
            public override string getStrVal()
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
        public class BoolNode : Node
        {
            public bool value;
            public override string getStrVal()
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
        public class NILNode : Node
        {
            public override string getStrVal()
            {
                return "NIL";
            }
            public override List<Node> GetChildren()
            {
                return new List<Node> { };
            }
        }
        public class SetNode : Node
        {
            List<Node> children;
            public override string getStrVal()
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
        public class RecordTypeNode : Node
        {
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
        public class ScalarTypeNode : Node
        {
            public Node identifierList;
            public override List<Node> GetChildren()
            {
                return new List<Node>() { identifierList };
            }
            public override string getStrVal()
            {
                return "ScalarType";
            }
            public ScalarTypeNode(Node identifierList)
            {
                this.identifierList = identifierList;
            }
        }
        public class SubrangeTypeNode : Node
        {
            public Node initial;
            public Node final;
            public override List<Node> GetChildren()
            {
                return new List<Node>() { initial, final };
            }
            public override string getStrVal()
            {
                return "SubrangeType";
            }
            public SubrangeTypeNode(Node initial, Node final)
            {
                this.initial = initial;
                this.final = final;
            }
        }
        public class StringTypeNode : Node
        {
            public Node value;
            public override List<Node> GetChildren()
            {
                return new List<Node>() { value };
            }
            public override string getStrVal()
            {
                return "StringType";
            }
            public StringTypeNode(Node value)
            {
                this.value = value;
            }
        }
        public class TypeIdentifierNode : Node
        {
            public string value;
            public override List<Node> GetChildren()
            {
                return new List<Node>() { };
            }
            public override string getStrVal()
            {
                return value.ToString();
            }
            public TypeIdentifierNode(string value)
            {
                this.value = value;
            }
        }
        public class ArrayTypeNode : Node
        {
            public Node conponentType;
            public Node typeList;
            public override List<Node> GetChildren()
            {
                return new List<Node>() { conponentType, typeList };
            }
            public override string getStrVal()
            {
                return "ArrayType";
            }
            public ArrayTypeNode(Node conponentType, Node typeList)
            {
                this.conponentType = conponentType;
                this.typeList = typeList;
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
            public Node type;
            public override List<Node> GetChildren()
            {
                return new List<Node>() { identifiers, type };
            }
            public override string getStrVal()
            {
                return "RecordSection";
            }
            public RecordSectionNode(Node identifiers, Node type)
            {
                this.identifiers = identifiers;
                this.type = type;
            }
        }
    }
}
