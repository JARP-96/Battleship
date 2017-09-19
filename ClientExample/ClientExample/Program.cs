using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientExample
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpClient client = null;
            try
            {
                client = new TcpClient("10.210.36.205", 2307);
            }
            catch (Exception ex)
            {
                Console.WriteLine("No se pudo conectar al cliente, adios");
                return;
            }

            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);

            try
            {

                // Bienvenida
                string noJugador = reader.ReadLine();
                Console.Write("Bienvenido jugador " + noJugador);
                Console.WriteLine();

                int bandera1 = 7;
                while (bandera1 > 0)
                {
                    // Recibe Tablero
                    for (int i = 0; i < 5; i++)
                    {
                        string tablero = reader.ReadLine();
                        Console.WriteLine(tablero);
                    }
                    int[,] barcos = { { 5, 1 }, { 4, 2 }, { 3, 2 }, { 2, 2 } };

                    Console.WriteLine("Introduzca:");
                    string tipoBarco, xini, yini, xfin, yfin;
                    Console.WriteLine("Tipo de Barco:");
                    tipoBarco = Console.ReadLine();
                    Console.WriteLine("Posicion Inicial 'x':");
                    xini = Console.ReadLine();
                    Console.WriteLine("Posicion Inicial 'y':");
                    yini = Console.ReadLine();
                    Console.WriteLine("Posicion Final 'x':");
                    xfin = Console.ReadLine();
                    Console.WriteLine("Posicion Final 'y':");
                    yfin = Console.ReadLine();

                    writer.WriteLine(noJugador + " " + tipoBarco.ToString() + xini.ToString() + yini.ToString() + xfin.ToString() + yfin.ToString());
                    writer.Flush();
                    string vida = reader.ReadLine();
                    if (vida.Equals("ok"))
                    {
                        bandera1--;
                    }
                    else
                    {
                        Console.WriteLine("Cheque su entrada, joven.");
                    }
                }

                string confirmacion = reader.ReadLine();

                while (true) {
                    string turno = reader.ReadLine();
                    if (turno.Equals("perdiste"))
                    {
                        Console.WriteLine("shape of an L on her forehead");
                        break;
                    }
                    Console.WriteLine("Es tu turno :D");

                    string x, y;
                    Console.WriteLine("Ataque Posicion 'y':");
                    x = Console.ReadLine();
                    Console.WriteLine("Ataque Posicion 'y':");
                    y = Console.ReadLine();
                    int a = 0, b = 0;
                    try
                    {
                        a = Int16.Parse(x);
                        b = Int16.Parse(y);
                    }
                    catch
                    {
                        Console.WriteLine("u suck");
                        continue;
                    }
                    if (a > 0 && a < 10 && b > 0 && b < 10)
                    {
                        Console.WriteLine("Tiene que estar dentro del rango, prro.");
                        continue;
                    }
                    writer.WriteLine(x + " " + y);
                    writer.Flush();
                    string atinar = reader.ReadLine();
                    if (atinar.Equals("0"))
                    {
                        Console.WriteLine("No le atinaste.");
                    }
                    else if (atinar.Equals("1"))
                    {
                        Console.WriteLine("Eres un pro.");
                    }
                    else
                    {
                        Console.WriteLine("Campeonazo de la vida.");
                        break;
                    }
                }

                writer.WriteLine("hola");
                writer.Flush();
                Console.WriteLine("Dijo " + reader.ReadLine());
                writer.WriteLine("adios");
                writer.Flush();

            }
            catch (Exception ex)
            {
                throw;
            }
            reader.Close();
            writer.Close();
            stream.Close();
        }
    }
}

// 449 553 3636; nacho@encalientes.com