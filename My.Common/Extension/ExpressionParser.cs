using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace My.Common.Extension
{
    internal class ExpressionParser
    {
        private struct Token
        {
            public TokenId id;

            public string text;

            public int pos;
        }

        private enum TokenId
        {
            Unknown,
            End,
            Identifier,
            StringLiteral,
            IntegerLiteral,
            RealLiteral,
            Exclamation,
            Percent,
            Amphersand,
            OpenParen,
            CloseParen,
            Asterisk,
            Plus,
            Comma,
            Minus,
            Dot,
            Slash,
            Colon,
            LessThan,
            Equal,
            GreaterThan,
            Question,
            OpenBracket,
            CloseBracket,
            Bar,
            ExclamationEqual,
            DoubleAmphersand,
            LessThanEqual,
            LessGreater,
            DoubleEqual,
            GreaterThanEqual,
            DoubleBar
        }

        private interface ILogicalSignatures
        {
            void F(bool x, bool y);

            void F(bool? x, bool? y);
        }

        private interface IArithmeticSignatures
        {
            void F(int x, int y);

            void F(uint x, uint y);

            void F(long x, long y);

            void F(ulong x, ulong y);

            void F(float x, float y);

            void F(double x, double y);

            void F(decimal x, decimal y);

            void F(int? x, int? y);

            void F(uint? x, uint? y);

            void F(long? x, long? y);

            void F(ulong? x, ulong? y);

            void F(float? x, float? y);

            void F(double? x, double? y);

            void F(decimal? x, decimal? y);
        }

        private interface IRelationalSignatures : IArithmeticSignatures
        {
            void F(string x, string y);

            void F(char x, char y);

            void F(DateTime x, DateTime y);

            void F(TimeSpan x, TimeSpan y);

            void F(char? x, char? y);

            void F(DateTime? x, DateTime? y);

            void F(TimeSpan? x, TimeSpan? y);
        }

        private interface IEqualitySignatures : IRelationalSignatures, IArithmeticSignatures
        {
            void F(bool x, bool y);

            void F(bool? x, bool? y);
        }

        private interface IAddSignatures : IArithmeticSignatures
        {
            void F(DateTime x, TimeSpan y);

            void F(TimeSpan x, TimeSpan y);

            void F(DateTime? x, TimeSpan? y);

            void F(TimeSpan? x, TimeSpan? y);
        }

        private interface ISubtractSignatures : IAddSignatures, IArithmeticSignatures
        {
            void F(DateTime x, DateTime y);

            void F(DateTime? x, DateTime? y);
        }

        private interface INegationSignatures
        {
            void F(int x);

            void F(long x);

            void F(float x);

            void F(double x);

            void F(decimal x);

            void F(int? x);

            void F(long? x);

            void F(float? x);

            void F(double? x);

            void F(decimal? x);
        }

        private interface INotSignatures
        {
            void F(bool x);

            void F(bool? x);
        }

        private interface IEnumerableSignatures
        {
            void Where(bool predicate);

            void Any();

            void Any(bool predicate);

            void All(bool predicate);

            void Count();

            void Count(bool predicate);

            void Min(object selector);

            void Max(object selector);

            void Sum(int selector);

            void Sum(int? selector);

            void Sum(long selector);

            void Sum(long? selector);

            void Sum(float selector);

            void Sum(float? selector);

            void Sum(double selector);

            void Sum(double? selector);

            void Sum(decimal selector);

            void Sum(decimal? selector);

            void Average(int selector);

            void Average(int? selector);

            void Average(long selector);

            void Average(long? selector);

            void Average(float selector);

            void Average(float? selector);

            void Average(double selector);

            void Average(double? selector);

            void Average(decimal selector);

            void Average(decimal? selector);
        }

        private class MethodData
        {
            public MethodBase MethodBase;

            public ParameterInfo[] Parameters;

            public Expression[] Args;
        }

        private static readonly Type[] predefinedTypes = new Type[20]
        {
        typeof(object),
        typeof(bool),
        typeof(char),
        typeof(string),
        typeof(sbyte),
        typeof(byte),
        typeof(short),
        typeof(ushort),
        typeof(int),
        typeof(uint),
        typeof(long),
        typeof(ulong),
        typeof(float),
        typeof(double),
        typeof(decimal),
        typeof(DateTime),
        typeof(TimeSpan),
        typeof(Guid),
        typeof(Math),
        typeof(Convert)
        };

        private static readonly Expression trueLiteral = Expression.Constant(true);

        private static readonly Expression falseLiteral = Expression.Constant(false);

        private static readonly Expression nullLiteral = Expression.Constant(null);

        private static readonly string keywordIt = "it";

        private static readonly string keywordIif = "iif";

        private static readonly string keywordNew = "new";

        private static Dictionary<string, object> keywords;

        private Dictionary<string, object> symbols;

        private IDictionary<string, object> externals;

        private Dictionary<Expression, string> literals;

        private ParameterExpression it;

        private string text;

        private int textPos;

        private int textLen;

        private char ch;

        private Token token;

        public ExpressionParser(ParameterExpression[] parameters, string expression, object[] values)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (ExpressionParser.keywords == null)
            {
                ExpressionParser.keywords = ExpressionParser.CreateKeywords();
            }
            this.symbols = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            this.literals = new Dictionary<Expression, string>();
            if (parameters != null)
            {
                this.ProcessParameters(parameters);
            }
            if (values != null)
            {
                this.ProcessValues(values);
            }
            this.text = expression;
            this.textLen = this.text.Length;
            this.SetTextPos(0);
            this.NextToken();
        }

        private void ProcessParameters(ParameterExpression[] parameters)
        {
            foreach (ParameterExpression pe in parameters)
            {
                if (!string.IsNullOrEmpty(pe.Name))
                {
                    this.AddSymbol(pe.Name, pe);
                }
            }
            if (parameters.Length == 1 && string.IsNullOrEmpty(parameters[0].Name))
            {
                this.it = parameters[0];
            }
        }

        private void ProcessValues(object[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                object value = values[i];
                if (i == values.Length - 1 && value is IDictionary<string, object>)
                {
                    this.externals = (IDictionary<string, object>)value;
                }
                else
                {
                    this.AddSymbol("@" + i.ToString(CultureInfo.InvariantCulture), value);
                }
            }
        }

        private void AddSymbol(string name, object value)
        {
            if (this.symbols.ContainsKey(name))
            {
                throw this.ParseError("The identifier '{0}' was defined more than once", name);
            }
            this.symbols.Add(name, value);
        }

        public Expression Parse(Type resultType)
        {
            int exprPos = this.token.pos;
            Expression expr = this.ParseExpression();
            if (resultType != (Type)null && (expr = this.PromoteExpression(expr, resultType, true)) == null)
            {
                throw this.ParseError(exprPos, "Expression of type '{0}' expected", ExpressionParser.GetTypeName(resultType));
            }
            this.ValidateToken(TokenId.End, "Syntax error");
            return expr;
        }

        public IEnumerable<DynamicOrdering> ParseOrdering()
        {
            List<DynamicOrdering> orderings = new List<DynamicOrdering>();
            while (true)
            {
                bool flag = true;
                Expression expr = this.ParseExpression();
                bool ascending = true;
                if (this.TokenIdentifierIs("asc") || this.TokenIdentifierIs("ascending"))
                {
                    this.NextToken();
                }
                else if (this.TokenIdentifierIs("desc") || this.TokenIdentifierIs("descending"))
                {
                    this.NextToken();
                    ascending = false;
                }
                orderings.Add(new DynamicOrdering
                {
                    Selector = expr,
                    Ascending = ascending
                });
                if (this.token.id == TokenId.Comma)
                {
                    this.NextToken();
                    continue;
                }
                break;
            }
            this.ValidateToken(TokenId.End, "Syntax error");
            return orderings;
        }

        private Expression ParseExpression()
        {
            int errorPos = this.token.pos;
            Expression expr4 = this.ParseLogicalOr();
            if (this.token.id == TokenId.Question)
            {
                this.NextToken();
                Expression expr3 = this.ParseExpression();
                this.ValidateToken(TokenId.Colon, "':' expected");
                this.NextToken();
                Expression expr2 = this.ParseExpression();
                expr4 = this.GenerateConditional(expr4, expr3, expr2, errorPos);
            }
            return expr4;
        }

        private Expression ParseLogicalOr()
        {
            Expression left = this.ParseLogicalAnd();
            while (this.token.id == TokenId.DoubleBar || this.TokenIdentifierIs("or"))
            {
                Token op = this.token;
                this.NextToken();
                Expression right = this.ParseLogicalAnd();
                this.CheckAndPromoteOperands(typeof(ILogicalSignatures), op.text, ref left, ref right, op.pos);
                left = Expression.OrElse(left, right);
            }
            return left;
        }

        private Expression ParseLogicalAnd()
        {
            Expression left = this.ParseComparison();
            while (this.token.id == TokenId.DoubleAmphersand || this.TokenIdentifierIs("and"))
            {
                Token op = this.token;
                this.NextToken();
                Expression right = this.ParseComparison();
                this.CheckAndPromoteOperands(typeof(ILogicalSignatures), op.text, ref left, ref right, op.pos);
                left = Expression.AndAlso(left, right);
            }
            return left;
        }

        private Expression ParseComparison()
        {
            Expression left = this.ParseAdditive();
            while (this.token.id == TokenId.Equal || this.token.id == TokenId.DoubleEqual || this.token.id == TokenId.ExclamationEqual || this.token.id == TokenId.LessGreater || this.token.id == TokenId.GreaterThan || this.token.id == TokenId.GreaterThanEqual || this.token.id == TokenId.LessThan || this.token.id == TokenId.LessThanEqual)
            {
                Token op = this.token;
                this.NextToken();
                Expression right = this.ParseAdditive();
                bool isEquality = op.id == TokenId.Equal || op.id == TokenId.DoubleEqual || op.id == TokenId.ExclamationEqual || op.id == TokenId.LessGreater;
                if (isEquality && !left.Type.IsValueType && !right.Type.IsValueType)
                {
                    if (left.Type != right.Type)
                    {
                        if (!left.Type.IsAssignableFrom(right.Type))
                        {
                            if (right.Type.IsAssignableFrom(left.Type))
                            {
                                left = Expression.Convert(left, right.Type);
                                goto IL_01e1;
                            }
                            throw this.IncompatibleOperandsError(op.text, left, right, op.pos);
                        }
                        right = Expression.Convert(right, left.Type);
                    }
                }
                else if (ExpressionParser.IsEnumType(left.Type) || ExpressionParser.IsEnumType(right.Type))
                {
                    if (left.Type != right.Type)
                    {
                        Expression e2;
                        if ((e2 = this.PromoteExpression(right, left.Type, true)) == null)
                        {
                            if ((e2 = this.PromoteExpression(left, right.Type, true)) != null)
                            {
                                left = e2;
                                goto IL_01e1;
                            }
                            throw this.IncompatibleOperandsError(op.text, left, right, op.pos);
                        }
                        right = e2;
                    }
                }
                else
                {
                    this.CheckAndPromoteOperands(isEquality ? typeof(IEqualitySignatures) : typeof(IRelationalSignatures), op.text, ref left, ref right, op.pos);
                }
                goto IL_01e1;
            IL_01e1:
                switch (op.id)
                {
                    case TokenId.Equal:
                    case TokenId.DoubleEqual:
                        left = this.GenerateEqual(left, right);
                        break;
                    case TokenId.ExclamationEqual:
                    case TokenId.LessGreater:
                        left = this.GenerateNotEqual(left, right);
                        break;
                    case TokenId.GreaterThan:
                        left = this.GenerateGreaterThan(left, right);
                        break;
                    case TokenId.GreaterThanEqual:
                        left = this.GenerateGreaterThanEqual(left, right);
                        break;
                    case TokenId.LessThan:
                        left = this.GenerateLessThan(left, right);
                        break;
                    case TokenId.LessThanEqual:
                        left = this.GenerateLessThanEqual(left, right);
                        break;
                }
            }
            return left;
        }

        private Expression ParseAdditive()
        {
            Expression left = this.ParseMultiplicative();
            while (this.token.id == TokenId.Plus || this.token.id == TokenId.Minus || this.token.id == TokenId.Amphersand)
            {
                Token op = this.token;
                this.NextToken();
                Expression right = this.ParseMultiplicative();
                switch (op.id)
                {
                    case TokenId.Plus:
                        if (!(left.Type == typeof(string)) && !(right.Type == typeof(string)))
                        {
                            this.CheckAndPromoteOperands(typeof(IAddSignatures), op.text, ref left, ref right, op.pos);
                            left = this.GenerateAdd(left, right);
                            break;
                        }
                        goto case TokenId.Amphersand;
                    case TokenId.Minus:
                        this.CheckAndPromoteOperands(typeof(ISubtractSignatures), op.text, ref left, ref right, op.pos);
                        left = this.GenerateSubtract(left, right);
                        break;
                    case TokenId.Amphersand:
                        left = this.GenerateStringConcat(left, right);
                        break;
                }
            }
            return left;
        }

        private Expression ParseMultiplicative()
        {
            Expression left = this.ParseUnary();
            while (this.token.id == TokenId.Asterisk || this.token.id == TokenId.Slash || this.token.id == TokenId.Percent || this.TokenIdentifierIs("mod"))
            {
                Token op = this.token;
                this.NextToken();
                Expression right = this.ParseUnary();
                this.CheckAndPromoteOperands(typeof(IArithmeticSignatures), op.text, ref left, ref right, op.pos);
                switch (op.id)
                {
                    case TokenId.Asterisk:
                        left = Expression.Multiply(left, right);
                        break;
                    case TokenId.Slash:
                        left = Expression.Divide(left, right);
                        break;
                    case TokenId.Identifier:
                    case TokenId.Percent:
                        left = Expression.Modulo(left, right);
                        break;
                }
            }
            return left;
        }

        private Expression ParseUnary()
        {
            if (this.token.id == TokenId.Minus || this.token.id == TokenId.Exclamation || this.TokenIdentifierIs("not"))
            {
                Token op = this.token;
                this.NextToken();
                if (op.id == TokenId.Minus && (this.token.id == TokenId.IntegerLiteral || this.token.id == TokenId.RealLiteral))
                {
                    this.token.text = "-" + this.token.text;
                    this.token.pos = op.pos;
                    return this.ParsePrimary();
                }
                Expression expr = this.ParseUnary();
                if (op.id == TokenId.Minus)
                {
                    this.CheckAndPromoteOperand(typeof(INegationSignatures), op.text, ref expr, op.pos);
                    expr = Expression.Negate(expr);
                }
                else
                {
                    this.CheckAndPromoteOperand(typeof(INotSignatures), op.text, ref expr, op.pos);
                    expr = Expression.Not(expr);
                }
                return expr;
            }
            return this.ParsePrimary();
        }

        private Expression ParsePrimary()
        {
            Expression expr = this.ParsePrimaryStart();
            while (true)
            {
                bool flag = true;
                if (this.token.id == TokenId.Dot)
                {
                    this.NextToken();
                    expr = this.ParseMemberAccess(null, expr);
                }
                else
                {
                    if (this.token.id != TokenId.OpenBracket)
                    {
                        break;
                    }
                    expr = this.ParseElementAccess(expr);
                }
            }
            return expr;
        }

        private Expression ParsePrimaryStart()
        {
            switch (this.token.id)
            {
                case TokenId.Identifier:
                    return this.ParseIdentifier();
                case TokenId.StringLiteral:
                    return this.ParseStringLiteral();
                case TokenId.IntegerLiteral:
                    return this.ParseIntegerLiteral();
                case TokenId.RealLiteral:
                    return this.ParseRealLiteral();
                case TokenId.OpenParen:
                    return this.ParseParenExpression();
                default:
                    throw this.ParseError("Expression expected");
            }
        }

        private Expression ParseStringLiteral()
        {
            this.ValidateToken(TokenId.StringLiteral);
            char quote = this.token.text[0];
            string s = this.token.text.Substring(1, this.token.text.Length - 2);
            int start = 0;
            while (true)
            {
                bool flag = true;
                int i = s.IndexOf(quote, start);
                if (i >= 0)
                {
                    s = s.Remove(i, 1);
                    start = i + 1;
                    continue;
                }
                break;
            }
            if (quote == '\'')
            {
                if (s.Length != 1)
                {
                    throw this.ParseError("Character literal must contain exactly one character");
                }
                this.NextToken();
                return this.CreateLiteral(s[0], s);
            }
            this.NextToken();
            return this.CreateLiteral(s, s);
        }

        private Expression ParseIntegerLiteral()
        {
            this.ValidateToken(TokenId.IntegerLiteral);
            string text = this.token.text;
            if (text[0] != '-')
            {
                ulong value = default(ulong);
                if (!ulong.TryParse(text, out value))
                {
                    throw this.ParseError("Invalid integer literal '{0}'", text);
                }
                this.NextToken();
                if (value <= 2147483647)
                {
                    return this.CreateLiteral((int)value, text);
                }
                if (value <= 4294967295u)
                {
                    return this.CreateLiteral((uint)value, text);
                }
                if (value <= 9223372036854775807L)
                {
                    return this.CreateLiteral((long)value, text);
                }
                return this.CreateLiteral(value, text);
            }
            long value2 = default(long);
            if (!long.TryParse(text, out value2))
            {
                throw this.ParseError("Invalid integer literal '{0}'", text);
            }
            this.NextToken();
            if (value2 >= -2147483648 && value2 <= 2147483647)
            {
                return this.CreateLiteral((int)value2, text);
            }
            return this.CreateLiteral(value2, text);
        }

        private Expression ParseRealLiteral()
        {
            this.ValidateToken(TokenId.RealLiteral);
            string text = this.token.text;
            object value = null;
            char last = text[text.Length - 1];
            double d = default(double);
            if (last == 'F' || last == 'f')
            {
                float f = default(float);
                if (float.TryParse(text.Substring(0, text.Length - 1), out f))
                {
                    value = f;
                }
            }
            else if (double.TryParse(text, out d))
            {
                value = d;
            }
            if (value == null)
            {
                throw this.ParseError("Invalid real literal '{0}'", text);
            }
            this.NextToken();
            return this.CreateLiteral(value, text);
        }

        private Expression CreateLiteral(object value, string text)
        {
            ConstantExpression expr = Expression.Constant(value);
            this.literals.Add(expr, text);
            return expr;
        }

        private Expression ParseParenExpression()
        {
            this.ValidateToken(TokenId.OpenParen, "'(' expected");
            this.NextToken();
            Expression e = this.ParseExpression();
            this.ValidateToken(TokenId.CloseParen, "')' or operator expected");
            this.NextToken();
            return e;
        }

        private Expression ParseIdentifier()
        {
            this.ValidateToken(TokenId.Identifier);
            object value = default(object);
            if (ExpressionParser.keywords.TryGetValue(this.token.text, out value))
            {
                if (value is Type)
                {
                    return this.ParseTypeAccess((Type)value);
                }
                if (value == ExpressionParser.keywordIt)
                {
                    return this.ParseIt();
                }
                if (value == ExpressionParser.keywordIif)
                {
                    return this.ParseIif();
                }
                if (value == ExpressionParser.keywordNew)
                {
                    return this.ParseNew();
                }
                this.NextToken();
                return (Expression)value;
            }
            if (this.symbols.TryGetValue(this.token.text, out value) || (this.externals != null && this.externals.TryGetValue(this.token.text, out value)))
            {
                Expression expr = value as Expression;
                if (expr == null)
                {
                    expr = Expression.Constant(value);
                }
                else
                {
                    LambdaExpression lambda = expr as LambdaExpression;
                    if (lambda != null)
                    {
                        return this.ParseLambdaInvocation(lambda);
                    }
                }
                this.NextToken();
                return expr;
            }
            if (this.it != null)
            {
                return this.ParseMemberAccess(null, this.it);
            }
            throw this.ParseError("Unknown identifier '{0}'", this.token.text);
        }

        private Expression ParseIt()
        {
            if (this.it == null)
            {
                throw this.ParseError("No 'it' is in scope");
            }
            this.NextToken();
            return this.it;
        }

        private Expression ParseIif()
        {
            int errorPos = this.token.pos;
            this.NextToken();
            Expression[] args = this.ParseArgumentList();
            if (args.Length != 3)
            {
                throw this.ParseError(errorPos, "The 'iif' function requires three arguments");
            }
            return this.GenerateConditional(args[0], args[1], args[2], errorPos);
        }

        private Expression GenerateConditional(Expression test, Expression expr1, Expression expr2, int errorPos)
        {
            if (test.Type != typeof(bool))
            {
                throw this.ParseError(errorPos, "The first expression must be of type 'Boolean'");
            }
            if (expr1.Type != expr2.Type)
            {
                Expression expr1as2 = (expr2 != ExpressionParser.nullLiteral) ? this.PromoteExpression(expr1, expr2.Type, true) : null;
                Expression expr2as = (expr1 != ExpressionParser.nullLiteral) ? this.PromoteExpression(expr2, expr1.Type, true) : null;
                if (expr1as2 != null && expr2as == null)
                {
                    expr1 = expr1as2;
                    goto IL_0152;
                }
                if (expr2as != null && expr1as2 == null)
                {
                    expr2 = expr2as;
                    goto IL_0152;
                }
                string type3 = (expr1 != ExpressionParser.nullLiteral) ? expr1.Type.Name : "null";
                string type2 = (expr2 != ExpressionParser.nullLiteral) ? expr2.Type.Name : "null";
                if (expr1as2 != null && expr2as != null)
                {
                    throw this.ParseError(errorPos, "Both of the types '{0}' and '{1}' convert to the other", type3, type2);
                }
                throw this.ParseError(errorPos, "Neither of the types '{0}' and '{1}' converts to the other", type3, type2);
            }
            goto IL_0152;
        IL_0152:
            return Expression.Condition(test, expr1, expr2);
        }

        private Expression ParseNew()
        {
            this.NextToken();
            this.ValidateToken(TokenId.OpenParen, "'(' expected");
            this.NextToken();
            List<DynamicProperty> properties = new List<DynamicProperty>();
            List<Expression> expressions = new List<Expression>();
            while (true)
            {
                bool flag = true;
                int exprPos = this.token.pos;
                Expression expr = this.ParseExpression();
                string propName;
                if (this.TokenIdentifierIs("as"))
                {
                    this.NextToken();
                    propName = this.GetIdentifier();
                    this.NextToken();
                }
                else
                {
                    MemberExpression me = expr as MemberExpression;
                    if (me == null)
                    {
                        throw this.ParseError(exprPos, "Expression is missing an 'as' clause");
                    }
                    propName = me.Member.Name;
                }
                expressions.Add(expr);
                properties.Add(new DynamicProperty(propName, expr.Type));
                if (this.token.id == TokenId.Comma)
                {
                    this.NextToken();
                    continue;
                }
                break;
            }
            this.ValidateToken(TokenId.CloseParen, "')' or ',' expected");
            this.NextToken();
            Type type = My.Common.Extension.DynamicExpression.CreateClass(properties);
            MemberBinding[] bindings = new MemberBinding[properties.Count];
            for (int i = 0; i < bindings.Length; i++)
            {
                bindings[i] = Expression.Bind(type.GetProperty(properties[i].Name), expressions[i]);
            }
            return Expression.MemberInit(Expression.New(type), bindings);
        }

        private Expression ParseLambdaInvocation(LambdaExpression lambda)
        {
            int errorPos = this.token.pos;
            this.NextToken();
            Expression[] args = this.ParseArgumentList();
            MethodBase method = default(MethodBase);
            if (this.FindMethod(lambda.Type, "Invoke", false, args, out method) != 1)
            {
                throw this.ParseError(errorPos, "Argument list incompatible with lambda expression");
            }
            return Expression.Invoke(lambda, args);
        }

        private Expression ParseTypeAccess(Type type)
        {
            int errorPos = this.token.pos;
            this.NextToken();
            if (this.token.id == TokenId.Question)
            {
                if (!type.IsValueType || ExpressionParser.IsNullableType(type))
                {
                    throw this.ParseError(errorPos, "Type '{0}' has no nullable form", ExpressionParser.GetTypeName(type));
                }
                type = typeof(Nullable<>).MakeGenericType(type);
                this.NextToken();
            }
            if (this.token.id == TokenId.OpenParen)
            {
                Expression[] args = this.ParseArgumentList();
                MethodBase method = default(MethodBase);
                switch (this.FindBestMethod((IEnumerable<MethodBase>)type.GetConstructors(), args, out method))
                {
                    case 0:
                        if (args.Length == 1)
                        {
                            return this.GenerateConversion(args[0], type, errorPos);
                        }
                        throw this.ParseError(errorPos, "No matching constructor in type '{0}'", ExpressionParser.GetTypeName(type));
                    case 1:
                        return Expression.New((ConstructorInfo)method, args);
                    default:
                        throw this.ParseError(errorPos, "Ambiguous invocation of '{0}' constructor", ExpressionParser.GetTypeName(type));
                }
            }
            this.ValidateToken(TokenId.Dot, "'.' or '(' expected");
            this.NextToken();
            return this.ParseMemberAccess(type, null);
        }

        private Expression GenerateConversion(Expression expr, Type type, int errorPos)
        {
            Type exprType = expr.Type;
            if (exprType == type)
            {
                return expr;
            }
            int num;
            if (exprType.IsValueType && type.IsValueType)
            {
                if ((ExpressionParser.IsNullableType(exprType) || ExpressionParser.IsNullableType(type)) && ExpressionParser.GetNonNullableType(exprType) == ExpressionParser.GetNonNullableType(type))
                {
                    return Expression.Convert(expr, type);
                }
                if (!ExpressionParser.IsNumericType(exprType) && !ExpressionParser.IsEnumType(exprType))
                {
                    goto IL_0088;
                }
                if (!ExpressionParser.IsNumericType(type))
                {
                    goto IL_0088;
                }
                num = 0;
                goto IL_0094;
            }
            goto IL_00a4;
        IL_00a4:
            if (exprType.IsAssignableFrom(type) || type.IsAssignableFrom(exprType) || exprType.IsInterface || type.IsInterface)
            {
                return Expression.Convert(expr, type);
            }
            throw this.ParseError(errorPos, "A value of type '{0}' cannot be converted to type '{1}'", ExpressionParser.GetTypeName(exprType), ExpressionParser.GetTypeName(type));
        IL_0094:
            if (num == 0)
            {
                return Expression.ConvertChecked(expr, type);
            }
            goto IL_00a4;
        IL_0088:
            num = ((!ExpressionParser.IsEnumType(type)) ? 1 : 0);
            goto IL_0094;
        }

        private Expression ParseMemberAccess(Type type, Expression instance)
        {
            if (instance != null)
            {
                type = instance.Type;
            }
            int errorPos = this.token.pos;
            string id = this.GetIdentifier();
            this.NextToken();
            if (this.token.id == TokenId.OpenParen)
            {
                if (instance != null && type != typeof(string))
                {
                    Type enumerableType = ExpressionParser.FindGenericType(typeof(IEnumerable<>), type);
                    if (enumerableType != (Type)null)
                    {
                        Type elementType = enumerableType.GetGenericArguments()[0];
                        return this.ParseAggregate(instance, elementType, id, errorPos);
                    }
                }
                Expression[] args = this.ParseArgumentList();
                MethodBase mb = default(MethodBase);
                switch (this.FindMethod(type, id, instance == null, args, out mb))
                {
                    case 0:
                        throw this.ParseError(errorPos, "No applicable method '{0}' exists in type '{1}'", id, ExpressionParser.GetTypeName(type));
                    case 1:
                        {
                            MethodInfo method = (MethodInfo)mb;
                            if (!ExpressionParser.IsPredefinedType(method.DeclaringType))
                            {
                                throw this.ParseError(errorPos, "Methods on type '{0}' are not accessible", ExpressionParser.GetTypeName(method.DeclaringType));
                            }
                            if (method.ReturnType == typeof(void))
                            {
                                throw this.ParseError(errorPos, "Method '{0}' in type '{1}' does not return a value", id, ExpressionParser.GetTypeName(method.DeclaringType));
                            }
                            return Expression.Call(instance, method, args);
                        }
                    default:
                        throw this.ParseError(errorPos, "Ambiguous invocation of method '{0}' in type '{1}'", id, ExpressionParser.GetTypeName(type));
                }
            }
            MemberInfo member = this.FindPropertyOrField(type, id, instance == null);
            if (member == (MemberInfo)null)
            {
                throw this.ParseError(errorPos, "No property or field '{0}' exists in type '{1}'", id, ExpressionParser.GetTypeName(type));
            }
            return (member is PropertyInfo) ? Expression.Property(instance, (PropertyInfo)member) : Expression.Field(instance, (FieldInfo)member);
        }

        private static Type FindGenericType(Type generic, Type type)
        {
            while (type != (Type)null && type != typeof(object))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == generic)
                {
                    return type;
                }
                if (generic.IsInterface)
                {
                    Type[] interfaces = type.GetInterfaces();
                    foreach (Type intfType in interfaces)
                    {
                        Type found = ExpressionParser.FindGenericType(generic, intfType);
                        if (found != (Type)null)
                        {
                            return found;
                        }
                    }
                }
                type = type.BaseType;
            }
            return null;
        }

        private Expression ParseAggregate(Expression instance, Type elementType, string methodName, int errorPos)
        {
            ParameterExpression outerIt = this.it;
            ParameterExpression innerIt = this.it = Expression.Parameter(elementType, "");
            Expression[] args2 = this.ParseArgumentList();
            this.it = outerIt;
            MethodBase signature = default(MethodBase);
            if (this.FindMethod(typeof(IEnumerableSignatures), methodName, false, args2, out signature) != 1)
            {
                throw this.ParseError(errorPos, "No applicable aggregate method '{0}' exists", methodName);
            }
            Type[] typeArgs = (!(signature.Name == "Min") && !(signature.Name == "Max")) ? new Type[1]
            {
            elementType
            } : new Type[2]
            {
            elementType,
            args2[0].Type
            };
            args2 = ((args2.Length != 0) ? new Expression[2]
            {
            instance,
            Expression.Lambda(args2[0], innerIt)
            } : new Expression[1]
            {
            instance
            });
            return Expression.Call(typeof(Enumerable), signature.Name, typeArgs, args2);
        }

        private Expression[] ParseArgumentList()
        {
            this.ValidateToken(TokenId.OpenParen, "'(' expected");
            this.NextToken();
            Expression[] args = (this.token.id != TokenId.CloseParen) ? this.ParseArguments() : new Expression[0];
            this.ValidateToken(TokenId.CloseParen, "')' or ',' expected");
            this.NextToken();
            return args;
        }

        private Expression[] ParseArguments()
        {
            List<Expression> argList = new List<Expression>();
            while (true)
            {
                bool flag = true;
                argList.Add(this.ParseExpression());
                if (this.token.id == TokenId.Comma)
                {
                    this.NextToken();
                    continue;
                }
                break;
            }
            return argList.ToArray();
        }

        private Expression ParseElementAccess(Expression expr)
        {
            int errorPos = this.token.pos;
            this.ValidateToken(TokenId.OpenBracket, "'(' expected");
            this.NextToken();
            Expression[] args = this.ParseArguments();
            this.ValidateToken(TokenId.CloseBracket, "']' or ',' expected");
            this.NextToken();
            if (expr.Type.IsArray)
            {
                if (expr.Type.GetArrayRank() != 1 || args.Length != 1)
                {
                    throw this.ParseError(errorPos, "Indexing of multi-dimensional arrays is not supported");
                }
                Expression index = this.PromoteExpression(args[0], typeof(int), true);
                if (index == null)
                {
                    throw this.ParseError(errorPos, "Array index must be an integer expression");
                }
                return Expression.ArrayIndex(expr, index);
            }
            MethodBase mb = default(MethodBase);
            switch (this.FindIndexer(expr.Type, args, out mb))
            {
                case 0:
                    throw this.ParseError(errorPos, "No applicable indexer exists in type '{0}'", ExpressionParser.GetTypeName(expr.Type));
                case 1:
                    return Expression.Call(expr, (MethodInfo)mb, args);
                default:
                    throw this.ParseError(errorPos, "Ambiguous invocation of indexer in type '{0}'", ExpressionParser.GetTypeName(expr.Type));
            }
        }

        private static bool IsPredefinedType(Type type)
        {
            Type[] array = ExpressionParser.predefinedTypes;
            foreach (Type t in array)
            {
                if (t == type)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        private static Type GetNonNullableType(Type type)
        {
            return ExpressionParser.IsNullableType(type) ? type.GetGenericArguments()[0] : type;
        }

        private static string GetTypeName(Type type)
        {
            Type baseType = ExpressionParser.GetNonNullableType(type);
            string s = baseType.Name;
            if (type != baseType)
            {
                s += '?';
            }
            return s;
        }

        private static bool IsNumericType(Type type)
        {
            return ExpressionParser.GetNumericTypeKind(type) != 0;
        }

        private static bool IsSignedIntegralType(Type type)
        {
            return ExpressionParser.GetNumericTypeKind(type) == 2;
        }

        private static bool IsUnsignedIntegralType(Type type)
        {
            return ExpressionParser.GetNumericTypeKind(type) == 3;
        }

        private static int GetNumericTypeKind(Type type)
        {
            type = ExpressionParser.GetNonNullableType(type);
            if (type.IsEnum)
            {
                return 0;
            }
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Char:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return 1;
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return 2;
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return 3;
                default:
                    return 0;
            }
        }

        private static bool IsEnumType(Type type)
        {
            return ExpressionParser.GetNonNullableType(type).IsEnum;
        }

        private void CheckAndPromoteOperand(Type signatures, string opName, ref Expression expr, int errorPos)
        {
            Expression[] args = new Expression[1]
            {
            expr
            };
            MethodBase method = default(MethodBase);
            if (this.FindMethod(signatures, "F", false, args, out method) != 1)
            {
                throw this.ParseError(errorPos, "Operator '{0}' incompatible with operand type '{1}'", opName, ExpressionParser.GetTypeName(args[0].Type));
            }
            expr = args[0];
        }

        private void CheckAndPromoteOperands(Type signatures, string opName, ref Expression left, ref Expression right, int errorPos)
        {
            Expression[] args = new Expression[2]
            {
            left,
            right
            };
            MethodBase method = default(MethodBase);
            if (this.FindMethod(signatures, "F", false, args, out method) != 1)
            {
                throw this.IncompatibleOperandsError(opName, left, right, errorPos);
            }
            left = args[0];
            right = args[1];
        }

        private Exception IncompatibleOperandsError(string opName, Expression left, Expression right, int pos)
        {
            return this.ParseError(pos, "Operator '{0}' incompatible with operand types '{1}' and '{2}'", opName, ExpressionParser.GetTypeName(left.Type), ExpressionParser.GetTypeName(right.Type));
        }

        private MemberInfo FindPropertyOrField(Type type, string memberName, bool staticAccess)
        {
            BindingFlags flags = (BindingFlags)(0x12 | (staticAccess ? 8 : 4));
            foreach (Type item in ExpressionParser.SelfAndBaseTypes(type))
            {
                MemberInfo[] members = item.FindMembers(MemberTypes.Field | MemberTypes.Property, flags, Type.FilterNameIgnoreCase, memberName);
                if (members.Length != 0)
                {
                    return members[0];
                }
            }
            return null;
        }

        private int FindMethod(Type type, string methodName, bool staticAccess, Expression[] args, out MethodBase method)
        {
            BindingFlags flags = (BindingFlags)(0x12 | (staticAccess ? 8 : 4));
            foreach (Type item in ExpressionParser.SelfAndBaseTypes(type))
            {
                MemberInfo[] members = item.FindMembers(MemberTypes.Method, flags, Type.FilterNameIgnoreCase, methodName);
                int count = this.FindBestMethod(members.Cast<MethodBase>(), args, out method);
                if (count != 0)
                {
                    return count;
                }
            }
            method = null;
            return 0;
        }

        private int FindIndexer(Type type, Expression[] args, out MethodBase method)
        {
            foreach (Type item in ExpressionParser.SelfAndBaseTypes(type))
            {
                MemberInfo[] members = item.GetDefaultMembers();
                if (members.Length != 0)
                {
                    IEnumerable<MethodBase> methods = from p in members.OfType<PropertyInfo>()
                                                      select p.GetGetMethod() into m
                                                      where m != (MethodBase)null
                                                      select m;
                    int count = this.FindBestMethod(methods, args, out method);
                    if (count != 0)
                    {
                        return count;
                    }
                }
            }
            method = null;
            return 0;
        }

        private static IEnumerable<Type> SelfAndBaseTypes(Type type)
        {
            if (type.IsInterface)
            {
                List<Type> types = new List<Type>();
                ExpressionParser.AddInterface(types, type);
                return types;
            }
            return ExpressionParser.SelfAndBaseClasses(type);
        }

        private static IEnumerable<Type> SelfAndBaseClasses(Type type)
        {
            while (type != (Type)null)
            {
                yield return type;
                type = type.BaseType;
            }
        }

        private static void AddInterface(List<Type> types, Type type)
        {
            if (!types.Contains(type))
            {
                types.Add(type);
                Type[] interfaces = type.GetInterfaces();
                foreach (Type t in interfaces)
                {
                    ExpressionParser.AddInterface(types, t);
                }
            }
        }

        private int FindBestMethod(IEnumerable<MethodBase> methods, Expression[] args, out MethodBase method)
        {
            MethodData[] applicable = (from m in methods.Select(delegate (MethodBase m)
            {
                MethodData methodData = new MethodData();
                methodData.MethodBase = m;
                methodData.Parameters = m.GetParameters();
                return methodData;
            })
                                       where this.IsApplicable(m, args)
                                       select m).ToArray();
            if (applicable.Length > 1)
            {
                applicable = (from m in applicable
                              where applicable.All((MethodData n) => m == n || ExpressionParser.IsBetterThan(args, m, n))
                              select m).ToArray();
            }
            if (applicable.Length == 1)
            {
                MethodData md = applicable[0];
                for (int i = 0; i < args.Length; i++)
                {
                    args[i] = md.Args[i];
                }
                method = md.MethodBase;
            }
            else
            {
                method = null;
            }
            return applicable.Length;
        }

        private bool IsApplicable(MethodData method, Expression[] args)
        {
            if (method.Parameters.Length != args.Length)
            {
                return false;
            }
            Expression[] promotedArgs = new Expression[args.Length];
            int i = 0;
            bool result;
            while (true)
            {
                if (i < args.Length)
                {
                    ParameterInfo pi = method.Parameters[i];
                    if (pi.IsOut)
                    {
                        result = false;
                        break;
                    }
                    Expression promoted = this.PromoteExpression(args[i], pi.ParameterType, false);
                    if (promoted == null)
                    {
                        return false;
                    }
                    promotedArgs[i] = promoted;
                    i++;
                    continue;
                }
                method.Args = promotedArgs;
                return true;
            }
            return result;
        }

        private Expression PromoteExpression(Expression expr, Type type, bool exact)
        {
            if (expr.Type == type)
            {
                return expr;
            }
            if (expr is ConstantExpression)
            {
                ConstantExpression ce = (ConstantExpression)expr;
                string text = default(string);
                if (ce == ExpressionParser.nullLiteral)
                {
                    if (!type.IsValueType || ExpressionParser.IsNullableType(type))
                    {
                        return Expression.Constant(null, type);
                    }
                }
                else if (this.literals.TryGetValue((Expression)ce, out text))
                {
                    Type target = ExpressionParser.GetNonNullableType(type);
                    object value = null;
                    switch (Type.GetTypeCode(ce.Type))
                    {
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                            value = ExpressionParser.ParseNumber(text, target);
                            break;
                        case TypeCode.Double:
                            if (target == typeof(decimal))
                            {
                                value = ExpressionParser.ParseNumber(text, target);
                            }
                            break;
                        case TypeCode.String:
                            value = ExpressionParser.ParseEnum(text, target);
                            break;
                    }
                    if (value != null)
                    {
                        return Expression.Constant(value, type);
                    }
                }
            }
            if (ExpressionParser.IsCompatibleWith(expr.Type, type))
            {
                if (type.IsValueType || exact)
                {
                    return Expression.Convert(expr, type);
                }
                return expr;
            }
            return null;
        }

        private static object ParseNumber(string text, Type type)
        {
            switch (Type.GetTypeCode(ExpressionParser.GetNonNullableType(type)))
            {
                case TypeCode.SByte:
                    {
                        sbyte sb = default(sbyte);
                        if (sbyte.TryParse(text, out sb))
                        {
                            return sb;
                        }
                        goto default;
                    }
                case TypeCode.Byte:
                    {
                        byte b = default(byte);
                        if (byte.TryParse(text, out b))
                        {
                            return b;
                        }
                        goto default;
                    }
                case TypeCode.Int16:
                    {
                        short s = default(short);
                        if (short.TryParse(text, out s))
                        {
                            return s;
                        }
                        goto default;
                    }
                case TypeCode.UInt16:
                    {
                        ushort us = default(ushort);
                        if (ushort.TryParse(text, out us))
                        {
                            return us;
                        }
                        goto default;
                    }
                case TypeCode.Int32:
                    {
                        int i = default(int);
                        if (int.TryParse(text, out i))
                        {
                            return i;
                        }
                        goto default;
                    }
                case TypeCode.UInt32:
                    {
                        uint ui = default(uint);
                        if (uint.TryParse(text, out ui))
                        {
                            return ui;
                        }
                        goto default;
                    }
                case TypeCode.Int64:
                    {
                        long j = default(long);
                        if (long.TryParse(text, out j))
                        {
                            return j;
                        }
                        goto default;
                    }
                case TypeCode.UInt64:
                    {
                        ulong ul = default(ulong);
                        if (ulong.TryParse(text, out ul))
                        {
                            return ul;
                        }
                        goto default;
                    }
                case TypeCode.Single:
                    {
                        float f = default(float);
                        if (float.TryParse(text, out f))
                        {
                            return f;
                        }
                        goto default;
                    }
                case TypeCode.Double:
                    {
                        double d = default(double);
                        if (double.TryParse(text, out d))
                        {
                            return d;
                        }
                        goto default;
                    }
                case TypeCode.Decimal:
                    {
                        decimal e = default(decimal);
                        if (decimal.TryParse(text, out e))
                        {
                            return e;
                        }
                        goto default;
                    }
                default:
                    return null;
            }
        }

        private static object ParseEnum(string name, Type type)
        {
            if (type.IsEnum)
            {
                MemberInfo[] memberInfos = type.FindMembers(MemberTypes.Field, BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public, Type.FilterNameIgnoreCase, name);
                if (memberInfos.Length != 0)
                {
                    return ((FieldInfo)memberInfos[0]).GetValue(null);
                }
            }
            return null;
        }

        private static bool IsCompatibleWith(Type source, Type target)
        {
            if (source == target)
            {
                return true;
            }
            if (!target.IsValueType)
            {
                return target.IsAssignableFrom(source);
            }
            Type st = ExpressionParser.GetNonNullableType(source);
            Type tt = ExpressionParser.GetNonNullableType(target);
            if (st != source && tt == target)
            {
                return false;
            }
            TypeCode sc = st.IsEnum ? TypeCode.Object : Type.GetTypeCode(st);
            TypeCode tc = tt.IsEnum ? TypeCode.Object : Type.GetTypeCode(tt);
            switch (sc)
            {
                case TypeCode.SByte:
                    switch (tc)
                    {
                        case TypeCode.SByte:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    goto IL_02b6;
                case TypeCode.Byte:
                    switch (tc)
                    {
                        case TypeCode.Byte:
                        case TypeCode.Int16:
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    goto IL_02b6;
                case TypeCode.Int16:
                    switch (tc)
                    {
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    goto IL_02b6;
                case TypeCode.UInt16:
                    switch (tc)
                    {
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    goto IL_02b6;
                case TypeCode.Int32:
                    switch (tc)
                    {
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    goto IL_02b6;
                case TypeCode.UInt32:
                    switch (tc)
                    {
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    goto IL_02b6;
                case TypeCode.Int64:
                    switch (tc)
                    {
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    goto IL_02b6;
                case TypeCode.UInt64:
                    switch (tc)
                    {
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    goto IL_02b6;
                case TypeCode.Single:
                    switch (tc)
                    {
                        case TypeCode.Single:
                        case TypeCode.Double:
                            return true;
                    }
                    goto IL_02b6;
                default:
                    {
                        if (st == tt)
                        {
                            return true;
                        }
                        goto IL_02b6;
                    }
                IL_02b6:
                    return false;
            }
        }

        private static bool IsBetterThan(Expression[] args, MethodData m1, MethodData m2)
        {
            bool better = false;
            for (int i = 0; i < args.Length; i++)
            {
                int c = ExpressionParser.CompareConversions(args[i].Type, m1.Parameters[i].ParameterType, m2.Parameters[i].ParameterType);
                if (c < 0)
                {
                    return false;
                }
                if (c > 0)
                {
                    better = true;
                }
            }
            return better;
        }

        private static int CompareConversions(Type s, Type t1, Type t2)
        {
            if (t1 == t2)
            {
                return 0;
            }
            if (s == t1)
            {
                return 1;
            }
            if (s == t2)
            {
                return -1;
            }
            bool t1t2 = ExpressionParser.IsCompatibleWith(t1, t2);
            bool t2t = ExpressionParser.IsCompatibleWith(t2, t1);
            if (t1t2 && !t2t)
            {
                return 1;
            }
            if (t2t && !t1t2)
            {
                return -1;
            }
            if (ExpressionParser.IsSignedIntegralType(t1) && ExpressionParser.IsUnsignedIntegralType(t2))
            {
                return 1;
            }
            if (ExpressionParser.IsSignedIntegralType(t2) && ExpressionParser.IsUnsignedIntegralType(t1))
            {
                return -1;
            }
            return 0;
        }

        private Expression GenerateEqual(Expression left, Expression right)
        {
            return Expression.Equal(left, right);
        }

        private Expression GenerateNotEqual(Expression left, Expression right)
        {
            return Expression.NotEqual(left, right);
        }

        private Expression GenerateGreaterThan(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.GreaterThan(this.GenerateStaticMethodCall("Compare", left, right), Expression.Constant(0));
            }
            return Expression.GreaterThan(left, right);
        }

        private Expression GenerateGreaterThanEqual(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.GreaterThanOrEqual(this.GenerateStaticMethodCall("Compare", left, right), Expression.Constant(0));
            }
            return Expression.GreaterThanOrEqual(left, right);
        }

        private Expression GenerateLessThan(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.LessThan(this.GenerateStaticMethodCall("Compare", left, right), Expression.Constant(0));
            }
            return Expression.LessThan(left, right);
        }

        private Expression GenerateLessThanEqual(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.LessThanOrEqual(this.GenerateStaticMethodCall("Compare", left, right), Expression.Constant(0));
            }
            return Expression.LessThanOrEqual(left, right);
        }

        private Expression GenerateAdd(Expression left, Expression right)
        {
            if (left.Type == typeof(string) && right.Type == typeof(string))
            {
                return this.GenerateStaticMethodCall("Concat", left, right);
            }
            return Expression.Add(left, right);
        }

        private Expression GenerateSubtract(Expression left, Expression right)
        {
            return Expression.Subtract(left, right);
        }

        private Expression GenerateStringConcat(Expression left, Expression right)
        {
            return Expression.Call(null, typeof(string).GetMethod("Concat", new Type[2]
            {
            typeof(object),
            typeof(object)
            }), new Expression[2]
            {
            left,
            right
            });
        }

        private MethodInfo GetStaticMethod(string methodName, Expression left, Expression right)
        {
            return left.Type.GetMethod(methodName, new Type[2]
            {
            left.Type,
            right.Type
            });
        }

        private Expression GenerateStaticMethodCall(string methodName, Expression left, Expression right)
        {
            return Expression.Call(null, this.GetStaticMethod(methodName, left, right), new Expression[2]
            {
            left,
            right
            });
        }

        private void SetTextPos(int pos)
        {
            this.textPos = pos;
            this.ch = ((this.textPos < this.textLen) ? this.text[this.textPos] : '\0');
        }

        private void NextChar()
        {
            if (this.textPos < this.textLen)
            {
                this.textPos++;
            }
            this.ch = ((this.textPos < this.textLen) ? this.text[this.textPos] : '\0');
        }

        private void NextToken()
        {
            while (char.IsWhiteSpace(this.ch))
            {
                this.NextChar();
            }
            int tokenPos = this.textPos;
            TokenId t;
            switch (this.ch)
            {
                case '!':
                    this.NextChar();
                    if (this.ch == '=')
                    {
                        this.NextChar();
                        t = TokenId.ExclamationEqual;
                    }
                    else
                    {
                        t = TokenId.Exclamation;
                    }
                    break;
                case '%':
                    this.NextChar();
                    t = TokenId.Percent;
                    break;
                case '&':
                    this.NextChar();
                    if (this.ch == '&')
                    {
                        this.NextChar();
                        t = TokenId.DoubleAmphersand;
                    }
                    else
                    {
                        t = TokenId.Amphersand;
                    }
                    break;
                case '(':
                    this.NextChar();
                    t = TokenId.OpenParen;
                    break;
                case ')':
                    this.NextChar();
                    t = TokenId.CloseParen;
                    break;
                case '*':
                    this.NextChar();
                    t = TokenId.Asterisk;
                    break;
                case '+':
                    this.NextChar();
                    t = TokenId.Plus;
                    break;
                case ',':
                    this.NextChar();
                    t = TokenId.Comma;
                    break;
                case '-':
                    this.NextChar();
                    t = TokenId.Minus;
                    break;
                case '.':
                    this.NextChar();
                    t = TokenId.Dot;
                    break;
                case '/':
                    this.NextChar();
                    t = TokenId.Slash;
                    break;
                case ':':
                    this.NextChar();
                    t = TokenId.Colon;
                    break;
                case '<':
                    this.NextChar();
                    if (this.ch == '=')
                    {
                        this.NextChar();
                        t = TokenId.LessThanEqual;
                    }
                    else if (this.ch == '>')
                    {
                        this.NextChar();
                        t = TokenId.LessGreater;
                    }
                    else
                    {
                        t = TokenId.LessThan;
                    }
                    break;
                case '=':
                    this.NextChar();
                    if (this.ch == '=')
                    {
                        this.NextChar();
                        t = TokenId.DoubleEqual;
                    }
                    else
                    {
                        t = TokenId.Equal;
                    }
                    break;
                case '>':
                    this.NextChar();
                    if (this.ch == '=')
                    {
                        this.NextChar();
                        t = TokenId.GreaterThanEqual;
                    }
                    else
                    {
                        t = TokenId.GreaterThan;
                    }
                    break;
                case '?':
                    this.NextChar();
                    t = TokenId.Question;
                    break;
                case '[':
                    this.NextChar();
                    t = TokenId.OpenBracket;
                    break;
                case ']':
                    this.NextChar();
                    t = TokenId.CloseBracket;
                    break;
                case '|':
                    this.NextChar();
                    if (this.ch == '|')
                    {
                        this.NextChar();
                        t = TokenId.DoubleBar;
                    }
                    else
                    {
                        t = TokenId.Bar;
                    }
                    break;
                case '"':
                case '\'':
                    {
                        char quote = this.ch;
                        do
                        {
                            this.NextChar();
                            while (this.textPos < this.textLen && this.ch != quote)
                            {
                                this.NextChar();
                            }
                            if (this.textPos == this.textLen)
                            {
                                throw this.ParseError(this.textPos, "Unterminated string literal");
                            }
                            this.NextChar();
                        }
                        while (this.ch == quote);
                        t = TokenId.StringLiteral;
                        break;
                    }
                default:
                    if (char.IsLetter(this.ch) || this.ch == '@' || this.ch == '_')
                    {
                        do
                        {
                            this.NextChar();
                        }
                        while (char.IsLetterOrDigit(this.ch) || this.ch == '_');
                        t = TokenId.Identifier;
                        break;
                    }
                    if (char.IsDigit(this.ch))
                    {
                        t = TokenId.IntegerLiteral;
                        do
                        {
                            this.NextChar();
                        }
                        while (char.IsDigit(this.ch));
                        if (this.ch == '.')
                        {
                            t = TokenId.RealLiteral;
                            this.NextChar();
                            this.ValidateDigit();
                            do
                            {
                                this.NextChar();
                            }
                            while (char.IsDigit(this.ch));
                        }
                        if (this.ch == 'E' || this.ch == 'e')
                        {
                            t = TokenId.RealLiteral;
                            this.NextChar();
                            if (this.ch == '+' || this.ch == '-')
                            {
                                this.NextChar();
                            }
                            this.ValidateDigit();
                            do
                            {
                                this.NextChar();
                            }
                            while (char.IsDigit(this.ch));
                        }
                        if (this.ch == 'F' || this.ch == 'f')
                        {
                            this.NextChar();
                        }
                        break;
                    }
                    if (this.textPos == this.textLen)
                    {
                        t = TokenId.End;
                        break;
                    }
                    throw this.ParseError(this.textPos, "Syntax error '{0}'", this.ch);
            }
            this.token.id = t;
            this.token.text = this.text.Substring(tokenPos, this.textPos - tokenPos);
            this.token.pos = tokenPos;
        }

        private bool TokenIdentifierIs(string id)
        {
            return this.token.id == TokenId.Identifier && string.Equals(id, this.token.text, StringComparison.OrdinalIgnoreCase);
        }

        private string GetIdentifier()
        {
            this.ValidateToken(TokenId.Identifier, "Identifier expected");
            string id = this.token.text;
            if (id.Length > 1 && id[0] == '@')
            {
                id = id.Substring(1);
            }
            return id;
        }

        private void ValidateDigit()
        {
            if (char.IsDigit(this.ch))
            {
                return;
            }
            throw this.ParseError(this.textPos, "Digit expected");
        }

        private void ValidateToken(TokenId t, string errorMessage)
        {
            if (this.token.id == t)
            {
                return;
            }
            throw this.ParseError(errorMessage);
        }

        private void ValidateToken(TokenId t)
        {
            if (this.token.id == t)
            {
                return;
            }
            throw this.ParseError("Syntax error");
        }

        private Exception ParseError(string format, params object[] args)
        {
            return this.ParseError(this.token.pos, format, args);
        }

        private Exception ParseError(int pos, string format, params object[] args)
        {
            return new ParseException(string.Format(CultureInfo.CurrentCulture, format, args), pos);
        }

        private static Dictionary<string, object> CreateKeywords()
        {
            Dictionary<string, object> d = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            d.Add("true", ExpressionParser.trueLiteral);
            d.Add("false", ExpressionParser.falseLiteral);
            d.Add("null", ExpressionParser.nullLiteral);
            d.Add(ExpressionParser.keywordIt, ExpressionParser.keywordIt);
            d.Add(ExpressionParser.keywordIif, ExpressionParser.keywordIif);
            d.Add(ExpressionParser.keywordNew, ExpressionParser.keywordNew);
            Type[] array = ExpressionParser.predefinedTypes;
            foreach (Type type in array)
            {
                d.Add(type.Name, type);
            }
            return d;
        }
    }
}
