
using DatosDecod;
using DatosDecod21;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Archivos
{
    public class Filtros
    {
        // aplicar los filtros seleccionados sobre la lista original de mensajes
        public List<Mensaje> AplicarFiltros(
            List<Mensaje> listaOriginal,
            bool cat48,
            bool cat21,
            bool eliminarBlancoPuro,
            bool eliminarTransponderFijo,
            bool filtrarTrayectoria,
            double trayectoriaMin,
            double trayectoriaMax,
            bool eliminarGround,
            bool filtrarAltura,
            double alturaMin,
            double alturaMax,
            bool filtrarP3 = false,
HashSet<Mensaje> mensajesP3 = null)
        {
            IEnumerable<Mensaje> listaFiltrada = listaOriginal;

            // mantener ambas categorías si están seleccionadas
            if (cat48 && cat21)
            {
                cat48 = false;
                cat21 = false;
            }

            // aplicar el filtro de categoría asterix
            if (cat48)
            {
                listaFiltrada = FiltroCategoria(listaFiltrada, 48);
            }

            if (cat21)
            {
                listaFiltrada = FiltroCategoria(listaFiltrada, 21);
            }

            // eliminar los blancos puros
            if (eliminarBlancoPuro)
            {
                listaFiltrada = FiltrarBlancoPuro(listaFiltrada);
            }

            // eliminar los mensajes de transpondedores fijos
            if (eliminarTransponderFijo)
            {
                listaFiltrada = FiltrarTransponderFijo(listaFiltrada);
            }

            // filtrar los mensajes según el rumbo de los aviones
            if (filtrarTrayectoria)
            {
                listaFiltrada = FiltrarTrayectoria(
                    listaFiltrada,
                    trayectoriaMin,
                    trayectoriaMax);
            }

            // eliminar los aviones detectados en tierra
            if (eliminarGround)
            {
                listaFiltrada = FiltrarGround(listaFiltrada);
            }

            // filtrar los mensajes según el nivel de vuelo
            if (filtrarAltura)
            {
                listaFiltrada = FiltrarAltura(
                    listaFiltrada,
                    alturaMin,
                    alturaMax);
            }
            // Conservar únicamente los vuelos del P3
            if (filtrarP3)
            {
                listaFiltrada = listaFiltrada.Where(m =>
                    m.categoria == 48 &&
                    mensajesP3 != null &&
                    mensajesP3.Contains(m));
            }
            // descartar los mensajes no válidos y devolver la lista filtrada
            listaFiltrada = FiltrarMensajesInvalidos(listaFiltrada);
            return listaFiltrada.ToList();
        }

        // conservar únicamente los mensajes dentro del intervalo de flight levels
        private IEnumerable<Mensaje> FiltrarAltura(
            IEnumerable<Mensaje> listaFiltro,
            double alturaMin,
            double alturaMax)
        {
            return listaFiltro.Where(mensaje =>
            {
                double flightLevel = double.NaN;

                // obtener el nivel de vuelo según la categoría del mensaje
                if (mensaje.categoria == 21)
                {
                    TiraDatosDecod021 mensaje21 =
                        (TiraDatosDecod021)mensaje.tira;

                    flightLevel = mensaje21.FlightLevel;
                }
                else if (mensaje.categoria == 48)
                {
                    TiraDatosDecod048 mensaje48 =
                        (TiraDatosDecod048)mensaje.tira;

                    flightLevel = mensaje48.FL.FL;
                }

                // descartar mensajes sin un nivel de vuelo válido
                if (double.IsNaN(flightLevel) ||
                    double.IsInfinity(flightLevel))
                {
                    return false;
                }

                return flightLevel >= alturaMin &&
                       flightLevel <= alturaMax;
            });
        }

        // conservar únicamente los mensajes de la categoría indicada
        private IEnumerable<Mensaje> FiltroCategoria(
            IEnumerable<Mensaje> listaFiltrada,
            int cat)
        {
            return listaFiltrada.Where(x => x.categoria == cat);
        }

        // eliminar los blancos puros conservando las detecciones modo s
        private IEnumerable<Mensaje> FiltrarBlancoPuro(
            IEnumerable<Mensaje> listaFiltro)
        {
            return listaFiltro.Where(mensaje =>
            {
                // mantener todos los mensajes que no sean cat48
                if (mensaje.categoria != 48)
                {
                    return true;
                }

                TiraDatosDecod048 mensaje48 =
                    (TiraDatosDecod048)mensaje.tira;

                string typ = mensaje48.TargetRep.TYP;

                // conservar únicamente las detecciones modo s
                return typ == "100" ||
                       typ == "101" ||
                       typ == "110" ||
                       typ == "111";
            });
        }

        // eliminar los mensajes identificados como transpondedores fijos
        private IEnumerable<Mensaje> FiltrarTransponderFijo(
            IEnumerable<Mensaje> listaFiltro)
        {
            return listaFiltro.Where(x => x.TransFijo == false);
        }

        // filtrar los mensajes según el rumbo calculado entre posiciones consecutivas
        private IEnumerable<Mensaje> FiltrarTrayectoria(
            IEnumerable<Mensaje> listaFiltro,
            double trayectoriaMin,
            double trayectoriaMax)
        {
            List<Mensaje> resultado = new List<Mensaje>();

            // guardar la última posición conocida de cada avión
            Dictionary<string, double[]> posicionesAnteriores =
                new Dictionary<string, double[]>();

            foreach (Mensaje mensaje in listaFiltro)
            {
                string identificador = "";
                double latitud = double.NaN;
                double longitud = double.NaN;

                // obtener la identificación y posición de los mensajes ads-b
                if (mensaje.categoria == 21)
                {
                    TiraDatosDecod021 mensaje21 =
                        (TiraDatosDecod021)mensaje.tira;

                    identificador = mensaje21.TargetAddress;

                    latitud = mensaje21.PosWGS84HighRes[0];
                    longitud = mensaje21.PosWGS84HighRes[1];
                }

                // obtener la identificación y posición de los mensajes radar
                else if (mensaje.categoria == 48)
                {
                    TiraDatosDecod048 mensaje48 =
                        (TiraDatosDecod048)mensaje.tira;

                    identificador = mensaje48.AircrftAddrs ?? "";

                    latitud = mensaje48.PositionCorrectedLatLonAlt[0];
                    longitud = mensaje48.PositionCorrectedLatLonAlt[1];
                }

                // descartar los mensajes sin una posición válida
                if (double.IsNaN(latitud) ||
                    double.IsNaN(longitud))
                {
                    continue;
                }

                // calcular el rumbo si existe una posición anterior del avión
                if (posicionesAnteriores.ContainsKey(identificador))
                {
                    double latAnterior =
                        posicionesAnteriores[identificador][0];

                    double lonAnterior =
                        posicionesAnteriores[identificador][1];

                    double rumbo = CalcularRumbo(
                        latAnterior,
                        lonAnterior,
                        latitud,
                        longitud);

                    // conservar el mensaje si el rumbo está dentro del intervalo
                    if (RumboDentroDelRango(
                        rumbo,
                        trayectoriaMin,
                        trayectoriaMax))
                    {
                        resultado.Add(mensaje);
                    }
                }

                // actualizar la última posición conocida del avión
                posicionesAnteriores[identificador] =
                    new double[] { latitud, longitud };
            }

            return resultado;
        }

        // calcular el rumbo entre dos posiciones geográficas
        private double CalcularRumbo(
            double lat1,
            double lon1,
            double lat2,
            double lon2)
        {
            double lat1Rad = lat1 * Math.PI / 180.0;
            double lat2Rad = lat2 * Math.PI / 180.0;

            double diferenciaLon =
                (lon2 - lon1) * Math.PI / 180.0;

            double y =
                Math.Sin(diferenciaLon) *
                Math.Cos(lat2Rad);

            double x =
                Math.Cos(lat1Rad) * Math.Sin(lat2Rad) -
                Math.Sin(lat1Rad) *
                Math.Cos(lat2Rad) *
                Math.Cos(diferenciaLon);

            double rumbo =
                Math.Atan2(y, x) * 180.0 / Math.PI;

            // convertir los rumbos negativos al intervalo de 0 a 360 grados
            if (rumbo < 0)
            {
                rumbo += 360;
            }

            return rumbo;
        }

        // comprobar si el rumbo está dentro del intervalo seleccionado
        private bool RumboDentroDelRango(
            double rumbo,
            double minimo,
            double maximo)
        {
            if (minimo <= maximo)
            {
                return rumbo >= minimo &&
                       rumbo <= maximo;
            }
            else
            {
                // permitir intervalos que atraviesan los 0 grados
                return rumbo >= minimo ||
                       rumbo <= maximo;
            }
        }

        // eliminar las detecciones radar correspondientes a aviones en tierra
        private IEnumerable<Mensaje> FiltrarGround(
            IEnumerable<Mensaje> listaFiltro)
        {
            return listaFiltro.Where(mensaje =>
            {
                // mantener los mensajes que no sean cat48
                if (mensaje.categoria != 48)
                {
                    return true;
                }

                TiraDatosDecod048 mensaje48 =
                    (TiraDatosDecod048)mensaje.tira;

                int stat = mensaje48.CommACAScapability.STAT;

                // conservar únicamente los estados considerados en vuelo
                return stat == 0 || stat == 2;
            });
        }

        // descartar los tracks radar no confirmados o sin identificar
        private IEnumerable<Mensaje> FiltrarMensajesInvalidos(
            IEnumerable<Mensaje> listaFiltro)
        {
            return listaFiltro.Where(mensaje =>
            {
                // mantener los mensajes que no sean cat48
                if (mensaje.categoria != 48)
                    return true;

                TiraDatosDecod048 mensaje48 =
                    (TiraDatosDecod048)mensaje.tira;

                // identificar las detecciones sin dirección ni identificador
                bool trackSinIdentificar =
                    string.IsNullOrEmpty(mensaje48.AircrftAddrs) &&
                    string.IsNullOrEmpty(mensaje48.AircrftIddent) &&
                    mensaje48.TargetRep.TYP == "001";

                return mensaje48.TrckStatus.CNF != 1 &&
                       !trackSinIdentificar;
            });
        }

        // preparar los mensajes eliminando únicamente las detecciones no válidas
        public List<Mensaje> PrepararMensajes(List<Mensaje> mensajes)
        {
            return FiltrarMensajesInvalidos(mensajes).ToList();
        }
        public List<Mensaje> FiltrarDespeguesP3(
            List<Mensaje> mensajes,
            HashSet<Mensaje> mensajesP3)
        {
            return mensajes
                .Where(m =>
                    m.categoria == 48 &&
                    mensajesP3.Contains(m))
                .ToList();
        }

    }
}
