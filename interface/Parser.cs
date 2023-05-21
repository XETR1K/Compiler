using System;
using System.Collections.Generic;

namespace @interface
{
    //действия над токенами
    public enum ActionOverTokens
    {
        InsertBefore, 
        InsertAfter,
        Replacement,
        Remove
    }

    //класс для ошибок
    public class ParsingError
    {
        public int Position { get; }
        public TokenType ExpectedTokenType { get; }
        public ActionOverTokens action { get; }
        public string Message { get; }

        public ParsingError(int position, TokenType expectedTokenType, ActionOverTokens _action ,string message)
        {
            Position = position;
            ExpectedTokenType = expectedTokenType;
            action= _action;
            Message = message;
        }
    }

    static class Parser
    {
        //Список лексем
        private static List<Token> tokens;
        //Tекущая лексема
        private static int currentTokenIndex;

        //Ошибки
        private static List<ParsingError> errors;
        public static List<ParsingError> getErrors() { return errors; }
        public static void clearErrorsList() { errors.Clear(); }


        //метод для проверки текущей лексемы и перехода к следующей
        private static void Match(TokenType expectedToken)
        {
            if (currentTokenIndex < tokens.Count)
            {
                if (tokens[currentTokenIndex].tokenType == expectedToken)
                    currentTokenIndex++;
                else
                {
                    if (!Check(TokenType.некорректный_токен) && !Check(TokenType.закрывающая_скобка))
                        errors.Add(new ParsingError(tokens[currentTokenIndex].start - 1, expectedToken, ActionOverTokens.InsertBefore, $"Ожидалась лексема {expectedToken}, получена {tokens[currentTokenIndex].tokenType}"));
                    else
                        errors.Add(new ParsingError(tokens[currentTokenIndex].start, expectedToken, ActionOverTokens.Replacement, $"Ожидалась лексема {expectedToken}, получена {tokens[currentTokenIndex].tokenType}"));
                    throw new Exception($"Ожидалась лексема {expectedToken}, получена {tokens[currentTokenIndex].tokenType}");
                }
            }
        }

        //метод для проверки текущей лексемы без перехода к следующей
        private static bool Check(TokenType expectedToken)
        {
            if (currentTokenIndex == tokens.Count)
                return false;
            return tokens[currentTokenIndex].tokenType == expectedToken;
        }

        //метод для проверки, является ли лексема литералом.
        private static bool isLiteral(int i)
        {
            return (tokens[currentTokenIndex + i].tokenType == TokenType.идентификатор || tokens[currentTokenIndex + i].tokenType == TokenType.открывающая_скобка ||
                                tokens[currentTokenIndex + i].tokenType == TokenType.логическая_константа || tokens[currentTokenIndex + i].tokenType == TokenType.целое_число ||
                                tokens[currentTokenIndex + i].tokenType == TokenType.число_с_плавающей_точкой);
        }

        //метод для запуска проверки
        public static void Parsing(List<Token> inputTokens)
        {
            tokens = inputTokens;
            currentTokenIndex = 0;
            errors = new List<ParsingError>();
            Parse();
        }


        //метод анализа строки
        public static void Parse()
        {
            try
            {
                Expr();
                if (currentTokenIndex != tokens.Count)
                {
                    string message = "Некорректный токен";
                    if (currentTokenIndex == tokens.Count- 1)
                    {
                        errors.Add(new ParsingError(tokens[currentTokenIndex].start, TokenType.некорректный_токен, ActionOverTokens.Remove, message));
                        throw new Exception(message);
                    }
                    else
                    {
                        if (isLiteral(0))
                        {
                            message = "Ожидался оператор сравнения";
                            errors.Add(new ParsingError(tokens[currentTokenIndex].start - 1, TokenType.оператор_сравнения, ActionOverTokens.InsertBefore, message));
                            throw new Exception(message);
                        }
                        else if(isLiteral(1))
                        {
                            message = "Ожидался оператор сравнения";
                            if (!Check(TokenType.некорректный_токен) && !Check(TokenType.закрывающая_скобка))
                                errors.Add(new ParsingError(tokens[currentTokenIndex].start - 1, TokenType.оператор_сравнения, ActionOverTokens.InsertBefore, message));
                            else
                                errors.Add(new ParsingError(tokens[currentTokenIndex].start, TokenType.оператор_сравнения, ActionOverTokens.Replacement, message));
                            throw new Exception(message);
                        }
                        else
                        {
                            errors.Add(new ParsingError(tokens[currentTokenIndex].start, TokenType.некорректный_токен, ActionOverTokens.Remove, message));
                            throw new Exception(message);
                        }    
                    }
                }
            }
            catch //Нейтрализация ошибки
            {
                var token = new Token(errors[errors.Count - 1].ExpectedTokenType, " ", errors[errors.Count - 1].Position, errors[errors.Count - 1].Position);
                if (tokens.Count > 1)
                {
                    if(currentTokenIndex < tokens.Count)
                    {
                        switch (errors[errors.Count - 1].action)
                        {
                            case ActionOverTokens.InsertBefore:
                                tokens.Insert(currentTokenIndex, token);
                                break;
                            case ActionOverTokens.InsertAfter:
                                tokens.Insert(currentTokenIndex + 1, token);
                                break;
                            case ActionOverTokens.Replacement:
                                tokens.RemoveAt(currentTokenIndex);
                                tokens.Insert(currentTokenIndex, token);
                                break;
                            case ActionOverTokens.Remove:
                                tokens.RemoveAt(currentTokenIndex);
                                break;

                        }
                    }
                    else
                        tokens.Add(token);
                    currentTokenIndex = 0;
                    Parse();
                }
                else
                {
                    tokens.Remove(token);
                    return;
                }
            }
        }

