//using Nancy.Json;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System.Collections;
//using System.Net.Http.Headers;
//using static System.Net.WebRequestMethods;
//using static System.Reflection.Metadata.BlobBuilder;
//using File = System.IO.File;

//string file = @"C:\test\ISBN_Input_File.txt";

//if (File.Exists(file))
//{
//    //Reading ISBN
//    var lines = File.ReadAllLines(file);
//    List<string> isbn = new List<string>();

//    foreach (string line in lines)
//    {
//        var values = line.Split(',');
//        foreach (string val in values)
//        {
//            isbn.Add(val);
//        }
//    }


//    int row = 1;
//    var filepath = @"c:\test\ISBN_Output_File.csv";
//    using (StreamWriter writer = new StreamWriter(new FileStream(filepath, FileMode.Create, FileAccess.Write)))
//    {
//        foreach (string isbn_number in isbn)
//        {
//            //Call API
//            using (var client = new HttpClient())
//            {
//                string url = @"https://openlibrary.org/isbn/" + isbn_number + ".json";
//                url = string.Format(@"https://openlibrary.org/api/books?bibkeys=ISBN:{0}&jscmd=data&format=json", isbn_number);
//                string urlParameters = String.Format("?ISBN={0}&jscmd=data&format=json", isbn_number);

//                client.BaseAddress = new Uri(url);
//                client.DefaultRequestHeaders.Accept.Clear();
//                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

//                HttpResponseMessage response = await client.GetAsync("");
//                if (response.IsSuccessStatusCode)
//                {

//                    var json = await response.Content.ReadAsStringAsync();
//                    var serializer = new JavaScriptSerializer();
//                    dynamic book = serializer.DeserializeObject(json);
 

//                    var title = book[0].ContainsKey("title") ? book[0]["title"] : "";
//                    var subtitle = book[0].ContainsKey("subtitle") ? book[0]["subtitle"] : "Not found";
//                    var authors = book[0]["authors"];
//                    var number_of_pages = book[0].ContainsKey("number_of_pages") ? book[0]["number_of_pages"] : "Not found";
//                    var publish_date = book[0].ContainsKey("publish_date") ? book[0]["publish_date"] : "";
                    
//                    List<string> al = new List<string>();
//                    foreach (var author in authors)
//                    { 
//                         al.Add(author.ContainsKey("name") ? author["name"] : "");
//                    }
//                    string authors_line = string.Join(";", al);
                    
//                    string line = String.Format("{0},{1},{2},{3},{4},{5},{6},{7}", row++,"Server", isbn_number, title, subtitle, authors_line, number_of_pages, publish_date);

//                    writer.WriteLine(line);


//                    Console.WriteLine(title);
//                    Console.WriteLine(subtitle);

                    
//                    foreach (var author in authors_line.Split(";"))
//                    {
//                        //string a = author.ContainsKey("name") ? author["name"] : ""; ;
//                        Console.WriteLine(author);
//                    }
//                    Console.WriteLine(number_of_pages);
//                    Console.WriteLine(publish_date);
//                    Console.WriteLine("-------------------------------------------");



//                }
//            }
//        }
//    }
//}


