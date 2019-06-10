using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace conwayGameOfLife 
    //crtl+shift+a voor nieuwe klasse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public delegate void DataTransfer(bool[,] data);
    public partial class MainWindow : Window
    {
        public const int canvasSize = 480;
        public const int cellSize = 12;
        public const int arraySize = canvasSize / cellSize;
        private Rectangle[,] cellsArray;
        private string initialDir;
        private string currentFile = "";
        private Patern patern;
        private bool saved;
        public DataTransfer transferDelegate;
        private Timer timer = new Timer();//dispatcherTimer

        //clickevents
        public MainWindow()
        {
            InitializeComponent();
            InitializeGrid();
            Setup();
            initialDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            saved = true;
            transferDelegate += new DataTransfer(DataMethod);
            startClock("", null);
        }

        private void Setup()
        {
            patern = new Patern(arraySize);
            genCount.Text = "0";
            cellsArray = new Rectangle[arraySize, arraySize];
            for (int i = 0; i < patern.LivingCells.GetLength(0); i++)
            {
                for (int j = 0; j < patern.LivingCells.GetLength(1); j++)
                {
                    patern.LivingCells[i, j] = false;
                    cellsArray[i, j] = new Rectangle()
                    {
                        Width = cellSize,
                        Height = cellSize,
                        Margin = new Thickness(i * cellSize, j * cellSize, 0, 0)

                    };
                    if (patern.LivingCells[i, j])
                    {
                        cellsArray[i, j].Stroke = new SolidColorBrush(Colors.Yellow);
                    }
                    else
                    {
                        cellsArray[i, j].Stroke = new SolidColorBrush(Colors.Gray);
                    }

                    cellsArray[i, j].Fill = cellsArray[i, j].Stroke;
                    field.Children.Add(cellsArray[i, j]);
                }
            }
        }

        private void InitializeGrid()
        {

            for (int i = 0; i <= arraySize; i++) {
                Rectangle squares = new Rectangle
                {
                    Width = 1,
                    Height = field.Height,
                    Margin = new Thickness(i * cellSize, 0, 0, 0),
                    Stroke = new SolidColorBrush(Colors.Black)
                };
                field.Children.Add(squares);

                Rectangle squaresHorizontal = new Rectangle
                {
                    Height = 1,
                    Width = field.Height,
                    Margin = new Thickness(0, i * cellSize, 0, 0),
                    Stroke = new SolidColorBrush(Colors.Black)
                };
                
            }

        }

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            nextStepBtn.IsEnabled = true;
            startBtn.IsEnabled = false;
            clearBtn.IsEnabled = true;
            editBtn.IsEnabled = true;
            PlayBtn.IsEnabled = true;
            PauseBtn.IsEnabled = true;
            startBtn.Content = "start simulation";
            saved = false;
        }

        private void nextStepBtn_Click(object sender, RoutedEventArgs e)
        {
            prevStepBtn.IsEnabled = true;
            genCount.Text = (Convert.ToInt32(genCount.Text) + 1).ToString();
            patern.PreviousGen = patern.LivingCells;
            patern.LivingCells = patern.checkCell();
            Draw();
            saved = false;
        }

        private void prevStepBtn_Click(object sender, RoutedEventArgs e)
        {
            prevStepBtn.IsEnabled = false;
            genCount.Text = (Convert.ToInt32(genCount.Text) - 1).ToString();
            patern.LivingCells = patern.PreviousGen;
            Draw();
            saved = false;
            timer.Stop();
        }

        private void clearBtn_Click(object sender, RoutedEventArgs e)
        {
            ButtonEnableStatusStart();
            Setup();
            timer.Stop();
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            ButtonEnableStatusStart();
            startBtn.Content = "continue simulation";
            timer.Stop();
        }

        private void field_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p = Mouse.GetPosition(field);
            changeCellStatus(Convert.ToInt32(Math.Floor(p.X / cellSize)), Convert.ToInt32(Math.Floor(p.Y / cellSize)));
            saved = false;
        }

        private void changeCellStatus(int x, int y)
        {
            if (!startBtn.IsEnabled) { return; }
            patern.LivingCells[x, y] = !patern.LivingCells[x, y];
            if (patern.LivingCells[x, y])
            {
                cellsArray[x, y].Stroke = new SolidColorBrush(Colors.Yellow);
            }
            else
            {
                cellsArray[x, y].Stroke = new SolidColorBrush(Colors.Gray);
            }
            cellsArray[x, y].Fill = cellsArray[x, y].Stroke;
        }

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
        }

        private void PauseBtn_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }

        //menubar functions
        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            if (!saved) {
               if (MessageBox.Show("Are you sure you want to quit without saving??", "Very importante", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.No) {
                    return;
               }
            }
            Environment.Exit(0);
        }

        private void SaveItem_Click(object sender, RoutedEventArgs e)
        {

            if (currentFile == "")
            {
                SaveAsItem_Click(sender, e);
            }
            else
            {
                SaveFile();
            }
        }

        private void SaveAsItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.InitialDirectory = initialDir;
            if (dialog.ShowDialog() == true)
            {
                currentFile = dialog.FileName;
            }
            SaveFile();

        }

        private void LoadItem_Click(object sender, RoutedEventArgs e)
        {
            string savedField = "";
            StreamReader inputstream;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = initialDir;
            if (dialog.ShowDialog() == true) {
                currentFile = dialog.FileName;
                inputstream = File.OpenText(currentFile);
                savedField = inputstream.ReadToEnd();
            }

            if (savedField == "") { MessageBox.Show("error loading file");return; }

            string[] savedRow = savedField.Split('\n');
            for (int i = 0; i < savedRow.Length;i++)
            {
                for (int j = 0; j < savedRow[i].Length; j++)
                {
                    if ((savedRow[i][j] == '0' || savedRow[i][j] == '1') && (i>= patern.LivingCells.GetLength(0) || j>= patern.LivingCells.GetLength(1)))
                    {
                        { MessageBox.Show("This file is not a valid save file!"); return; }
                    }
                    else {
                        if (savedRow[i][j] == '1')
                        {
                            patern.LivingCells[i, j] = true;
                        }
                        else if (savedRow[i][j] == '0')
                        {
                            patern.LivingCells[i, j] = false;
                        }
                    }
                }
            }
            saved = true;
            Draw();
        }

        private void PremadeItem_Click(object sender, RoutedEventArgs e)
        {
            premadeSelection P = new premadeSelection(transferDelegate);
            P.Show();
            this.Hide();
        }

        //helpfunctions
        private void ButtonEnableStatusStart()
        {
            startBtn.IsEnabled = true;
            clearBtn.IsEnabled = false;
            prevStepBtn.IsEnabled = false;
            nextStepBtn.IsEnabled = false;
            clearBtn.IsEnabled = false;
            editBtn.IsEnabled = false;
            saved = false;
            PlayBtn.IsEnabled = false;
            PauseBtn.IsEnabled = false;
        }

        private void Draw()
        {
            for (int x = 0; x < patern.LivingCells.GetLength(0); x++)
            {
                for (int y = 0; y < patern.LivingCells.GetLength(1); y++)
                {
                    if (patern.LivingCells[x, y])
                    {
                        cellsArray[x, y].Stroke = new SolidColorBrush(Colors.Yellow);
                    }
                    else
                    {
                        cellsArray[x, y].Stroke = new SolidColorBrush(Colors.Gray);
                    }
                    cellsArray[x, y].Fill = cellsArray[x, y].Stroke;
                }
            }
        }

        private void SaveFile()
        {
            try
            {
                string savedField = "";
                for (int rowIndex = 0; rowIndex < patern.LivingCells.GetLength(0); rowIndex++)
                {
                    for (int collIndex = 0; collIndex < patern.LivingCells.GetLength(1); collIndex++)
                    {
                        if (patern.LivingCells[rowIndex, collIndex])
                        {
                            savedField += "1";
                        }
                        else
                        {
                            savedField += "0";
                        }
                    }
                    savedField += Environment.NewLine;
                }
                StreamWriter outputstream = File.CreateText(currentFile);
                outputstream.Write(savedField);
                outputstream.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("error WHILE saving file", "woops!", MessageBoxButton.OK, MessageBoxImage.Error);//without button you can't have an Image
            }
            saved = true;
        }

        public void DataMethod(bool[,] livingCells)
        {
            this.patern.LivingCells = livingCells;
            saved = true;
            this.Show();
            Draw();
        }

        private void startClock(object sender, EventArgs e)
        {
            timer.Interval = 5000 / speedTimer.Value;
            timer.Elapsed += new ElapsedEventHandler(this.timeUpdate);
        }

        private void timeUpdate(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                nextStepBtn_Click(null, null);
            });
        }

        private void speedTimer_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            timer.Interval = 5000 / speedTimer.Value;
        }
    }
}
