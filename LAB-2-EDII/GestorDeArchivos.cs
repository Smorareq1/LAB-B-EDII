namespace LAB_2_EDII;

using System.Text;
using Newtonsoft.Json;

public class GestorDeArchivos
{
    public static Dictionary<string, Libro> NombreLibro = new Dictionary<string, Libro>();
    public static Dictionary<string, string> ISBNNombre = new Dictionary<string, string>();
    private static int EqualInt = 0, DecompressInt = 0, HuffmanInt = 0, ArithmeticInt = 0; 
    
    

    //////////////// ARCHIVO PARA INSERTAR ////////////////////

    public static void ProcesarArchivoInsertar(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        foreach (var line in lines)
        {
            ProcesarLineaInsertar(line);
        }
    }
    
    private static void InsertarLibro(Libro book)
    {
        NombreLibro[book.name] = book;
        ISBNNombre[book.isbn] = book.name;
    }
    
    private static void DeleteLibro(string isbn)
    {
        // Ensure we have the book in our records
        if (ISBNNombre.TryGetValue(isbn, out var nombre))
        {
            NombreLibro.Remove(nombre);
            ISBNNombre.Remove(isbn);
        }
        else
        {
            //Console.WriteLine($"ISBN {isbn} not found to delete");
        }
    }

    private static void PatchLibro(string isbn, Dictionary<string, object> patchData)
    {
        // Check if the book exists
        if (ISBNNombre.TryGetValue(isbn, out var nombre))
        {
            if (NombreLibro.TryGetValue(nombre, out var book))
            {
                string oldName = book.name; // Store the old name for dictionary updates

                // Iterate through the patch data and update the book's properties
                foreach (var kvp in patchData)
                {
                    switch (kvp.Key.ToLower())
                    {
                        case "name":
                            // Update the name in both dictionaries
                            var newName = kvp.Value.ToString();
                            NombreLibro.Remove(oldName); // Remove the old entry
                            book.name = newName; // Update the name in the object
                            NombreLibro[newName] = book; // Add the book with the new name
                            ISBNNombre[isbn] = newName; // Update the name in the ISBN dictionary
                            break;
                        case "author":
                            book.author = kvp.Value.ToString();
                            break;
                        case "category":
                            book.category = kvp.Value.ToString();
                            break;
                        case "price":
                            book.price = kvp.Value.ToString();
                            break;
                        case "quantity":
                            book.quantity = kvp.Value.ToString();
                            break;
                        default:
                            //Console.WriteLine($"Campo desconocido: {kvp.Key}");
                            break;
                    }
                }
            }
            else
            {
                //Console.WriteLine($"El libro con nombre {nombre} no se encontró.");
            }
        }
        else
        {
            //Console.WriteLine($"ISBN {isbn} no se encontró.");
        }
    }
    
    private static void ProcesarLineaInsertar(string linea)
    {
        if (linea.StartsWith("INSERT;"))
        {
            var part = linea.Replace("INSERT;", "").Trim();
            var book = JsonConvert.DeserializeObject<Libro>(part);

            if (book == null || string.IsNullOrEmpty(book.name) || string.IsNullOrEmpty(book.isbn))
            {
                //Console.WriteLine("Error: Libro deserializado es nulo o tiene datos incompletos.");
                return;
            }

            // Insert into dictionaries
            InsertarLibro(book);
        }
        else if (linea.StartsWith("PATCH;"))
        {
            var part = linea.Replace("PATCH;", "").Trim();
            var patchData = JsonConvert.DeserializeObject<Dictionary<string, object>>(part);

            // PATCH
            if (patchData.ContainsKey("isbn"))
            {
                var isbn = patchData["isbn"].ToString();
                PatchLibro(isbn, patchData);
            }
            
        }
        else if (linea.StartsWith("DELETE;"))
        {
            var part = linea.Replace("DELETE;", "").Trim();
            var deleteData = JsonConvert.DeserializeObject<Dictionary<string, string>>(part);

            if (deleteData.ContainsKey("isbn"))
            {
                var isbn = deleteData["isbn"];
                DeleteLibro(isbn);
            }
        }
    }
    
    //////////////// ARCHIVO PARA BUSCAR ////////////////////
    
    public static void ProcesarArchivoBusqueda(string filePath, string outputFilePath)
    {
        var lines = File.ReadAllLines(filePath);

        // Ensure the output TXT file is created and ready for writing
        using (var writer = new StreamWriter(outputFilePath))
        {
            foreach (var line in lines)
            {
                ProcesarLineaBusqueda(line, writer);
            }
            
            writer.WriteLine(mensajeFinal());
        }
    }

    private static void ProcesarLineaBusqueda(string linea, StreamWriter writer)
    {
        if (linea.StartsWith("SEARCH;"))
        {
            var part = linea.Replace("SEARCH;", "").Trim();
            var searchData = JsonConvert.DeserializeObject<Dictionary<string, string>>(part);

            if (searchData.ContainsKey("name"))
            {
                var name = searchData["name"];
                BuscarPorNombre(name, writer);
            }
        }
    }

    private static void BuscarPorNombre(string name, StreamWriter writer)
    {
        if (NombreLibro.TryGetValue(name, out var book))
        {
            int originalbytes = ASCIIEncoding.Unicode.GetByteCount(book.name);
            int huffmanbits = Huffman.ComprimirTexto(book.name);
            long aritmeticobytes = CompresionAritmeticaInt.CompresionAritmeticaBytes(book.name);
            
            var bookJson = new
            {
                isbn = book.isbn,
                name = book.name,
                author = book.author,
                category = book.category,
                price = book.price,
                quantity = book.quantity,
                namesize = originalbytes.ToString(),
                namesizehuffman = huffmanbits.ToString(),
                namesizearithmetic = aritmeticobytes.ToString()
            };
            
            ComparacionesMetodos(originalbytes, huffmanbits, aritmeticobytes);

            var json = JsonConvert.SerializeObject(bookJson);

            writer.WriteLine(json); // Write the JSON string to the output file
        }
        else
        {
            //Console.WriteLine($"Libro con nombre {name} no encontrado.");
        }
    }
    
    //////////////// Comparaciones ////////////////////
    public static void ComparacionesMetodos(int normal, int huffmanIntFuncion, long aritmeticoInt)
    {
        double huffmanIntFuncionLong = (double)huffmanIntFuncion / 8; //Pasar a bytes
        
        if (normal == huffmanIntFuncionLong && huffmanIntFuncionLong == aritmeticoInt)
        {
            EqualInt++;
        }
        else if (normal <= huffmanIntFuncionLong && normal <= aritmeticoInt)
        {
            DecompressInt++;
        }
        else if (huffmanIntFuncionLong <= normal && huffmanIntFuncionLong <= aritmeticoInt)
        {
            HuffmanInt++;
        }
        else
        {
            ArithmeticInt++;
        }
    }

    public static string mensajeFinal()
    {
        string mensaje = $"Equal: {EqualInt}\nDecompress: {DecompressInt}\nHuffman: {HuffmanInt}\nArithmetic: {ArithmeticInt}";
        return mensaje; 
    }
}
