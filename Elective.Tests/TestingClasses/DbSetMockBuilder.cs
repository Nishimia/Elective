using Elective.Tests.TestingClasses;
using Moq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace Elective.Tests.TestingClasses
{
	class DbSetMockBuilder
	{
		public static Mock<DbSet<T>> BuildMockDbSet<T>(List<T> data)
			where T : class
		{
			var mockSet = new Mock<DbSet<T>>();
			var source = data.AsQueryable();

			mockSet.As<IDbAsyncEnumerable<T>>()
				.Setup(x => x.GetAsyncEnumerator())
				.Returns(new TestDbAsyncEnumerator<T>(source.GetEnumerator()));

			mockSet.As<IQueryable<T>>()
				.Setup(x => x.Provider)
				.Returns(new TestDbAsyncQueryProvider<T>(source.Provider));

			mockSet.As<IQueryable<T>>()
				.Setup(x => x.Expression)
				.Returns(source.Expression);

			mockSet.As<IQueryable<T>>()
				.Setup(x => x.ElementType)
				.Returns(source.ElementType);

			mockSet.As<IQueryable<T>>()
				.Setup(x => x.GetEnumerator())
				.Returns(source.GetEnumerator());

			mockSet
				.Setup(x => x.Add(It.IsAny<T>()))
				.Callback<T>((s) => data.Add(s));

			mockSet
				.Setup(x => x.Remove(It.IsAny<T>()))
				.Callback<T>((s) => data.Remove(s));

			return mockSet;
		}
	}
}