        //‹Expr› → ‹AndExpr› {||‹AndExpr›}
        private static void Expr()
        {
            AndExpr();
            while ((currentTokenIndex < tokens.Count) && Check(TokenType.логический_оператор) && tokens[currentTokenIndex].token == "||")
            {
                Match(TokenType.логический_оператор);
                AndExpr();
            }
        }

        //‹AndExpr› → ‹NotExpr› {&&‹NotExpr›}
        private static void AndExpr()
        {
            NotExpr();
            while ((currentTokenIndex < tokens.Count) && Check(TokenType.логический_оператор) && tokens[currentTokenIndex].token == "&&")
            {
                Match(TokenType.логический_оператор);
                NotExpr();
            }
        }

        //‹NotExpr› → !‹SimpleExpr› | ‹SimpleExpr›
        private static void NotExpr()
        {
            if (Check(TokenType.оператор_отрицания))
                Match(TokenType.оператор_отрицания);
            ComparisonExpr();
        }

        //‹ComparisonExpr› → ‹SimpleExpr› {‹оператор_сравнения› ‹SimpleExpr›}
        private static void ComparisonExpr()
        {
            SimpleExpr();
            while ((currentTokenIndex < tokens.Count) && Check(TokenType.оператор_сравнения))
            {
                Match(TokenType.оператор_сравнения);
                SimpleExpr();
            }
        }

        //‹SimpleExpr› → ‹Term› {‹оператор_сложения_вычитания›‹Term›}
        private static void SimpleExpr()
        {
            Term();
            while ((currentTokenIndex < tokens.Count) && Check(TokenType.оператор_сложения_вычитания))
            {
                Match(TokenType.оператор_сложения_вычитания);
                Term();
            }
        }

        //‹Term› → ‹Factor› {‹оператор_умножения_деления›‹Factor›}
        private static void Term()
        {
            Factor();
            while ((currentTokenIndex < tokens.Count) && Check(TokenType.оператор_умножения_деления))
            {
                Match(TokenType.оператор_умножения_деления);
                Factor();
            }
        }

        //‹Factor› → (‹Expr›) | ‹идентификатор› | ‹целое_число› | ‹число_с_плавающей_точкой› | ‹логическая_константа›
        private static void Factor()
        {
            if (currentTokenIndex < tokens.Count)
            {
                switch (tokens[currentTokenIndex].tokenType)
                {
                    case TokenType.открывающая_скобка:
                        Match(TokenType.открывающая_скобка);
                        Expr();
                        if (!Check(TokenType.закрывающая_скобка))
                        {
                            if (currentTokenIndex < tokens.Count)
                            {
                                if (isLiteral(0))
                                {
                                    errors.Add(new ParsingError(tokens[currentTokenIndex].start - 1, TokenType.оператор_сравнения, ActionOverTokens.InsertBefore, "Ожидался оператор сравнения"));
                                    throw new Exception("Ожидался оператор сравнения");
                                }
                                else if (!Check(TokenType.некорректный_токен) && !Check(TokenType.оператор_отрицания))
                                {
                                    errors.Add(new ParsingError(tokens[currentTokenIndex].end + 1, TokenType.закрывающая_скобка, ActionOverTokens.InsertAfter, "Ожидалась закрывающая скобка"));
                                    throw new Exception("Ожидалась закрывающая скобка");
                                }
                            }
                            else
                            {
                                errors.Add(new ParsingError(tokens[tokens.Count - 1].end + 1, TokenType.закрывающая_скобка, ActionOverTokens.InsertAfter, "Ожидалась закрывающая скобка"));
                                throw new Exception("Ожидалась закрывающая скобка");
                            }
                        }
                        else
                            Match(TokenType.закрывающая_скобка);
                        break;
                    case TokenType.идентификатор:
                        Match(TokenType.идентификатор);
                        break;
                    case TokenType.целое_число:
                        Match(TokenType.целое_число);
                        break;
                    case TokenType.число_с_плавающей_точкой:
                        Match(TokenType.число_с_плавающей_точкой);
                        break;
                    case TokenType.логическая_константа:
                        Match(TokenType.логическая_константа);
                        break;
                    default:
                        Match(TokenType.идентификатор);
                        break;
                }
            }
            else
            {
                if (tokens.Count > 1) 
                    errors.Add(new ParsingError(tokens[currentTokenIndex - 1].end + 1, TokenType.идентификатор, ActionOverTokens.InsertAfter, "Некорректный токен"));
                else
                {
                    errors.Add(new ParsingError(tokens[currentTokenIndex - 1].end, TokenType.некорректный_токен, ActionOverTokens.Remove, "Некорректный токен"));
                    throw new Exception("Некорректный токен");
                }

            }
        }
    }
}