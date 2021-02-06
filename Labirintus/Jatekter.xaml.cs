using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace Labirintus
{
    /// <summary>
    /// Interaction logic for Jatekter.xaml
    /// </summary>
    public partial class Jatekter : Window
    {
        public Jatekter()
        {
            InitializeComponent();
            Labyrinth lab = new Labyrinth(labirintus);
            lab.Megjelenit();
        }
        public Jatekter(string fajlnev)
        {
            InitializeComponent();
            
        }

        private void BtnVissza_Click(object sender, RoutedEventArgs e)
        {

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

        }
    }
    class Labyrinth
    {
        private char[,] Labirint;
        private Grid labirintus;
        private Border[,] cellak;
        private Pos jatekos = new Pos(1, 1);
        public Labyrinth(Grid labirintus, string fajlnev)
        {
            this.labirintus = labirintus;
            Labirint = new char[9, 9];
            string[] forras;
            if (fajlnev.Contains(".txt"))
            {
                forras = File.ReadAllLines(fajlnev);
            }
            else
            {
                forras = File.ReadAllLines(fajlnev + ".txt");
            }             
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
            GenerateLabyrinth();
        }
        public void Megjelenit()
        {
            for (int i = 0; i < 9; i++)
            {
                labirintus.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                labirintus.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }
            cellak = new Border[9, 9];
            for (int sor = 0; sor < 9; sor++)
            {
                for (int oszlop = 0; oszlop < 9; oszlop++)
                {
                    Border cella = new Border();
                    cella.KeyDown += Lenyom;
                    if (oszlop == 1 && sor == 1)
                    {
                        // 1. sor, 1. sor, a játékos alphelyzete
                        cella.Child = new Label() { Content = "V" };
                    }
                    else if (Labirint[sor, oszlop] == '*')
                    {
                        cella.Child = new Label() { Content = "*" };
                    }
                    else
                    {
                        cella.Child = new Label() { Content = "." };
                    }
                    cellak[sor, oszlop] = cella;
                    labirintus.Children.Add(cella);
                    Grid.SetRow(cella, sor);
                    Grid.SetColumn(cella, oszlop);
                }
            }
            void Lenyom(object sender, KeyEventArgs args)
            {

            }
        }
        struct Pos
        {
            public int x, y;
            public Pos(int x, int y)
            {
                this.x = x;
                this.y = y;
                
            }
        }
        struct Jatekos
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
        enum LabyrinthOptions
        {
            Nothing, Road, Wall
        }
        LabyrinthOptions[] generatedLabyrinth = new LabyrinthOptions[9 * 9];
        Random rnd = new Random();
        int canCreateExit = 10;
        public void GenerateLabyrinth()
        {
            Stack<Pos> createdWalls = new Stack<Pos>();
            createdWalls.Push(new Pos(1, 1));
            bool CreateWallIfHasTo(Pos position)
            {
                if (position.x == 0 || position.y == 0 || position.x == 8 || position.y == 8)
                {
                    if (canCreateExit < 1)
                    {
                        FillOutline();
                        generatedLabyrinth[GetIndexAt(position)] = LabyrinthOptions.Road;
                    }
                    return true;
                }
                else if(generatedLabyrinth[GetIndexAt(position)] == LabyrinthOptions.Nothing)
                {
                    int roadCountAroundPosition = 0;
                    if (generatedLabyrinth[GetIndexAt(new Pos(position.x + 1, position.y))] == LabyrinthOptions.Road) roadCountAroundPosition++;
                    if (generatedLabyrinth[GetIndexAt(new Pos(position.x - 1, position.y))] == LabyrinthOptions.Road) roadCountAroundPosition++;
                    if (generatedLabyrinth[GetIndexAt(new Pos(position.x, position.y + 1))] == LabyrinthOptions.Road) roadCountAroundPosition++;
                    if (generatedLabyrinth[GetIndexAt(new Pos(position.x, position.y - 1))] == LabyrinthOptions.Road) roadCountAroundPosition++;
                    if (roadCountAroundPosition > 1)
                    {
                        generatedLabyrinth[GetIndexAt(position)] = LabyrinthOptions.Wall;
                        return true;
                    }
                    return false;
                }
                else
                {
                    return true;
                }

            }
            int GetIndexAt(Pos position)
            {
                return position.y * 9 + position.x;
            }
            void FillOutline()
            {
                for (int i = 0; i < 9; i++)
                {
                    generatedLabyrinth[i] = LabyrinthOptions.Wall;
                    generatedLabyrinth[71 + i] = LabyrinthOptions.Wall;
                    generatedLabyrinth[i * 9] = LabyrinthOptions.Wall;
                    generatedLabyrinth[i * 9 + 8] = LabyrinthOptions.Wall;
                }
            }
            while (generatedLabyrinth.Any(x => x == 0))
            {
                Pos currentPos = createdWalls.Peek();
                generatedLabyrinth[GetIndexAt(currentPos)] = LabyrinthOptions.Road;
                List<Pos> canGo = new List<Pos>();
                if (!CreateWallIfHasTo(new Pos(currentPos.x + 1, currentPos.y))) canGo.Add(new Pos(currentPos.x + 1, currentPos.y));
                if (!CreateWallIfHasTo(new Pos(currentPos.x - 1, currentPos.y))) canGo.Add(new Pos(currentPos.x - 1, currentPos.y));
                if (!CreateWallIfHasTo(new Pos(currentPos.x, currentPos.y + 1))) canGo.Add(new Pos(currentPos.x, currentPos.y + 1));
                if (!CreateWallIfHasTo(new Pos(currentPos.x, currentPos.y - 1))) canGo.Add(new Pos(currentPos.x, currentPos.y - 1));
                if (canGo.Count > 0)
                {
                    Pos goTo = canGo[rnd.Next(0, canGo.Count)];
                    createdWalls.Push(goTo);
                    canCreateExit--;
                }
                else
                {
                    createdWalls.Pop();
                }
            }
            Labirint = new char[9, 9];
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    switch(generatedLabyrinth[y * 9 + x])
                    {
                        case LabyrinthOptions.Road:
                            Labirint[x, y] = '.';
                            break;
                        case LabyrinthOptions.Wall:
                            Labirint[x, y] = '*';
                            break;
                        default:
                            throw new Exception("Nem lett tökéletes a labirintus leenerálása!");
                            break;
                    }
                }
            }
        }
        int poz = 0;
        public void Fordulas(char f, int s, int o)
        {
            char fordul = f; int sor = s; int oszlop = o;

            char[] poziciok = new char[] { 'V', '<', '^', '>' };

            char jatekos = Labirint[sor, oszlop];
            if (f.Equals('J')) { jatekos = poziciok[(++poz) % 4] }
            else { jatekos = poziciok[(--poz) % 4] }

            Labirint[sor, oszlop] = jatekos;
        }
        public void Elore(char f, int s, int o)
        {
            char fordul = f; int sor = s; int oszlop = o;

            switch(f)
            {
                case 'V':
                        if (Labirint[sor + 1, oszlop] != '*' )
                        { ((Label)cellak[sor + 1, oszlop].Child).Content = 'V';
                          ((Label)cellak[sor, oszlop].Child).Content = '.';
                        }
                        break;
                case '<':
                        if (Labirint[sor, oszlop - 1] != '*' ) 
                        { ((Label)cellak[sor, oszlop - 1].Child).Content = '<';
                          ((Label)cellak[sor, oszlop].Child).Content = '.';
                        }
                        break;
                case '^':
                        if (Labirint[sor - 1, oszlop] != '*' ) 
                        { ((Label)cellak[sor - 1, oszlop].Child).Content = '^';
                          ((Label)cellak[sor, oszlop].Child).Content = '.';
                        }
                        break;
                case '>':
                        if (Labirint[sor, oszlop + 1] != '*' ) 
                        { ((Label)cellak[sor, oszlop + 1].Child).Content = '>';
                          ((Label)cellak[sor, oszlop].Child).Content = '.';
                        }
                        break;
                default:
                    break;
            }
        }
    }
}
