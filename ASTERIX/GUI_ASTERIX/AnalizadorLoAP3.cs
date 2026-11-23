
using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;

namespace P3
{
    public class ResultadoLoAP3
    {
        public string categoriaPrecedente { get; set; } = "";
        public string categoriaSeguidor { get; set; } = "";

        public bool? mismaSID { get; set; } = null;
        public double minimaNM { get; set; } = double.NaN;
        public double distanciaNM { get; set; } = double.NaN;

        public bool? incumple { get; set; } = null;
        public string estado { get; set; } = "NO EVALUADA";
    }

    public class AnalizadorLoAP3
    {
        // Clasificación ICAO de tipo de aeronave para LoA.
        private readonly Dictionary<string, string> clasificaciones =
            new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase);

        // Orden de las columnas de la tabla del PDF
        private static readonly string[] categorias =
        {
            "HP", "R", "LP", "NR+", "NR-", "NR"
        };

        // Tabla de la página 25:
        // cada pareja contiene {misma SID, distinta SID}.
        // Filas: precedente; columnas: seguidor.
        private static readonly double[,,] minimos =
        {
            // Precedente HP
            {
                {5, 3}, {5, 3}, {5, 3},
                {3, 3}, {3, 3}, {3, 3}
            },

            // Precedente R
            {
                {7, 5}, {5, 3}, {5, 3},
                {3, 3}, {3, 3}, {3, 3}
            },

            // Precedente LP
            {
                {8, 6}, {6, 4}, {5, 3},
                {3, 3}, {3, 3}, {3, 3}
            },

            // Precedente NR+
            {
                {11, 8}, {9, 6}, {9, 6},
                {5, 3}, {3, 3}, {3, 3}
            },

            // Precedente NR-
            {
                {9, 9}, {9, 9}, {9, 9},
                {9, 6}, {5, 3}, {3, 3}
            },

            // Precedente NR
            {
                {9, 9}, {9, 9}, {9, 9},
                {9, 9}, {9, 9}, {5, 3}
            }
        };

        public AnalizadorLoAP3(string rutaExcel)
        {
            CargarClasificacion(rutaExcel);
        }

        private void CargarClasificacion(string ruta)
        {
            if (!File.Exists(ruta))
                throw new FileNotFoundException(
                    "No se encuentra el Excel de clasificación.", ruta);

            using (XLWorkbook libro = new XLWorkbook(ruta))
            {
                IXLWorksheet hoja = libro.Worksheet(1);

                IXLRow cabecera = hoja.FirstRowUsed();

                if (cabecera == null)
                    throw new InvalidDataException(
                        "El Excel de clasificación está vacío.");

                int primeraFila = cabecera.RowNumber() + 1;
                int ultimaFila = hoja.LastRowUsed().RowNumber();

                foreach (IXLCell encabezado in cabecera.CellsUsed())
                {
                    string categoria = encabezado.GetString()
                        .Trim()
                        .ToUpperInvariant()
                        .Replace(" ", "");

                    if (categoria != "HP" &&
                        categoria != "LP" &&
                        categoria != "NR" &&
                        categoria != "NR+" &&
                        categoria != "NR-")
                        continue;

                    int columna = encabezado.Address.ColumnNumber;

                    for (int fila = primeraFila;
                         fila <= ultimaFila; fila++)
                    {
                        string modelo = NormalizarModelo(
                            hoja.Cell(fila, columna)
                                .GetFormattedString());

                        if (modelo == "")
                            continue;

                        // No permitir clasificaciones contradictorias.
                        if (clasificaciones.TryGetValue(
                            modelo, out string anterior))
                        {
                            if (anterior != categoria)
                                throw new InvalidDataException(
                                    "El modelo " + modelo +
                                    " aparece en " + anterior +
                                    " y " + categoria);

                            continue;
                        }

                        clasificaciones.Add(modelo, categoria);
                    }
                }
            }

            if (clasificaciones.Count == 0)
                throw new InvalidDataException(
                    "No se encontraron modelos de aeronaves.");
        }

        private static string NormalizarModelo(string modelo)
        {
            return (modelo ?? "")
                .Trim()
                .ToUpperInvariant()
                .Replace(" ", "");
        }

        public string ObtenerCategoria(string modelo)
        {
            string clave = NormalizarModelo(modelo);

            if (clave == "")
                return "";

            if (clasificaciones.TryGetValue(
                clave, out string categoria))
                return categoria;

            // Regla del enunciado: si no aparece, Reactor.
            return "R";
        }
        public bool ModeloEncontradoEnExcel(string modelo)
        {
            string clave = NormalizarModelo(modelo);

            if (string.IsNullOrEmpty(clave))
                return false;

            return clasificaciones.ContainsKey(clave);
        }

        public static double ObtenerMinimo(
            string precedente,
            string seguidor,
            bool mismaSID)
        {
            int fila = Array.IndexOf(categorias, precedente);
            int columna = Array.IndexOf(categorias, seguidor);

            if (fila < 0 || columna < 0)
                return double.NaN;

            return minimos[fila, columna, mismaSID ? 0 : 1];
        }

        public ResultadoLoAP3 Evaluar(
            PlanVueloP3 precedente,
            PlanVueloP3 seguidor,
            double separacionTWR)
        {
            ResultadoLoAP3 resultado = new ResultadoLoAP3();
            resultado.distanciaNM = separacionTWR;

            if (precedente == null || seguidor == null)
            {
                resultado.estado = "SIN PLAN DE VUELO";
                return resultado;
            }

            // La LoA se aplica a despegues consecutivos
            // de la misma pista.
            if (precedente.pistaDespegue !=
                seguidor.pistaDespegue)
            {
                resultado.estado = "PISTAS DIFERENTES";
                return resultado;
            }

            resultado.categoriaPrecedente =
                ObtenerCategoria(precedente.tipoAeronave);

            resultado.categoriaSeguidor =
                ObtenerCategoria(seguidor.tipoAeronave);

            if (resultado.categoriaPrecedente == "" ||
                resultado.categoriaSeguidor == "")
            {
                resultado.estado = "TIPO DE AVIÓN DESCONOCIDO";
                return resultado;
            }

            // Los grupos SID ya están calculados por P3.
            string grupoPrecedente =
                (precedente.grupoSID ?? "").Trim().ToUpperInvariant();

            string grupoSeguidor =
                (seguidor.grupoSID ?? "").Trim().ToUpperInvariant();

            if ((grupoPrecedente != "G1" &&
                 grupoPrecedente != "G2" &&
                 grupoPrecedente != "G3") ||
                (grupoSeguidor != "G1" &&
                 grupoSeguidor != "G2" &&
                 grupoSeguidor != "G3"))
            {
                resultado.estado = "SID NO IDENTIFICADA";
                return resultado;
            }

            resultado.mismaSID =
                grupoPrecedente == grupoSeguidor;

            resultado.minimaNM = ObtenerMinimo(
                resultado.categoriaPrecedente,
                resultado.categoriaSeguidor,
                resultado.mismaSID.Value);

            if (!double.IsFinite(separacionTWR))
            {
                resultado.estado = "SIN SEPARACIÓN TWR";
                return resultado;
            }

            resultado.incumple =
                separacionTWR < resultado.minimaNM;

            resultado.estado = resultado.incumple.Value
                ? "INCUMPLE"
                : "CUMPLE";

            return resultado;
        }
    }
}
