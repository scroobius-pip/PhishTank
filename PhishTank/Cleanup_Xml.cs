using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Cleanup_Xml
{
    public static string ReplaceTextInFile(string input)
    {
        return new string(input.Where(value =>
    (value >= 0x0020 && value <= 0xD7FF) ||
    (value >= 0xE000 && value <= 0xFFFD) ||
    value == 0x0009 ||
    value == 0x000A ||
    value == 0x000D).ToArray());
    }
}