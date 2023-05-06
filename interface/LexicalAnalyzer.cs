using System;
using System.Collections.Generic;

public static class LexicalAnalyzer
{
    // Перечисление для описания типов лексем
    enum TokenType
    {
        Identifier, //идентификатор
        IntegerLiteral, //целое число
        FloatingPointLiteral, //число с плавающей точкой
        BooleanLiteral, //true и false
        RelationalOp, //Операторы сравнения
        AdditiveOp, MultiplicativeOp,//арифметические операторы
        LogicalOp, //логические операторы
        Not, //оператор отрицания
        LeftParen, RightParen, //скобки
        Invalid
    }

    // Словарь для хранения лексем
    private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
    {
        // Ключевые слова
        { "true", TokenType.BooleanLiteral },
        { "false", TokenType.BooleanLiteral },

        // Операторы
        { "!", TokenType.Not },
        { "&&", TokenType.LogicalOp },
        { "||", TokenType.LogicalOp },
        { "<", TokenType.RelationalOp },
        { ">", TokenType.RelationalOp },
        { "<=", TokenType.RelationalOp },
        { ">=", TokenType.RelationalOp },
        { "==", TokenType.RelationalOp },
        { "!=", TokenType.RelationalOp },
        { "+", TokenType.AdditiveOp },
        { "-", TokenType.AdditiveOp },
        { "*", TokenType.MultiplicativeOp },
        { "/", TokenType.MultiplicativeOp },
        { "%", TokenType.MultiplicativeOp },

        // Скобки
        { "(", TokenType.LeftParen },
        { ")", TokenType.RightParen },
    };

    // Метод для проверки, является ли символ буквой
    private static bool IsLetter(char c)
    {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
    }

    // Метод для проверки, является ли символ цифрой
    private static bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

    // Метод для выделения лексем
    private static List<Tuple<TokenType, string, int, int>> Tokenize(string input)
    {
        var tokens = new List<Tuple<TokenType, string, int, int>>();

        int position = 0;

        while (position < input.Length)
        {
            char currentChar = input[position];

            // Пропускаем пробелы и символы перевода строки
            if (char.IsWhiteSpace(currentChar))
            {
                position++;
                continue;
            }

            // Идентификаторы и ключевые слова
            if (IsLetter(currentChar))
            {
                int start = position;
                while (position < input.Length && (IsLetter(input[position]) || IsDigit(input[position])))
                {
                    position++;
                }
                string identifier = input.Substring(start, position - start);
                if (keywords.ContainsKey(identifier))
                    tokens.Add(new Tuple<TokenType, string, int, int>(keywords[identifier], identifier, start, position - 1));
                else
                    tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.Identifier, identifier, start, position - 1));
                continue;
            }

            // Целые и дробные числа
            if (IsDigit(currentChar) || currentChar == '.')
            {
                int start = position;
                bool hasDecimalPoint = false;

                while (position < input.Length && (IsDigit(input[position]) || (!hasDecimalPoint && input[position] == '.')))
                {
                    if (input[position] == '.')
                    {
                        hasDecimalPoint = true;
                    }
                    position++;
                }

                string number = input.Substring(start, position - start);

                if (hasDecimalPoint)
                {
                    tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.FloatingPointLiteral, number, start, position - 1));
                }
                else
                {
                    tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.IntegerLiteral, number, start, position - 1));
                }

                continue;
            }

            // Операторы и скобки
            switch (currentChar)
            {
                case '!':
                    tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.Not, "!", position, position));
                    position++;
                    break;
                case '&':
                    if (position < input.Length - 1 && input[position + 1] == '&')
                    {
                        tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.LogicalOp, "&&", position, position + 1));
                        position += 2;
                    }
                    else
                    {
                        tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.Invalid, currentChar.ToString(), position, position));
                        position++;
                    }
                    break;
                case '|':
                    if (position < input.Length - 1 && input[position + 1] == '|')
                    {
                        tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.LogicalOp, "||", position, position + 1));
                        position += 2;
                    }
                    else
                    {
                        tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.Invalid, currentChar.ToString(), position, position));
                        position++;
                    }
                    break;
                case '<':
                    if (position < input.Length - 1 && input[position + 1] == '=')
                    {
                        // Оператор <=
                        tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.RelationalOp, "<=", position, position + 1));
                        position += 2;
                    }
                    else
                    {
                        // Оператор <
                        tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.RelationalOp, "<", position, position));
                        position++;
                    }
                    break;

                case '>':
                    if (position < input.Length - 1 && input[position + 1] == '=')
                    {
                        // Оператор >=
                        tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.RelationalOp, ">=", position, position + 1));
                        position += 2;
                    }
                    else
                    {
                        // Оператор >
                        tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.RelationalOp, ">", position, position));
                        position++;
                    }
                    break;
                case '=':
                    if (position < input.Length - 1 && input[position + 1] == '=')
                    {
                        // Оператор ==
                        tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.RelationalOp, "==", position, position + 1));
                        position += 2;
                    }
                    else
                    {   //Некорректный символ
                        tokens.Add(Tuple.Create(TokenType.Invalid, "=", position, position));
                        position++;
                    }
                    break;

                case '+':
                    // Оператор +
                    tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.AdditiveOp, "+", position, position));
                    position++;
                    break;

                case '-':
                    // Оператор -
                    tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.AdditiveOp, "-", position, position));
                    position++;
                    break;

                case '*':
                    // Оператор *
                    tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.MultiplicativeOp, "*", position, position));
                    position++;
                    break;

                case '/':
                    // Оператор /
                    tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.MultiplicativeOp, "/", position, position));
                    position++;
                    break;

                case '%':
                    // Оператор %
                    tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.MultiplicativeOp, "%", position, position));
                    position++;
                    break;

                case '(':
                    // Открывающая скобка
                    tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.LeftParen, "(", position, position));
                    position++;
                    break;

                case ')':
                    // Закрывающая скобка
                    tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.RightParen, ")", position, position));
                    position++;
                    break;

                default:
                    // Некорректный символ
                    tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.Invalid, input[position].ToString(), position, position));
                    position++;
                    break;
            }
        }
        return tokens;
    }
    //Метод для запуска сканера
    public static string RunScanner(string input)
    {
        var tokens = LexicalAnalyzer.Tokenize(input);
        string result = "";
        foreach (var token in tokens)
        {
            result += $"{token.Item1,-20} {token.Item2,-20} с {token.Item3} до {token.Item4}\n";
        }
        return result;
    }
}

