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
        private ReversiEngine engine = new ReversiEngine(1);

        private SolidColorBrush[] colors = { Brushes.Ivory, Brushes.Purple, Brushes.Orange };
        string[] namesPlayer = { "", "purple", "orange" };
        private Button[,] board;
        private bool boardOn
        {
            get
            {
                return board[engine.widthBoard - 1, engine.heightBoard - 1] != null;
            }
        }

        private void contentBoard()
        {
            if (!boardOn) return;

            for (int i = 0; i < engine.widthBoard; i++)
                for(int j = 0; j < engine.heightBoard; j++)
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
        }
    }
}
