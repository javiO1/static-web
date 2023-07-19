
namespace ps_redis_demo
{
    class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }

        public Course(int id, string name, string author, string description)
        {
            Id = id;
            Name = name;
            Author = author;
            Description = description;
        }
    }
}
