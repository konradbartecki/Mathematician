using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Threading;
using Mathematician.Resources;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Text;

namespace Mathematician
{
    public partial class Game : PhoneApplicationPage
    {
        #region members
        int difficultyTMinusValue = 7;
        double equation1StElement, equation2StElement;
        string equationOperator, equationString;
        int equationTypeInt;
        int diffMin = 2;
        int diffMax = 2;
        int diffType = 1;
        int difficulty = 4; //def: 4
        char[] equationTypes = { '+', '-', 'x', '/' };
        int classicModeTime = 180;
        int recentEquationAnswer = 4;
        /// <summary>
        /// Game Type:
        /// 0 = Classic Mode
        /// 1 = One try
        /// 2 = Three times lucky
        /// </summary>
        int gameType = 0;
        string[] gameTypeString = {AppResources.gamemodeClassic, AppResources.gamemodeOneTry, AppResources.gamemodeThreeTimesLucky};
        DispatcherTimer gameTimer = new DispatcherTimer();
        DispatcherTimer animateTimer = new DispatcherTimer();
        //Statistics variables
        int answeredCorrectly = 0;
        int answeredWrong = 0;
        #endregion

        public Game()
        {
            InitializeComponent();
            Prepare();

            StartGame();
        }

        void Prepare()
        {
            timeLeftLabel.Text = "Time left: " + classicModeTime.ToString() + " s";
            char[] myunicodechar = { '\u232B' };
            Backspace.Content = new string(myunicodechar);
            Points = 0;
           // ptsChangeLabel += 
        }
        
        void StartGame()
        {
            //Gauge Timer
            DispatcherTimer gaugeTimer = new DispatcherTimer();
            gaugeTimer.Interval = TimeSpan.FromSeconds(1);
            gaugeTimer.Tick += OnTimerTick;
            gaugeTimer.Start();
            ResetTimer();
            //
            //Game Timer
            
            gameTimer.Interval = TimeSpan.FromSeconds(1);
            gameTimer.Tick += gameTimer_Tick;
            gameTimer.Start();
            NewEquation();
        }

        void gameTimer_Tick(object sender, EventArgs e)
        {
            classicModeTime--;
            if (classicModeTime == -1)
            {
                gameTimer.Stop();
                gameOver();

            }
            else
                timeLeftLabel.Text = AppResources.gameTimeLeft + classicModeTime.ToString() + " s";
        }

        private void gameOver()
        {
            var sb = new StringBuilder();
            sb.AppendLine(AppResources.gameCorrectAnswers + answeredCorrectly.ToString());
            sb.AppendLine(AppResources.gameWrongAnswers + answeredWrong.ToString());
            sb.AppendLine(AppResources.gamePoints + Points);
            MessageBox.Show(sb.ToString(), gameTypeString[gameType], MessageBoxButton.OK);
            NavigationService.GoBack();
//            MessageBox.Show(@"Statistics
//Correct answers: " + answeredCorrectly.ToString() + @"
//Wrong answers: " + answeredWrong.ToString() + @"
//Points: " + Points.ToString() + " PTS", gameTypeString[gameType], MessageBoxButton.OK);
        }

        Int32 Seed()
        {
            return Guid.NewGuid().GetHashCode();
        }
        private int _Points = 0;
        public int Points
        {
            get { return _Points; }
            set 
            {
                _Points = value;
                PointsLabel.Text = _Points.ToString() + " " + AppResources.gamePTS; 
            }
        }

