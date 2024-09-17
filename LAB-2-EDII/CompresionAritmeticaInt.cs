    

    using System.Text;
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
        
        
        public static long C
        public CompresionAritmeticaInt(string source)
        {
            _source = source;
            _probabilities = [];
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
            Dictionary<char, ushort> frequencies = [];

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
            ushort low, high;
            // Initialize output buffer
            BitWriter _output_stream = new();

            // Initialize low and high to their default values
            low = default_low;
            high = default_high;
            underflow_bits = 0;
            long range;
            string output = string.Empty;

            // Iterate through each character in the input string
            foreach (char symbol in input.ToCharArray())
            {
                // Calculate the range
                range = (long)(high - low) + 1;
                // Update high and low based on the symbol's probability range
                
                high = (ushort)(low + range * _probabilities[symbol].High / scale - 1);

                
                low = (ushort)(low + range * _probabilities[symbol].Low / scale);
                
                // Normalize the range to avoid overflow and underflow
                while (true)
                {
                    // If the most significant bits of high and low are the same
                    if ((high & MSD) == (low & MSD))
                    {
                        // Output the most significant bit
                        _output_stream.WriteBit(Convert.ToBoolean(high & MSD));
                        output += Convert.ToBoolean(high & MSD) ? '1' : '0';
                        
                        while (underflow_bits > 0)
                        {
                            _output_stream.WriteBit(!Convert.ToBoolean(high & MSD));
                            output += Convert.ToBoolean(~high & MSD) ? '1' : '0';
                            underflow_bits--;
                        }
                    }
                    // If low is in the upper half and high is in the lower half
                    else
                    {
                        
                        if (Convert.ToBoolean(low & SSD) && !Convert.ToBoolean(high & SSD))
                        {
                            // Increment underflow bits
                            underflow_bits += 1;
                            // Adjust low and high
                            low &= 0x3fff;
                            high |= 0x4000;
                        }
                        else
                        {
                            break;
                        }
                    }

                    // Shift low and high to normalize the range
                    low <<= 1;
                    high <<= 1;
                    high |= 1;

                }
            }

            // Output the final bits
            _output_stream.WriteBit(Convert.ToBoolean(low & 0x4000));
            output += Convert.ToBoolean(low & 0x4000) ? '1' : '0';
            

            underflow_bits++;
            while (underflow_bits-- > 0)
            {
                _output_stream.WriteBit(Convert.ToBoolean(~low & 0x4000));
                output += Convert.ToBoolean(~low & 0x4000) ? '1' : '0';
            }
            
            // Write the last byte to the output buffer
            var compressed2 = _output_stream.Flush();

            // Return the compressed data
            return compressed2;
        }

        public static string GetMemoryStreamBinaryString(MemoryStream memoryStream)
        {
            // Save the current position
            long originalPosition = memoryStream.Position;

            // Read the MemoryStream into a byte array
            byte[] bytes = memoryStream.ToArray();

            // Restore the original position
            memoryStream.Position = originalPosition;

            // Convert each byte to binary and concatenate the results
            StringBuilder binaryStringBuilder = new();
            foreach (byte b in bytes)
            {
                // Convert byte to binary string and pad with leading zeros to ensure it's 8 bits
                string binaryString = Convert.ToString(b, 2).PadLeft(8, '0');
                binaryStringBuilder.Append(binaryString);
            }

            // Get the combined binary string
            string combinedBinaryString = binaryStringBuilder.ToString();

            // Divide the combined binary string into 4-bit groups separated by hyphens
            StringBuilder formattedBinaryStringBuilder = new();
            for (int i = 0; i < combinedBinaryString.Length; i += 4)
            {
                if (i > 0)
                {
                    formattedBinaryStringBuilder.Append('-');
                }

                // Append each 4-bit group
                formattedBinaryStringBuilder.Append(combinedBinaryString.AsSpan(i, 4));
            }

            return formattedBinaryStringBuilder.ToString();
        }
    }

