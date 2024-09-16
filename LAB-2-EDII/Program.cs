﻿namespace LAB_2_EDII;

class Program
{
    static void Main(string [] args)
    {
        try
        {
            // C:\Users\smora\Downloads\lab01_books.csv - C:\Users\smora\Downloads\lab01_search.csv - C:\Users\smora\Downloads\100Klab01\100Klab01_books.csv
            // C:\Users\smora\Downloads\100Klab01\100Klab01_search.csv
            
            string archivoResultados = "C:\\Users\\smora\\Downloads\\resultados_busquedas.txt";
            
            Console.WriteLine("Ingrese la ruta del archivo: ");
            string archivoInsertar = Console.ReadLine();
            Console.WriteLine("Ingrese el archivo de busqueda: ");
            string archivoBusquedas = Console.ReadLine();
            
            // Verificar si el archivo existe
            if (!System.IO.File.Exists(archivoInsertar) || !System.IO.File.Exists(archivoBusquedas))
            {
                Console.WriteLine($"El archivo {archivoInsertar} o {archivoBusquedas} no existe.");
                return;
            }
            
            GestorDeArchivos.ProcesarArchivoInsertar(archivoInsertar);
            GestorDeArchivos.ProcesarArchivoBusqueda(archivoBusquedas, archivoResultados);
            Console.WriteLine("Archivo de salida genrado en descargas con el nombre de resultados_busquedas.txt");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}