        void RandomizeEquation()
        {
            
            equationTypeInt = new Random(Seed()).Next(0, (diffType + 1));
            equationOperator = equationTypes[equationTypeInt].ToString();
            switch (equationOperator)
            {
                case "+":
                case "-":
                    Randomize();
                    break;
                case "x":
                    diffMin = Convert.ToInt32(Math.Sqrt(diffMin));
                    diffMax = Convert.ToInt32(Math.Sqrt(diffMax));
                    Randomize();
                    break;
                case "/":
                    Randomize();
                    equation1StElement = Math.Floor(Math.Sqrt(equation1StElement));
                    equation2StElement = Math.Floor(Math.Sqrt(equation2StElement));
                    equation1StElement = equation1StElement * equation2StElement;
                    break;
                #region OldStuff
                //diffMin = 0;
                    //diffMax = Convert.ToInt32(Math.Sqrt(diffMax));
                    //Randomize();
                    //int ModuloNotCorrectCount = 0;
                    //while(!IsModuloCorrect() || (CalculateEquation() == 1) )
                    //{
                    //    ModuloNotCorrectCount++;
                    //    if (ModuloNotCorrectCount > 50)
                    //    {
                    //        equationOperator = equationTypes[3].ToString();
                    //    }
                    //    setDiffMinMax();
                    //    Randomize();
                //}
                #endregion

                default:
                    throw new FormatException();
            }
            equationString = equation1StElement.ToString() + equationOperator.ToString() + equation2StElement.ToString();
        }

        void PointsChange(int value, bool IsAnswerCorrect)
        {
            if (IsAnswerCorrect)
            {
                //ptsChangeLabel.Foreground = "Lime";
                ptsChangeLabel.Text = "+" + value.ToString() + " PTS";
            }
            else
            {
                //ptsChangeLabel.Foreground = "Red";
                ptsChangeLabel.Text = "-" + value.ToString() + " PTS";
            }
        }
        void Randomize()
        {
            equation1StElement = new Random(Seed()).Next(diffMin, diffMax);
            equation2StElement = new Random(Seed()).Next(diffMin, diffMax);
        }

        void NewEquation()
        {
            RandomizeEquation();
            while(!(CalculateEquation() >= 1))
            {
                RandomizeEquation();
            }
            DisplayEquation();

        }

        double CalculateEquation()
        {
            switch(equationOperator)
            {
                case "+":
                    return equation1StElement + equation2StElement;
                case "-":
                    return equation1StElement - equation2StElement;
                case "x":
                    return equation1StElement * equation2StElement;
                case "/":
                    return equation1StElement / equation2StElement;
                default:
                    throw new FormatException();
            }
        }
        ///// <summary>
        ///// Checks equation modulo.
        ///// </summary>
        ///// <returns>Returns true if equation is divisible. False if equation answer will contain decimal places.</returns>
        //bool IsModuloCorrect()
        //{
        //    if ((equation1StElement % equation2StElement) == 0)
        //        return true;
        //    else
        //        return false;
        //}

        void ChangeDifficulty(bool IsCorrectAnswer)
        {
            if (IsCorrectAnswer)
            {
                if (difficulty < 20)
                {
                    difficulty = difficulty + 1;
                    diffType = 1;
                }
                else if (difficulty < 50)
                {
                    difficulty = difficulty + 2;
                    diffType = 3;
                }
                else if (difficulty < 200)
                {
                    difficulty = difficulty + 5;
                    diffType = 3;
                }
                    
            }
            else
            {
                if (difficulty > 4)
                {
                    difficulty = difficulty - 1;
                }
            }
            setDiffMinMax();
        }

        void setDiffMinMax()
        {
            diffMax = difficulty; //100%
            diffMin = (difficulty / 4); //* 3; //25%
        }
        
