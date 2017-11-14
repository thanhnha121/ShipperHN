using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text.RegularExpressions;
using ShipperHN.Business.Entities;
using ShipperHN.Business.LOG;

namespace ShipperHN.Business
{
    class PhoneNumberBusiness
    {
        private readonly ShipperHNDBcontext _shipperHndBcontext;
        private readonly LogControl _logControl;

        public PhoneNumberBusiness(ShipperHNDBcontext shipperHndBcontext)
        {
            _logControl = new LogControl();
            _shipperHndBcontext = shipperHndBcontext;
        }

        public List<Match> DetetectPhoneNumber(string input)
        {
            if (input != null)
            {
                //remove " " and "."
                input = input.Replace(".", "");
                input = input.Replace(" ", "");

                //detect phone 10 number
                string exp10 = "(?<A>\\d{10})";
                MatchCollection phone10 = Regex.Matches(input, exp10);

                //detect phone 11 number
                string exp11 = "(?<A>\\d{11})";
                MatchCollection phone11 = Regex.Matches(input, exp11);

                //remove 10 phone number contain in phone 11 numb
                List<Match> rs = phone10.Cast<Match>().Where(phone => (phone.ToString()[1] == '1' || phone.ToString()[1] == '4' || phone.ToString()[1] == '9') && phone.ToString()[0] == '0').ToList();
                rs.AddRange(phone11.Cast<Match>().Where(phone => (phone.ToString()[1] == '1' || phone.ToString()[1] == '4' || phone.ToString()[1] == '9') && phone.ToString()[0] == '0'));

                //remove 10 phone number contain in phone 11 numb
                for (int i = 0; i < rs.Count(); i++)
                {
                    for (int j = i + 1; j < rs.Count(); j++)
                    {
                        if (rs[i].ToString().Contains(rs[j].ToString()))
                        {
                            rs.RemoveAt(j);
                            j--;
                        }
                        if (rs[j].ToString().Contains(rs[i].ToString()))
                        {
                            rs.RemoveAt(i);
                            i--;
                            break;
                        }
                    }
                }

                //count > 5 >> sim seller
                if (rs.Count > 5)
                {
                    return null;
                }
                return rs;
            }
            return null;

        }

        public void GetPhone(Match match, User user)
        {
            PhoneNumber pn = new PhoneNumber
            {
                Phone = match.ToString()
            };
            User u = _shipperHndBcontext.Users
                    .Include(x => x.PhoneNumbers)
                    .FirstOrDefault(x => x.Id.Equals(user.Id));
            {

                if (u != null)
                {
                    try
                    {
                        if (u.PhoneNumbers.FirstOrDefault(y => y.Phone.Equals(pn.Phone)) == null)
                        {
                            u.PhoneNumbers.Add(pn);
                            _shipperHndBcontext.SaveChanges();
                        }
                    }
                    catch (Exception e)
                    {
                        if (e.GetType() == typeof(DbUpdateException))
                        {
                            _logControl.AddLog(1, "PhoneNumberBusiness.cs/GetPhone", "Type: " + e.GetType()
                                    + " | Message: " + e.Message + " | InnerException: " + e.InnerException);
                            u.PhoneNumbers.Remove(pn);
                            _shipperHndBcontext.SaveChanges();
                        }
                    }
                }
            }
        }
    }
}
