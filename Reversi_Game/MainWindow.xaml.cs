using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Reversi_Game
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ReversiEngine engine;

        private SolidColorBrush[] colors = { Brushes.Ivory, Brushes.Purple, Brushes.Orange };
        string[] namesPlayer = { "", "purple", "orange" };
        private Button[,] board;
        private bool boardOn
        {
            get
            {
                return board[engine.WidthBoard - 1, engine.HeightBoard - 1] != null;
            }
        }

        private void contentBoard()
        {
            if (!boardOn) return;

            for (int i = 0; i < engine.WidthBoard; i++)
                for(int j = 0; j < engine.HeightBoard; j++)
                {
                    board[i, j].Background = colors[engine.fieldState(i, j)];
                    board[i, j].Content = engine.fieldState(i, j).ToString();
                }
            ColorOfPlayer.Background = colors[engine.numberPlayerOfNextRound];
            numberFieldsPurple.Text = engine.numberFieldsOfPlayer1.ToString();
            numberFieldsOrange.Text = engine.numberFieldsOfPlayer2.ToString();
        }

        public MainWindow()
        {
            InitializeComponent();

            engine = new ReversiEngine(1);

            for (int i = 0; i < engine.WidthBoard; i++)
                Board.ColumnDefinitions.Add(new ColumnDefinition());

            for(int j = 0; j < engine.HeightBoard; j++)
                Board.RowDefinitions.Add(new RowDefinition());

            board = new Button[engine.WidthBoard, engine.HeightBoard];
            for (int i = 0; i < engine.WidthBoard; i++)
                for (int j = 0; j < engine.HeightBoard; j++)
                {
                    Button button = new Button();
                    button.Margin = new Thickness(0);
                    Board.Children.Add(button);
                    Grid.SetColumn(button, i);
                    Grid.SetRow(button, j);
                    board[i,j] = button;
                }

            contentBoard();           
        }
    }
}
