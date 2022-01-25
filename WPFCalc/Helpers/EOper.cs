using System;

namespace WPFCalc.ViewModels
{ 
    public enum EOper
    {
        NONE,
        PLUS,
        MINUS,
        MULTIPLY,
        DIVIDE,
        Xpow2,
        Xpow3,
        SQRTx, 
        OneDivX 
    }

    public static class EOperUtil
    {
        public const string EOPER_PLUS_S = "+";
        public const string EOPER_MINUS_S = "-";
        public const string EOPER_MULTIPLY_S = "*";
        public const string EOPER_DIVIDE_S = "÷";
        public const string EOPER_RETDIV_S = "1divx";
        public const string EOPER_XPOW2_S = "x^2";
        public const string EOPER_XPOW3_S = "x^3";
        public const string EOPER_XSQRT_S = "sqrtx"; 
        
        public static EOper ToEOper(string str)
        {
            switch (str)
            {
                case EOPER_PLUS_S: return EOper.PLUS;
                case EOPER_MINUS_S: return EOper.MINUS;
                case EOPER_MULTIPLY_S: return EOper.MULTIPLY;
                case EOPER_DIVIDE_S: return EOper.DIVIDE;
                case EOPER_XPOW2_S: return EOper.Xpow2;
                case EOPER_XPOW3_S: return EOper.Xpow3;
                case EOPER_XSQRT_S: return EOper.SQRTx; 
                case EOPER_RETDIV_S: return EOper.OneDivX;
                default:
                    break;
            }

            return EOper.NONE;
        }


        /**
         * Функция расчета результата 
         */
        internal static double Solve(double FirstVar, double SecondVar, EOper operation)
        {

            switch (operation)
            {
                case EOper.NONE:
                    return FirstVar; 
                case EOper.PLUS:
                    return FirstVar + SecondVar;
                case EOper.MINUS:
                    return FirstVar - SecondVar;
                case EOper.MULTIPLY:
                    return FirstVar * SecondVar;
                case EOper.DIVIDE:
                    if (SecondVar == 0) return double.MinValue;
                    return FirstVar / SecondVar;
                case EOper.Xpow2:
                    return Math.Pow(FirstVar, 2);
                case EOper.Xpow3:
                    return Math.Pow(FirstVar, 3);
                case EOper.SQRTx:
                    return Math.Sqrt(FirstVar);
                case EOper.OneDivX:
                    return 1/FirstVar;
                default: return 0;
            }

        }

        /**
         * Текстовая формула
         * */
        public static string ToEquationString(EOper cmd, double x, double y = 0)
        {
            switch (cmd)
            {
                case EOper.NONE:
                    return $"{x} =";
                case EOper.PLUS:
                    return $"{x} + {y} =";
                case EOper.MINUS:
                    return $"{x} - {y} =";
                case EOper.MULTIPLY:
                    return $"{x} x {y} =";
                case EOper.DIVIDE:
                    return $"{x} ÷ {y} ="; 
                case EOper.Xpow2:
                    return $"{x}² =";
                case EOper.Xpow3:
                    return $"{x}³ =";
                case EOper.SQRTx:
                    return $"√{x} =";
                case EOper.OneDivX:
                    return $"1/{x} =";
                default:
                    return "";
            }

        }

        /**
         * Текстовый вариант оператора
         **/
        public static string ToString(EOper cmd) 
        {
            switch (cmd)
            {
                case EOper.PLUS:
                    return EOPER_PLUS_S;
                case EOper.MINUS:
                    return EOPER_MINUS_S;
                case EOper.MULTIPLY:
                    return EOPER_MULTIPLY_S;
                case EOper.DIVIDE:
                    return EOPER_DIVIDE_S;
                case EOper.Xpow2:
                    return EOPER_XPOW2_S;
                case EOper.Xpow3:
                    return EOPER_XPOW3_S;
                case EOper.SQRTx:
                    return EOPER_XSQRT_S;
                case EOper.OneDivX:
                    return EOPER_RETDIV_S;
                case EOper.NONE:
                default:
                    return "";
            }
        }
    }
}