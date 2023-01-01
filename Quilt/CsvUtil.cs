using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Quilt
{
    public static class CsvUtil
    {
        public static bool SavePatternsToCsv(List<List<HexPatternPickerVM>> hexPatterns)
        {
            string csv = GenerateCsv(hexPatterns);

            var dlg = new SaveFileDialog();
            dlg.DefaultExt = ".csv";
            dlg.OverwritePrompt = false;
            if (dlg.ShowDialog() == true)
            {
                File.AppendAllText(dlg.FileName, csv);
            }

            return true;
        }

        private static string GenerateCsv(List<List<HexPatternPickerVM>> hexPatterns)
        {
            var csv = "";

            foreach (var region in hexPatterns)
            {
                foreach (var hex in region)
                {
                    foreach (var pattern in hex.GetColorPatterns())
                    {
                        csv += pattern.Count + "," + pattern.Color.Color.ToString().Remove(1,2);
                        csv += Environment.NewLine;
                    }
                    csv += "#,,";
                    csv += hex.Count;
                    {
                        csv += Environment.NewLine;
                    }
                }
            }

            return csv;
        }

        public static List<List<List<ColorPattern>>> GetPatternsFromCSV()
        {
            List<string> fileNames = new List<string>() 
            {
                "red", "purple", "orange", "yellow", "pink", "blue", "green", "multi"
            };
            var groupedPatterns = new List<List<List<ColorPattern>>>();

            foreach(string fileName in fileNames)
            {
                var patterns = new List<List<ColorPattern>>();
                groupedPatterns.Add(patterns);
                var quiltPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName($@"{Directory.GetCurrentDirectory()}")));
                var csvRows = File.ReadAllLines($@"{quiltPath}\colorgroups\{fileName}.csv", Encoding.Default).ToList();
                var currentList = new List<ColorPattern>();
                patterns.Add(currentList);
                foreach (var row in csvRows)
                {
                    if (!groupedPatterns.Contains(patterns))
                        groupedPatterns.Add(patterns);
                    if (!patterns.Contains(currentList))
                        patterns.Add(currentList);
                    var rowValues = row.Split(',');
                    var firstValue = rowValues[0];
                    if (!int.TryParse(firstValue, out int count))
                    {
                        if (firstValue == "#")
                        {
                            if (rowValues.Length > 2)
                            {
                                if (!int.TryParse(rowValues[2], out int multiple))
                                {
                                    multiple = 1;
                                }
                                for (int i = 1; i < multiple; i++)
                                {
                                    patterns.Add(currentList);
                                }
                            }
                            currentList = new List<ColorPattern>();
                        }
                        continue;
                    }

                    var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(rowValues[1]));
                    currentList.Add(new ColorPattern(count, brush));
                }

            }
            return groupedPatterns;
        }
    }
}
