using System;
using System.Collections.Generic;

public static class LexicalAnalyzer
{
    // Перечисление для описания типов лексем
    enum TokenType
    {
        Keyword,    // Ключевое слово
        Identifier, // Идентификатор
        Separator,  // Разделитель
        Assignment, // Оператор присваивания
        Semicolon,  // Конец оператора
        String,     // Строка
        Integer,    // Целое число
        Double,     // Вещественное число
        Invalid     // Недопустимый символ
    }

    // Словарь для хранения ключевых слов
    private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
    {
        { "int", TokenType.Keyword },
        { "long", TokenType.Keyword },
        { "short", TokenType.Keyword },
        { "boolean", TokenType.Keyword },
        { "double", TokenType.Keyword },
        { "String", TokenType.Keyword },
        { "true", TokenType.Keyword },
        { "false", TokenType.Keyword }
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
    private static List<Tuple<int, TokenType, string>> Tokenize(string input)
    {
        List<Tuple<int, TokenType, string>> tokens = new List<Tuple<int, TokenType, string>>(); // Список для хранения лексем
        int i = 0; // Индекс текущего символа
        int line = 1; //Номер текущей строки

        while (i < input.Length)
        {
            char c = input[i];

            // Если текущий символ является разделителем, пропускаем его
            if (char.IsWhiteSpace(c))
            {
                // Если это пробел или перевод строки, добавляем соответствующую лексему
                if (c == ' ')
                {
                    tokens.Add(new Tuple<int, TokenType, string>(line, TokenType.Separator, "(space)"));
                }
                else if (c == '\n')
                {
                    tokens.Add(new Tuple<int, TokenType, string>(line, TokenType.Separator, "\\n"));
                    line++;
                }
                else if (c == '\t')
                {
                    tokens.Add(new Tuple<int, TokenType, string>(line, TokenType.Separator, "\\t"));
                }
                i++;
                continue;
            }

            // Если текущий символ является оператором присваивания (=)
            if (c == '=')
            {
                tokens.Add(new Tuple<int, TokenType, string>(line, TokenType.Assignment, "="));
                i++;
                continue;
            }

            // Если текущий символ является концом оператора (;)
            if (c == ';')
            {
                tokens.Add(new Tuple<int, TokenType, string>(line, TokenType.Semicolon, ";"));
                i++;
                continue;
            }

            // Если текущий символ является концом строки (\n)
            if (c == '\n')
            {
                i++;
                continue;
            }

            // Если текущий символ является кавычкой (") - начало строки
            if (c == '"')
            {
                string str = "\"";
                i++;

                // Пока не найдем закрывающую кавычку или конец строки
                while (i < input.Length && input[i] != '\n' && input[i] != '"')
                {
                    str += input[i];
                    i++;
                }

                // Если строка закончилась на кавычку, то добавляем лексему в список
                if (i < input.Length && input[i] == '"')
                {
                    str += "\"";
                    tokens.Add(new Tuple<int, TokenType, string>(line, TokenType.String, str));
                    i++;
                }
                else // Иначе строка была не закрыта - это ошибка
                {
                    tokens.Add(new Tuple<int, TokenType, string>(line, TokenType.Invalid, str));
                }

                continue;
            }

            // Если текущий символ является цифрой, то это может быть число
            if (IsDigit(c))
            {
                string number = "";

                // Пока текущий символ является цифрой или точкой, добавляем его к числу
                while (i < input.Length && (IsDigit(input[i]) || input[i] == '.'))
                {
                    number += input[i];
                    i++;
                }

                // Если число закончилось на точку, то это ошибка
                if (number.EndsWith("."))
                {
                    tokens.Add(new Tuple<int, TokenType, string>(line, TokenType.Invalid, number));
                }
                else // Иначе добавляем лексему в список
                {
                    if (number.Contains(".")) // Если число содержит точку, то это вещественное число
                    {
                        tokens.Add(new Tuple<int, TokenType, string>(line, TokenType.Double, number));
                    }
                    else // Иначе это целое число
                    {
                        tokens.Add(new Tuple<int, TokenType, string>(line, TokenType.Integer, number));
                    }
                }

                continue;
            }

            // Если текущий символ является буквой, то это может быть ключевое слово или идентификатор
            if (IsLetter(c))
            {
                string word = "";

                // Пока текущий символ является буквой или цифрой, добавляем его к слову
                while (i < input.Length && (IsLetter(input[i]) || IsDigit(input[i])))
                {
                    word += input[i];
                    i++;
                }

                // Если слово является ключевым, то добавляем лексему в список
                if (keywords.ContainsKey(word))
                {
                    tokens.Add(new Tuple<int, TokenType, string>(line, keywords[word], word));
                }
                else // Иначе это идентификатор
                {
                    tokens.Add(new Tuple<int, TokenType, string>(line, TokenType.Identifier, word));
                }

                continue;
            }

            // Недопустимый символ - добавляем лексему в список
            tokens.Add(new Tuple<int, TokenType, string>(line, TokenType.Invalid, c.ToString()));
            i++;
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
            result += $"{token.Item1,-20} {token.Item2,-20} {token.Item3}\n";
        }
        return result;
    }
}


