// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
using Newtonsoft.Json;

public class Author
{
    public string url { get; set; }
    public string name { get; set; }
}

public class Classifications
{
    public List<string> lc_classifications { get; set; }
    public List<string> dewey_decimal_class { get; set; }
}

public class Cover
{
    public string small { get; set; }
    public string medium { get; set; }
    public string large { get; set; }
}

public class Ebook
{
    public string preview_url { get; set; }
    public string availability { get; set; }
    public Formats formats { get; set; }
    public string read_url { get; set; }
}

public class Epub
{
    public string url { get; set; }
}

public class Formats
{
    public Pdf pdf { get; set; }
    public Epub epub { get; set; }
    public Text text { get; set; }
}

public class Identifiers
{
    public List<string> librarything { get; set; }
    public List<string> wikidata { get; set; }
    public List<string> goodreads { get; set; }
    public List<string> isbn_10 { get; set; }
    public List<string> lccn { get; set; }
    public List<string> openlibrary { get; set; }
}
public class ISBN0984782869
{
    public string url { get; set; }
    public string key { get; set; }
    public string title { get; set; }
    public string subtitle { get; set; }
    public List<Author> authors { get; set; }
    public int number_of_pages { get; set; }
    public string pagination { get; set; }
    public string by_statement { get; set; }
    public Identifiers identifiers { get; set; }
    public Classifications classifications { get; set; }
    public List<Publisher> publishers { get; set; }
    public List<PublishPlace> publish_places { get; set; }
    public string publish_date { get; set; }
    public List<Subject> subjects { get; set; }
    public string notes { get; set; }
    public List<Ebook> ebooks { get; set; }
    public Cover cover { get; set; }
}
public class ISBN0201558025
{
    public string url { get; set; }
    public string key { get; set; }
    public string title { get; set; }
    public string subtitle { get; set; }
    public List<Author> authors { get; set; }
    public int number_of_pages { get; set; }
    public string pagination { get; set; }
    public string by_statement { get; set; }
    public Identifiers identifiers { get; set; }
    public Classifications classifications { get; set; }
    public List<Publisher> publishers { get; set; }
    public List<PublishPlace> publish_places { get; set; }
    public string publish_date { get; set; }
    public List<Subject> subjects { get; set; }
    public string notes { get; set; }
    public List<Ebook> ebooks { get; set; }
    public Cover cover { get; set; }
}

public class Pdf
{
    public string url { get; set; }
}

public class Publisher
{
    public string name { get; set; }
}

public class PublishPlace
{
    public string name { get; set; }
}

public class Root
{
    [JsonProperty("ISBN:0201558025")]
    public ISBN0201558025? ISBN0201558025 { get; set; }


    [JsonProperty("ISBN:0984782869")]
    public ISBN0984782869? ISBN0984782869 { get; set; }

}

public class Subject
{
    public string name { get; set; }
    public string url { get; set; }
}

public class Text
{
    public string url { get; set; }
}

