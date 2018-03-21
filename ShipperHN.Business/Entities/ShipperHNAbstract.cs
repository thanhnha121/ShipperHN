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
            "crimson",
            "chocolate",
            "royalblue",
            "purple",
            "orange",
            "navy",
            "gray",
            "cornflowerblue",
            "brown",
            "limegreen"
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
