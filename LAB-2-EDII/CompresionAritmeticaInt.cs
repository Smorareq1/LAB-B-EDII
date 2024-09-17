
    namespace LAB_2_EDII;
    public class CompresionAritmeticaInt
    {
        private readonly Dictionary<char, (ushort Low, ushort High)> _probabilities;
        private readonly string _source;
        private ulong underflow_bits;

        private const ushort default_low = 0;
        private const ushort default_high = 0xffff;
        private const ushort MSD = 0x8000;
        private const ushort SSD = 0x4000;
        private ushort scale;

        public static long CompresionAritmeticaBytes(string texto)
        {
            CompresionAritmeticaInt compresor = new CompresionAritmeticaInt(texto); // Pasa el texto aquí
            var encoded = compresor.Compress(texto);
            return encoded.Length;
        }
        
        public CompresionAritmeticaInt(string source)
        {
            _source = source;
            _probabilities = new Dictionary<char, (ushort Low, ushort High)>();
            CalculateProbabilities();
        }

        public CompresionAritmeticaInt(Dictionary<char, (ushort Low, ushort High)> probabilities, ushort scale)
        {
            _source = string.Empty;
            _probabilities = probabilities;
            this.scale = scale;
        }

        private void CalculateProbabilities()
        {
            Dictionary<char, ushort> frequencies = new();

            foreach (char symbol in _source)
            {
                if (!frequencies.TryGetValue(symbol, out ushort value))
                {
                    value = 0;
                    frequencies[symbol] = value;
                }
                frequencies[symbol] = ++value;
            }

            frequencies = frequencies.OrderBy(x => x.Value).ThenBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

            scale = (ushort)_source.Length;

            ushort low = 0;

            foreach (var symbol in frequencies)
            {
                ushort high = (ushort)(low + symbol.Value);
                _probabilities[symbol.Key] = (low, high);
                low = high;
            }
        }

        public MemoryStream Compress(string input)
        {
            ushort low = default_low;
            ushort high = default_high;
            underflow_bits = 0;
            long range;

            BitWriter _output_stream = new();

            foreach (char symbol in input)
            {
                range = (long)(high - low) + 1;
                high = (ushort)(low + range * _probabilities[symbol].High / scale - 1);
                low = (ushort)(low + range * _probabilities[symbol].Low / scale);

                while (true)
                {
                    if ((high & MSD) == (low & MSD))
                    {
                        _output_stream.WriteBit(Convert.ToBoolean(high & MSD));

                        while (underflow_bits > 0)
                        {
                            _output_stream.WriteBit(!Convert.ToBoolean(high & MSD));
                            underflow_bits--;
                        }
                    }
                    else if ((low & SSD) != 0 && (high & SSD) == 0)
                    {
                        underflow_bits++;
                        low &= 0x3fff;
                        high |= 0x4000;
                    }
                    else
                    {
                        break;
                    }

                    low <<= 1;
                    high <<= 1;
                    high |= 1;
                }
            }

            _output_stream.WriteBit(Convert.ToBoolean(low & 0x4000));
            underflow_bits++;

            while (underflow_bits-- > 0)
            {
                _output_stream.WriteBit(!Convert.ToBoolean(low & 0x4000));
            }

            return _output_stream.Flush();
        }
    }

    public class BitWriter
    {
        // Implementation of BitWriter goes here
        public void WriteBit(bool bit) { }
        public MemoryStream Flush() => new MemoryStream();
    }

    public class BitReader
    {
        public BitReader(MemoryStream stream) { }
        public bool ReadBit() => true;
    }

