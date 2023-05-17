using System;
using System.Collections.Generic;

namespace @interface
{
    static class Parser
    {
        //Список лексем
        private static List<Tuple<TokenType, string, int, int>> tokens;
        //Позиция текущей лексемы
        private static int currentTokenIndex;
        private static bool isBoolIndetifier;

        //Ошибки
        private static List<string> errors;
        public static List<string> getErrors() { return errors; }
        public static void clearErrorsList() { errors.Clear(); }
/*        public static void errorAdd(Exception e) //Добавление ошибок
        {
            if (errors.Contains(e.Message))
                return;
            else
                errors.Add(e.Message);
        }*/

        //метод для обработки ошибок
        private static void Error(string message)
        {
            if (currentTokenIndex >= tokens.Count) 
            {
                errors.Add($"Ошибка в позиции {tokens[tokens.Count - 1].Item4 + 1}: {message}");
                throw new Exception($"Ошибка в позиции {tokens[tokens.Count - 1].Item4 + 1}: {message}");
            }
            else
            {
                errors.Add($"Ошибка в позиции {tokens[currentTokenIndex].Item3}: {message}");
                throw new Exception($"Ошибка в позиции {tokens[currentTokenIndex].Item3}: {message}");
            }
            
        }

        //метод для проверки текущей лексемы и перехода к следующей
        private static void Match(TokenType expectedToken)
        {
            if (currentTokenIndex < tokens.Count)
            {
                if (tokens[currentTokenIndex].Item1 == expectedToken)
                    currentTokenIndex++;
                else
                    Error($"Ожидалась лексема {expectedToken}, получена {tokens[currentTokenIndex].Item1}");
            }
        }

        //метод для проверки текущей лексемы без перехода к следующей
        private static bool Check(TokenType expectedToken)
        {
            if (currentTokenIndex == tokens.Count)
                return false;
            return tokens[currentTokenIndex].Item1 == expectedToken;
        }

        //метод анализа строки
        public static void Parse(List<Tuple<TokenType, string, int, int>> inputTokens)
        {
            tokens = inputTokens;
            currentTokenIndex = 0;
            errors = new List<string>();

            try
            {
                Expr();
                if (currentTokenIndex != tokens.Count - 1)
                    Error("Некорректное выражение");
            }
            catch
            {
                return;
            }
        }

        //‹Expr› → ‹AndExpr› {||‹AndExpr›}
        private static void Expr()
        {
            AndExpr();
            while ((currentTokenIndex < tokens.Count) && Check(TokenType.LogicalOp) && tokens[currentTokenIndex].Item2 == "||")
            {
                Match(TokenType.LogicalOp);
                AndExpr();
            }
        }

        //‹AndExpr› → ‹NotExpr› {&&‹NotExpr›}
        private static void AndExpr()
        {
            NotExpr();
            while ((currentTokenIndex < tokens.Count) && Check(TokenType.LogicalOp) && tokens[currentTokenIndex].Item2 == "&&")
            {
                Match(TokenType.LogicalOp);
                NotExpr();
            }
        }

        //‹NotExpr› → !‹SimpleExpr› | ‹SimpleExpr›
        private static void NotExpr()
        {
            if (Check(TokenType.Not))
            {
                isBoolIndetifier = true;
                Match(TokenType.Not);
            }
            SimpleExpr();
        }
        //метод для определения, нужно ли переходить к ComparisonExpr
        private static bool isComparisonExpr()
        {
            if (!isBoolIndetifier && currentTokenIndex < tokens.Count && (Check(TokenType.RelationalOp) || Check(TokenType.AdditiveOp) || Check(TokenType.MultiplicativeOp)))
            {
                currentTokenIndex--;
                ComparisonExpr();
                return true;
            }
            return false;
        }
        //‹SimpleExpr› → ‹Identifier› | ‹BooleanLiteral› | (‹Expr›) | ‹ComparisonExpr›
        private static void SimpleExpr()
        {
            if (currentTokenIndex < tokens.Count)
            {
                switch (tokens[currentTokenIndex].Item1)
                {
                    case TokenType.Identifier:
                        Match(TokenType.Identifier);
                        isComparisonExpr();
                        isBoolIndetifier = false;
                        break;
                    case TokenType.IntegerLiteral:
                        Match(TokenType.IntegerLiteral);
                        if (!isComparisonExpr())
                            Error("Ожидался идентификатор, логическая константа, число или открывающая скобка");
                        break;
                    case TokenType.FloatingPointLiteral:
                        Match(TokenType.FloatingPointLiteral);
                        if(!isComparisonExpr())
                            Error("Ожидался идентификатор, логическая константа, число или открывающая скобка");
                        break;
                    case TokenType.BooleanLiteral:
                        Match(TokenType.BooleanLiteral);
                        break;
                    case TokenType.LeftParen:
                        Match(TokenType.LeftParen);
                        isBoolIndetifier = false;
                        Expr();
                        if (currentTokenIndex < tokens.Count)
                        {
                            if (Check(TokenType.RightParen))
                                Match(TokenType.RightParen);
                            else
                                Error("Отсутсвует закрывающая скобка.");
                        }
                        else
                            Error("Отсутсвует закрывающая скобка.");
                        break;
                    default:
                        Error("Ожидался идентификатор, логическая константа, число или открывающая скобка");
                        break;
                }
            }
            else
                Error("Некорректное выражение");
        }

        //‹ComparisonExpr› → ‹ArithmeticExpr› ‹RelationalOp› ‹ArithmeticExpr›
        private static void ComparisonExpr()
        {
            ArithmeticExpr();
            Match(TokenType.RelationalOp);
            ArithmeticExpr();
        }

        //‹ArithmeticExpr› → ‹Term› {‹AdditiveOp›‹Term›}
        private static void ArithmeticExpr()
        {
            Term();
            while ((currentTokenIndex < tokens.Count) && Check(TokenType.AdditiveOp))
            {
                Match(TokenType.AdditiveOp);
                Term();
            }
        }

        //‹Term› → ‹Factor› {‹MultiplicativeOp›‹Factor›}
        private static void Term()
        {
            Factor();
            while ((currentTokenIndex < tokens.Count) && Check(TokenType.MultiplicativeOp))
            {
                Match(TokenType.MultiplicativeOp);
                Factor();
            }
        }

        //‹Factor› → (‹ArithmeticExpr›) | ‹Identifier› | ‹IntegerLiteral› | ‹FloatingPointLiteral›
        private static void Factor()
        {
            if (currentTokenIndex < tokens.Count)
            {
                switch (tokens[currentTokenIndex].Item1)
                {
                    case TokenType.LeftParen:
                        Match(TokenType.LeftParen);
                        ArithmeticExpr();
                        Match(TokenType.RightParen);
                        break;
                    case TokenType.IntegerLiteral:
                        Match(TokenType.IntegerLiteral);
                        break;
                    case TokenType.FloatingPointLiteral:
                        Match(TokenType.FloatingPointLiteral);
                        break;
                    case TokenType.Identifier:
                        Match(TokenType.Identifier);
                        break;
                    default:
                        Error("Ожидался идентификатор, число или открывающая скобка");
                        break;
                }
            }
            else
                Error("Некорректное выражение");
        }
    }
}