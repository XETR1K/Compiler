using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace @interface
{
    class Parser
    {
        private string input;
        private int pos;

        public Parser(string input)
        {
            this.input = input;
            this.pos = 0;
        }

        public bool Parse()
        {
            return ParseExpr();
        }

        private bool ParseExpr()
        {
            return ParseOrExpr() || ParseAndExpr();
        }

        private bool ParseOrExpr()
        {
            // P[‹OrExpr›]: ‹AndExpr› {||‹AndExpr›}
            int startPos = pos;

            if (ParseAndExpr())
            {
                while (pos < input.Length - 1 && input[pos] == '|' && input[pos + 1] == '|')
                {
                    pos += 2;

                    if (!ParseAndExpr())
                    {
                        pos = startPos;
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        private bool ParseAndExpr()
        {
            // P[‹AndExpr›]: ‹NotExpr› {&&‹NotExpr›}
            int startPos = pos;

            if (ParseNotExpr())
            {
                while (pos < input.Length - 1 && input[pos] == '&' && input[pos + 1] == '&')
                {
                    pos += 2;

                    if (!ParseNotExpr())
                    {
                        pos = startPos;
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        private bool ParseNotExpr()
        {
            // P[‹NotExpr›]: !‹SimpleExpr› | ‹SimpleExpr›
            int startPos = pos;

            if (pos < input.Length && input[pos] == '!')
            {
                pos++;

                if (ParseSimpleExpr())
                {
                    return true;
                }

                pos = startPos;
                return false;
            }

            return ParseSimpleExpr();
        }

        private bool ParseSimpleExpr()
        {
            // P[‹SimpleExpr›]: ‹Identifier› | ‹BooleanLiteral› | (‹Expr›) | ‹ComparisonExpr›
            return true;
        }

        private bool ParseComparisonExpr()
        {
            // P[‹ComparisonExpr›]: ‹ArithmeticExpr› ‹RelationalOp› ‹ArithmeticExpr›
            return true;
        }

        private bool ParseArithmeticExpr()
        {
            // P[‹ArithmeticExpr›]: ‹Term› {‹AdditiveOp›‹Term›}
            return true;
        }

        private bool ParseTerm()
        {
            // P[‹Term›]: ‹Factor› {‹MultiplicativeOp›‹Factor›}
            return true;
        }

        private bool ParseFactor()
        {
            // P[‹Factor›]: (‹ArithmeticExpr›) | ‹Identifier› | ‹IntegerLiteral› | ‹FloatingPointLiteral›
            return true;
        }

        private bool ParseRelationalOp()
        {
            // P[‹RelationalOp›]: < | > | <= | >= | == | !=
            return true;
        }

        private bool ParseAdditiveOp()
        {
            // P[‹AdditiveOp›]: + | -
            return true;
        }

        private bool ParseMultiplicativeOp()
        {
            // P[‹MultiplicativeOp›]: * | / | %
            return true;
        }
    }

}
