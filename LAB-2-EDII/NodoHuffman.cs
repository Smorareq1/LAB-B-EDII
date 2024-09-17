namespace LAB_2_EDII;

public class NodoHuffman
{
    public char Simbolo { get; set; }
    public int Frecuencia { get; set; }
    public NodoHuffman Izquierda { get; set; }
    public NodoHuffman Derecha { get; set; }

    public bool EsHoja => Izquierda == null && Derecha == null;
}