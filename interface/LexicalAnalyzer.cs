using System;
using System.Collections.Generic;

public static class LexicalAnalyzer
{
    // Перечисление для описания типов лексем
    enum TokenType
    {
        KeywordInt = 1,     // Ключевое слово int
        KeywordLong,        // Ключевое слово long
        KeywordShort,       // Ключевое слово short
        KeywordBoolean,     // Ключевое слово boolean
        KeywordDouble,      // Ключевое слово double
        KeywordString,      // Ключевое слово String
        KeywordTrue,        // Ключевое слово true
        KeywordFalse,       // Ключевое слово false
        Identifier,         // Идентификатор
        SeparatorSpace,     // Разделитель пробел
        SeparatorNewLine,   // Разделитель \n
        SeparatorTab,       // Разделитель \t 
        Assignment,         // Оператор присваивания
        Semicolon,          // Конец оператора
        String,             // Строка
        Integer,            // Целое число
        Double,             // Вещественное число
        Invalid             // Недопустимый символ
    }

    // Словарь для хранения ключевых слов
    private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
    {
        { "int", TokenType.KeywordInt },
        { "long", TokenType.KeywordLong },
        { "short", TokenType.KeywordShort },
        { "boolean", TokenType.KeywordBoolean },
        { "double", TokenType.KeywordDouble },
        { "String", TokenType.KeywordString },
        { "true", TokenType.KeywordTrue },
        { "false", TokenType.KeywordFalse }
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
    private static List<Tuple<int, int, TokenType, string, int, int>> Tokenize(string input)
    {
        List<Tuple<int, int, TokenType, string, int, int>> tokens = new List<Tuple<int, int, TokenType, string, int, int>>(); // Список для хранения лексем
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
                    tokens.Add(new Tuple<int, int, TokenType, string, int, int>(line, (int)TokenType.SeparatorSpace, TokenType.SeparatorSpace, "(space)", i + 1, i + 1));
                }
                else if (c == '\n')
                {
                    tokens.Add(new Tuple<int, int, TokenType, string, int, int>(line, (int)TokenType.SeparatorNewLine, TokenType.SeparatorNewLine, "\\n", i + 1, i + 1));
                    line++;
                }
                else if (c == '\t')
                {
                    tokens.Add(new Tuple<int, int, TokenType, string, int, int>(line, (int)TokenType.SeparatorTab, TokenType.SeparatorTab, "\\t", i + 1, i + 1));
                }
                i++;
                continue;
            }

            // Если текущий символ является оператором присваивания (=)
            if (c == '=')
            {
                tokens.Add(new Tuple<int, int, TokenType, string, int, int>(line, (int)TokenType.Assignment, TokenType.Assignment, "=", i + 1, i + 1));
                i++;
                continue;
            }

            // Если текущий символ является концом оператора (;)
            if (c == ';')
            {
                tokens.Add(new Tuple<int, int, TokenType, string, int, int>(line, (int)TokenType.Semicolon, TokenType.Semicolon, ";", i + 1 , i + 1));
                i++;
                continue;
            }

            // Если текущий символ является кавычкой (") - начало строки
            if (c == '"')
            {
                string str = "\"";
                int start = i;
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
                    tokens.Add(new Tuple<int, int, TokenType, string, int, int>(line, (int)TokenType.String, TokenType.String, str, start + 1, i));
                    i++;
                }
                else // Иначе строка была не закрыта - это ошибка
                {
                    tokens.Add(new Tuple<int, int, TokenType, string, int, int>(line, (int)TokenType.Invalid, TokenType.Invalid, str, start + 1, i));
                }

                continue;
            }

            // Если текущий символ является цифрой, то это может быть число
            if (IsDigit(c))
            {
                string number = "";
                int start = i;

                // Пока текущий символ является цифрой или точкой, добавляем его к числу
                while (i < input.Length && (IsDigit(input[i]) || input[i] == '.'))
                {
                    number += input[i];
                    i++;
                }

                // Если число закончилось на точку, то это ошибка
                if (number.EndsWith("."))
                {
                    tokens.Add(new Tuple<int, int, TokenType, string, int, int>(line, (int)TokenType.Invalid, TokenType.Invalid, number, start + 1, i));
                }
                else // Иначе добавляем лексему в список
                {
                    if (number.Contains(".")) // Если число содержит точку, то это вещественное число
                    {
                        tokens.Add(new Tuple<int, int, TokenType, string, int, int>(line, (int)TokenType.Double, TokenType.Double, number, start + 1, i));
                    }
                    else // Иначе это целое число
                    {
                        tokens.Add(new Tuple<int, int, TokenType, string, int, int>(line, (int)TokenType.Integer, TokenType.Integer, number, start + 1, i));
                    }
                }

                continue;
            }

            // Если текущий символ является буквой, то это может быть ключевое слово или идентификатор
            if (IsLetter(c))
            {
                string word = "";
                int start = i;

                // Пока текущий символ является буквой или цифрой, добавляем его к слову
                while (i < input.Length && (IsLetter(input[i]) || IsDigit(input[i])))
                {
                    word += input[i];
                    i++;
                }

                // Если слово является ключевым, то добавляем лексему в список
                if (keywords.ContainsKey(word))
                {
                    tokens.Add(new Tuple<int, int, TokenType, string, int, int>(line, (int)keywords[word], keywords[word], word, start + 1, i));
                }
                else // Иначе это идентификатор
                {
                    tokens.Add(new Tuple<int, int, TokenType, string, int, int>(line, (int)TokenType.Identifier, TokenType.Identifier, word, start + 1, i));
                }

                continue;
            }

            // Недопустимый символ - добавляем лексему в список
            tokens.Add(new Tuple<int, int, TokenType, string, int, int>(line, (int)TokenType.Invalid, TokenType.Invalid, c.ToString(), i + 1, i + 1));
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
            result += $"{token.Item1,-20} код:{token.Item2,-20} {token.Item3, -20} {token.Item4, -20} c {token.Item5} по {token.Item6}\n";
        }
        return result;
    }
}

