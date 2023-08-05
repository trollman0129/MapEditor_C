using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MapEditer
{
    internal class Program
    {
        static int size_x, size_y, posx, posy;
        static List<string> map = new List<string>();
        static List<string> mark = new List<string>();
        static char[] setting = new char[4];

        static Encoding enc = Encoding.GetEncoding("UTF-8");

        static void New()
        {
            map.Clear();
            mark.Clear();
            for (int i = 0; i < size_y; i++)
            {
                map.Add("");
                for (int j = 0; j < size_x; j++) map[i] += " ";

            }
        }

        static bool IsContain(int x, int y)
        {
            for(int i = 0; i < mark.Count; i ++)
            {
                if (mark[i].Contains(x + "," + y)) return true;
            }
            return false;
        }

        static void Draw()
        {
            int m, num = 0;
            Console.Clear();
            Console.WriteLine("x:" + posx + "y:" + posy);
            for (int y = posy - 5; y < posy + 5; y++)
            {
                if (y < size_y && y >= 0)
                {
                    for (int x = posx - 5; x < posx + 5; x++)
                    {
                        if (x >= 0 && x < size_x)
                        {
                            if (x == posx && y == posy)
                            {
                                Console.BackgroundColor = ConsoleColor.White;
                                Console.ForegroundColor = ConsoleColor.Black;
                            }
                            else if (IsContain(x, y))
                            {
                                for(int i = 0; i < mark.Count; i ++)
                                {
                                    if (mark[i].Contains(x + "," + y))
                                    {
                                        num = i;
                                        break;
                                    }
                                }
                                m = int.Parse(mark[num].Split(',', ' ')[3]);
                                if (m == 0)
                                {
                                    Console.BackgroundColor = ConsoleColor.Red;
                                    Console.ForegroundColor = ConsoleColor.White;
                                }
                                else if(m == 1)
                                {
                                    Console.BackgroundColor = ConsoleColor.Blue;
                                    Console.ForegroundColor = ConsoleColor.White;
                                }
                                else if(m == 2)
                                {
                                    Console.BackgroundColor = ConsoleColor.Green;
                                    Console.ForegroundColor = ConsoleColor.White;
                                }
                                else
                                {
                                    Console.BackgroundColor = ConsoleColor.Yellow;
                                    Console.ForegroundColor = ConsoleColor.Black;
                                }
                            }
                            else
                            {
                                Console.BackgroundColor = ConsoleColor.Black;
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            Console.Write(map[y][x] + " ");
                        }
                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine();
                }
            }
        }

        static void LoadSetting()
        {
            StreamReader sr = new StreamReader(@"../../../Setting.txt", Encoding.GetEncoding("UTF-8"));
            string str = sr.ReadToEnd();
            sr.Close();
            string r = str.Replace("\r", "");
            var w = r.Split('\n');
            setting[0] = w[0].Split(':')[1][0];
            setting[1] = w[1].Split(':')[1][0];
            setting[2] = w[2].Split(':')[1][0];
            setting[3] = w[3].Split(':')[1][0];
        }

        static void Load()
        {
            StreamReader sr = new StreamReader(@"../../../map.txt", Encoding.GetEncoding("UTF-8"));
            string str = sr.ReadToEnd();
            sr.Close();
            var w = str.Split('\n');
            var b = w[0].Split(',');
            size_y = int.Parse(b[0]);
            size_x = int.Parse(b[1]);
            mark.Clear();
            New();
            for (int i = 0; i < size_y; i++)
            {
                map[i] = w[i+2].Replace("{", "").Replace("}", "").Replace(",", "").Replace(";", "").Replace("\"", "");
            }
            for (int i = size_y + 2; i < w.Length - 2; i++) mark.Add(w[i]);
        }

        static void Save()
        {
            string s = size_x + "," + size_y + "\n(ここから)\n";
            for (int i = 0; i < size_y-1; i++) s += "{\"" + map[i] + "\"},\n";
            s += "{\"" + map[size_y-1] + "\"}};\n(ここまで)\n";
            for (int i = 0; i < mark.Count; i++) s += mark[i] + "\n";
            StreamWriter writer = new StreamWriter(@"../../../map.txt", false, enc);
            writer.WriteLine(s);
            writer.Close();
        }

        static string Change(int x, int y, char o)
        {
            string a = "";
            for (int i = 0; i < map[y].Length; i++)
            {
                if (i == x) a += o;
                else a += map[y][i];
            }
            return a;
        }

        static void Main(string[] args)
        {
            int num = 0;
            LoadSetting();
            posx = 0;
            posy = 0;
            Console.WriteLine("MapEditor\nサイズを入力してください。");
            var w = Console.ReadLine().Split(' ');
            size_x = int.Parse(w[0]);
            size_y = int.Parse(w[1]);
            New();
            while (true)
            {
                Draw();
                string key = Console.ReadKey().Key.ToString();
                if (key == "A" && posx > 0) posx--;
                else if (key == "D" && posx < size_x - 1) posx++;
                else if (key == "W" && posy > 0) posy--;
                else if (key == "S" && posy < size_y - 1) posy++;
                else if (key == "NumPad0") map[posy] = Change(posx, posy, setting[0]);
                else if (key == "NumPad1") map[posy] = Change(posx, posy, setting[1]);
                else if (key == "NumPad2") map[posy] = Change(posx, posy, setting[2]);
                else if (key == "NumPad3") map[posy] = Change(posx, posy, setting[3]);
                else if (key == "Escape")
                {
                    Console.WriteLine("\n　セーブしますか？(y/n)");
                    while (true)
                    {
                        char k = Console.ReadKey().KeyChar;
                        if (k == 'y')
                        {

                    Save();
                    Console.WriteLine("\n　セーブしました。");
                    Console.ReadKey();
                            break;
                        }
                        else if (k == 'n') break;
                    }
                }
                else if (key == "Delete")
                {
                    Console.WriteLine("\n　ロードしますか？(y/n)");
                    while (true)
                    {
                        char k = Console.ReadKey().KeyChar;
                        if (k == 'y')
                        {
                            Load();
                            Console.WriteLine("\n　ロードしました。");
                            Console.ReadKey();
                            break;
                        }
                        else if (k == 'n') break;
                    }
                }
                else if (key == "Oem1")
                {
                    w = Console.ReadLine().Split(' ');
                    if (w[0] == "new")
                    {
                        Console.WriteLine("\n初期化しますか？(y/n)");
                        while (true)
                        {
                            char k = Console.ReadKey().KeyChar;
                            if (k == 'y')
                            {
                                New();
                                Console.WriteLine("\n　初期化しました。");
                                Console.ReadKey();
                                break;
                            }
                            else if (k == 'n') break;
                        }
                    }
                    else if (w[0] == "mark")
                    {
                        if (!IsContain(posx, posy))
                        {
                            if (w.Length == 1) mark.Add(posx + "," + posy + ", 0");
                            else mark.Add(posx + "," + posy + ", " + w[1]);
                        }
                    }
                    else if (w[0] == "remark")
                    {
                        for (int i = 0; i < mark.Count; i++)
                        {
                            if (mark[i].Contains(posx + "," + posy))
                            {
                                num = i;
                                break;
                            }
                        }
                        mark.RemoveAt(num);
                    }
                    else if (w[0] == "pos")
                    {
                        if (int.Parse(w[1]) < size_x && int.Parse(w[2]) < size_y && int.Parse(w[1]) >= 0 && int.Parse(w[2]) >= 0)
                        posx = int.Parse(w[1]);
                        posy = int.Parse(w[2]);
                    }
                    else if (w[0] == "size")
                    {
                        size_x = int.Parse(w[1]);
                        size_y = int.Parse(w[2]);
                    }
                }
            }
        }
    }
}
