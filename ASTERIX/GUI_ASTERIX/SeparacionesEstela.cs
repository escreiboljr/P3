
using System;
using System.Collections.Generic;

namespace P3
{
    public static class SeparacionesEstela
    {
        private static readonly Dictionary<
            (string, string), (double nm, double? segundos)> tablaTWR =
            new Dictionary<
                (string, string), (double, double?)>
        {
            { ("J", "H"), (6, 120) },
            { ("J", "M"), (7, 180) },
            { ("J", "L"), (8, 180) },

            { ("H", "H"), (4, null) },
            { ("H", "M"), (5, 120) },
            { ("H", "L"), (6, 120) },

            { ("M", "L"), (5, 120) }
        };

        private static readonly Dictionary<
            (string, string), double> tablaTMA =
            new Dictionary<(string, string), double>
        {
            { ("J", "H"), 6 },
            { ("J", "M"), 7 },
            { ("J", "L"), 8 },

            { ("H", "H"), 4 },
            { ("H", "M"), 5 },
            { ("H", "L"), 6 },

            { ("M", "L"), 5 }
        };

        // Convertir la categoría del Excel a un código uniforme
        public static string NormalizarCategoria(string categoria)
        {
            string valor = (categoria ?? "")
                .Trim()
                .ToUpperInvariant();

            switch (valor)
            {
                case "J":
                case "SUPER":
                case "SUPERPESADA":
                    return "J";

                case "H":
                case "HEAVY":
                case "PESADA":
                    return "H";

                case "M":
                case "MEDIUM":
                case "MEDIA":
                    return "M";

                case "L":
                case "LIGHT":
                case "LIGERA":
                    return "L";

                default:
                    return "";
            }
        }
        public static bool CategoriaValida(string categoria)
        {
            return NormalizarCategoria(categoria) != "";
        }

        public static double? MinimaTWR(
            string precedente,
            string seguidor)
        {
            var clave = (
                NormalizarCategoria(precedente),
                NormalizarCategoria(seguidor)
            );

            if (tablaTWR.TryGetValue(clave, out var minimo))
                return minimo.nm;

            return null;
        }

        public static double? TiempoTWR(
            string precedente,
            string seguidor)
        {
            var clave = (
                NormalizarCategoria(precedente),
                NormalizarCategoria(seguidor)
            );

            if (tablaTWR.TryGetValue(clave, out var minimo))
                return minimo.segundos;

            return null;
        }

        public static double? MinimaTMA(
            string precedente,
            string seguidor)
        {
            var clave = (
                NormalizarCategoria(precedente),
                NormalizarCategoria(seguidor)
            );

            if (tablaTMA.TryGetValue(clave, out double minimo))
                return minimo;

            return null;
        }

    }
}
