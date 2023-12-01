using Nancy.Diagnostics;
using Nancy.Json;
using System.Net.Http.Headers;
using File = System.IO.File;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var isbn = get_isbn(@"C:\test\ISBN_Input_File.txt", ',');

        if (isbn.Length > 0)
        {
            var filepath = @"c:\test\ISBN_Output_File.csv";
            int rowNumber = 1;

            using (StreamWriter writer = new StreamWriter(new FileStream(filepath, FileMode.Create, FileAccess.Write)))
            {
                foreach (string isbn_number in isbn)
                {
                    //Call API
                    HttpResponseMessage response = new HttpResponseMessage();

                    if ( await GetInfo(isbn_number, response) )
                        if (response.IsSuccessStatusCode)
                        {
                            var json = await response.Content.ReadAsStringAsync();

                            //TODO Server/Cache
                            var book = getBook(json);
                            
                            writer.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", rowNumber++, "Server", isbn_number, book.title, book.subtitle, book.authors_line, book.number_of_pages, book.publish_date));
                            ConsoleLog(book);
                        }
                }
            }
        }
    }

    private static async Task<bool> GetInfo(string isbn_number, HttpResponseMessage response)
    {
        try
        {
            using (var client = new HttpClient())
            {
                string url = @"https://openlibrary.org/isbn/" + isbn_number + ".json";
                url = string.Format(@"https://openlibrary.org/api/books?bibkeys=ISBN:{0}&jscmd=data&format=json", isbn_number);
                string urlParameters = string.Format("?ISBN={0}&jscmd=data&format=json", isbn_number);

                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                response = await client.GetAsync("");
                return true;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }

    private static Book getBook(string json)
    {
        var serializer = new JavaScriptSerializer();
        dynamic book = serializer.DeserializeObject(json);

        Book _ret = new Book();

        _ret.title = book[0].ContainsKey("title") ? book[0]["title"] : "Not found";

        _ret.subtitle = book[0].ContainsKey("subtitle") ? book[0]["subtitle"] : "Not found";
        var authors = book[0]["authors"];
        _ret.number_of_pages = book[0].ContainsKey("number_of_pages") ? book[0]["number_of_pages"] : "Not found";
        _ret.publish_date = book[0].ContainsKey("publish_date") ? book[0]["publish_date"] : "Not found";
        List<string> al = new List<string>();

        foreach (var author in authors)
        {
            al.Add(author.ContainsKey("name") ? author["name"] : "");
        }
        _ret.authors_line = string.Join(";", al);
 
        return _ret; 

    }

    private static void ConsoleLog(Book book)
    {
        Console.WriteLine(book.title);
        Console.WriteLine(book.subtitle);

        foreach (var author in book.authors_line.Split(";"))
        {
            Console.WriteLine(author);
        }

        Console.WriteLine(book.number_of_pages);
        Console.WriteLine(book.publish_date);
        Console.WriteLine("-------------------------------------------");
    }
    
    private static string[] get_isbn(string path, char separator)
    {
        try
        {
            if (File.Exists(path))
            {
                var lines = File.ReadAllLines(path);
                List<string> isbn_numbers = new List<string>();

                foreach (string line in lines)
                {
                    var values = line.Split(separator);
                    foreach (string val in values)
                    {
                        isbn_numbers.Add(val);
                    }
                }

                return isbn_numbers.ToArray();
            }
            return Array.Empty<string>();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return Array.Empty<string>();
        }
    }

   
}
public class Book
{
    public string? title { get; set; }
    public string? subtitle { get; set; }
    public long? number_of_pages { get; set; }
    public string? publish_date { get; set; }
    public string? authors_line { get; set; }
 
   

}
