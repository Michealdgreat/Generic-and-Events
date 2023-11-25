using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrapUpDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<PersonModel> people = new List<PersonModel> {

                new PersonModel {FirstName = "Micheal", LastName= "Shodamola", Email="mick@Micheal.com"},
                new PersonModel {FirstName = "Mike", LastName= "Shodamola", Email="mike@Micheal.com"},
                new PersonModel {FirstName = "Mitchel", LastName= "Shodamola", Email="mitchel@Micheal.com"},
            };


            List<CarModel> cars = new List<CarModel>
            {
                new CarModel {Manufacturer = "Toyota", Model = "Corolla"},
                new CarModel {Manufacturer = "Toyota", Model = "HighLander"},
                new CarModel {Manufacturer = "Ford", Model = "Mustang"},

            };

            DataAccess<PersonModel> peopleData = new DataAccess<PersonModel>();
            peopleData.BadEntryFound += PeopleData_BadEntryFound;
            peopleData.SaveToCSV(people, @"C:\Temp");

            DataAccess<CarModel> CarData = new DataAccess<CarModel>();
            CarData.BadEntryFound += PeopleData_BadEntryFound;

            CarData.SaveToCSV(cars, @"C:\Temp");
               

            Console.ReadLine();
        }

        private static void PeopleData_BadEntryFound(object sender, CarModel e)
        {
            Console.WriteLine($"Bad Entry founf for {e.Manufacturer} {e.Model}");

        }

        private static void PeopleData_BadEntryFound(object sender, PersonModel e)
        {
            Console.WriteLine($"Bad Entry founf for { e.FirstName} { e.LastName}");
        }
    }

        public class DataAccess<T> where T: new()
        {

        public event EventHandler<T> BadEntryFound;

        public void SaveToCSV<T>(List<T> items, string filePath)
        {
            List<string> rows = new List<string>();
            //reflection
            T entry = new T();

            var cols = entry.GetType().GetProperties();

            string row = "";
            foreach (var col in cols)
            {
                row += $", {col.Name},";
            }

            row = row.Substring(1); 
            rows.Add(row);

            foreach (var item in items)
            {
                row = "";
                bool BadWordDetected = false;

                foreach (var col in cols)
                {
                    string val = col.GetValue(item, null).ToString();

                    BadWordDetected = BadWordDetector(val);

                    if (BadWordDetected==true)
                    {

                        BadEntryFound?.Invoke(this, item);
                        break;
                    }
                    row += $",{val}";
                }
                    
                if (BadWordDetected == false)
                {
                    row = row.Substring(1);
                    rows.Add(row);
                }
                
                row = row.Substring(1);
                rows.Add (row);

            }

            File.WriteAllLines(filePath, rows);
        }

        private bool BadWordDetector(string stringToText)
        {
            bool output = false;
            string LowerCaseTest = stringToText.ToLower();

            if (LowerCaseTest.Contains("darn") || LowerCaseTest.Contains("heck"))
            {
                output = true;
            }

            return output;
        }

    }
}
