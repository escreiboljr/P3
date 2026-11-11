namespace P3
{
    public class PlanVueloP3
    {
        public string id { get; set; } = "";
        public string indicativo { get; set; } = "";
        public string origen { get; set; } = "";
        public string destino { get; set; } = "";

        public double horadespegue { get; set; } = double.NaN;
        public double atot { get; set; } = double.NaN;

        public string rutaSACTA { get; set; } = "";
        public string tipoAeronave { get; set; } = "";
        public string estela { get; set; } = "";
        public string estelaRECAT { get; set; } = "";
        public string procDesp { get; set; } = "";
        public string pistaDespegue { get; set; } = "";
    }
}