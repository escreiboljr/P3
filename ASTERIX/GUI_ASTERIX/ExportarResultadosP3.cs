
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace P3
{
    public static class ExportarResultadosP3
    {
        // Columnas del CSV, en el orden deseado.
        public static readonly string[] columnas =
        {
            "Precedente",
            "Seguidor",
            "ATOT precedente",
            "ATOT seguidor",

            "ToD TWR",
            "Distancia TWR NM",
            "Diferencia altitud TWR ft",

            "ToD minima TMA",
            "Distancia minima TMA NM",
            "Diferencia altitud minima TMA ft",

            "Inc Radar TWR",
            "Inc Radar TMA",
            "Perdida operativa Radar TWR",
            "Perdida operativa Radar TMA",

            "Tiempo entre DEP s",
            "Tiempo minimo estela TWR s",

            "Min distancia estela TWR NM",
            "Inc Estela TWR distancia",
            "Inc Estela TWR tiempo",

            "Min distancia estela TMA NM",
            "Inc Estela TMA distancia",

            "Perdida operativa Estela TWR",
            "Perdida operativa Estela TMA",

            "Inc LoA",
            "Min distancia LoA NM",
            "Misma agrupacion SID",

            "Estela precedente",
            "Estela seguidor",

            "Clasificacion LoA precedente",
            "Clasificacion LoA seguidor",

            "SID precedente",
            "SID seguidor",
            "Grupo SID precedente",
            "Grupo SID seguidor",

            "Modelo precedente",
            "Modelo seguidor",

            "Runway DEP",
            "Estado separacion",
            "Estado estela",
            "Estado LoA"
        };

        public static void Exportar(
            string ruta,
            IEnumerable<Dictionary<string, string>> filas)
        {
            using (StreamWriter escritor = new StreamWriter(
                ruta, false, new UTF8Encoding(true)))
            {
                // Encabezados
                escritor.WriteLine(string.Join(
                    ";", columnas.Select(Escapar)));

                foreach (Dictionary<string, string> fila in filas)
                {
                    List<string> valores = new List<string>();

                    foreach (string columna in columnas)
                    {
                        string valor;

                        if (!fila.TryGetValue(columna, out valor))
                            valor = "N/A";

                        valores.Add(Escapar(valor));
                    }

                    escritor.WriteLine(
                        string.Join(";", valores));
                }
            }
        }

        // Escapar caracteres especiales de CSV
        private static string Escapar(string valor)
        {
            valor = valor ?? "";

            if (valor.Contains(";") ||
                valor.Contains("\"") ||
                valor.Contains("\n") ||
                valor.Contains("\r"))
            {
                return "\"" + valor.Replace("\"", "\"\"") + "\"";
            }

            return valor;
        }

        public static string Numero(double valor, int decimales = 3)
        {
            if (!double.IsFinite(valor))
                return "N/A";

            return valor.ToString(
                "F" + decimales,
                CultureInfo.GetCultureInfo("es-ES"));
        }

        public static string Hora(double segundos)
        {
            if (!double.IsFinite(segundos) || segundos < 0)
                return "N/A";

            return TimeSpan.FromSeconds(segundos)
                .ToString(@"hh\:mm\:ss\.fff");
        }

        public static string Indicador(bool? valor)
        {
            if (!valor.HasValue)
                return "N/A";

            return valor.Value ? "True" : "False";
        }

        public static string Texto(string valor)
        {
            return string.IsNullOrWhiteSpace(valor)
                ? "N/A"
                : valor.Trim();
        }
    }
}
