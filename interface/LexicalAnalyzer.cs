using System;
using System.Collections.Generic;

// Перечисление для описания типов лексем
public enum TokenType
{
    Identifier, //идентификатор
    IntegerLiteral, //целое число
    FloatingPointLiteral, //число с плавающей точкой
    BooleanLiteral, //true и false
    RelationalOp, //операторы сравнения
    AdditiveOp, MultiplicativeOp,//арифметические операторы
    LogicalOp, //логические операторы
    Not, //оператор отрицания
    LeftParen, RightParen, //скобки
    Invalid //некорректный символ
}

public static class LexicalAnalyzer
{
    // Словарь для хранения лексем
    private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
    {
        // Ключевые слова
        { "true", TokenType.BooleanLiteral },
        { "false", TokenType.BooleanLiteral },
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
    public static List<Tuple<TokenType, string, int, int>> Tokenize(string input)
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
                    tokens.Add(new Tuple<TokenType, string, int, int>(keywords[identifier], identifier, start + 1, position));
                else
                    tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.Identifier, identifier, start + 1, position));
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
                    tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.FloatingPointLiteral, number, start + 1, position));
                }
                else
                {
                    tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.IntegerLiteral, number, start + 1, position));
                }

                continue;
            }

            // Операторы и скобки
            switch (currentChar)
            {
                case '!':
                    if (position < input.Length - 1 && input[position + 1] == '=')
                    {
                        // Оператор !=
                        tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.RelationalOp, "!=", position + 1, position + 2));
                        position += 2;
                    }
                    else
                    {   // Оператор !
                        tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.Not, "!", position + 1, position + 1));
                        position++;
                    }
                    break;

                case '&':
                    if (position < input.Length - 1 && input[position + 1] == '&')
                    {
                        tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.LogicalOp, "&&", position + 1, position + 2));
                        position += 2;
                    }
                    else
                    {
                        tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.Invalid, currentChar.ToString(), position + 1, position + 1));
                        position++;
                    }
                    break;

                case '|':
                    if (position < input.Length - 1 && input[position + 1] == '|')
                    {
                        tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.LogicalOp, "||", position + 1, position + 2));
                        position += 2;
                    }
                    else
                    {
                        tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.Invalid, currentChar.ToString(), position + 1, position + 1));
                        position++;
                    }
                    break;

                case '<':
                    // Оператор <
                    tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.RelationalOp, "<", position + 1, position + 1));
                    position++;
                    break;

                case '>':
                    // Оператор >
                    tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.RelationalOp, ">", position + 1, position + 1));
                    position++;
                    break;

                case '=':
                    if (position < input.Length - 1 && input[position + 1] == '=')
                    {
                        // Оператор ==
                        tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.RelationalOp, "==", position + 1, position + 2));
                        position += 2;
                    }
                    else
                    {   //Некорректный символ
                        tokens.Add(Tuple.Create(TokenType.Invalid, "=", position + 1, position + 1));
                        position++;
                    }
                    break;

                case '+':
                    // Оператор +
                    tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.AdditiveOp, "+", position + 1, position + 1));
                    position++;
                    break;

                case '-':
                    // Оператор -
                    tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.AdditiveOp, "-", position + 1, position + 1));
                    position++;
                    break;

                case '*':
                    // Оператор *
                    tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.MultiplicativeOp, "*", position + 1, position + 1));
                    position++;
                    break;

                case '/':
                    // Оператор /
                    tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.MultiplicativeOp, "/", position + 1, position + 1));
                    position++;
                    break;

                case '(':
                    // Открывающая скобка
                    tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.LeftParen, "(", position + 1, position + 1));
                    position++;
                    break;

                case ')':
                    // Закрывающая скобка
                    tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.RightParen, ")", position + 1, position + 1));
                    position++;
                    break;

                default:
                    // Некорректный символ
                    tokens.Add(new Tuple<TokenType, string, int, int>(TokenType.Invalid, input[position].ToString(), position + 1, position + 1));
                    position++;
                    break;
            }
        }
        return tokens;
    }
}

