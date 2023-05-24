using System;
using System.Collections.Generic;
using System.Linq;

// Перечисление для описания типов лексем
public enum TokenType
{
    число,
    знак,
    точка,
    некорректный_символ
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
    public static string Reverse(string s)
    {
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    // Метод для выделения лексем
    public static List<Token> Tokenize(string input)
    {
        var tokens = new List<Token>();
        input = Reverse(input);
        int position = 0;

        while (position < input.Length)
        {
            char currentChar = input[position];
            switch (currentChar)
            {
                case '0':
                case '1':
                    tokens.Add(new Token(TokenType.число, currentChar.ToString(), position + 1, position + 1));
                    break;
                case '+':
                case '-':
                    tokens.Add(new Token(TokenType.знак, currentChar.ToString(), position + 1, position + 1));
                    break;
                case '.':
                    tokens.Add(new Token(TokenType.точка, currentChar.ToString(), position + 1, position + 1));
                    break;
                default:
                    tokens.Add(new Token(TokenType.некорректный_символ, currentChar.ToString(), position + 1, position + 1));
                    break;
            }
            position++;
        }
        return tokens;
    }
}
