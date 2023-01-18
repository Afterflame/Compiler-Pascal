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
                var lex = lexer.Token;
                lexer.NextToken();
                return new ConstNode(new VariableNode(false, new IdentifierNode(lex)), sign);
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
            if (!lexer.Token.Value.Equals(Lexem.KeyWord.CHR))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "CHR"));
            lexer.NextToken();
            if (!lexer.Token.Value.Equals(Lexem.SpecialSymbol.LParenthese))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "("));
            lexer.NextToken();
            Lexem lex = lexer.Token;
            if (!lexer.Token.Type.Equals(Lexem.Types.UInteger))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "Unsigned Int"));
            lexer.NextToken();
            if (!lex.Value.Equals(Lexem.SpecialSymbol.RParenthese))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, ")"));
            lexer.NextToken();
            return new ConstChrNode(lex);
        }
        public Node ParseConstDef()
        {
            var identifier = lexer.Token;
            if (identifier.Type != Lexem.Types.Identifier)
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "Identifier"));
            lexer.NextToken();
            if (!lexer.Token.Value.Equals(Lexem.SpecialSymbol.Equal))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "="));
            lexer.NextToken();
            return new ConstDefinitionNode(new VariableNode(false, new IdentifierNode(identifier)), ParseUConst());
        }
        /*public Node ParseFunctionType()
        {
            if (!lexer.Token.Value.Equals(Lexem.KeyWord.FUNCTION))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "FUNCTION"));
            lexer.NextToken();
            if (!lexer.Token.Value.Equals(Lexem.SpecialSymbol.Equal))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "="));
            lexer.NextToken();
            return new ConstDefinitionNode(new VariableNode(identifier), ParseConst());
        }*/
        public Node ParseCompoundStatement()
        {
            if (!lexer.Token.Value.Equals(Lexem.KeyWord.BEGIN))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "BEGIN"));
            lexer.NextToken();
            var statements = ParseStatements();
            if (!lexer.Token.Value.Equals(Lexem.KeyWord.END))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "END"));
            lexer.NextToken();
            return statements;
        }
        public Node ParseStatements()
        {
            List<Node> statements = new List<Node>();
            statements.Add(ParseStatement());
            while (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Semicolon))
            {
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
                if (!lexer.Token.Value.Equals(Lexem.SpecialSymbol.Colon))
                    throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, ":"));
                lexer.NextToken();
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
            if (!lexer.Token.Value.Equals(Lexem.KeyWord.IF))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "IF"));
            lexer.NextToken();
            var expression = ParseExpression();
            if (!lexer.Token.Value.Equals(Lexem.KeyWord.THEN))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "THEN"));
            lexer.NextToken();
            var statement = ParseStatement();
            Node altStatement = null;
            if (lexer.Token.Value.Equals(Lexem.KeyWord.ELSE)) {
                altStatement = ParseStatement();
            }
            return new IfStatementNode(expression, statement, altStatement);
        }
        public Node ParseWithStatement()
        {
            return new EmptyStatementNode();
        }
        public Node ParseForStatement()
        {
            if (!lexer.Token.Value.Equals(Lexem.KeyWord.FOR))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "IF"));
            lexer.NextToken();
            if (lexer.Token.Type!=Lexem.Types.Identifier)
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "Identifier"));
            var identifier = new IdentifierNode(lexer.Token);
            lexer.NextToken();
            if (!lexer.Token.Value.Equals(Lexem.SpecialSymbol.Assign))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, ":="));
            lexer.NextToken();
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
            if (!lexer.Token.Value.Equals(Lexem.KeyWord.DO))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "DO"));
            lexer.NextToken();
            var statement = ParseStatement();
            return new ForStatementNode(identifier, initial, final, statement, dir);
        }
        public Node ParseWhileStatement()
        {
            return new EmptyStatementNode();
        }
        public Node ParseRepeatStatement()
        {
            return new EmptyStatementNode();
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
            if (!lexer.Token.Value.Equals(Lexem.SpecialSymbol.Assign))
                throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, ":="));
            var expr = ParseExpression();
            return new AssignStatementNode(variable, expr);
        }
        public Node ParseProcedureStatement()
        {
            if (lexer.Token.Type != Lexem.Types.Identifier)
                throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "Identifier"));
            var lex = lexer.Token;
            lexer.NextToken();
            if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.LParenthese))
                return new ProcedureStatementNode(new IdentifierNode(lex), ParseParameterList());
            return new ProcedureStatementNode(new IdentifierNode(lex));
        }
        public Node ParseParameterList()
        {
            if (!lexer.Token.Value.Equals(Lexem.SpecialSymbol.LParenthese))
                throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "("));
            List<Node> parameterList = new List<Node>();
            parameterList.Add(ParseExpression());
            while (!lexer.Token.Value.Equals(Lexem.SpecialSymbol.RParenthese))
            {
                if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Comma))
                {
                    lexer.NextToken();
                    parameterList.Add(ParseExpression());
                }
                else throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, ")"));
            }
            return new ListNode("Parameters", parameterList);
        }
        public Node ParseConstDefPart()
        {
            if (!lexer.Token.Value.Equals(Lexem.KeyWord.CONST))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "CONST"));
            lexer.NextToken();
            var answer = new ConstDefinitionPartNode(ParseConstDef());
            if (!lexer.Token.Value.Equals(Lexem.SpecialSymbol.Semicolon))
                throw new ArgumentException(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, ";"));
            lexer.NextToken();
            return answer;
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
                        return ParseFunction();
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
        public Node ParseFunction()
        {
            if (lexer.Token.Type != Lexem.Types.Identifier)
                throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "Identifier"));
            var lex = lexer.Token;
            lexer.NextToken();
            if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.LParenthese))
                throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "("));
            lexer.NextToken();
            List<Node> result = new List<Node>();
            result.Add(ParseExpression());
            while (!lexer.Token.Value.Equals(Lexem.SpecialSymbol.RParenthese))
            {
                if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Comma))
                {
                    lexer.NextToken();
                    result.Add(ParseExpression());
                }
                else throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, ")"));
            }
            return new FunctionDesignatorNode(new IdentifierNode(lex), new ListNode("parametres", result));
        }
        public Node ParseSet()
        {
            return null;
        }
        public Node ParseProcedureStatement(Lexem inputIdentifier = null)
        {

            var lex = lexer.Token;
            if (inputIdentifier != null)
            {
                lex = inputIdentifier;
                goto objecttion;
            }
            if (lexer.Token.Type != Lexem.Types.Identifier)
                throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "Identifier"));
            lexer.NextToken();
            objecttion:
            if (!lexer.Token.Value.Equals(Lexem.SpecialSymbol.LParenthese))
                return new ProcedureStatementNode(new VariableNode(false, new IdentifierNode(lex)));
            lexer.NextToken();
            List<Node> result = new List<Node>();
            result.Add(ParseExpression());
            while (!lexer.Token.Value.Equals(Lexem.SpecialSymbol.RParenthese))
            {
                if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.Comma))
                {
                    lexer.NextToken();
                    result.Add(ParseExpression());
                }
                else throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, ")"));
            }
            return new ProcedureStatementNode(new VariableNode(false, new IdentifierNode(lex)), new ParameterListNode(result));
        }
        public Node ParseVariable(Lexem inputIdentifier = null)
        {
            Lexem lex;
            bool isRef = false;
            if (inputIdentifier != null)
            {
                lex = inputIdentifier;
                goto objection;
            }
            if (lexer.Token.Value.Equals(Lexem.SpecialSymbol.At))
            {
                isRef = true;
                lexer.NextToken();
            }
            lex = lexer.Token;
            if (lexer.Token.Type!= Lexem.Types.Identifier)
                throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.XExpexted, "Variable"));
            lexer.NextToken();
            objection:;
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
                    if (lexer.Token.Type != Lexem.Types.Identifier)
                        throw new Exception(ErrorConstructor.GetPositionMassage(lexer.Line, lexer.Idx, Error.InvalidArgs));
                    args.Add(new VariableNode(false, new IdentifierNode(lexer.Token)));
                    lexer.NextToken();
                }

            }
            return new VariableNode(isRef, new IdentifierNode(lex), args);
        }
    }
}
