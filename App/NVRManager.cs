//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;

//namespace App
//{
//    public class NVRManager
//    {
//        public static NVRManager I = new NVRManager();


//        HttpClient client = new HttpClient() { BaseAddress = new Uri("https://localhost:5004") };


//        public async Task<Dictionary<NVRInfo, TResult>> HttpPost<TResult>(string query)
//        {
//            return await HttpPost<TResult, string>(query, null);
//        }

//        public async Task<Dictionary<NVRInfo, TResult>> HttpPost<TResult, TBody>(string query, TBody body)
//        {
//            var r = new Dictionary<NVRInfo, TResult>();
//            try
//            {
//                var nvrList = NVRInfo.LoadNVRInfo();
//                foreach (var item in nvrList)
//                {
//                    var r2 = await HttpPostByHost<TResult, TBody>(item, query, body);
//                    if (r2 == null)
//                        continue;

//                    r.Add(item, r2);

//                }

//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e.Message);
//            }

//            return r;
//        }

//        public async Task<TResult> HttpPostByHost<TResult, TBody>(NVRInfo nvrInfo, string query, TBody body)
//        {
//            //using (var client = new HttpClient() { BaseAddress = new Uri("https://localhost:5004") })
//            try
//            {
//                using (var client = new HttpClient() { BaseAddress = new Uri(nvrInfo.IP) })
//                {

//                    StringContent content = null;

//                    if (body != null)
//                    {
//                        content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
//                    }


//                    HttpResponseMessage response = await client.PostAsync(query, content);

//                    if (response.IsSuccessStatusCode)
//                    {
//                        if (typeof(TResult) == typeof(string))
//                        {
//                            var r = await response.Content.ReadAsStringAsync();
//                            return (TResult)Convert.ChangeType(r, typeof(TResult));
//                        }

//                        var str = await response.Content.ReadAsStreamAsync();

//                        return await JsonSerializer.DeserializeAsync<TResult>(str);
//                    }

//                }

//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e.Message);
//            }

//            return default(TResult);

//        }

//    }
//}
