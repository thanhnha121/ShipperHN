using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ShipperHN.Business.Entities
{
    public abstract class ShipperHNAbstract
    {
        private static readonly string[] Colors =
        {
            "linear-gradient(to bottom, #599bb3 5%, #408c99 100%)",
            "linear-gradient(to bottom, gold 0,darkorange 50%)",
            "linear-gradient(to bottom, #26759e 0,#133d5b 100%)",
            "linear-gradient(to bottom, #48d1cc 0,#9370db 50%)",
            "linear-gradient(to bottom, #bdb76b 0,#fc492d 50%)",
            "linear-gradient(to bottom, #ffc477 5%, #fb9e25 100%)",
            "linear-gradient(to bottom, #63b8ee 5%, #468ccf 100%)",
            "linear-gradient(to bottom, #c123de 5%, #a20dbd 100%)",
            "linear-gradient(to bottom, #fe1a00 5%, #ce0100 100%)",
            "linear-gradient(to bottom, #79bbff 5%, #378de5 100%)"
        };

        public static readonly string[] Locations =
        {
            "ba đình",
            "hoàn kiếm",
            "tây hồ",
            "long biên",
            "cầu giấy",
            "đống đa",
            "hai bà trưng",
            "hoàng mai",
            "thanh xuân",
            "hà đông",
            "bắc từ liêm",
            "nam từ liêm",
            "ngoại thành"
        };

        private readonly DateTime[] _curLocationsTimes = new DateTime[13];

        public void SetCurLocationsTimes(int index, DateTime dateTime)
        {
            _curLocationsTimes[index] = dateTime;
        }

        public DateTime GetCurLocationsDateTime(int index)
        {
            return _curLocationsTimes[index];
        }

        public string[] GetLocations()
        {
            return Locations;
        }

        public int GetLocationIndex(string input)
        {
            for (int i = 0; i < Locations.Count(); i++)
            {
                if (Locations[i].Equals(input))
                {
                    return i;
                }
            }
            return 13;
        }

        public DateTime[] GetLocationsDateTimes()
        {
            return _curLocationsTimes;
        }

        public string GetColor(string input)
        {
            int k = input.GetHashCode() % 10;
            if (k < 0)
            {
                k = 0 - k;
            }
            return Colors[k];
        }

        public string GetSHNCode()
        {
            string nothing = "nothing";
            DateTime dateTime = DateTime.Now;
            nothing += dateTime.ToString(CultureInfo.InvariantCulture);

            byte[] dateToBytes = Encoding.UTF8.GetBytes(nothing);

            string result = "";
            foreach (byte dateToByte in dateToBytes)
            {
                result += Convert.ToString(dateToByte, 2);
            }

            string result2 = result.Substring(1, result.Length - 1) + result.Substring(0, 1);

            return result2;
        }

        public bool Decode(string code)
        {
            return false;
        }
    }
}
