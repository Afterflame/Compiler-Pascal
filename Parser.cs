﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Compiler.Program;
using static Compiler.SimpleParser;
using static Compiler.Parser;

namespace Compiler
{
    public  partial class Parser
    {
        public SymTableStack symTableStack;
        Lexer lexer;
        List<SymType> builtIn = new List<SymType>()
        {
            new SymBool("BOOLEAN"),
            new SymInteger("INTEGER"),
            new SymReal("REAL"),
            new SymString("STRING"),
        };

        public Parser(ref Lexer lexer)
        {
            this.lexer = lexer;
            this.lexer.NextToken();
            symTableStack = new SymTableStack();
            symTableStack.AddTable(new SymTable());
            foreach (var item in builtIn)
            {
                symTableStack.Add(item);
            }
            symTableStack.Add(new SymProc("READ", new SymTable(new SymVar("internal", symTableStack.Get("STRING").AsType())), null, new CompoundStatementNode(new List<Node>())));
            symTableStack.Add(new SymProc("WRITE", new SymTable(new SymVar("internal", symTableStack.Get("STRING").AsType())), null, new CompoundStatementNode(new List<Node>())));
        }
        public static void PrintNodeTree(Node tree, string indent, bool last)
        {
            var value = (tree != null ? tree.GetStrVal() : "null");
            if (indent == "")
            {
                Console.WriteLine(indent + " " + value);
                indent += last ? " " : "│ ";
            }
            else
            {
                Console.WriteLine(indent + (last ? "└── " : "├── ") + value);
                indent += last ? "    " : "│   ";
            }

            List<Node> list = (tree != null ? tree.GetChildren() : new List<Node>() { });
            foreach (Node node in list)
            {
                if (node == null)
                {
                    if (list.Last() == null)
                        PrintNodeTree(node, indent, true);
                    else
                        PrintNodeTree(node, indent, false);
                    return;
                }
                if (node.Equals(list.Last()))
                {
                    PrintNodeTree(node, indent, true);
                }
                else
                {
                    PrintNodeTree(node, indent, false);
                }
            }
        }

