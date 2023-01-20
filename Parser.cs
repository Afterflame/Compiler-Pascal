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
            if (indent == "")
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
            foreach (Node node in list)
            {
                if (node.Equals(list.Last()))
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
        public void Throw_OtherValueOfLexer_KeyWord(object value)
        {
            if (!lexer.Token.Value.Equals(value))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, value.ToString()));
            lexer.NextToken();
        }
        public void Throw_OtherValueOfLexer_Special(object value, string msg)
        {
            if (!lexer.Token.Value.Equals(value))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, msg));
            lexer.NextToken();
        }


        public Node ParseIdentifier()
        {
            if (lexer.Token.Type != Lexem.Types.Identifier)
                throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "Identifier"));
            var node = new IdentifierNode(lexer.Token);
            lexer.NextToken();
            return node;
        }
        public Node ParseConst()
        {
            if (lexer.Token.Value.Equals(Lexem.KeyWord.CHR))
            {
                return new UConstNode(ParseConstChr());
            }
            if (lexer.Token.Type == Lexem.Types.Literal)
            {
                var lex = lexer.Token;
                lexer.NextToken();
                return new UConstNode(new StringNode(lex));
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
                return new ConstNode(new VariableNode(false, ParseIdentifier()), sign);
            }
            throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "UConstatant"));
        }
        public Node ParseUConst()
        {
            if (lexer.Token.Value.Equals(Lexem.KeyWord.CHR))
            {
                return new UConstNode(ParseConstChr());
            }
            if (lexer.Token.Type == Lexem.Types.Literal)
            {
                var lex = lexer.Token;
                lexer.NextToken();
                return new UConstNode(new StringNode(lex));
            }
            if (lexer.Token.Type == Lexem.Types.UReal || lexer.Token.Type == Lexem.Types.UInteger)
            {
                var lex = lexer.Token;
                lexer.NextToken();
                return new UConstNode(new NumberNode(lex));
            }
            if (lexer.Token.Value.Equals(Lexem.KeyWord.NIL))
            {
                lexer.NextToken();
                return new UConstNode(new NILNode());
            }
            throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "UConstatant"));
        }
        public Node ParseConstChr()
        {
            Throw_OtherValueOfLexer_KeyWord(Lexem.KeyWord.CHR);
            Throw_OtherValueOfLexer_Special(Lexem.SpecialSymbol.LParenthese, "(");
            Lexem lex = lexer.Token;
            if (!lexer.Token.Type.Equals(Lexem.Types.UInteger))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "Unsigned Int"));
            lexer.NextToken();
            Throw_OtherValueOfLexer_Special(Lexem.SpecialSymbol.RParenthese, ")");
            return new ConstChrNode(lex);
        }
        public Node ParseVariable()
        {
            bool isRef = false;
            if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.At))
            {
                isRef = true;
                lexer.NextToken();
            }
            var node = ParseIdentifier();
            List<Node> args = new List<Node>();
            while (lexer.Token.Value.Equals(Lexem.SpecialSymbol.LBracket) || lexer.Token.Value.Equals(Lexem.SpecialSymbol.Dot))
            {
                if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.LBracket))
                {
                    List<Node> list = new List<Node>();
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
                    lexer.NextToken();
                    args.Add(new ListNode("variable arg", list));
                }
                if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Dot))
                {
                    lexer.NextToken();
                    args.Add(new VariableNode(false, ParseIdentifier()));
                    lexer.NextToken();
                }

            }
            return new VariableNode(isRef, node, args);
        }
        public Node ParseSet()
        {
            Throw_OtherValueOfLexer_Special(Lexem.SpecialSymbol.LBracket, "[");
            List<Node> variables = new List<Node>();
            variables.Add(ParseVariable());
            while (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Comma))
            {
                lexer.NextToken();
                variables.Add(ParseElement());
            }
            Throw_OtherValueOfLexer_Special(Lexem.SpecialSymbol.RBracket, "]");
            var statement = ParseStatement();
            return new WhileStatementNode(new ListNode("variables", variables), statement);
        }
        public Node ParseElement()
        {
            Node left = ParseExpression();
            Node right = null;
            if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.DotDot))
            {
                lexer.NextToken();
                right = ParseExpression();
            }
            return new ElementNode(left, right);
        }


        //statements
        public Node ParseCompoundStatement()
        {
            Throw_OtherValueOfLexer_KeyWord(Lexem.KeyWord.BEGIN);
            var statements = ParseStatements();
            Throw_OtherValueOfLexer_KeyWord(Lexem.KeyWord.END);
            return statements;
        }
        public Node ParseStatements()
        {
            List<Node> statements = new List<Node>();
            statements.Add(ParseStatement());
            while (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Semicolon))
            {
                lexer.NextToken();
                ParseStatement();
            }
            return new ListNode("statements", statements);
        }
        public Node ParseStatement()
        {
            Node label = null;
            if (lexer.Token.Type == Lexem.Types.UInteger)
            {
                label = new NumberNode(lexer.Token);
                lexer.NextToken();
                Throw_OtherValueOfLexer_Special(Lexem.SpecialSymbol.Colon, ":");
            }
            Node statement;
            if (lexer.Token.Type == Lexem.Types.KeyWord)
                statement = ParseCompoundStatement();
            else
                statement = ParseSimpleStatement();
            if (label != null)
                return new LabelStatementNode(label, statement);
            return statement;
        }
        public Node ParseStructuredStatement()
        {
            switch(lexer.Token.Value)
            {
                case Lexem.KeyWord.BEGIN:
                    return ParseCompoundStatement();
                case Lexem.KeyWord.IF:
                    return ParseIfStatement();
                case Lexem.KeyWord.WITH:
                    return ParseWithStatement();
                case Lexem.KeyWord.FOR:
                    return ParseForStatement();
                case Lexem.KeyWord.WHILE:
                    return ParseWhileStatement();
                case Lexem.KeyWord.REPEAT:
                    return ParseRepeatStatement();
                default:
                    throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "Relevant Keyword"));
            }
        }
        public Node ParseIfStatement()
        {
            Throw_OtherValueOfLexer_KeyWord(Lexem.KeyWord.IF);
            var expression = ParseExpression();
            Throw_OtherValueOfLexer_KeyWord(Lexem.KeyWord.THEN);
            var statement = ParseStatement();
            Node altStatement = null;
            if (lexer.Token.Value.Equals(Lexem.KeyWord.ELSE)) {
                altStatement = ParseStatement();
            }
            return new IfStatementNode(expression, statement, altStatement);
        }
        public Node ParseWithStatement()
        {
            Throw_OtherValueOfLexer_KeyWord(Lexem.KeyWord.WITH);
            List<Node> variables = new List<Node>();
            variables.Add(ParseVariable());
            while (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Comma))
            {
                lexer.NextToken();
                variables.Add(ParseVariable());
            }
            Throw_OtherValueOfLexer_KeyWord(Lexem.KeyWord.DO);
            var statement = ParseStatement();
            return new WhileStatementNode(new ListNode("variables", variables), statement);
        }
        public Node ParseForStatement()
        {
            Throw_OtherValueOfLexer_KeyWord(Lexem.KeyWord.FOR);
            var identifier = ParseIdentifier();
            Throw_OtherValueOfLexer_Special(Lexem.SpecialSymbol.Assign, ":=");
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
            Throw_OtherValueOfLexer_KeyWord(Lexem.KeyWord.DO);
            var statement = ParseStatement();
            return new ForStatementNode(identifier, initial, final, statement, dir);
        }
        public Node ParseWhileStatement()
        {
            Throw_OtherValueOfLexer_KeyWord(Lexem.KeyWord.WHILE);
            var expression = ParseExpression();
            Throw_OtherValueOfLexer_KeyWord(Lexem.KeyWord.DO);
            var statement = ParseStatement();
            return new WhileStatementNode(expression, statement);
    }
        public Node ParseRepeatStatement()
        {
            Throw_OtherValueOfLexer_KeyWord(Lexem.KeyWord.REPEAT);
            var statements = ParseStatements();
            Throw_OtherValueOfLexer_KeyWord(Lexem.KeyWord.UNTIL);
            var expression = ParseExpression();
            return new RepeatStatementNode(expression, statements);
        }
        public Node ParseSimpleStatement()
        {
            lexer.SaveState();
            try
            {
                return ParseAssignStatement();
            }
            catch
            {
                lexer.RestoreState();
            }
            try
            {
                return ParseProcedureStatement();
            }
            catch
            {
                lexer.RestoreState();
            }
            return new EmptyStatementNode();
        }
        public Node ParseAssignStatement()
        {
            var variable = ParseVariable();
            Throw_OtherValueOfLexer_Special(Lexem.SpecialSymbol.Assign, ":=");
            var expr = ParseExpression();
            return new AssignStatementNode(variable, expr);
        }
        public Node ParseProcedureStatement()
        {
            var node = ParseIdentifier();
            if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.LParenthese))
            {
                lexer.NextToken();
                var ans = new ProcedureStatementNode(node, ParseParameterList());
                Throw_OtherValueOfLexer_Special(Lexem.SpecialSymbol.RParenthese, ")");
                return ans;
            }
            return new ProcedureStatementNode(node);
        }

        //expression
        public Node ParseParameterList()
        {
            List<Node> parameterList = new List<Node>();
            parameterList.Add(ParseExpression());
            while (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Comma))
            {
                    lexer.NextToken();
                    parameterList.Add(ParseExpression());
            }
            return new ListNode("Parameters", parameterList);
        }
        public Node ParseExpression()
        {
            Node left = ParseSimpleExpression();
            Lexem lex = lexer.Token;
            while (lex.Type == Lexem.Types.Relational_Op)
            {
                lexer.NextToken();
                left = new RelationalOp(lex, left, ParseSimpleExpression());
                lex = lexer.Token;
            }
            return left;
        }
        public Node ParseSimpleExpression()
        {
            Node left = ParseTerm();
            Lexem lex = lexer.Token;
            while (lex.Type == Lexem.Types.Additive_Op)
            {
                lexer.NextToken();
                left = new BinOpNode(lex, left, ParseTerm());
                lex = lexer.Token;
            }
            return left;
        }
        public Node ParseTerm()
        {
            Node left = ParseSignedFactor();
            Lexem lex = lexer.Token;
            while (lex.Type == Lexem.Types.Multiplicative_Op)
            {
                lexer.NextToken();
                left = new BinOpNode(lex, left, ParseSignedFactor());
                lex = lexer.Token;
            }
            return left;
        }
        public Node ParseSignedFactor()
        {
            int sign = 1;
            while (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Plus)|| lexer.Token.Value.Equals(Lexem.SpecialSymbol.Minus))
            {
                if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Minus))
                {
                    sign = -1;
                }
                lexer.NextToken();
            }
            return new SignedFactorNode(sign, ParseFactor());
        }
        public Node ParseFactor()
        {
            Lexem lex = lexer.Token;
            if(lex.Value==null)
                throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "Factor"));
            if (lex.Value.Equals(Lexem.KeyWord.NOT))
            {
                lexer.NextToken();
                return new NotFactorNode(ParseFactor());
            }
            if (lex.Value.Equals(Lexem.KeyWord.TRUE))
            {
                lexer.NextToken();
                return new BoolNode(true);
            }
            if (lex.Value.Equals(Lexem.KeyWord.FALSE))
            {
                lexer.NextToken();
                return new BoolNode(false);
            }
            if (lex.Value.Equals(Lexem.SpecialSymbol.At))
            {
                return ParseVariable();
            }
            if (lex.Value.Equals(Lexem.SpecialSymbol.LBracket))
            {
                lexer.NextToken();
                return ParseSet();
            }
            if (lex.Value.Equals(Lexem.SpecialSymbol.LParenthese))
            {
                lexer.NextToken();
                Node exp = ParseExpression();
                if (lexer.Token.Value == null || !(lexer.Token.Value.Equals(Lexem.SpecialSymbol.RParenthese)))
                    throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, ")"));
                lexer.NextToken();
                return exp;
            }
            switch (lex.Type)
            {
                case Lexem.Types.UInteger:
                    lexer.NextToken();
                    return new NumberNode(lex);

                case Lexem.Types.Identifier:
                    lexer.SaveState();
                    try
                    {
                        return ParseFunctionDesignator();
                    }
                    catch
                    {
                        lexer.RestoreState();
                        try
                        {
                            return ParseVariable();
                        }
                        catch
                        {
                            lexer.RestoreState();
                        }
                    }
                    throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "Function or Variable"));

                default:
                    try
                    {
                        return ParseUConst();
                    }
                    catch
                    {
                        throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "Factor"));
                    }
            }
        }
        public Node ParseFunctionDesignator()
        {
            var node = ParseIdentifier();
            Throw_OtherValueOfLexer_Special(Lexem.SpecialSymbol.LParenthese, "(");
            var list = ParseParameterList();
            Throw_OtherValueOfLexer_Special(Lexem.SpecialSymbol.RParenthese, ")");
            return new FunctionDesignatorNode(node, list);
        }


        //Def's
        public Node ParseConstDef()
        {
            Throw_OtherValueOfLexer_KeyWord(Lexem.KeyWord.CONST);
            var identifier = ParseIdentifier();
            Throw_OtherValueOfLexer_Special(Lexem.SpecialSymbol.Equal, "=");
            var answer = new ConstDefinitionNode(new VariableNode(false, identifier), ParseConst());
            Throw_OtherValueOfLexer_Special(Lexem.SpecialSymbol.Semicolon, ";");
            return answer;
        }
        public Node ParseVariableDeclPart()
        {
            Throw_OtherValueOfLexer_KeyWord(Lexem.KeyWord.VAR);
            var list = new List<Node>(); 
            list.Add(ParseVariableDecl());
            while (true)
            {
                lexer.SaveState();
                Throw_OtherValueOfLexer_Special(Lexem.SpecialSymbol.Semicolon, ";");
                try
                {
                    list.Add(ParseVariableDecl());
                }
                catch
                {
                    lexer.RestoreState();
                    return new ListNode("VariableDecls", list);
                }
            }
        }
        public Node ParseVariableDecl()
        {
            var list = ParseIdentifierList();
            Throw_OtherValueOfLexer_Special(Lexem.SpecialSymbol.Colon, ":");
            return new VariableDeclNode(list, ParseType());
        }


        //types
        public Node ParseSimpleType()
        {
            lexer.SaveState();
            try
            {
                return ParseScalarType();
            }
            catch
            {
                lexer.RestoreState();
            }
            try
            {
                return ParseSubrangeType();
            }
            catch
            {
                lexer.RestoreState();
            }
            try
            {
                return ParseStringType();
            }
            catch
            {
                lexer.RestoreState();
            }
            try
            {
                return ParseTypeIdentifier();
            }
            catch
            {
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "SimpleType"));
            }
        }
        public Node ParseStringType()
        {
            Throw_OtherValueOfLexer_KeyWord(Lexem.KeyWord.STRING);
            Throw_OtherValueOfLexer_Special(Lexem.SpecialSymbol.LBracket, "[");
            var lex = lexer.Token;
            if ((lexer.Token.Type != Lexem.Types.Identifier) && (lexer.Token.Type != Lexem.Types.UInteger))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "Identifier or UInt"));
            Node answer;
            if (lexer.Token.Type == Lexem.Types.Identifier)
                answer = new IdentifierNode(lex);
            else
                answer = new NumberNode(lex);
            lexer.NextToken();
            Throw_OtherValueOfLexer_Special(Lexem.SpecialSymbol.RBracket, "]");
            return new StringTypeNode(answer);
        }
        public Node ParseFixedPart()
        {
            List<Node> parameterList = new List<Node>();
            parameterList.Add(ParseRecordSection());
            while (!lexer.Token.Value.Equals(Lexem.SpecialSymbol.Comma))
            {
                    lexer.NextToken();
                    parameterList.Add(ParseRecordSection());
            }
            return new ListNode("Parameters", parameterList);
        }
        public Node ParseType()
        {
            if (lexer.Token.Value.Equals(Lexem.KeyWord.ARRAY))
                return ParseArrayType();
            if (lexer.Token.Value.Equals(Lexem.KeyWord.RECORD))
                return ParseRecordSection();
            return ParseSimpleType();
        }
        public Node ParseTypeList()
        {
            List<Node> list = new List<Node>();
            list.Add(ParseIdentifier());
            while (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Comma))
            {
                lexer.NextToken();
                list.Add(ParseSimpleType());
            }
            return new ListNode("Types", list);
        }
        public Node ParseFieldList()
        {
            List<Node> list = new List<Node>();
            list.Add(ParseRecordSection());
            while (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Semicolon))
            {
                lexer.NextToken();
                list.Add(ParseRecordSection());
            }
            return new ListNode("RecordSections", list);
        }
        public Node ParseIdentifierList()
        {
            List<Node> identifierList = new List<Node>();
            identifierList.Add(ParseIdentifier());
            while (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Comma))
            {
                lexer.NextToken();
                identifierList.Add(ParseIdentifier());
            }
            return new ListNode("Identifiers", identifierList);
        }
        public Node ParseArrayType()
        {
            Throw_OtherValueOfLexer_KeyWord(Lexem.KeyWord.ARRAY);
            Throw_OtherValueOfLexer_Special(Lexem.SpecialSymbol.LBracket, "[");
            var list = ParseTypeList();
            Throw_OtherValueOfLexer_Special(Lexem.SpecialSymbol.RBracket, "]");
            Throw_OtherValueOfLexer_KeyWord(Lexem.KeyWord.OF);
            return new ArrayTypeNode(ParseType(), list);
        }
        public Node ParseScalarType()
        {
            Throw_OtherValueOfLexer_Special(Lexem.SpecialSymbol.LParenthese, "(");
            var identifierList = ParseIdentifierList();
            Throw_OtherValueOfLexer_Special(Lexem.SpecialSymbol.RParenthese, ")");
            return new ScalarTypeNode(identifierList);
        }
        public Node ParseSubrangeType()
        {
            var left = ParseConst();
            Throw_OtherValueOfLexer_Special(Lexem.SpecialSymbol.DotDot, "..");
            var right = ParseConst();
            return new SubrangeTypeNode(left, right);
        }
        public Node ParseRecordType()
        {
            Throw_OtherValueOfLexer_KeyWord(Lexem.KeyWord.RECORD);
            var list = ParseFieldList();
            Throw_OtherValueOfLexer_KeyWord(Lexem.KeyWord.END);
            return new RecordTypeNode(list);
        }
        public Node ParseRecordSection()
        {
            Node left = ParseIdentifierList();
            Throw_OtherValueOfLexer_Special(Lexem.SpecialSymbol.Colon, ":");
            Node right = ParseType();
            return new RecordSectionNode(left, right);
        }
        public Node ParseTypeIdentifier()
        {
            if (lexer.Token.Type == Lexem.Types.Identifier)
            {
                var node = new IdentifierNode(lexer.Token);
                lexer.NextToken();
                return new TypeIdentifierNode(Lexem.Types.Identifier, node);
            }
            switch (lexer.Token.Value)
            {
                case Lexem.KeyWord.BOOLEAN:
                    lexer.NextToken();
                    return new TypeIdentifierNode(Lexem.KeyWord.BOOLEAN);
                case Lexem.KeyWord.CHAR:
                    lexer.NextToken();
                    return new TypeIdentifierNode(Lexem.KeyWord.CHAR);
                case Lexem.KeyWord.INTEGER:
                    lexer.NextToken();
                    return new TypeIdentifierNode(Lexem.KeyWord.INTEGER);
                case Lexem.KeyWord.REAL:
                    lexer.NextToken();
                    return new TypeIdentifierNode(Lexem.KeyWord.REAL);
                case Lexem.KeyWord.STRING:
                    lexer.NextToken();
                    return new TypeIdentifierNode(Lexem.KeyWord.STRING);
                default:
                    throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "TypeIdentifier"));
            }
        }
    }
}