        /// <summary>
        /// Please return true if so and...
        /// you should call WrongAnswer() manually
        /// </summary>
        /// <returns></returns>
        bool CheckAnswer()
        {
            double tempEquation = CalculateEquation();
            int tempEquationLenght = tempEquation.ToString().Length;

            if(!EditBox.Text.EndsWith("."))
            {
                if (!(EditBox.Text.Length > tempEquationLenght))
                {
                    if ((EditBox.Text.Length == (tempEquationLenght)))
                    {
                        if (EditBox.Text == tempEquation.ToString())
                        {
                            CorrectAnswer();
                            return true;
                        }
                        else
                        {
                            WrongAnswer();
                            return false;
                        }
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            else
                return false;


            #region OldStuff
            //if (!EditBox.Text.EndsWith(".") //Check if it does not end with dot
            //    &&
            //    !(EditBox.Text.Length > tempEquationLenght) //Check if entered digit is longer than answer
            //    &&
            //    (EditBox.Text.Length == (tempEquationLenght)) // Check if entered digit is as long as the answer
            //    &&
            //    EditBox.Text == tempEquation.ToString())  //Check if entered digit is the answer
            //{
            //    CorrectAnswer();
            //    return true;
            //}
            //else
            //{
            //    WrongAnswer();
            //    return false;
            //}
            #endregion


        }

        
        private void OnTimerTick(object sender, EventArgs e)
        {
            if(GaugeTimeIndicator.Value <= 0)
            {
                //Timeout
                WrongAnswer();
                return;
            }

            GaugeTimeIndicator.Value = GaugeTimeIndicator.Value - difficultyTMinusValue;
        }
        int PointReward()
        {
            int result = difficulty / 10;
            if (result <= 0)
                return 1;
            else
                return result;
        }
        void WrongAnswer()
        {
            ChangeDifficulty(false);
            answeredWrong++;
            Points = Points - (PointReward() * 2);
            ResetTimer();
            NewEquation();
            ClearEditBox();
        }
        void CorrectAnswer()
        {
            ChangeDifficulty(true);
            answeredCorrectly++;
            Points = Points + PointReward();
            ResetTimer();
            NewEquation();
            ClearEditBox();
        }
        void ClearEditBox()
        {
            EditBox.Text = "0";
        }
        void ResetTimer()
        {
            GaugeTimeIndicator.Value = 100;
        }

        #region UI Functions

        void DisplayEquation()
        {
            Equation.Text = equationString;
        }
        bool IsEditBoxEmpty()
        {
            if ((EditBox.Text == "") || (EditBox.Text == "0"))
            { return true; }
            else
            { return false; }
        }
        void AddEditBox(string s)
        {
            if (IsEditBoxEmpty())
            {
                EditBox.Text = s;
            }
            else
                EditBox.Text = EditBox.Text + s;
        }    
        void BackspaceEditBox()
        {
            if (!IsEditBoxEmpty())
            {
                EditBox.Text = EditBox.Text.Remove(EditBox.Text.Length - 1);
            }
            if (EditBox.Text.Length == 0)
            {
                EditBox.Text = "0";
            }
        } 
        private void One_Click(object sender, RoutedEventArgs e)
        {
            AddEditBox("1");
        }
        private void Two_Click(object sender, RoutedEventArgs e)
        {
            AddEditBox("2");
        }
        private void Three_Click(object sender, RoutedEventArgs e)
        {
            AddEditBox("3");
        }
        private void Four_Click(object sender, RoutedEventArgs e)
        {
            AddEditBox("4");
        }
        private void Five_Click(object sender, RoutedEventArgs e)
        {
            AddEditBox("5");
        }
        private void Six_Click(object sender, RoutedEventArgs e)
        {
            AddEditBox("6");
        }
        private void Seven_Click(object sender, RoutedEventArgs e)
        {
            AddEditBox("7");
        }
        private void Eight_Click(object sender, RoutedEventArgs e)
        {
            AddEditBox("8");
        }
        private void Nine_Click(object sender, RoutedEventArgs e)
        {
            AddEditBox("9");
        }
        private void Dot_Click(object sender, RoutedEventArgs e)
        {
            //If EditBox does not contain "."
            if( !(EditBox.Text.Contains(".")) && !IsEditBoxEmpty() )
            {
                AddEditBox(".");
            }
        }
        private void Zero_Click(object sender, RoutedEventArgs e)
        {
            AddEditBox("0");
        }
        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            BackspaceEditBox();

        }
        private void EditBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!IsEditBoxEmpty())
            {
                CheckAnswer();
            }
            
        }

        #endregion

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            gameTimer.Stop();
        }

        private void AnimatePointLabel()
        {
            animateTimer.Interval = TimeSpan.FromMilliseconds(5);
            animateTimer.Tick += animateTimer_Tick;
            animateTimer.Start();
        }

        void animateTimer_Tick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}