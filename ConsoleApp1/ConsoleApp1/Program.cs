using Nancy.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

internal class Program
{

    private static async Task Main()
    {
        var isbns = readISBN(@"C:\test\ISBN_Input_File.txt", ',');

        if (isbns.Length <= 0)
            return;

        List<Book> books = new List<Book>();

        foreach (string isbn in isbns)
        {
            if (getBookCache(ref books, isbn))
                continue;

            Response r = await getOpenLibraryBook(isbn);
            if (!r.valid)
                continue;

            if(object.Equals(r.response,null))
                continue;

            string json = await r.response.Content.ReadAsStringAsync();
            var book = readBookInfo(json, isbn);
            books.Add(book);


        }

        saveCSV(books);

    }

    private static string[] readISBN(string path, char separator)
    {
        try
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("No isbn numbers found");
                return Array.Empty<string>();
            }
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
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return Array.Empty<string>();
        }
    }

    private static bool getBookCache(ref List<Book> books, string isbn)
    {
        try
        {
            foreach (Book b in books)
            {
                if (b.isbn != isbn)
                    continue;

                books.Add(new Book { isbn = isbn, isCache = ENUM.Cache, title = b.title, subtitle = b.subtitle, number_of_pages = b.number_of_pages, publish_date = b.publish_date, authors_line = b.authors_line });

                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }

    }

    private static async Task<Response> getOpenLibraryBook(string isbn_number)
    {
        try
        {
            var r = new Response { };

            using (var client = new HttpClient())
            {
                string url = string.Format("https://openlibrary.org/api/books?bibkeys=ISBN:{0}&jscmd=data&format=json", isbn_number);

                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                r.response = await client.GetAsync("");
                r.valid = r.response.IsSuccessStatusCode;
            }
            return r;
        }
        catch (Exception e)
        {
            var r = new Response { valid = false };
            Console.WriteLine(e.Message);
            return r;
        }
    }

    private static Book readBookInfo(string json, string isbn)
    {
        var serializer = new JavaScriptSerializer();
        dynamic book = serializer.DeserializeObject(json);
        List<string> al = new List<string>();

        if (book[0].ContainsKey("authors")) 
        {
            var authors = book[0]["authors"];
            foreach (var author in authors)
            {
                al.Add(author.ContainsKey("name") ? author["name"] : "");
            }
        }

        var _ret = new Book
        {
            isbn = isbn,
            isCache = ENUM.Server,
            title = book[0].ContainsKey("title") ? book[0]["title"] : "Not found",
            subtitle = book[0].ContainsKey("subtitle") ? book[0]["subtitle"] : "Not found",
            number_of_pages = book[0].ContainsKey("number_of_pages") ? book[0]["number_of_pages"] : 0,
            publish_date = book[0].ContainsKey("publish_date") ? book[0]["publish_date"] : "Not found",
            //_ret.publish_date = _ret.publish_date.Replace(',',' ');
            authors_line = string.Join(";", al)
        };

        return _ret; 
    }

    private static int saveCSV(List<Book> books)
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
            return 0;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return -1;
        }
    }

    private static int ConsoleLog(Book book)
    {
        try
        {
            Console.WriteLine(book.title);
            Console.WriteLine(book.isCache);
            Console.WriteLine(book.subtitle);

            if (!object.Equals(book.authors_line, null)){
                foreach (var author in book.authors_line.Split(";"))
                {
                    Console.WriteLine(author);
                }
            }

            Console.WriteLine(book.number_of_pages);
            Console.WriteLine(book.publish_date);
            Console.WriteLine("-------------------------------------------");
            return 0;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return -1;
        }
    }


}

public enum ENUM { Server, Cache }

public class Book
{

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
