using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace NaftanRailway.UnitTests.Rail {
    [TestClass]
    public class LINQ {

        public class Person {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Location { get; set; }

            public Person(int id, string name, string location) {
                Id = id;
                Name = name;
                Location = location;
            }
        };

        [TestMethod]
        public void AutoCompleteDispatch() {
            // Arrange
            var seq = new[] { "First", "Second", "Third" };

            var stuff = new List<Person> {
                new Person(1, "A", "City A"),
                new Person(2, "A", "City B"),
                new Person(3, "A", "City A"),
                new Person(4, "B", "City B"),
                new Person(5, "B", "City B"),
                new Person(6, "C", "City C"),
                new Person(7, "D", "City D"),
            }.AsQueryable();

            //var mockSet = new Mock<DbSet<Person>>();
            //mockSet.As<IDbAsyncEnumerable<Person>>()
            //    .Setup(m => m.GetAsyncEnumerator())
            //    .Returns(new TestDbAsyncEnumerator<Person>(stuff.GetEnumerator()));

            //mockSet.As<IQueryable<Person>>()
            //    .Setup(m => m.Provider)
            //    .Returns(new TestDbAsyncQueryProvider<Person>(stuff.Provider));

            //mockSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(stuff.Expression);
            //mockSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(stuff.ElementType);
            //mockSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(stuff.GetEnumerator());

            //var mockContext = new Mock<StuffContext>();
            //mockContext.Setup(c => c.stuff).Returns(mockSet.Object);

            //var service = new BlogService(mockContext.Object);
            //var blogs = await service.GetAllBlogsAsync();

            //Assert.AreEqual(3, blogs.Count);
            //Assert.AreEqual("AAA", blogs[0].Name);
            //Assert.AreEqual("BBB", blogs[1].Name);
            //Assert.AreEqual("ZZZ", blogs[2].Name);

            // Act
            //var result = collObj.GroupBy(x => x.Name).OrderByDescending(x => x).SelectMany(x => x.Select(y => y.Name));

            // Arrange
            //seq.GetGroup<v_nach, string>(x => x.num_doc, x => x.num_doc.StartsWith(templShNumber)
            //        && (new[] { "3494", "349402" }.Contains(x.cod_kl))
            //        && x.type_doc == 1 && (x.date_raskr >= startDate && x.date_raskr <= endDate))
            //    .OrderByDescending(x => x).Select(x => x.num_doc).Take(10);
        }

        //internal class TestDbAsyncQueryProvider<TEntity> : IDbAsyncQueryProvider {
        //    private readonly IQueryProvider _inner;

        //    internal TestDbAsyncQueryProvider(IQueryProvider inner) {
        //        _inner = inner;
        //    }

        //    public IQueryable CreateQuery(Expression expression) {
        //        return new TestDbAsyncEnumerable<TEntity>(expression);
        //    }

        //    public IQueryable<TElement> CreateQuery<TElement>(Expression expression) {
        //        return new TestDbAsyncEnumerable<TElement>(expression);
        //    }

        //    public object Execute(Expression expression) {
        //        return _inner.Execute(expression);
        //    }

        //    public TResult Execute<TResult>(Expression expression) {
        //        return _inner.Execute<TResult>(expression);
        //    }

        //    public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken) {
        //        return Task.FromResult(Execute(expression));
        //    }

        //    public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken) {
        //        return Task.FromResult(Execute<TResult>(expression));
        //    }
        //}

        //internal class TestDbAsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T> {
        //    public TestDbAsyncEnumerable(IEnumerable<T> enumerable)
        //        : base(enumerable) { }

        //    public TestDbAsyncEnumerable(Expression expression)
        //        : base(expression) { }

        //    public IDbAsyncEnumerator<T> GetAsyncEnumerator() {
        //        return new TestDbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        //    }

        //    IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator() {
        //        return GetAsyncEnumerator();
        //    }

        //    IQueryProvider IQueryable.Provider {
        //        get { return new TestDbAsyncQueryProvider<T>(this); }
        //    }
        //}

        //internal class TestDbAsyncEnumerator<T> : IDbAsyncEnumerator<T> {
        //    private readonly IEnumerator<T> _inner;

        //    public TestDbAsyncEnumerator(IEnumerator<T> inner) {
        //        _inner = inner;
        //    }

        //    public void Dispose() {
        //        _inner.Dispose();
        //    }

        //    public Task<bool> MoveNextAsync(CancellationToken cancellationToken) {
        //        return Task.FromResult(_inner.MoveNext());
        //    }

        //    public T Current {
        //        get { return _inner.Current; }
        //    }

        //    object IDbAsyncEnumerator.Current {
        //        get { return Current; }
        //    }
        //}

        //public class StuffContext : DbContext {
        //    public virtual DbSet<Person> stuff { get; set; }
        //    public virtual DbSet<Post> Posts { get; set; }
        //}

        //public class Blog {
        //    public int BlogId { get; set; }
        //    public string Name { get; set; }
        //    public string Url { get; set; }

        //    public virtual List<Post> Posts { get; set; }
        //}

        //public class Post {
        //    public int PostId { get; set; }
        //    public string Title { get; set; }
        //    public string Content { get; set; }

        //    public int BlogId { get; set; }
        //    public virtual Blog Blog { get; set; }
        //}

        //public class BlogService {
        //    private StuffContext _context;

        //    public BlogService(StuffContext context) {
        //        _context = context;
        //    }

        //    public Blog AddBlog(string name, string url) {
        //        //var blog = _context.stuff.Add(new Person { Name = name, Url = url });
        //        //_context.SaveChanges();

        //        //return blog;
        //    }

        //    public List<Blog> GetAllBlogs() {
        //        var query = from b in _context.Blogs
        //                    orderby b.Name
        //                    select b;

        //        return query.ToList();
        //    }

        //    public async Task<List<Blog>> GetAllBlogsAsync() {
        //        var query = from b in _context.Blogs
        //                    orderby b.Name
        //                    select b;

        //        return await query.ToListAsync();
        //    }
        //}
    }
}