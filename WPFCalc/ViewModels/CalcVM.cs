using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPFCalc.Models;


namespace WPFCalc.ViewModels
{
    public class CalcVM : INotifyPropertyChanged
    {

        CalcModel model;
        private bool isLightSkin = true;

        public bool IsLightSkin
        {
            get { return isLightSkin; }
            set
            {
                if (isLightSkin != value)
                {
                    isLightSkin = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsLightSkin"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsDarkSkin"));
                }
            }
        }

        public bool IsDarkSkin
        {
            get { return !isLightSkin; }
            set { }//не используется
        }

        //Команды меню
        public ICommand LightSkinCmd { get; }//светлый
        public ICommand DarkSkinCmd { get; }//темный
        public ICommand ExitCmd { get; } //выход
        public ICommand AboutCmd { get; } //о программе

        //контекстное меню TextBlock основного
        public ICommand CopyValue;

        //команды кнопок
        public ICommand OperationCmd { get; } //ui команда для + - * / =
        public ICommand DigCmd { get; }//ui команда для 0-9,
        public ICommand KeyUpCmd { get; }//ui команда event keyup


        string upStrokeProp;
        public string UpStrokeProp//верхняя текстовая часть
        {
            get => upStrokeProp;
            set
            {
                upStrokeProp = value;
                OnPropChanged();
            }
        }


        string mainStrokeProp;
        public string MainStrokeProp //основная(нижняя,ввод) текстовая часть
        {
            get => mainStrokeProp;
            set
            {
                mainStrokeProp = value;
                OnPropChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropChanged([CallerMemberName] string propertName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertName));
        }

        public CalcVM()
        {
            ToLightSkin(true);
            model = new CalcModel(this);
            UpStrokeProp = "";
            MainStrokeProp = "0";
            OperationCmd = new RelayCommand(OperationCmdExecute, null);
            DigCmd = new RelayCommand(DigCmdExecute, null);
            KeyUpCmd = new RelayCommand(KeyUp, null);
            LightSkinCmd = new RelayCommand((object o) => { ToLightSkin(true); }, null);
            DarkSkinCmd = new RelayCommand((object o) => { ToLightSkin(false); }, null);
            ExitCmd = new RelayCommand((object o) => { Application.Current.Shutdown(); }, null);
            AboutCmd = new RelayCommand((object o) => { MessageBox.Show("Калькулятор by dizz74"); }, null);
            CopyValue = new RelayCommand((object o) => { Clipboard.SetText(MainStrokeProp); }, null);
        }

        private void ToLightSkin(bool isLight)
        {


            IsLightSkin = isLight;
            Application.Current.MainWindow.Resources.MergedDictionaries.Clear();
            Uri theme = new Uri(isLight ? "LightTheme.xaml" : "DarkTheme.xaml", UriKind.Relative);
            ResourceDictionary themeDict = Application.LoadComponent(theme) as ResourceDictionary;
            Application.Current.MainWindow.Resources.MergedDictionaries.Add(themeDict);
        }


        public void ShowError(string msg)
        {
            System.Diagnostics.StackTrace t = new System.Diagnostics.StackTrace();
            Util.Debug(t.ToString());
            model.SelectOper(EOper.NONE);
            Util.Debug($"Show error{msg}");
            MainStrokeProp = "0";
            UpStrokeProp = $"Ошибка:{msg}";
        }

        /**
         * IКоманда для кнопок операций(кнопки кроме цифр) C,CE,+  - * x2 =
         * принимает комманд параметр с контрола
         * */
        private void OperationCmdExecute(object obj)
        {
            try
            {
                string cmd = $"{obj}";
                Util.Debug("Oper CMD:" + obj);
                switch (cmd)
                {

                    case "CE":  //очистка ввода
                        ClearInput();
                        break;
                    case "C":                       //очистка всего
                        ResetCalc();
                        break;
                    case "changesig":
                        ChangeSig();
                        break;
                    case "pi":
                        EnterPI();
                        break;
                    case EOperUtil.EOPER_PLUS_S:
                    case EOperUtil.EOPER_MINUS_S:
                    case EOperUtil.EOPER_MULTIPLY_S:
                    case EOperUtil.EOPER_DIVIDE_S:
                    case EOperUtil.EOPER_XPOW2_S:
                    case EOperUtil.EOPER_XPOW3_S:
                    case EOperUtil.EOPER_XSQRT_S:
                    case EOperUtil.EOPER_RETDIV_S:
                        model.SelectOper(EOperUtil.ToEOper(cmd));
                        break;
                    case "=":
                        model.ResultPress();
                        break;
                }
            }
            catch (Exception e)
            {
                ShowError($"OperationCmdExecute={e.Message}");
            }
        }

        private void EnterPI() //3.14.....
        {
            MainStrokeProp = Math.PI.ToString();
        }

        private void ChangeSig()// +-
        {
            try
            {
                double d = Convert.ToDouble(MainStrokeProp);
                MainStrokeProp = (-d).ToString();
            }
            catch (Exception e)
            {
                ShowError($"Changesig{e.Message}");
            }
        }

        private void ResetCalc()//Сброс
        {
            model.SelectOper(EOper.NONE);
        }

        private void ClearInput()//CE  оочистка строки ввода
        {
            model.SecondVar = 0;
            MainStrokeProp = "0";
            model.showResult = false;
        }



        private void KeyUp(object cmd)//Привязка команд к KeyUP
        {
            Key key = (Key)cmd;
            switch (key)
            {
                case Key.Escape:
                    OperationCmdExecute("C");
                    break;
                case Key.Delete:
                    OperationCmdExecute("CE");
                    break;
                case Key.Add:
                    OperationCmdExecute(EOperUtil.EOPER_PLUS_S);
                    break;
                case Key.Subtract:
                    OperationCmdExecute(EOperUtil.EOPER_MINUS_S);
                    break;
                case Key.Multiply:
                    OperationCmdExecute(EOperUtil.EOPER_MULTIPLY_S);
                    break;
                case Key.Divide:
                    OperationCmdExecute(EOperUtil.EOPER_DIVIDE_S);
                    break;
                case Key.Decimal:
                    DigCmdExecute(",");
                    break;

                case Key.Enter:
                    OperationCmdExecute("=");
                    break;
            }

            if (key >= Key.D0 && key <= Key.D9) DigCmdExecute(key - 34);
            if (key >= Key.NumPad0 && key <= Key.NumPad9) DigCmdExecute(key - 74);
        }


        private void DigCmdExecute(object newDig)//команда ввода для 0-9 и +,
        {
            Util.Debug("Dig CMD:" + newDig);
            try
            {
                if (model.operChanged)//при смене оператора обнулять ввод
                {
                    MainStrokeProp = "0";
                    model.operChanged = false;
                    model.SecondVar = 0;
                }


                if (model.showResult)
                {//после  вычисление 2х3=6 и нажатия цифры  - новый расчет, сброс.
                    ResetCalc();
                }

                if (newDig.Equals(","))
                {
                    if (!MainStrokeProp.Contains(",")) MainStrokeProp += $",";  //добавить , если нет
                }
                else //ввод 0-9
                {

                    int digit = Convert.ToInt32(newDig); //0-9
                    if (MainStrokeProp.Equals("0")) MainStrokeProp = $"{digit}";
                    else MainStrokeProp += $"{digit}";

                }

                if (model.operation != EOper.NONE) model.secVarChanged = true; //если оператор задан  и число ввелось, то флаг SecVarChanged 

            }
            catch (Exception e)
            {
                ShowError($"void DIGcmd={e.Message}");
            }
        }


    }
}
