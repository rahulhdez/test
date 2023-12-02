using Nancy.Json;
using System.Net.Http.Headers;
using File = System.IO.File;

internal class Program
{


    private static async Task Main(string[] args)
    {
        var isbns = get_isbn(@"C:\test\ISBN_Input_File.txt", ',');

        if (isbns.Length > 0)
        {
            List<Book> books = new List<Book>();

            foreach (string isbn in isbns)
            {
                
                if (!getBookCache(ref books, isbn))
                {
                    Response r = await GetInfo(isbn);
                    if (r.valid & !object.Equals(r.response, null))
                    {
                        Book book;
                        string json = await r.response.Content.ReadAsStringAsync();
                        book = saveBook(json, isbn);
                        books.Add(book);
                    }
                }
            }

            saveCSV(books);
        }
    }

    private static void saveCSV(List<Book> books)
    {
        try
        {
            string outputfile = @"c:\test\ISBN_Output_File.csv";
            using (StreamWriter writer = new StreamWriter(new FileStream(outputfile, FileMode.Create, FileAccess.Write)))
            {
                int rowNumber = 1;
                foreach (Book b in books)
                {
                    writer.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", rowNumber++, b.isCache, b.isbn, b.title, b.subtitle, b.authors_line, b.number_of_pages, b.publish_date));
                    ConsoleLog(b);
                }
            }
            return;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return;
        }
    }

    private static bool getBookCache(ref List<Book> books, string isbn)
    {
        bool found = false;
        if (books.Count > 0)
        {
            foreach (Book b in books)
            {
                if (b.isbn == isbn)
                {
                    books.Add(new Book() { isbn = isbn, isCache=ENUM.Cache, title=b.title, subtitle = b.subtitle, number_of_pages = b.number_of_pages, publish_date = b.publish_date, authors_line = b.authors_line});

                    found = true;
                    break;

                }
            }
            if (found)
                return true;
            else
                return false;
        }
        else
            return false;
    }

    private static async Task<Response> GetInfo(string isbn_number)
    {
        try
        {
            var r = new Response();
            r.valid = false;
            using (var client = new HttpClient())
            {
                string url = @"https://openlibrary.org/isbn/" + isbn_number + ".json";
                url = string.Format(@"https://openlibrary.org/api/books?bibkeys=ISBN:{0}&jscmd=data&format=json", isbn_number);
                string urlParameters = string.Format("?ISBN={0}&jscmd=data&format=json", isbn_number);

                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                r.response = await client.GetAsync("");
                 
                r.valid = r.response.IsSuccessStatusCode;
               
                return r;
            }
        }
        catch (Exception e)
        {
            var r = new Response();
            r.valid = false;
            Console.WriteLine(e.Message);
            return r;
        }
    }

    private static Book saveBook(string json, string isbn)
    {
        //TODO : avoid using keyword dynamic
        var serializer = new JavaScriptSerializer();
        dynamic book = serializer.DeserializeObject(json);

        var _ret = new Book();

        _ret.isbn = isbn;
        _ret.isCache = ENUM.Server;
        _ret.title = book[0].ContainsKey("title") ? book[0]["title"] : "Not found";
        _ret.subtitle = book[0].ContainsKey("subtitle") ? book[0]["subtitle"] : "Not found";
        var authors = book[0]["authors"];
        _ret.number_of_pages = book[0].ContainsKey("number_of_pages") ? book[0]["number_of_pages"] : 0;
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
        Console.WriteLine(book.isCache);
        foreach (var author in book.authors_line.Split(";"))
        {
            Console.WriteLine(author);
        }

        Console.WriteLine(book.number_of_pages);
        Console.WriteLine(book.publish_date);
        Console.WriteLine("-------------------------------------------");
        return;
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

public enum ENUM { Server, Cache };

public class Book
{
    //TODO : Constructor
    public string? isbn { get; set; }
    public ENUM isCache { get; set; }
    public string? title { get; set; }
    public string? subtitle { get; set; }
    public long? number_of_pages { get; set; }
    public string? publish_date { get; set; }
    public string? authors_line { get; set; }



}

public class Response
{
    public bool valid { get; set; }

    public HttpResponseMessage? response { get; set; }
}
