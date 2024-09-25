using System.Security.Principal;
using System.Text;

namespace LAB_2_EDII;

class Program
{
    static void Main(string [] args)
    {
        Principal();
    }
    
    private static void Principal()
    {
        try
        {
            // C:\Users\smora\Downloads\Ejemplo\Ejemplo_lab01_books.csv - C:\Users\smora\Downloads\Ejemplo\Ejemplo_lab01_search.csv 
            // C:\Users\smora\Downloads\Ejemplo2\lab01_books.csv - C:\Users\smora\Downloads\Ejemplo2\lab01_search.csv
            // C:\Users\smora\Downloads\100K\100Klab01_books.csv - C:\Users\smora\Downloads\100K\100Klab01_search.csv
            
            
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

           
                try
                {
                    // Crear la ruta del archivo de resultados en la misma carpeta que el archivo de inserción
                    string carpetaResultados = System.IO.Path.GetDirectoryName(archivoInsertar);
                    string archivoResultados = System.IO.Path.Combine(carpetaResultados, "resultados.txt");
                    // Crear el archivo de resultados si no existe
                    if (!System.IO.File.Exists(archivoResultados))
                    {
                        System.IO.File.Create(archivoResultados).Close();
                    }

                    Console.WriteLine($"Archivo de salida generado en {archivoResultados}");
                    Console.WriteLine("Procesando archivos...");
                    GestorDeArchivos.ProcesarArchivoInsertar(archivoInsertar);
                    GestorDeArchivos.ProcesarArchivoBusqueda(archivoBusquedas, archivoResultados);
                    Console.WriteLine("Listo :) ");

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}

