using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFCalc.ViewModels;

namespace WPFCalc.Models
{
    /**
     * Модель приближенная к Windows 10 калькулятору
     */
    public class CalcModel
    {
        public EOper lastOperation = EOper.NONE;//ппред операция +-*/ 
        public EOper operation = EOper.NONE;//выбранная операция +-*/ 
        public double FirstVar = 0; //первое число v1
        public double SecondVar = 0; //второе число v2
        private CalcVM VM;

        public bool showResult = false; //флаг вывода окончательного результата с = 
        public bool operChanged = false;//выбран оператор, флаг для ввода второй цифры 
        public bool secVarChanged = false;//флаг ввода второго числа 

        public CalcModel(CalcVM mainWindowVM)
        {
            this.VM = mainWindowVM;
        }


        public void Parse1Var(string s)
        {
            Util.Debug($"{s}=>v1");
            ParseVar(s, true);
        }

        public void Parse2Var(string s)
        {
            Util.Debug($"{s}=>v2");
            ParseVar(s, false);
        }

        private void ParseVar(string s, bool first = true) //парсинг введего текста в переменные
        {
            Util.Debug($"Input: {s} to : {(first ? "first" : "second")}");
            try
            {
                if (first) FirstVar = Convert.ToDouble(s);
                else SecondVar = Convert.ToDouble(s);
            }
            catch (Exception e)
            {
                VM.ShowError(e.Message);
            }
        }


        /**
         * 
         * функция выбора операции  EOper       
         */ 
        public void SelectOper(EOper cmd)// 
        {

            Util.Debug(">SelectOper");
            try
            { 
                showResult = false;
                //        operChanged = operation != cmd; ;
                operChanged = true;
                lastOperation = operation;
                operation = cmd; 
                switch (cmd)
                {
                    case EOper.NONE: //reset calc (click C) 
                        Util.Debug("SelectOper NONE, reseting calc");
                        FirstVar = 0;
                        SecondVar = 0;
                        VM.UpStrokeProp = "";
                        VM.MainStrokeProp = "0";
                        break;

                   //однодейственные операции
                    case EOper.Xpow2:
                    case EOper.Xpow3:
                    case EOper.SQRTx:
                    case EOper.OneDivX:
                         
                        Util.Debug("SelectOper Xpow2");
                        Parse1Var(VM.MainStrokeProp);
                        showResult = true;
                        break; 

                    //двудейственные операции
                    case EOper.PLUS:
                    case EOper.MINUS:
                    case EOper.MULTIPLY:
                    case EOper.DIVIDE:
                        {
                            if (FirstVar == 0)
                            {
                                Util.Debug("SelectOper  задаем FirstVar");
                                Parse1Var(VM.MainStrokeProp);
                                secVarChanged = false;
                            }
                            else
                            {

                                Util.Debug("SelectOper secVarChanged="+ secVarChanged);
                                if (secVarChanged)
                                {
                                    Parse2Var(VM.MainStrokeProp);
                                }                                
                            }

                        }
                        break;
                    default:
                        break;
                }

            }
            catch (Exception e)
            {
                VM.ShowError(e.Message);
            }

            UpdateUI();
        }


        /*
         Вычисление результата
         */
        private string GetCalcResult(bool end)//вычисление результата
        {

            string valid = Validate();
            if (valid != null) return valid;//error

            Util.Debug($"Calculate: FirstVar {FirstVar} SecondVar {SecondVar} operation={(end ? operation:lastOperation)}");
            double result = EOperUtil.Solve(FirstVar, SecondVar, end?operation:lastOperation);
            Util.Debug($"Result:  => {result}");

            FirstVar = result;//для след операции результат идет в v1            
            return result.ToString();
        }


        public void ResultPress()//при нажатии =
        {
            try
            {
                showResult = true;
                operChanged = true;//?
                if (operation == EOper.NONE)
                { //операция не выбрана, просто нажали число и = 
                    Parse1Var(VM.MainStrokeProp);
                }
                else if (SecondVar == 0) Parse2Var(VM.MainStrokeProp);
                UpdateUI();
            }
            catch (Exception e)
            {
                VM.ShowError(e.Message);
            }
        }






        private void UpdateUI()//обновление текстовых полей
        {
            if (showResult)//Отобразить результат с =
            {
                secVarChanged = false;
                DisplayResult();
            }
            else //обновление строк при операциях/вводе цифр
            {
                Util.Debug("UpdateUI; выбор операции/цирф");
                if (operation != EOper.NONE)
                {

                    Util.Debug($"UpdateUI, FirstVar={FirstVar} operation={operation}; OperChanged={operChanged}; SecVarChanged={secVarChanged}");

                    VM.UpStrokeProp = FirstVar + EOperUtil.ToString(operation);//v1 *
                    if (secVarChanged) //при смене второго var пересчитывать
                    {
                        string intermediateNotUsed=GetCalcResult(false);//Считаем, результат будет в FirstVar, промежуточно его выводим
                        VM.UpStrokeProp = FirstVar + EOperUtil.ToString(operation);//v1 *
                        secVarChanged = false;
                    }
                }
            }
        }

        private void DisplayResult()
        {
            Util.Debug("UpdateUI;DisplayResult() Выводим результат");
            string valid = Validate();
            if (valid != null)//проверка на правильность, если valid задан, то выводим текст ошибки
            {
                VM.ShowError(valid);
                return;
            }


            if (operation == EOper.NONE)
            {
                VM.UpStrokeProp = $"{FirstVar} =";//оператор не задан, просто выводим введенное число
            }
            else //оператор задан, считаем, выводим
            {
                VM.UpStrokeProp = EOperUtil.ToEquationString(operation, FirstVar, SecondVar);
                VM.MainStrokeProp = GetCalcResult(true);
            }
        }

        private string Validate() //проверка коорректности данных
        {
            if (operation.Equals(EOper.DIVIDE) && SecondVar.Equals(0)) return "Делить на 0 нельзя!";
            if (operation.Equals(EOper.OneDivX) && FirstVar.Equals(0)) return "Делить на 0 нельзя!";
            if (operation.Equals(EOper.SQRTx) && FirstVar < 0) return "Неверный ввод. Нельзя так...";
            return null;//нет ошибок
        }
    }

}
