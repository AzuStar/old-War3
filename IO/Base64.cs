namespace NoxRaven.IO
{
    public static class Base64
    {
        public const string base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

        // convert string to base64 manually
        public static string Base64Encode(string input)
        {
            string output = "";
            int[] buffer = new int[3];
            int bufferCount = 0;

            foreach (char c in input)
            {
                buffer[bufferCount++] = (int)c;

                if (bufferCount == 3)
                {
                    output += base64Chars[buffer[0] >> 2];
                    output += base64Chars[((buffer[0] & 3) << 4) | (buffer[1] >> 4)];
                    output += base64Chars[((buffer[1] & 15) << 2) | (buffer[2] >> 6)];
                    output += base64Chars[buffer[2] & 63];
                    bufferCount = 0;
                }
            }

            if (bufferCount > 0)
            {
                output += base64Chars[buffer[0] >> 2];
                if (bufferCount == 1)
                {
                    output += base64Chars[(buffer[0] & 3) << 4];
                    output += "==";
                }
                else // bufferCount == 2
                {
                    output += base64Chars[((buffer[0] & 3) << 4) | (buffer[1] >> 4)];
                    output += base64Chars[(buffer[1] & 15) << 2];
                    output += "=";
                }
            }

            return output;
        }

        public static string Base64Decode(string input)
        {
            string output = "";
            int[] buffer = new int[4];
            int bufferCount = 0;

            foreach (char c in input)
            {
                if (c == '=') break;
                buffer[bufferCount++] = base64Chars.IndexOf(c);

                if (bufferCount == 4)
                {
                    output += (char)((buffer[0] << 2) | ((buffer[1] & 48) >> 4));
                    if (buffer[2] != 64)
                    {
                        output += (char)(((buffer[1] & 15) << 4) | ((buffer[2] & 60) >> 2));
                        if (buffer[3] != 64)
                        {
                            output += (char)(((buffer[2] & 3) << 6) | buffer[3]);
                        }
                    }
                    bufferCount = 0;
                }
            }

            return output;
        }
    }
}