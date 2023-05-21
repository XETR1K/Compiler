using System;
using System.Collections.Generic;

// Перечисление для описания типов лексем
public enum TokenType
{
    идентификатор, //идентификатор
    целое_число, //целое число
    число_с_плавающей_точкой, //число с плавающей точкой
    логическая_константа, //true и false
    оператор_сравнения, //операторы сравнения
    оператор_сложения_вычитания, оператор_умножения_деления,//арифметические операторы
    логический_оператор, //логические операторы
    оператор_отрицания, //оператор отрицания
    открывающая_скобка, закрывающая_скобка, //скобки
    некорректный_токен //некорректный токен
}

//Класс токен
public class Token
{
    public TokenType tokenType { get; set; }
    public string token { get; set; }
    public int start { get; set; }
    public int end { get; set; }

    public Token(TokenType _tokenType, string _token, int _start, int _end)
    {
        tokenType = _tokenType;
        token = _token;
        start = _start;
        end = _end;
    }
}

public static class LexicalAnalyzer
{
    // Словарь для хранения лексем
    private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
    {
        // Ключевые слова
        { "true", TokenType.логическая_константа },
        { "false", TokenType.логическая_константа },
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
    public static List<Token> Tokenize(string input)
    {
        var tokens = new List<Token>();

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
                    tokens.Add(new Token(keywords[identifier], identifier, start + 1, position));
                else
                    tokens.Add(new Token(TokenType.идентификатор, identifier, start + 1, position));
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

                if (number == ".")
                {
                    tokens.Add(new Token(TokenType.некорректный_токен, number, start + 1, position));
                }
                else
                {

                    if (hasDecimalPoint)
                    {
                        tokens.Add(new Token(TokenType.число_с_плавающей_точкой, number, start + 1, position));
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.целое_число, number, start + 1, position));
                    }

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
                        tokens.Add(new Token(TokenType.оператор_сравнения, "!=", position + 1, position + 2));
                        position += 2;
                    }
                    else
                    {   // Оператор !
                        tokens.Add(new Token(TokenType.оператор_отрицания, "!", position + 1, position + 1));
                        position++;
                    }
                    break;

                case '&':
                    if (position < input.Length - 1 && input[position + 1] == '&')
                    {
                        tokens.Add(new Token(TokenType.логический_оператор, "&&", position + 1, position + 2));
                        position += 2;
                    }
                    else
                    {
                        int _start = position;
                        while (position < input.Length && !char.IsWhiteSpace(input[position]) && !IsLetter(input[position]) && !IsDigit(input[position]))
                        {
                            position++;
                        }
                        string _invalidToken = input.Substring(_start, position - _start);
                        tokens.Add(new Token(TokenType.некорректный_токен, _invalidToken, _start + 1, position));
                    }
                    break;

                case '|':
                    if (position < input.Length - 1 && input[position + 1] == '|')
                    {
                        tokens.Add(new Token(TokenType.логический_оператор, "||", position + 1, position + 2));
                        position += 2;
                    }
                    else
                    {
                        int _start = position;
                        while (position < input.Length && !char.IsWhiteSpace(input[position]) && !IsLetter(input[position]) && !IsDigit(input[position]))
                        {
                            position++;
                        }
                        string _invalidToken = input.Substring(_start, position - _start);
                        tokens.Add(new Token(TokenType.некорректный_токен, _invalidToken, _start + 1, position));
                    }
                    break;

                case '<':
                    // Оператор <
                    tokens.Add(new Token(TokenType.оператор_сравнения, "<", position + 1, position + 1));
                    position++;
                    break;

                case '>':
                    // Оператор >
                    tokens.Add(new Token(TokenType.оператор_сравнения, ">", position + 1, position + 1));
                    position++;
                    break;

                case '=':
                    if (position < input.Length - 1 && input[position + 1] == '=')
                    {
                        // Оператор ==
                        tokens.Add(new Token(TokenType.оператор_сравнения, "==", position + 1, position + 2));
                        position += 2;
                    }
                    else
                    {   //Некорректный символ
                        int _start = position;
                        while (position < input.Length && !char.IsWhiteSpace(input[position]) && !IsLetter(input[position]) && !IsDigit(input[position]))
                        {
                            position++;
                        }
                        string _invalidToken = input.Substring(_start, position - _start);
                        tokens.Add(new Token(TokenType.некорректный_токен, _invalidToken, _start + 1, position));
                    }
                    break;

                case '+':
                    // Оператор +
                    tokens.Add(new Token(TokenType.оператор_сложения_вычитания, "+", position + 1, position + 1));
                    position++;
                    break;

                case '-':
                    // Оператор -
                    tokens.Add(new Token(TokenType.оператор_сложения_вычитания, "-", position + 1, position + 1));
                    position++;
                    break;

                case '*':
                    // Оператор *
                    tokens.Add(new Token(TokenType.оператор_умножения_деления, "*", position + 1, position + 1));
                    position++;
                    break;

                case '/':
                    // Оператор /
                    tokens.Add(new Token(TokenType.оператор_умножения_деления, "/", position + 1, position + 1));
                    position++;
                    break;

                case '(':
                    // Открывающая скобка
                    tokens.Add(new Token(TokenType.открывающая_скобка, "(", position + 1, position + 1));
                    position++;
                    break;

                case ')':
                    // Закрывающая скобка
                    tokens.Add(new Token(TokenType.закрывающая_скобка, ")", position + 1, position + 1));
                    position++;
                    break;

                default:
                    // Некорректный символ
                    int start = position;
                    while (position < input.Length && !char.IsWhiteSpace(input[position]) && !IsLetter(input[position]) && !IsDigit(input[position]))
                    {
                        position++;
                    }
                    string invalidToken = input.Substring(start, position - start);
                    tokens.Add(new Token(TokenType.некорректный_токен, invalidToken, start + 1, position));
                    break;
            }
        }
        return tokens;
    }
}

