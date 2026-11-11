
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace P3
{
    public class LectorPlanesVueloP3
    {
        public List<PlanVueloP3> LeerExcel(string ruta)
        {
            List<PlanVueloP3> planes = new List<PlanVueloP3>();

            using (XLWorkbook libro = new XLWorkbook(ruta))
            {
                IXLWorksheet hoja = libro.Worksheet(1);

                IXLRow encabezado = hoja.FirstRowUsed()
                    ?? throw new InvalidDataException(
                        "El Excel está vacío.");

                // Guardar las posiciones de las columnas
                Dictionary<string, int> columnas =
                    new Dictionary<string, int>();

                foreach (IXLCell celda in encabezado.CellsUsed())
                {
                    string nombre = Normalizar(celda.GetString());

                    columnas[nombre] = celda.Address.ColumnNumber;
                }

                // Comprobar columnas obligatorias
                string[] obligatorias =
                {
                    "indicativo", "atot", "pistadesp"
                };

                foreach (string nombre in obligatorias)
                {
                    if (!columnas.ContainsKey(nombre))
                    {
                        throw new InvalidDataException(
                            "Falta la columna " + nombre);
                    }
                }

                int ultimaFila = hoja.LastRowUsed().RowNumber();

                for (int i = encabezado.RowNumber() + 1;
                     i <= ultimaFila; i++)
                {
                    IXLRow fila = hoja.Row(i);

                    string indicativo = Texto(
                        fila, columnas, "indicativo")
                        .ToUpperInvariant();

                    if (string.IsNullOrWhiteSpace(indicativo))
                        continue;

                    // Eliminar el prefijo LEBL-
                    string pista = Texto(
                        fila, columnas, "pistadesp")
                        .ToUpperInvariant()
                        .Replace("LEBL-", "")
                        .Replace("RWY", "")
                        .Replace(" ", "");

                    // Solamente 24L y 06R
                    if (pista != "24L" && pista != "06R")
                        continue;

                    double atot = Tiempo(
                        fila, columnas, "atot");

                    if (!double.IsFinite(atot) ||
                        atot < 0 || atot >= 86400)
                    {
                        throw new InvalidDataException(
                            "ATOT no válido en fila " + i);
                    }

                    PlanVueloP3 plan = new PlanVueloP3
                    {
                        id = Texto(fila, columnas, "id"),

                        indicativo = indicativo,

                        origen = Texto(
                            fila, columnas, "origen"),

                        destino = Texto(
                            fila, columnas, "destino"),

                        horadespegue = Tiempo(
                            fila, columnas, "horadespegue"),

                        atot = atot,

                        rutaSACTA = Texto(
                            fila, columnas, "rutasacta"),

                        tipoAeronave = Texto(
                            fila, columnas, "tipoaeronave"),

                        estela = Texto(
                            fila, columnas, "estela"),

                        estelaRECAT = Texto(
                            fila, columnas, "estelarecat"),

                        procDesp = Texto(
                            fila, columnas, "procdesp"),

                        pistaDespegue = pista
                    };

                    planes.Add(plan);
                }
            }

            return planes.OrderBy(p => p.atot).ToList();
        }

        private static string Normalizar(string valor)
        {
            return new string(valor.Trim()
                .ToLowerInvariant()
                .Where(char.IsLetterOrDigit)
                .ToArray());
        }

        private static string Texto(
            IXLRow fila,
            Dictionary<string, int> columnas,
            string nombre)
        {
            if (!columnas.TryGetValue(nombre, out int indice))
                return "";

            return fila.Cell(indice)
                .GetFormattedString().Trim();
        }

        private static double Tiempo(
            IXLRow fila,
            Dictionary<string, int> columnas,
            string nombre)
        {
            if (!columnas.TryGetValue(nombre, out int indice))
                return double.NaN;

            IXLCell celda = fila.Cell(indice);

            if (celda.IsEmpty())
                return double.NaN;

            if (celda.TryGetValue<DateTime>(out DateTime fecha))
                return fecha.TimeOfDay.TotalSeconds;

            if (celda.TryGetValue<TimeSpan>(out TimeSpan hora))
                return hora.TotalSeconds;

            if (celda.TryGetValue<double>(out double numero))
                return (numero - Math.Floor(numero)) * 86400;

            string texto = celda.GetFormattedString().Trim();

            if (TimeSpan.TryParse(
                texto,
                CultureInfo.GetCultureInfo("es-ES"),
                out hora))
            {
                return hora.TotalSeconds;
            }

            if (DateTime.TryParse(
                texto,
                CultureInfo.GetCultureInfo("es-ES"),
                DateTimeStyles.None,
                out fecha))
            {
                return fecha.TimeOfDay.TotalSeconds;
            }

            throw new InvalidDataException(
                "Hora incorrecta en fila " +
                fila.RowNumber() + ": " + texto);
        }
    }
}
