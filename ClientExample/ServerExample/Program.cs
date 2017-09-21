using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServidorBattleShip
{
    class Program
    {
        private static TcpListener escuchador = null;
        private static int[,] tableroa, tablerob;
        private static int turno;
        private static bool listoa, listob;
        static void Main(string[] args)
        {
            escuchador = new TcpListener(System.Net.IPAddress.Any, 2307);
            escuchador.Start();
            Task[] clientes = new Task[2];
            tableroa = new int[10, 10];
            tablerob = new int[10, 10];
            turno = 0;
            listoa = listob = false;
            for (int i = 0; i < 2; i++)
            {
                clientes[i] = Escucha(i + 1);
            }
            Task.WaitAll(clientes);
            Console.WriteLine("Adios");

        }

        public static async Task Escucha(int n)
        {
            Socket socket = await escuchador.AcceptSocketAsync();
            if (socket.Connected)
            {
                Console.WriteLine("Cliente " + socket.RemoteEndPoint + " se conecto: jugador " + n.ToString());
                NetworkStream stream = new NetworkStream(socket);
                StreamWriter writer = new StreamWriter(stream);
                StreamReader reader = new StreamReader(stream);

                await writer.WriteLineAsync(n.ToString());
                await writer.FlushAsync();

                int[,] mat = { { 5, 1 }, { 4, 2 }, { 3, 2 }, { 2, 2 } };
                while (true)
                {
                    if (n == 1 && listoa)
                        if (listob)
                        {
                            Console.WriteLine("Empezando juego");
                            await writer.WriteLineAsync("Empieza jugador 1");
                            await writer.FlushAsync();
                            break;
                        }
                        else
                            continue;
                    if (n == 2 && listob)
                        if (listoa)
                        {
                            Console.WriteLine("Empezando juego");
                            await writer.WriteLineAsync("Empieza jugador 1");
                            await writer.FlushAsync();
                            break;
                        }
                        else
                            continue;
                    await writer.WriteLineAsync("Tamano  cantidad");
                    for (int i = 0; i < 4; i++)
                    {
                        await writer.WriteLineAsync(mat[i, 0].ToString() + " " + mat[i, 1].ToString());
                    }
                    await writer.FlushAsync();
                    string line = await reader.ReadLineAsync();
                    string[] param = line.Split(' ');
                    if (param.Length != 6)
                    {
                        await writer.WriteLineAsync("error");
                        await writer.FlushAsync();
                        continue;
                    }

                    if (!barco(n, Int16.Parse(param[2]), Int16.Parse(param[3]), Int16.Parse(param[4]), Int16.Parse(param[5]), Int16.Parse(param[1])))
                    {
                        await writer.WriteLineAsync("error");
                        await writer.FlushAsync();
                        continue;
                    }
                    if (mat[5 - Int16.Parse(param[1]), 1] == 0)
                    {
                        await writer.WriteLineAsync("error");
                        await writer.FlushAsync();
                        continue;
                    }
                    mat[5 - Int16.Parse(param[1]), 1]--;

                    await writer.WriteLineAsync("ok");
                    await writer.FlushAsync();
                    Console.WriteLine("Jugador " + n.ToString() + " barco ok");
                    if (mat[0, 1] == 0 && mat[1, 1] == 0 && mat[2, 1] == 0 && mat[3, 1] == 0)
                        if (n == 1)
                        {
                            listoa = true;
                            Console.WriteLine("Jugador " + n.ToString() + " listo");
                        }
                        else
                        {
                            Console.WriteLine("Jugador " + n.ToString() + " listo");
                            listob = true;
                        }
                }
                listoa = listob = false;
                while (true)
                {
                    if (turno != n)
                    {
                        if (n == 1 && listob)
                        {
                            await writer.WriteLineAsync("perdiste");
                            await writer.FlushAsync();
                            break;
                        }
                        if (n == 2 && listoa)
                        {
                            await writer.WriteLineAsync("perdiste");
                            await writer.FlushAsync();
                            break;
                        }
                        continue;
                    }
                    await writer.WriteLineAsync("tu turno");
                    await writer.FlushAsync();

                    string line = await reader.ReadLineAsync();
                    string[] param = line.Split(' ');
                    if (n == 1)
                    {
                        if (tablerob[Int16.Parse(param[1]), Int16.Parse(param[2])] > 0)
                        {
                            tablerob[Int16.Parse(param[1]), Int16.Parse(param[2])] = 0;
                            int s = 0;
                            for (int i = 0; i < 10; i++)
                                for (int j = 0; j < 10; j++)
                                {
                                    s += tablerob[i, j];
                                }
                            if (s == 0)
                            {
                                await writer.WriteLineAsync("Ganaste");
                                await writer.FlushAsync();
                                listoa = true;
                                break;
                            }
                            await writer.WriteLineAsync("1");
                            await writer.FlushAsync();
                        }
                        else
                        {
                            await writer.WriteLineAsync("0");
                            await writer.FlushAsync();
                        }
                    }
                    else
                    {
                        if (tableroa[Int16.Parse(param[1]), Int16.Parse(param[2])] > 0)
                        {
                            tableroa[Int16.Parse(param[1]), Int16.Parse(param[2])] = 0;
                            int s = 0;
                            for (int i = 0; i < 10; i++)
                                for (int j = 0; j < 10; j++)
                                {
                                    s += tableroa[i, j];
                                }
                            if (s == 0)
                            {
                                await writer.WriteLineAsync("Ganaste");
                                await writer.FlushAsync();
                                listob = true;
                                break;
                            }
                            await writer.WriteLineAsync("1");
                            await writer.FlushAsync();
                        }
                        else
                        {
                            await writer.WriteLineAsync("0");
                            await writer.FlushAsync();

                        }
                    }


                }


                reader.Close();
                writer.Close();
                stream.Close();

            }
            socket.Close();
        }
        public static bool barco(int tab, int x1, int y1, int x2, int y2, int t)
        {
            if (x1 != x2 && y1 != y2)
                return false;
            if (x1 > 9 || x2 > 9 || y1 > 9 || y2 > 9 || t > 5 || t < 2)
                return false;
            if (x1 == x2 && abs(y1 - y2) == t - 1)
            {
                if (y1 > y2)
                {
                    y1 = y1 + y2;
                    y2 = y1 - y2;
                    y1 = y1 - y2;
                }
                for (int i = y1; i <= y2; i++)
                {
                    if (tab == 1)
                    {
                        if (tableroa[x1, i] > 0)
                            return false;
                        tableroa[x1, i] = 1;
                    }
                    else
                    {
                        if (tablerob[x1, i] > 0)
                            return false;
                        tablerob[x1, i] = 1;

                    }

                }
                return true;
            }
            if (y1 == y2 && abs(x1 - x2) == t - 1)
            {
                if (x1 > x2)
                {
                    x1 = x1 + x2;
                    x2 = x1 - x2;
                    x1 = x1 - x2;
                }
                for (int i = x1; i <= x2; i++)
                {
                    if (tab == 1)
                    {
                        if (tableroa[y1, i] > 0)
                            return false;
                        tableroa[y1, i] = 1;
                    }
                    else
                    {
                        if (tablerob[y1, i] > 0)
                            return false;
                        tablerob[y1, i] = 1;

                    }

                }
                return true;

            }

            return false;
        }

        private static int abs(int v)
        {
            if (v < 0)
                v = -v;
            return v;
        }
    }
}