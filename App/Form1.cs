using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization;

namespace App
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            
        }


        async Task<string> QueryToString(string url)
        {
            using (var client = new HttpClient())
            {
                using (var stream = await client.GetStreamAsync(url))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        {
                            var line = reader.ReadLine();
                            listBox1.Items.Insert(0, line);
                            return line;
                        }
                    }
                }
            }
        }

        async Task<TResult> QueryToJson<TResult>(string url)
        {
            return JsonSerializer.Deserialize<TResult>(await QueryToString(url));
        }


        public class sse
        {
            public string name { get; set; }
        }

        public class recordings
        { 
            public string _id { get; set; }
        }

        public class recordings_result
        {
            public List<recordings> recordings { get; set; } = new List<recordings>();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if( button2.Text == "stop")
            {
                button2.Text = "start";
            }
            else
            {
                button2.Text = "stop";

            }
        }
        public class RefResolution
        {
            public int w { get; set; }
            public int h { get; set; }
        }


        public class Location
        {
            public Point point1 { get; set; } = new Point();
            public Point point2 { get; set; } = new Point();
            public RefResolution refResolution { get; set; }
        }

        public class Point
        {
            public int x { get; set; }
            public int y { get; set; }
        }



        public class Computed
        {
            public double a { get; set; }
            public double b { get; set; }
            public List<double> lineBearings { get; set; }
            public Point point1 { get; set; }
            public Point point2 { get; set; }
        }

        public class areas
        {
            public string color { get; set; }
            public string type { get; set; }
            public Location location { get; set; }
            public Computed computed { get; set; }
            public string name { get; set; }
            public int _total { get; set; }
            public int car { get; set; }
        }

        public class CounterHistory
        {
            public int frameId { get; set; }
            public DateTime timestamp { get; set; }
            public string area { get; set; }
            public string name { get; set; }
            public int id { get; set; }
            public double bearing { get; set; }
            public string countingDirection { get; set; }
            public double angleWithCountingLine { get; set; }
        }
        //class RolesConverter : JsonConverter
        //{
        //    public override bool CanConvert(Type objectType)
        //    {
        //        return objectType == typeof(areas[]);
        //    }

        //    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        //    {
        //        // deserialize as object
        //        var roles = serializer.Deserialize<JObject>(reader);
        //        var result = new List<Role>();

        //        // create an array out of the properties
        //        foreach (JProperty property in roles.Properties())
        //        {
        //            var role = property.Value.ToObject<Role>();
        //            role.Id = int.Parse(property.Name);
        //            result.Add(role);
        //        }

        //        return result.ToArray();
        //    }


        //    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}


        public class counter_result
        {
            //[JsonConverter(typeof(RolesConverter))]

            //public Dictionary<string, areas> areas { get; set; }

            public object areas { get; set; }
            public List<CounterHistory> counterHistory { get; set; } = new List<CounterHistory>();

        }

        Dictionary<int, CounterHistory> _FindCars = new Dictionary<int, CounterHistory>();

        async Task UpdateCheck()
        {
            try
            {
                var r = await QueryToJson<recordings_result>("http://172.19.88.167:8080/recordings?offset=0&limit=1");

                var r2 = await QueryToString("http://172.19.88.167:8080/recording/" + r.recordings[0]._id + "/counter");


                var counter_result = JsonSerializer.Deserialize<counter_result>(r2);

                //var jsonDoc = Newtonsoft.Json.JsonConvert.SerializeObject(r3.areas.ToString());
                var areas = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<MongoDB.Bson.BsonDocument>(counter_result.areas.ToString());


                foreach (var item in counter_result.counterHistory)
                {
                    if( _FindCars.ContainsKey(item.id) ==false)
                    {
                        _FindCars.Add(item.id, item);

                        listBox1.Items.Insert(0, "event car : "+ item.id.ToString());

                        var a = GetArea(areas, item.area);

                        listBox1.Items.Insert(0, "event line : " + a.name);

                    }
                    

                }

                //var dic = bsonDoc.ToDictionary();


                //var a = BsonSerializer.Deserialize<List<string,areas>>(bsonDoc);

                //bsonDoc.FindAs<MyType>(_document);


                //foreach (var item in a)
                //{

                //}

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }


        }

        private areas GetArea(MongoDB.Bson.BsonDocument areas, string area)
        {
            foreach (var item in areas)
            {
                if( item.Name == area)
                {
                    
                    //MongoDB.Bson.Serialization.BsonSerializer.Deserialize<areas>(item.Value);

                    var a = new areas();
                    Location location = new Location();
                    location.point1.x = item.Value["location"]["point1"]["x"].AsInt32;
                    location.point1.y = item.Value["location"]["point1"]["y"].AsInt32;

                    location.point2.x = item.Value["location"]["point2"]["x"].AsInt32;
                    location.point2.y = item.Value["location"]["point2"]["y"].AsInt32;

                    a.location = location;
                    a.type = item.Value["type"].AsString;
                    a.name = item.Value["name"].AsString;
                    //a.location.point1.x
                    return a;
                }
            }

            return null;
        }

        private void button3_Click(object sender, EventArgs e)
        {

            //await UpdateCheck();

            //JsonSerializer.Deserialize<TResult>(r2);



            if ( button3.Text != "stop")
            {
                button3.Text = "stop";
                timer1.Stop();
            }
            else
            {
                timer1.Start();
                button3.Text = "start";
            }
            
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            await UpdateCheck();
            //using (var client = new HttpClient())
            //{
            //    using (var stream = await client.GetStreamAsync("http://172.19.90.198:8080/tracker/sse"))
            //    {
            //        using (var reader = new StreamReader(stream))
            //        {
            //            while (button2.Text == "stop")
            //            {
            //                await Task.Delay(1000);
            //                listBox1.Items.Insert(0, reader.ReadLine());


            //            }
            //        }
            //    }
            //}
        }
    }
}
