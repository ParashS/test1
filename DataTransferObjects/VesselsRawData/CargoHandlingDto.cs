using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObjects.VesselsRawData
{
    public class CargoHandlingDto
    {
        public long? CargoId { get;set; }
        public long? VesseldataRawID { get;set; }
        public int? CargoTypeID { get; set; }
        public string? Function { get; set; }
        public string? Berth { get; set; }
        public DateTime?  BLDate { get; set; }
        public decimal? BLGross { get; set; }
        public string? BLCode { get; set; }
        public string? CargoName { get; set; }
        public decimal? ShipGross { get; set; }
        public decimal? LoadTemp { get; set; }
        public decimal? APIGravity { get; set; }
        public string? UnitCode { get; set; }
        public string? Charterer { get; set; }
        public string? Consignee { get; set; }
        public string? Receiver { get; set; }
        public string? Shipper { get; set; }
        public string? Destination { get; set; }
        public string? LetterOfProtest { get; set; }
        public List<StowageDto>? Stowage { get; set; }
    }



}
