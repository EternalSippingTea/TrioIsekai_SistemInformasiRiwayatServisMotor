using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemServisMotor
{
    internal class Servis
    {
        public string Nama_User { get; set; }
        public string Role { get; set; }
        public string Merk { get; set; }
        public int Tahun { get; set; }
        public string Nama_Pelanggan { get; set; }
        public string No_Plat { get; set; }          
        public DateTime Tanggal { get; set; }
        public string Jenis_Servis { get; set; }     
        public string Suku_Cadang { get; set; }      
        public int Biaya { get; set; }
        public string Catatan { get; set; }
    }
}