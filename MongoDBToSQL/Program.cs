using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MongoDBToSQL
{
    class Program
    {
        public Program()
        {

        }

        private static string _sourceFile = @"C:\Users\Tolu\Documents\Visual Studio 2015\Projects\MongoDBToSQL\DataSet\RestaurantSmall.json";
        private static string _dataSet =
                "{\"address\": {\"building\": \"1007\", \"coord\": [-73.856077, 40.848447], \"street\": \"Morris Park Ave\", \"zipcode\": \"10462\"}, \"borough\": \"Bronx\", \"cuisine\": \"Bakery\", \"grades\": [{\"date\": {\"$date\": 1393804800000}, \"grade\": \"A\", \"score\": 2}, {\"date\": {\"$date\": 1378857600000}, \"grade\": \"A\", \"score\": 6}, {\"date\": {\"$date\": 1358985600000}, \"grade\": \"A\", \"score\": 10}, {\"date\": {\"$date\": 1322006400000}, \"grade\": \"A\", \"score\": 9}, {\"date\": {\"$date\": 1299715200000}, \"grade\": \"B\", \"score\": 14}], \"name\": \"Morris Park Bake Shop\", \"restaurant_id\": \"30075445\"}"
            ;

        private static ICollection<Restaurant> _allRestaurants = new List<Restaurant>();

        static void Main(string[] args)
        {
            //Read from file =>
            using (StreamReader file = File.OpenText(_sourceFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                Restaurant restaurant2 = (Restaurant) serializer.Deserialize(file, typeof(Restaurant));

                Console.WriteLine(restaurant2.Address.Building);
            }

            Restaurant restaurant = new Restaurant();
            JsonConvert.PopulateObject(_dataSet, restaurant);
            Console.WriteLine(restaurant.Address.Building);



            //var output = JsonConvert.DeserializeObject<Restaurant>(_dataSet);

            //foreach (var r in output.Grades)
            //{
            //    Console.WriteLine("Grade: {0} Score: {1} Date: {2}", r.GradeAwarded, r.Score, r.GradeDate);
            //    Console.WriteLine(r.GradeAwarded);
            //}

            ////Console.WriteLine(output.Name);
            //_allRestaurants.Add(output);
            //var dt = CreateDataTable(_allRestaurants);

            //ExportToCsv(dt,
            //    @"C:\Users\Tolu\Documents\Visual Studio 2015\Projects\MongoDBToSQL\DataSet\Output\Sample.csv");


            Console.ReadKey();
        }

        public static void ExportToCsv(DataTable dt, string filePath)
        {
            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dt.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }

            File.WriteAllText(filePath, sb.ToString());
        }

        public static DataTable JsonStringToTable(string jsonContent)
        {
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(jsonContent);
            return dt;
        }


        public static DataTable CreateDataTable<T>(IEnumerable<T> list)
        {
            Type type = typeof(T);
            var properties = type.GetProperties();

            DataTable dataTable = new DataTable();
            foreach (PropertyInfo info in properties)
            {
                dataTable.Columns.Add(new DataColumn(info.Name,
                    Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }

            foreach (T entity in list)
            {
                object[] values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(entity);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        public static string JsonToCsv(string jsonContent, string delimiter)
        {
            StringWriter csvString = new StringWriter();
            using (var csv = new CsvWriter(csvString))
            {
                csv.Configuration.SkipEmptyRecords = true;
                csv.Configuration.WillThrowOnMissingField = false;
                csv.Configuration.Delimiter = delimiter;

                using (var dt = JsonStringToTable(jsonContent))
                {
                    foreach (DataColumn column in dt.Columns)
                    {
                        csv.WriteField(column.ColumnName);
                    }
                    csv.NextRecord();

                    foreach (DataRow row in dt.Rows)
                    {
                        for (var i = 0; i < dt.Columns.Count; i++)
                        {
                            csv.WriteField(row[i]);
                        }
                        csv.NextRecord();
                    }
                }
            }
            return csvString.ToString();
        }



    }



    class Restaurant
    {
        public Address Address { get; set; }
        public string Borough { get; set; }
        public string Cuisine { get; set; }
        public ICollection<Grade> Grades { get; set; }
        public string Name { get; set; }
        public string Restaurant_Id { get; set; }
    }

        class Address
    {
        public string Building { get; set; }
        public ICollection<string> Coord { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
    }

    class Grade
    {
        public DateTime GradeDate { get; set; }
        public string grade { get; set; }
        public string Score { get; set; }
    }


    //class Coord
    //{
    //    ICollection<string> Coordinates { get; set; }
    //}
}
