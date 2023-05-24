using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace @interface
{
    class FiniteStateMachine
    {
        private List<Token> tokens;
        private int currentTokenIndex;
        public string result;

        private bool Compare(TokenType type)
        {
            if (tokens[tokens.Count - 1].tokenType == TokenType.точка)
               Error("Некорректное выражение.");
            if (tokens[currentTokenIndex].tokenType == type)
            {
                currentTokenIndex++;
                return true;
            }
            return false;
        }

        private void Error(string message)
        {
            if (currentTokenIndex >= tokens.Count)
            {
                result += "\nНекорректное выражение.";
                throw new Exception("Некорректное выражение.");
            }
            result += $"\n{message} Получено '{tokens[currentTokenIndex].token}'";
            throw new Exception($"\n{message} Получено '{tokens[currentTokenIndex].token}'");
        }

        public void start(List<Token> _tokens)
        {
            tokens = _tokens;
            currentTokenIndex = 0;
            result= string.Empty;
            if (tokens == null || tokens.Count == 0)
                return;
            else
            {
                result = "q0->";
                try
                {
                    State0();
                }
                catch
                {
                    return;
                }
            }
        }

        private void State0()
        {
            if (Compare(TokenType.число))
            {
                result += "q1";
                State1();
            }
            else if (Compare(TokenType.точка))
            {
                result += "q2->";
                State2();
            }
            else
                Error("Ожидалось '0','1' или '.'.");
        }

        private void State1() 
        {
            if (Compare(TokenType.число))
            {
                result += "->q1";
                State1();
            }
            else if (Compare(TokenType.знак))
            {
                result += "->q3";
                State3();
            }
            else if (Compare(TokenType.точка))
            {
                result += "->q2->";
                State2();
            }
            else
                Error("Ожидалось '0','1', 'конец выражения','+', '-' или '.'.");
        }
        private void State2()
        {
            if (Compare(TokenType.число))
            {
                result += "q4";
                State4();
            }
            else
                Error("Ожидалось '0' или '1'");
        }
        private void State3()
        {
            if (currentTokenIndex == tokens.Count)
                return;
            else
                Error("Ожидался 'конец выражения'.");
        }
        private void State4()
        {
            if (Compare(TokenType.знак))
            {
                result += "->q3";
                State3();
            }
            else if (Compare(TokenType.число))
            {
                result += "->q4";
                State4();
            }
            else
                Error("Ожидалось '0','1', 'конец выражения', '+' или '-'.");
        }

    }
}