        public static void PrintSymbolTable(Symbol tree, string indent, bool last)
        {
            var value = (tree != null ? String.Format("{0} : {1}", tree.GetStrVal(), tree.ToString().Substring(tree.ToString().LastIndexOf('.') + 1)) : "null");
            if (indent == "")
            {
                Console.WriteLine(indent + " " + value);
                indent += last ? " " : "│ ";
            }
            else
            {
                Console.WriteLine(indent + (last ? "└── " : "├── ") + value);
                indent += last ? "    " : "│   ";
            }

            SymTable table = (tree != null ? tree.GetChildren() : new SymTable());
            foreach (Symbol symbol in table.data.Select(kvp => kvp.Value).ToList())
            {
                if (symbol.Equals(table.data.Select(kvp => kvp.Value).ToList().Last()))
                {
                    PrintSymbolTable(symbol, indent, true);
                }
                else
                {
                    PrintSymbolTable(symbol, indent, false);
                }
            }
        }
        private void Require_KeyWord(object value)
        {
            if (!lexer.Token.Value.Equals(value))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, value.ToString()));
            lexer.NextToken();
        }
        private void Require_Special(object value, string msg)
        {
            if (!lexer.Token.Value.Equals(value))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpextedYGot, msg, lexer.Token.Value.ToString()));
            lexer.NextToken();
        }




        public Node ParseProgramm()
        {
            var locals = new SymTable();
            symTableStack.AddTable(locals);
            var heading = ParseHeading();
            List<Node> declarationNodes = ParseLocals();
            var body = ParseCompoundStatement();
            Require_Special(Lexem.SpecialSymbol.Dot, ".");
            Require_Special(Lexem.SpecialSymbol.EOF, "EOF");
            var main = new SymProc("main", new SymTable(), locals, body);
            symTableStack.PopTable();
            symTableStack.Add(main);
            return new ProgrammNode(heading, declarationNodes, body);
        }
        public Node ParseHeading()
        {
            Require_KeyWord(Lexem.KeyWord.PROGRAM);
            var identifier = ParseIdentifier();
            Require_Special(Lexem.SpecialSymbol.Semicolon, ";");
            return new HeadingNode(identifier);
        }
        public IdentifierNode ParseIdentifier()
        {
            if (lexer.Token.Type != Lexem.Types.Identifier)
                throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "Identifier"));
            var node = new IdentifierNode(lexer.Token.Value.ToString());
            lexer.NextToken();
            return node;
        }

        //Def's
        public List<Node> ParseLocals()
        {
            List<Node> declarations = new List<Node>();
            while (!lexer.Token.Value.Equals(Lexem.KeyWord.BEGIN))
            {
                declarations.AddRange(ParseLocalsElement());
            }
            return declarations;
        }
        public List<Node> ParseLocalsElement()
        {
            switch (lexer.Token.Value)
            {
                case Lexem.KeyWord.TYPE:
                    return ParseTypeDefinition ();
                case Lexem.KeyWord.LABEL:
                    return ParseLabelDecl();
                case Lexem.KeyWord.VAR:
                    return ParseVariableDecl();
                case Lexem.KeyWord.CONST:
                    return ParseConstDecl();
                case Lexem.KeyWord.PROCEDURE:
                    return new List<Node>() { ParseProcedureDecl() };
                case Lexem.KeyWord.FUNCTION:
                    return new List<Node>() { ParseFunctionDecl() };
                default:
                    break;
            }
            throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "Declaration Part"));
        }
        public List<Node> ParseConstDecl()
        {
            Require_KeyWord(Lexem.KeyWord.CONST);
            List<Node> list = new List<Node>();
            while (lexer.Token.Type == Lexem.Types.Identifier)
            {
                var identifier = ParseIdentifier();
                Require_Special(Lexem.SpecialSymbol.Equal, "=");
                var ConstN = ParseConstVariable();
                list.Add(ConstN);
                ConstN.variable.name = identifier.value;
                symTableStack.Add(ConstN.variable);
                Require_Special(Lexem.SpecialSymbol.Semicolon, ";");
            }
            return list;
        }
        public ConstDNode ParseConstVariable()
        {
            if (lexer.Token.Type == Lexem.Types.Literal)
            {
                var lex = lexer.Token;
                lexer.NextToken();
                var valueNode = new StringNode(lex.Value.ToString(), (SymString)symTableStack.Get("STRING"));
                return new ConstDNode(new SymConst(null, (SymString)symTableStack.Get("STRING")), valueNode);
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
            if (lexer.Token.Type == Lexem.Types.UInteger)
            {
                var lex = lexer.Token;
                lexer.NextToken();
                var valueNode = new IntNode((int)lex.Value * sign, (SymInteger)symTableStack.Get("INTEGER"));
                return new ConstDNode(new SymConst(null, (SymInteger)symTableStack.Get("INTEGER")), valueNode);
            }
            if (lexer.Token.Type == Lexem.Types.UReal)
            {
                var lex = lexer.Token;
                lexer.NextToken();
                var valueNode = new RealNode((int)lex.Value * sign, (SymReal)symTableStack.Get("REAL"));
                return new ConstDNode(new SymConst(null, (SymReal)symTableStack.Get("REAL")), valueNode);
            }
            throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "UConstatant"));
        }
        public List<Node> ParseVariableDecl()
        {
            Require_KeyWord(Lexem.KeyWord.VAR);
            List<Node> list = new List<Node>();
            while (lexer.Token.Type == Lexem.Types.Identifier)
            {
                var identifierList = ParseIdentifierList();
                list.AddRange(identifierList);
                Require_Special(Lexem.SpecialSymbol.Colon, ":");
                var type = ParseType();
                foreach (var identifier in identifierList)
                {
                    symTableStack.Add(new SymVar(identifier.value, type));
                }
                Require_Special(Lexem.SpecialSymbol.Semicolon, ";");
            }
            if(list.Count==0)
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "Variable"));
            return list;
        }
        public List<Node> ParseLabelDecl()
        {
            Require_KeyWord(Lexem.KeyWord.LABEL);

            if (lexer.Token.Type != Lexem.Types.UInteger)
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "UInt"));
            symTableStack.Add(new SymLabel(lexer.Token.Value.ToString()));
            lexer.NextToken();
            List<Node> list = new List<Node>();
            while (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Comma))
            {
                lexer.NextToken();
                if (lexer.Token.Type != Lexem.Types.UInteger)
                    throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "UInt"));
                symTableStack.Add(new SymLabel(lexer.Token.Value.ToString()));
                list.Add(new IntNode((int)lexer.Token.Value, symTableStack.Get("INTEGER").AsType()));
                lexer.NextToken();
            }
            Require_Special(Lexem.SpecialSymbol.Semicolon, ";");
            return list;
        }
        public Node ParseFunctionDecl()
        {
            Require_KeyWord(Lexem.KeyWord.FUNCTION);
            var identifier = ParseIdentifier();
            var symFunc = new SymFunc(identifier.value, new SymTable(), new SymTable(), new SymType(null), new CompoundStatementNode(new List<Node>()));
            symFunc.locals_.Add(symFunc);
            SymTable all_vars = new SymTable();
            symTableStack.AddTable(symFunc.params_);
            if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.LParenthese))
            {
                ParseFParamenerList();
            }
            all_vars.AddRange(symFunc.params_);
            Require_Special(Lexem.SpecialSymbol.Colon, ":");
            symFunc.type_ = ParseTypeIdentifier();
            Require_Special(Lexem.SpecialSymbol.Semicolon, ";");
            symTableStack.AddTable(symFunc.locals_);
            ParseLocals();
            symFunc.locals_.Add(new SymVar("RESULT", symFunc.type_));
            all_vars.AddRange(symFunc.locals_);
            symFunc.body = ParseCompoundStatement();
            symFunc.locals_.PopFirst();
            symTableStack.PopTable();
            symTableStack.PopTable();
            symTableStack.Add(symFunc);
            Require_Special(Lexem.SpecialSymbol.Semicolon, ";");
            return new FunctionDeclNode(symFunc);
        }
        public Node ParseProcedureDecl()
        {
            Require_KeyWord(Lexem.KeyWord.PROCEDURE);
            var identifier = ParseIdentifier();
            var symProc = new SymProc(identifier.value, new SymTable(), new SymTable(), new CompoundStatementNode(new List<Node>()));
            symProc.locals_.Add(symProc);
            SymTable all_vars = new SymTable();
            symTableStack.AddTable(symProc.params_);
            if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.LParenthese))
            {
                ParseFParamenerList();
            }
            all_vars.AddRange(symProc.params_);
            Require_Special(Lexem.SpecialSymbol.Semicolon, ";");
            symTableStack.AddTable(symProc.locals_);
            ParseLocals();
            all_vars.AddRange(symProc.locals_);
            symProc.body = ParseCompoundStatement();
            symProc.locals_.PopFirst();
            symTableStack.PopTable();
            symTableStack.PopTable();
            symTableStack.Add(symProc);
            Require_Special(Lexem.SpecialSymbol.Semicolon, ";");
            return new ProcedureDeclNode(symProc);
        }
        public List<Node> ParseTypeDefinition()
        {
            Require_KeyWord(Lexem.KeyWord.TYPE);
            List<Node> list = new List<Node>();
            while (lexer.Token.Type == Lexem.Types.Identifier)
            {
                var identifier = ParseIdentifier();
                list.Add(identifier);
                Require_Special(Lexem.SpecialSymbol.Equal, "=");
                var type = ParseType();
                type.name = identifier.value;
                symTableStack.Add(type);
                Require_Special(Lexem.SpecialSymbol.Semicolon, ";");
            }
            return list;
        }

        public void ParseFParamenerList()
        {
            Require_Special(Lexem.SpecialSymbol.LParenthese, "(");
            ParseParamenerGroup();
            while (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Semicolon))
            {
                ParseParamenerGroup();
            }
            Require_Special(Lexem.SpecialSymbol.RParenthese, ")");
        }
        public void ParseParamenerGroup()
        {
            bool isRef = false;
            if (lexer.Token.Value.Equals(Lexem.KeyWord.VAR))
            {
                isRef = true;
                lexer.NextToken();
            }
            var list = ParseIdentifierList();
            Require_Special(Lexem.SpecialSymbol.Colon, ":");
            var type = ParseTypeIdentifier();
            foreach (var item in list)
            {
                if (isRef)
                    symTableStack.Add(new SymParamRef(item.value, type));
                else
                    symTableStack.Add(new SymParam(item.value, type));

            }
        }


        public ExprNode ParseUConst()
        {
            if (lexer.Token.Type == Lexem.Types.Literal)
            {
                var lex = lexer.Token;
                lexer.NextToken();
                return new UConstNode(new StringNode((string)lex.Value, symTableStack.Get("STRING").AsType()), symTableStack.Get("STRING").AsType());
            }
            if (lexer.Token.Type == Lexem.Types.UReal)
            {
                var lex = lexer.Token;
                lexer.NextToken();
                return new UConstNode(new RealNode((double)lex.Value, symTableStack.Get("REAL").AsType()), symTableStack.Get("REAL").AsType());
            }

            if (lexer.Token.Type == Lexem.Types.UInteger)
            {
                var lex = lexer.Token;
                lexer.NextToken();
                return new UConstNode(new IntNode((int)lex.Value, symTableStack.Get("INTEGER").AsType()), symTableStack.Get("INTEGER").AsType());
            }
            throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "UConstatant"));
        }
        public ExprNode ParseVariable(bool OnlyVar = false)
        {
            bool isRef = false;
            if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.At))
            {
                isRef = true;
                lexer.NextToken();
            }
            ExprNode lastNode = new VariableNode(isRef, symTableStack.Get(ParseIdentifier().value).AsVar());
            if(OnlyVar && ((VariableNode)lastNode).variable.IsConst())
                throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.Custom, "Can not use const"));
            while (lexer.Token.Value.Equals(Lexem.SpecialSymbol.LBracket) || lexer.Token.Value.Equals(Lexem.SpecialSymbol.Dot))
            {
                if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.LBracket))
                {
                    List<ExprNode> list = new List<ExprNode>();
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
                    for (int i = 0; i < list.Count(); i++)
                    {
                        lastNode.GetNodeType().AsArray();
                        lastNode = new ArrayAccessNode(lastNode, list[i]);
                    }
                    lexer.NextToken();
                }
                if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Dot))
                {
                    lexer.NextToken();
                    var identifier = ParseIdentifier();
                    var field = lastNode.GetNodeType().AsRecord().fields.Get(identifier.value).AsVar();
                    lastNode = new RecordAccessNode(lastNode, field);
                }

            }
            return lastNode;
        }
        

        //statements
        public CompoundStatementNode ParseCompoundStatement()
        {
            Require_KeyWord(Lexem.KeyWord.BEGIN);
            var statements = ParseStatements();
            Require_KeyWord(Lexem.KeyWord.END);
            return new CompoundStatementNode(statements);
        }
        public List<Node> ParseStatements()
        {
            List<Node> statements = new List<Node>();
            statements.Add(ParseStatement());
            while (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Semicolon))
            {
                lexer.NextToken();
                statements.Add(ParseStatement());
            }
            return statements;
        }
        public Node ParseStatement()
        {
            Node label = null;
            if (lexer.Token.Type == Lexem.Types.UInteger)
            {
                label = new IntNode((int)lexer.Token.Value, (SymInteger)symTableStack.Get("INTEGER"));
                lexer.NextToken();
                Require_Special(Lexem.SpecialSymbol.Colon, ":");
            }
            Node statement;
            switch (lexer.Token.Value)
            {
                case Lexem.KeyWord.BEGIN:
                    statement = ParseCompoundStatement();
                    break;
                case Lexem.KeyWord.IF:
                    statement = ParseIfStatement();
                    break;
                case Lexem.KeyWord.WITH:
                    statement = ParseWithStatement();
                    break;
                case Lexem.KeyWord.FOR:
                    statement = ParseForStatement();
                    break;
                case Lexem.KeyWord.WHILE:
                    statement = ParseWhileStatement();
                    break;
                case Lexem.KeyWord.REPEAT:
                    statement = ParseRepeatStatement();
                    break;
                default:
                    statement = ParseSimpleStatement();
                    break;
            }
            if (label != null)
                return new LabelStatementNode(label, statement);
            return statement;
        }
        public Node ParseSimpleStatement()
        {
            if (lexer.Token.Type == Lexem.Types.Identifier)
            {
                if (symTableStack.Get(lexer.Token.Value.ToString()) is SymProc)
                    return ParseProcedureStatement();
                return ParseAssignStatement();
            }
            return new EmptyStatementNode();
        }
        public Node ParseIfStatement()
        {
            Require_KeyWord(Lexem.KeyWord.IF);
            var expression = TryCast(ParseExpression(), symTableStack.Get("BOOLEAN").AsType());
            Require_KeyWord(Lexem.KeyWord.THEN);
            var statement = ParseStatement();
            Node altStatement = null;
            if (lexer.Token.Value.Equals(Lexem.KeyWord.ELSE))
            {
                lexer.NextToken();
                altStatement = ParseStatement();
            }
            return new IfStatementNode(expression, statement, altStatement);
        }
        public Node ParseWithStatement()
        {
            Require_KeyWord(Lexem.KeyWord.WITH);
            List<Node> variables = new List<Node>();
            variables.Add(ParseVariable());
            while (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Comma))
            {
                lexer.NextToken();
                variables.Add(ParseVariable());
            }
            Require_KeyWord(Lexem.KeyWord.DO);
            var statement = ParseStatement();
            return new WhileStatementNode(new ListNode("variables", variables), statement);
        }
        public Node ParseForStatement()
        {
            Require_KeyWord(Lexem.KeyWord.FOR);
            var identifier = ParseIdentifier();
            Require_Special(Lexem.SpecialSymbol.Assign, ":=");
            var initial = ParseExpression();
            int dir = 0;
            switch (lexer.Token.Value)
            {
                case Lexem.KeyWord.TO:
                    dir = 1;
                    break;
                case Lexem.KeyWord.DOWNTO:
                    dir = -1;
                    break;
                default:
                    throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "TO / DOWNTO"));
            }
            lexer.NextToken();
            var final = ParseExpression();
            Require_KeyWord(Lexem.KeyWord.DO);
            var statement = ParseStatement();
            return new ForStatementNode(identifier, initial, final, statement, dir);
        }
        public Node ParseWhileStatement()
        {
            Require_KeyWord(Lexem.KeyWord.WHILE);
            var expression = TryCast(ParseExpression(), symTableStack.Get("BOOLEAN").AsType());
            Require_KeyWord(Lexem.KeyWord.DO);
            var statement = ParseStatement();
            return new WhileStatementNode(expression, statement);
        }
        public Node ParseRepeatStatement()
        {
            Require_KeyWord(Lexem.KeyWord.REPEAT);
            var statements = ParseStatements();
            Require_KeyWord(Lexem.KeyWord.UNTIL);
            var expression = TryCast(ParseExpression(), symTableStack.Get("BOOLEAN").AsType());
            return new RepeatStatementNode(expression, statements);
        }
        public Node ParseAssignStatement()
        {
            var variable = ParseVariable(true);
            Require_Special(Lexem.SpecialSymbol.Assign, ":=");
            var expr = TryCast( ParseExpression(), variable.GetNodeType());
            var node = new AssignStatementNode(variable, expr);
            return node;
        }
        public Node ParseProcedureStatement()
        {
            var identifier = ParseIdentifier();
            var list = new List<ExprNode>();
            if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.LParenthese))
            {
                lexer.NextToken();
                list = ParseParameterList();
                Require_Special(Lexem.SpecialSymbol.RParenthese, ")");
            }
            SymProc proc = (SymProc)symTableStack.Get(identifier.value);
            if (proc.params_.Count() != list.Count())
                throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.InvalidArgs));
            for (int i = 0; i < list.Count(); i++)
            {
                TryCast(list[i], proc.params_.GetAt(i).AsVar().type);
            }
            return new ProcedureStatementNode(proc, list);
        }

        //expression
        public ExprNode TryCast(ExprNode expr, SymType type)
        {
            if (expr.GetNodeType() == type) return expr;
            if (expr.GetNodeType() == symTableStack.Get("INTEGER") && type == symTableStack.Get("REAL"))
            {
                expr = new CastNode(expr, symTableStack.Get("REAL").AsType());
                return expr;
            }
            throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.CantCast, expr.GetNodeType().name, type.name));
        }
        public BinOpNode BinOpCast(BinOpNode binOp)
        {
            var ifRealThenHere = binOp.left.GetNodeType() == symTableStack.Get("REAL") ? binOp.left : binOp.right;
            var other = binOp.left.GetNodeType() == symTableStack.Get("REAL") ? binOp.right : binOp.left;
            TryCast(other, ifRealThenHere.GetNodeType());
            if (binOp is RelationalOp)
            {
                binOp.cachedType = symTableStack.Get("BOOLEAN").AsType();
            }
            else
            {
                binOp.cachedType = ifRealThenHere.GetNodeType();
            }
            if (binOp is AdditiveOp || binOp is MultiplicativeOp )
            {
                if (binOp.left.GetNodeType() != symTableStack.Get("INTEGER") && binOp.left.GetNodeType() != symTableStack.Get("REAL") ||
                (binOp.right.GetNodeType() != symTableStack.Get("INTEGER") && binOp.right.GetNodeType() != symTableStack.Get("REAL")))
                    throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.InvalidTypes, binOp.left.GetNodeType().name, binOp.right.GetNodeType().name));
            }
            return binOp;
        }
        public List<ExprNode> ParseParameterList()
        {
            List<ExprNode> parameterList = new List<ExprNode>();
            parameterList.Add(ParseExpression());
            while (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Comma))
            {
                lexer.NextToken();
                parameterList.Add(ParseExpression());
            }
            return parameterList;
        }
        public ExprNode ParseExpression()
        {
            ExprNode left = ParseSimpleExpression();
            Lexem lex = lexer.Token;
            while (lex.Type == Lexem.Types.Relational_Op)
            {
                lexer.NextToken();
                left = new RelationalOp(lex, left, ParseSimpleExpression(), null);
                BinOpCast((BinOpNode)left);
                lex = lexer.Token;
            }
            return left;
        }
        public ExprNode ParseSimpleExpression()
        {
            ExprNode left = ParseTerm();
            Lexem lex = lexer.Token;
            while (lex.Type == Lexem.Types.Additive_Op)
            {
                lexer.NextToken();
                left = new AdditiveOp(lex, left, ParseTerm(), null);
                BinOpCast((BinOpNode)left);
                lex = lexer.Token;
            }
            return left;
        }
        public ExprNode ParseTerm()
        {
            ExprNode left = ParseSignedFactor();
            Lexem lex = lexer.Token;
            while (lex.Type == Lexem.Types.Multiplicative_Op)
            {
                lexer.NextToken();
                left = new MultiplicativeOp(lex, left, ParseSignedFactor(), null);
                BinOpCast((BinOpNode)left);
                lex = lexer.Token;
            }
            return left;
        }
        public ExprNode ParseSignedFactor()
        {
            int sign = 39;

            while (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Plus) || lexer.Token.Value.Equals(Lexem.SpecialSymbol.Minus))
            {
                if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Minus))
                {
                    sign *= -1;
                }
                else
                {
                    sign *= 1;
                }
                sign = Math.Sign(sign);
                lexer.NextToken();
            }
            var factor = ParseFactor();
            if (sign != 39 && (factor.GetNodeType() != symTableStack.Get("INTEGER") && factor.GetNodeType() != symTableStack.Get("REAL")))
            {
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.InvalidAction, sign > 0 ? "+" : "-"));
            }
            return new SignedFactorNode(sign == 0 ? 1 : sign, factor, factor.GetNodeType());
        }
        public ExprNode ParseFactor()
        {
            Lexem lex = lexer.Token;
            if (lex.Value.Equals(Lexem.KeyWord.NOT))
            {
                lexer.NextToken();
                var factor = ParseFactor();
                if (factor.GetNodeType() != symTableStack.Get("BOOLEAN"))
                    throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.InvalidAction, "NOT"));
                return new NotFactorNode(factor, factor.GetNodeType());
            }
            if (lex.Value.Equals(Lexem.KeyWord.TRUE))
            {
                lexer.NextToken();
                return new BoolNode(true, (SymBool)symTableStack.Get("BOOLEAN"));
            }
            if (lex.Value.Equals(Lexem.KeyWord.FALSE))
            {
                lexer.NextToken();
                return new BoolNode(false, (SymBool)symTableStack.Get("BOOLEAN"));
            }
            if (lex.Value.Equals(Lexem.SpecialSymbol.At))
            {
                return ParseVariable();
            }
            if (lex.Value.Equals(Lexem.SpecialSymbol.LParenthese))
            {
                lexer.NextToken();
                ExprNode exp = ParseExpression();
                if (lexer.Token.Value == null || !(lexer.Token.Value.Equals(Lexem.SpecialSymbol.RParenthese)))
                    throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, ")"));
                lexer.NextToken();
                return exp;
            }
            switch (lex.Type)
            {
                case Lexem.Types.UInteger:
                    lexer.NextToken();
                    return new IntNode((int)lex.Value, (SymInteger)symTableStack.Get("INTEGER"));

                case Lexem.Types.Identifier:
                    if (symTableStack.Get(lexer.Token.Value.ToString()) is SymFunc)
                        return ParseFunctionCall();
                    return ParseVariable();

                default:
                    return ParseUConst();
            }
        }
        public ExprNode ParseFunctionCall()
        {
            var identifier = ParseIdentifier();
            var list = new List<ExprNode>();
            if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.LParenthese))
            {
                lexer.NextToken();
                list = ParseParameterList();
                Require_Special(Lexem.SpecialSymbol.RParenthese, ")");
            }
            SymFunc func = (SymFunc)symTableStack.Get(identifier.value);
             if(func.params_.Count() != list.Count())
                throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.InvalidArgs));
            for (int i=0; i< list.Count(); i++)
            {
                if(list[i].GetNodeType() != func.params_.GetAt(i).AsVar().type)
                    throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.InvalidArgs));
            }
            return new FunctionCallNode(func, list);
        }


        

        //types
        public SymType ParseType()
        {
            if (lexer.Token.Value.Equals(Lexem.KeyWord.ARRAY))
                return ParseArrayType();
            if (lexer.Token.Value.Equals(Lexem.KeyWord.RECORD))
                return ParseRecordType();
            return ParseTypeIdentifier();
        }
        public List<SubrangeTypeNode> ParseSubrangeList()
        {
            List<SubrangeTypeNode> list = new List<SubrangeTypeNode>();
            list.Add(ParseSubrangeType());
            while (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Comma))
            {
                lexer.NextToken();
                list.Add(ParseSubrangeType());
            }
            return list;
        }
        public SymRecord ParseRecordType()
        {
            Require_KeyWord(Lexem.KeyWord.RECORD);
            var table = ParseFieldList();
            Require_KeyWord(Lexem.KeyWord.END);
            return new SymRecord(null, table);
        }
        public SymTable ParseFieldList()
        {
            var table = ParseRecordSection();
            while (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Semicolon))
            {
                lexer.NextToken();
                table.AddRange(ParseRecordSection());
            }
            return table;
        }
        public SymTable ParseRecordSection()
        {
            List<IdentifierNode> left = ParseIdentifierList();
            Require_Special(Lexem.SpecialSymbol.Colon, ":");
            SymType right = ParseType();
            var table = new SymTable();
            foreach (var item in left)
            {
                table.Add(new SymVar(item.GetStrVal(), right));
            }
            return table;
        }
        public List<IdentifierNode> ParseIdentifierList()
        {
            List<IdentifierNode> identifierList = new List<IdentifierNode>();
            identifierList.Add(ParseIdentifier());
            while (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Comma))
            {
                lexer.NextToken();
                identifierList.Add(ParseIdentifier());
            }
            return identifierList;
        }
        public SymArray ParseArrayType()
        {
            Require_KeyWord(Lexem.KeyWord.ARRAY);
            Require_Special(Lexem.SpecialSymbol.LBracket, "[");
            var list = ParseSubrangeList();
            Require_Special(Lexem.SpecialSymbol.RBracket, "]");
            Require_KeyWord(Lexem.KeyWord.OF);
            SymType type = ParseType();
            for(int i=list.Count-1; i>=0; i--)
            {
                type = new SymArray(null, type, (int)list[i].initial.getVal(), (int)list[i].final.getVal());
            }
            return type.AsArray();
        }
        public SubrangeTypeNode ParseSubrangeType()
        {
            var left = lexer.NextToken();
            if (left.Type != Lexem.Types.UInteger)
            {
                throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "UInteger"));
            }
            Require_Special(Lexem.SpecialSymbol.DotDot, "..");
            var right = lexer.NextToken();
            if (left.Type != Lexem.Types.UInteger)
            {
                throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "UInteger"));
            }
            return new SubrangeTypeNode(new IntNode((int)left.Value, (SymInteger)symTableStack.Get("INTEGER")), new IntNode((int)right.Value, (SymInteger)symTableStack.Get("INTEGER")));
        }
        public SymType ParseTypeIdentifier()
        {
            var lex = lexer.NextToken();
            return symTableStack.Get(lex.Value.ToString()).AsType();
        }
    }
}
