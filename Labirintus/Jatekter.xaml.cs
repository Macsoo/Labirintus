using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Labirintus
{
    /// <summary>
    /// Interaction logic for Jatekter.xaml
    /// </summary>
    public partial class Jatekter : Window
    {
        private Labyrinth lab;

        public Jatekter()
        {
            InitializeComponent();
            lab = new Labyrinth(labirintus);
            lab.Megjelenit();
        }

        public Jatekter(string fajlnev)
        {
            InitializeComponent();
            lab = new Labyrinth(labirintus, fajlnev);
            lab.Megjelenit();
        }

        private void BtnVissza_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Biztosan ki akar lépni a főmenübe?", "Visszalépés", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }

        private void BtnBalra_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BtnElore_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BtnJobbra_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BtnAutomata_Click(object sender, RoutedEventArgs e)
        {
            lab.GenerateLabyrinth();
        }
    }

    internal class Labyrinth
    {
        private char[,] Labirint;
        private Grid labirintus;
        private Border[,] cellak;
        private Pos jatekos = new Pos(1, 1);

        public Labyrinth(Grid labirintus, string fajlnev)
        {
            this.labirintus = labirintus;
            Labirint = new char[9, 9];
            for (int i = 0; i < 9; i++)
            {
                labirintus.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                labirintus.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }
            cellak = new Border[9, 9];
            for (int i = 0; i < 9 * 9; i++)
            {
                cellak[i / 9, i % 9] = new Border();
                cellak[i / 9, i % 9].KeyDown += Lenyom;
                labirintus.Children.Add(cellak[i / 9, i % 9]);
                Grid.SetRow(cellak[i / 9, i % 9], i / 9);
                Grid.SetColumn(cellak[i / 9, i % 9], i % 9);
            }
            string[] forras = new string[9];
            int index = 0;
            StreamReader sr = new StreamReader(fajlnev);
            while (!sr.EndOfStream)
            {
                forras[index] = sr.ReadLine();
                index++;
            }
            sr.Close();
            for (int sor = 0; sor < 9; sor++)
            {
                for (int oszlop = 0; oszlop < 9; oszlop++)
                {
                    Labirint[sor, oszlop] = forras[sor][oszlop];
                }
            }
        }

        public Labyrinth(Grid labirintus)
        {
            this.labirintus = labirintus;
            Labirint = new char[9, 9];
            for (int i = 0; i < 9; i++)
            {
                labirintus.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                labirintus.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }
            cellak = new Border[9, 9];
            for (int i = 0; i < 9 * 9; i++)
            {
                cellak[i / 9, i % 9] = new Border();
                cellak[i / 9, i % 9].KeyDown += Lenyom;
                labirintus.Children.Add(cellak[i / 9, i % 9]);
                Grid.SetRow(cellak[i / 9, i % 9], i / 9);
                Grid.SetColumn(cellak[i / 9, i % 9], i % 9);
            }
            //GenerateLabyrinth();
        }

        public void Megjelenit()
        {
            for (int sor = 0; sor < 9; sor++)
            {
                for (int oszlop = 0; oszlop < 9; oszlop++)
                {
                    Border cella = cellak[sor, oszlop];
                    if (oszlop == 1 && sor == 1)
                    {
                        // 1. sor, 1. oszlop, a játékos alphelyzete
                        System.Windows.Point Point1 = new System.Windows.Point(26.8, 40.2);
                        System.Windows.Point Point2 = new System.Windows.Point(13.4, 13.4);
                        System.Windows.Point Point3 = new System.Windows.Point(40.2, 13.4);
                        PointCollection myPointCollection = new PointCollection();
                        myPointCollection.Add(Point1);
                        myPointCollection.Add(Point2);
                        myPointCollection.Add(Point3);
                        cella.Child = new Polygon() { Stroke = Brushes.Black, StrokeThickness = 4, Fill = Brushes.Green, Points = myPointCollection, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
                    }
                    else if (Labirint[sor, oszlop] == '*')
                    {
                        cella.Background = Brushes.Black;
                    }
                    else if (Labirint[sor, oszlop] == '.')
                    {
                        cella.Background = Brushes.White;
                    }
                }
            }
        }

        private void Lenyom(object sender, KeyEventArgs args)
        {
        }

        private struct Pos
        {
            public int x, y;

            public Pos(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        private struct Jatekos
        {
            public Pos pozició;
            public char irány;

            public Jatekos(Pos poz, char ir)
            {
                pozició = poz;
                irány = ir;
            }

            public Jatekos(int x, int y, char ir)
            {
                pozició = new Pos(x, y);
                irány = ir;
            }
        }

        private enum LabyrinthOptions
        {
            Nothing, Road, Wall
        }

        private Random rnd = new Random();
        private int canCreateExit = 10;

        public async void GenerateLabyrinth()
        {
            Stack<Pos> createdRoads = new Stack<Pos>();
            createdRoads.Push(new Pos(1, 1));
            bool[,] visited = new bool[9, 9];
            visited[1, 1] = true;
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    Labirint[x, y] = (x % 2 == 1 && y % 2 == 1) ? '.' : '*';
                }
            }
            void Visit(Pos position)
            {
                List<Pos> unvisitedNeighbours = new List<Pos>();
                if (IsNotVisited(position.x + 2, position.y)) unvisitedNeighbours.Add(new Pos(position.x + 2, position.y));
                if (IsNotVisited(position.x - 2, position.y)) unvisitedNeighbours.Add(new Pos(position.x - 2, position.y));
                if (IsNotVisited(position.x, position.y + 2)) unvisitedNeighbours.Add(new Pos(position.x, position.y + 2));
                if (IsNotVisited(position.x, position.y - 2)) unvisitedNeighbours.Add(new Pos(position.x, position.y - 2));
                if (unvisitedNeighbours.Count > 0)
                {
                    createdRoads.Push(position);
                    Pos choosenPos = unvisitedNeighbours[rnd.Next(0, unvisitedNeighbours.Count)];
                    Labirint[(choosenPos.x + position.x) / 2, (choosenPos.y + position.y) / 2] = '.';
                    visited[choosenPos.x, choosenPos.y] = true;
                    createdRoads.Push(choosenPos);
                }
            }
            bool IsNotVisited(int x, int y)
            {
                return x > 0 && x < 8 && y > 0 && y < 8 && !visited[x, y];
            }
            while (createdRoads.Count > 0)
            {
                Pos currentPos = createdRoads.Pop();
                Visit(currentPos);
                Megjelenit();
                await Task.Delay(100);
            }
            Labirint[8, 7] = '.';
        }

        //int poz = 0;
        /*public void Fordulas(char f, int s, int o)
        {
            char fordul = f; int sor = s; int oszlop = o;

            char[] poziciok = new char[] { 'V', '<', '^', '>' };

            char jatekos = Labirint[sor, oszlop];
            if (f.Equals('J')) { jatekos = poziciok[(++poz) % 4] }
            else { jatekos = poziciok[(--poz) % 4] }

            Labirint[sor, oszlop] = jatekos;
        }*/

        public void Elore(char f, int s, int o)
        {
            char fordul = f; int sor = s; int oszlop = o;

            switch (f)
            {
                case 'V':
                    if (Labirint[sor + 1, oszlop] != '*')
                    {
                        ((Label)cellak[sor + 1, oszlop].Child).Content = 'V';
                        ((Label)cellak[sor, oszlop].Child).Content = '.';
                    }
                    break;

                case '<':
                    if (Labirint[sor, oszlop - 1] != '*')
                    {
                        ((Label)cellak[sor, oszlop - 1].Child).Content = '<';
                        ((Label)cellak[sor, oszlop].Child).Content = '.';
                    }
                    break;

                case '^':
                    if (Labirint[sor - 1, oszlop] != '*')
                    {
                        ((Label)cellak[sor - 1, oszlop].Child).Content = '^';
                        ((Label)cellak[sor, oszlop].Child).Content = '.';
                    }
                    break;

                case '>':
                    if (Labirint[sor, oszlop + 1] != '*')
                    {
                        ((Label)cellak[sor, oszlop + 1].Child).Content = '>';
                        ((Label)cellak[sor, oszlop].Child).Content = '.';
                    }
                    break;

                default:
                    break;
            }
        }
    }
}