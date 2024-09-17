namespace LAB_2_EDII;

public class Huffman
{
        private Dictionary<char, string> _tablaCodigos;
        private NodoHuffman _raiz;
        
        public static int ComprimirTexto(string texto)
        {
            var huffman = new Huffman();
            huffman.Construir(texto);
            string textoComprimido = huffman.Comprimir(texto);
            return NumeroBits(textoComprimido);
        }

        // Paso 1: Crear el árbol de Huffman a partir de las frecuencias de los caracteres
        public void Construir(string texto)
        {
            // Calcular las frecuencias de cada carácter
            var frecuencias = new Dictionary<char, int>();
            foreach (var simbolo in texto)
            {
                if (!frecuencias.ContainsKey(simbolo))
                {
                    frecuencias[simbolo] = 0;
                }
                frecuencias[simbolo]++;
            }
            
            // Crear una lista de nodos hoja
            var prioridadCola = new List<NodoHuffman>();

            foreach (var simbolo in frecuencias)
            {
                prioridadCola.Add(new NodoHuffman { Simbolo = simbolo.Key, Frecuencia = simbolo.Value });
            }

            // Ordenar la lista según las frecuencias
            prioridadCola = prioridadCola.OrderBy(n => n.Frecuencia).ToList();

            // Construir el árbol de Huffman
            while (prioridadCola.Count > 1)
            {
                var nodo1 = prioridadCola[0];
                var nodo2 = prioridadCola[1];

                prioridadCola.Remove(nodo1);
                prioridadCola.Remove(nodo2);

                var nuevoNodo = new NodoHuffman
                {
                    Simbolo = '*', // Nodo interno
                    Frecuencia = nodo1.Frecuencia + nodo2.Frecuencia,
                    Izquierda = nodo1,
                    Derecha = nodo2
                };

                prioridadCola.Add(nuevoNodo);
                prioridadCola = prioridadCola.OrderBy(n => n.Frecuencia).ToList(); // Reordenar después de añadir
            }

            // La raíz del árbol es el único nodo restante
            _raiz = prioridadCola.First();

            // Generar los códigos para cada símbolo
            _tablaCodigos = new Dictionary<char, string>();
            GenerarCodigos(_raiz, "");
        }

        // Paso 2: Generar los códigos de Huffman para cada carácter (recorrido en preorden)
        private void GenerarCodigos(NodoHuffman nodo, string codigo)
        {
            if (nodo == null) return;

            if (nodo.EsHoja)
            {
                _tablaCodigos[nodo.Simbolo] = codigo;
            }

            GenerarCodigos(nodo.Izquierda, codigo + "0");
            GenerarCodigos(nodo.Derecha, codigo + "1");
        }

        // Paso 3: Comprimir el texto de entrada
        public string Comprimir(string texto)
        {
            string resultado = string.Empty;
            foreach (var simbolo in texto)
            {
                if (_tablaCodigos.ContainsKey(simbolo))
                {
                    resultado += _tablaCodigos[simbolo];
                }
                else
                {
                    Console.WriteLine($"Error: No se encontró un código para el símbolo '{simbolo}'");
                }
            }
            return resultado;
        }

        // Paso 4: Descomprimir el texto comprimido
        public string Descomprimir(string textoComprimido)
        {
            string resultado = string.Empty;
            var nodoActual = _raiz;

            foreach (var bit in textoComprimido)
            {
                if (bit == '0')
                {
                    nodoActual = nodoActual.Izquierda;
                }
                else
                {
                    nodoActual = nodoActual.Derecha;
                }

                if (nodoActual.EsHoja)
                {
                    resultado += nodoActual.Simbolo;
                    nodoActual = _raiz;
                }
            }

            return resultado;
        }

        // Método para mostrar los códigos generados
        public void MostrarCodigos()
        {
            Console.WriteLine("Códigos de Huffman:");
            foreach (var codigo in _tablaCodigos)
            {
                Console.WriteLine($"'{codigo.Key}': {codigo.Value}");
            }
        }

        // Método para calcular el tamaño en bits y bytes del texto comprimido
        public void MostrarTamañoComprimido(string textoComprimido)
        {
            int numeroBits = NumeroBits(textoComprimido);
            double numeroBytes = numeroBits / 8.0;

            Console.WriteLine($"\nTamaño del texto comprimido:");
            Console.WriteLine($"{numeroBits} bits");
            Console.WriteLine($"{numeroBytes:F2} bytes");
        }
        
        public static int NumeroBits(string textoComprimido)
        {
            return textoComprimido.Length;
        }
}